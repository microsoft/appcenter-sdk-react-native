using Microsoft.Azure.Mobile.Channel;
using Microsoft.Azure.Mobile.Ingestion.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Mobile.Rum
{
    public partial class RealUserMeasurements : MobileCenterService
    {
        #region static

        // Rum configuration endpoint.
        private const string ConfigurationEndpoint = "http://www.atmrum.net/conf/v1/atm";

        // JSON configuration file name.
        private const string ConfigurationFileName = "fpconfig.min.json";

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
                    MobileCenterLog.Error(LogTag, "Rum key is invalid.");
                    return;
                }
                rumKey = rumKey.Trim();
                if (rumKey.Length != 32)
                {
                    MobileCenterLog.Error(LogTag, "Rum key is invalid.");
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
                        MobileCenterLog.Error(LogTag, "Rum key must be configured before start.");
                        return;
                    }

                    // Create cancellation token and snapshot token for the task to avoid race conditions.
                    _cancellationTokenSource = new CancellationTokenSource();
                    var cancellationToken = _cancellationTokenSource.Token;
                    Task.Run(async () =>
                    {
                        try
                        {
                            await RunTestsAsync(rumKey, cancellationToken);
                        }
                        catch (OperationCanceledException)
                        {
                            MobileCenterLog.Debug(LogTag, "Measurements were canceled.");
                        }
                        catch (Exception e)
                        {
                            MobileCenterLog.Error(LogTag, "Could not run tests.", e);
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

        private async Task RunTestsAsync(string rumKey, CancellationToken cancellationToken)
        {
            // TODO handle network state, requires refactoring to reuse ingestion logic here
            using (var httpNetworkAdapter = new HttpNetworkAdapter())
            {
                // Get remote configuration.
                var jsonConfiguration = await httpNetworkAdapter.SendAsync($"{ConfigurationEndpoint}/{ConfigurationFileName}", HttpMethod.Get, Headers, "", cancellationToken);

                // Parse configuration.
                var configuration = JsonConvert.DeserializeObject<RumConfiguration>(jsonConfiguration);

                // Prepare random weighted selection.
                var configurationOk = false;
                if (configuration.TestEndpoints != null)
                {
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

                    // Run the tests.
                    var stopWatch = new Stopwatch();
                    for (var i = 0; i < testUrls.Count; i++)
                    {
                        var testUrl = testUrls[i];
                        MobileCenterLog.Verbose(LogTag, "Calling " + testUrl.Url);
                        try
                        {
                            stopWatch.Restart();
                            await httpNetworkAdapter.SendAsync(testUrl.Url, HttpMethod.Get, Headers, "", cancellationToken);
                            testUrl.Result = stopWatch.ElapsedMilliseconds;
                        }
                        catch (OperationCanceledException)
                        {
                            throw;
                        }
                        catch (Exception e)
                        {
                            testUrls.RemoveAt(i--);
                            MobileCenterLog.Error(LogTag, testUrl.Url + " call failed", e);
                        }
                    }

                    // Generate report.
                    var jsonReport = JsonConvert.SerializeObject(testUrls);
                    MobileCenterLog.Verbose(LogTag, $"Report payload={jsonReport}");

                    // Send it.
                    if (configuration.ReportEndpoints != null)
                    {
                        configurationOk = true;
                        var reportId = ProbeId();
                        var reportQueryString = $"?MonitorID=atm&rid={reportId}&w3c=false&prot=https&v=2017061301&tag={rumKey}&DATA={Uri.EscapeDataString(jsonReport)}";
                        var hadFailure = false;
                        var hadSuccess = false;
                        foreach (var reportEndpoint in configuration.ReportEndpoints)
                        {
                            var reportUrl = $"http://{reportEndpoint}{reportQueryString}";
                            MobileCenterLog.Verbose(LogTag, "Calling " + reportUrl);
                            try
                            {
                                await httpNetworkAdapter.SendAsync(reportUrl, HttpMethod.Get, Headers, "", cancellationToken);
                                hadSuccess = true;
                            }
                            catch (OperationCanceledException)
                            {
                                throw;
                            }
                            catch (Exception e)
                            {
                                hadFailure = true;
                                MobileCenterLog.Error(LogTag, $"Failed to report measurements at {reportEndpoint}", e);
                            }
                        }
                        if (hadFailure)
                        {
                            if (hadSuccess)
                            {
                                MobileCenterLog.Warn(LogTag, "Measurements report failed on some report endpoints.");
                            }
                            else
                            {
                                MobileCenterLog.Error(LogTag, "Measurements report failed on all report endpoints.");
                            }
                        }
                        else
                        {
                            MobileCenterLog.Info(LogTag, "Measurements reported to all report endpoints.");
                        }
                    }
                }
                if (!configurationOk)
                {
                    throw new MobileCenterException("Invalid remote configuration for Rum.");
                }
            }
        }

        #endregion
    }
}
