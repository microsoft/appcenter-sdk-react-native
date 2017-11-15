using Microsoft.AppCenter.Channel;
using Microsoft.AppCenter.Ingestion.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.AppCenter.Rum
{
    public partial class RealUserMeasurements : AppCenterService
    {
        #region static

        // Rum configuration endpoint.
        private static readonly string[] ConfigurationEndpoints =
        {
            "http://www.atmrum.net/conf/v1/atm/fpconfig.min.json"
        };

        // Warm up image path.
        private const string WarmUpImage = "trans.gif";

        // TestUrl image path.
        private const string SeventeenkImage = "17k.gif";

        // Flag to support https.
        private const int FlagHttps = 1;

        // Flag to use the 17k image to test.
        private const int FlagSeventeenk = 12;

        // Empty headers to use for HTTP
        private static readonly IDictionary<string, string> Headers = new Dictionary<string, string>();

        private static readonly object RealUserMeasurementsLock = new object();

        private static RealUserMeasurements _instanceField;

        // ReSharper disable once MemberCanBePrivate.Global
        // (Used by reflection from the core).
        public static RealUserMeasurements Instance
        {
            get
            {
                lock (RealUserMeasurementsLock)
                {
                    return _instanceField ?? (_instanceField = new RealUserMeasurements());
                }
            }
        }

        private static Task<bool> PlatformIsEnabledAsync()
        {
            lock (RealUserMeasurementsLock)
            {
                return Task.FromResult(Instance.InstanceEnabled);
            }
        }

        private static Task PlatformSetEnabledAsync(bool enabled)
        {
            lock (RealUserMeasurementsLock)
            {
                Instance.InstanceEnabled = enabled;
                return Task.FromResult(default(object));
            }
        }

        private static void PlatformSetRumKey(string rumKey)
        {
            lock (RealUserMeasurementsLock)
            {
                Instance.InstanceSetRumKey(rumKey);
            }
        }

        // All unique identifiers in Rum have no dash.
        private static string ProbeId()
        {
            return Guid.NewGuid().ToString().Replace("-", "");
        }

        #endregion

        #region instance

        private string _rumKey;

        private CancellationTokenSource _cancellationTokenSource;

        public override bool InstanceEnabled
        {
            get => base.InstanceEnabled;

            set
            {
                lock (_serviceLock)
                {
                    var prevValue = InstanceEnabled;
                    base.InstanceEnabled = value;
                    if (value != prevValue)
                    {
                        ApplyEnabledState(value);
                    }
                }
            }
        }

        private void InstanceSetRumKey(string rumKey)
        {
            lock (_serviceLock)
            {
                if (rumKey == null)
                {
                    AppCenterLog.Error(LogTag, "Rum key is invalid.");
                    return;
                }
                rumKey = rumKey.Trim();
                if (rumKey.Length != 32)
                {
                    AppCenterLog.Error(LogTag, "Rum key is invalid.");
                    return;
                }
                _rumKey = rumKey;
            }
        }

        public override string ServiceName => "RealUserMeasurements";

        protected override string ChannelName => "rum";

        public override void OnChannelGroupReady(IChannelGroup channelGroup, string appSecret)
        {
            lock (_serviceLock)
            {
                base.OnChannelGroupReady(channelGroup, appSecret);
                ApplyEnabledState(InstanceEnabled);
            }
        }


        private void ApplyEnabledState(bool enabled)
        {
            lock (_serviceLock)
            {
                if (enabled)
                {
                    // Snapshot RUM key for this run, updates will work only if disabling/enabling again.
                    var rumKey = _rumKey;
                    if (rumKey == null)
                    {
                        AppCenterLog.Error(LogTag, "Rum key must be configured before start.");
                        return;
                    }

                    // Create cancellation token and snapshot token for the task to avoid race conditions.
                    _cancellationTokenSource = new CancellationTokenSource();
                    var cancellationToken = _cancellationTokenSource.Token;
                    Task.Run(async () =>
                    {
                        try
                        {
                            await RunMeasurementsAsync(rumKey, cancellationToken);
                        }
                        catch (OperationCanceledException)
                        {
                            AppCenterLog.Debug(LogTag, "Measurements were canceled.");
                        }
                        catch (AppCenterException e)
                        {
                            AppCenterLog.Error(LogTag, $"Could not run measurements: {e.Message}");
                        }
                        catch (Exception e)
                        {
                            AppCenterLog.Error(LogTag, "Could not run measurements.", e);
                        }
                        finally
                        {
                            lock (_serviceLock)
                            {
                                if (cancellationToken == _cancellationTokenSource?.Token)
                                {
                                    DisposeCancellationSource();
                                }
                            }
                        }
                    }, cancellationToken);
                }
                else
                {
                    _cancellationTokenSource?.Cancel();
                    DisposeCancellationSource();
                }
            }
        }

        private void DisposeCancellationSource()
        {
            _cancellationTokenSource?.Dispose();
            _cancellationTokenSource = null;
        }

        private async Task RunMeasurementsAsync(string rumKey, CancellationToken cancellationToken)
        {
            // TODO handle network state, requires refactoring to reuse ingestion logic here
            using (var httpNetworkAdapter = new HttpNetworkAdapter())
            {
                // Get remote configuration.
                var jsonConfiguration = await GetRemoteConfigurationAsync(cancellationToken, httpNetworkAdapter);

                // Parse configuration.
                var configuration = JsonConvert.DeserializeObject<RumConfiguration>(jsonConfiguration);

                // If configuration looks good so far.
                if (configuration.TestEndpoints != null)
                {
                    // Generate test urls.
                    var testUrls = GenerateTestUrls(configuration);

                    // Run the tests.
                    await RunTestUrlsAsync(cancellationToken, httpNetworkAdapter, testUrls);

                    // Report the results.
                    await ReportMeasurementsAsync(rumKey, cancellationToken, httpNetworkAdapter, configuration, testUrls);
                }
                else
                {
                    throw new AppCenterException("Configuration does not contain test endpoints.");
                }
            }
        }

        private async Task<string> GetRemoteConfigurationAsync(CancellationToken cancellationToken, IHttpNetworkAdapter httpNetworkAdapter)
        {
            foreach (var url in ConfigurationEndpoints)
            {
                try
                {
                    AppCenterLog.Verbose(LogTag, "Calling " + url);
                    return await httpNetworkAdapter.SendAsync(url, "GET", Headers, "", cancellationToken);
                }
                catch (Exception e) when (!(e is OperationCanceledException))
                {
                    AppCenterLog.Error(LogTag, "Could not get configuration file at " + url, e);
                }
            }
            throw new AppCenterException("Could not get configuration on any endpoint.");
        }

        private static List<TestUrl> GenerateTestUrls(RumConfiguration configuration)
        {
            // Prepare weighted random selection.
            var totalWeight = 0;
            var weightedEndpoints = new List<TestEndpoint>(configuration.TestEndpoints.Count);
            foreach (var endpoint in configuration.TestEndpoints)
            {
                var weight = endpoint.Weight;
                if (weight > 0)
                {
                    totalWeight += weight;
                    endpoint.CumulatedWeight = totalWeight;
                    weightedEndpoints.Add(endpoint);
                }
            }

            // Select n endpoints randomly with respect to weight.
            var testUrls = new List<TestUrl>();
            var random = new Random();
            var testCount = Math.Min(configuration.TestCount, weightedEndpoints.Count);
            for (var n = 0; n < testCount; n++)
            {
                // Select random endpoint
                var randomWeight = Math.Floor(random.NextDouble() * totalWeight);
                TestEndpoint endpoint = null;
                for (var i = 0; i < weightedEndpoints.Count; i++)
                {
                    var weightedEndpoint = weightedEndpoints[i];
                    var cumulatedWeight = weightedEndpoint.CumulatedWeight;
                    if (endpoint == null)
                    {
                        if (randomWeight <= cumulatedWeight)
                        {
                            endpoint = weightedEndpoint;
                            weightedEndpoints.RemoveAt(i--);
                        }
                    }

                    // Update subsequent endpoints cumulated weights since we removed an element.
                    else
                    {
                        weightedEndpoint.CumulatedWeight -= endpoint.Weight;
                    }
                }

                // Update total weight since we removed the picked endpoint.
                if (endpoint == null)
                {
                    continue;
                }
                totalWeight -= endpoint.Weight;

                // Use endpoint to generate test urls.
                var protocolSuffix = "";
                var measurementType = endpoint.MeasurementType;
                if ((measurementType & FlagHttps) > 0)
                {
                    protocolSuffix = "s";
                }
                var requestId = endpoint.Url;

                // Handle backward compatibility with FPv1.
                var baseUrl = requestId;
                if (!requestId.Contains("."))
                {
                    baseUrl += ".clo.footprintdns.com";
                }

                // Port this Javascript behavior regarding url and requestId.
                else if (requestId.StartsWith("*") && requestId.Length > 2)
                {
                    var domain = requestId.Substring(2);
                    var uuid = ProbeId();
                    baseUrl = uuid + "." + domain;
                    requestId = domain.Equals("clo.footprintdns.com", StringComparison.OrdinalIgnoreCase) ? uuid : domain;
                }

                // Generate test urls.
                var probeId = ProbeId();
                var testUrl = $"http{protocolSuffix}://{baseUrl}/apc/{WarmUpImage}?{probeId}";
                testUrls.Add(new TestUrl { Url = testUrl, RequestId = requestId, Object = WarmUpImage, Conn = "cold" });
                var testImage = (measurementType & FlagSeventeenk) > 0 ? SeventeenkImage : WarmUpImage;
                probeId = ProbeId();
                testUrl = $"http{protocolSuffix}://{baseUrl}/apc/{testImage}?{probeId}";
                testUrls.Add(new TestUrl { Url = testUrl, RequestId = requestId, Object = testImage, Conn = "warm" });
            }
            return testUrls;
        }

        private async Task RunTestUrlsAsync(CancellationToken cancellationToken, IHttpNetworkAdapter httpNetworkAdapter, IList<TestUrl> testUrls)
        {
            var stopWatch = new Stopwatch();
            for (var i = 0; i < testUrls.Count; i++)
            {
                var testUrl = testUrls[i];
                AppCenterLog.Verbose(LogTag, "Calling " + testUrl.Url);
                try
                {
                    stopWatch.Restart();
                    await httpNetworkAdapter.SendAsync(testUrl.Url, "GET", Headers, "", cancellationToken);
                    testUrl.Result = stopWatch.ElapsedMilliseconds;
                }
                catch (Exception e) when (!(e is OperationCanceledException))
                {
                    testUrls.RemoveAt(i--);
                    AppCenterLog.Error(LogTag, testUrl.Url + " call failed", e);
                }
            }
        }

        private async Task ReportMeasurementsAsync(string rumKey, CancellationToken cancellationToken, IHttpNetworkAdapter httpNetworkAdapter, RumConfiguration configuration, IList<TestUrl> testUrls)
        {
            // Generate report.
            var jsonReport = JsonConvert.SerializeObject(testUrls);
            AppCenterLog.Verbose(LogTag, $"Report payload={jsonReport}");

            // Send it.
            if (configuration.ReportEndpoints != null)
            {
                var reportId = ProbeId();
                var reportQueryString = $"?MonitorID=atm-mc&rid={reportId}&w3c=false&prot=https&v=2017061301&tag={rumKey}&DATA={Uri.EscapeDataString(jsonReport)}";
                foreach (var reportEndpoint in configuration.ReportEndpoints)
                {
                    var reportUrl = $"https://{reportEndpoint}{reportQueryString}";
                    AppCenterLog.Verbose(LogTag, "Calling " + reportUrl);
                    try
                    {
                        await httpNetworkAdapter.SendAsync(reportUrl, "GET", Headers, "", cancellationToken);
                        AppCenterLog.Info(LogTag, "Measurements reported successfully.");

                        // Stop when we encounter the first working report endpoint.
                        return;
                    }
                    catch (Exception e) when (!(e is OperationCanceledException))
                    {
                        // Fall back on next report endpoint.
                        AppCenterLog.Error(LogTag, $"Failed to report measurements at {reportEndpoint}", e);
                    }
                }
                AppCenterLog.Error(LogTag, "Measurements report failed on all report endpoints.");
            }
            else
            {
                throw new AppCenterException("Configuration does not contain report endpoints.");
            }
        }

        #endregion
    }
}
