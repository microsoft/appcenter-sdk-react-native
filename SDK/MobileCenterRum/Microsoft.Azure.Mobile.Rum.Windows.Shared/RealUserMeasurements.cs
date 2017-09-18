using Microsoft.Azure.Mobile.Channel;
using Microsoft.Azure.Mobile.Ingestion.Http;
using System.Threading.Tasks;
using System;
using System.Net.Http;
using System.Collections.Generic;
using System.Threading;
using Newtonsoft.Json.Linq;

namespace Microsoft.Azure.Mobile.Rum
{
    public partial class RealUserMeasurements : MobileCenterService
    {
        #region static

        // Rum configuration endpoint.
        private const string ConfigurationEndpoint = "https://rumconfig.trafficmanager.net";

        // JSON configuration file name.
        private const string ConfigurationFileName = "rumConfig.js";

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

        public static RealUserMeasurements Instance
        {
            get
            {
                lock (RealUserMeasurementsLock)
                {
                    return _instanceField ?? (_instanceField = new RealUserMeasurements());
                }
            }
            set
            {
                lock (RealUserMeasurementsLock)
                {
                    _instanceField = value;
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
        }

        private static void PlatformSetConfigurationUrl(string url)
        {
            Instance.InstanceSetConfigurationUrl(url);
        }

        #endregion

        #region instance

        private string _configurationUrl = ConfigurationEndpoint;

        private JObject _configuration;

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

        public override string ServiceName => "RealUserMeasurements";

        protected override string ChannelName => "rum";

        private void InstanceSetConfigurationUrl(string url)
        {
            lock (_serviceLock)
            {
                _configurationUrl = url;
            }
        }

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
                    Task.Run(async () =>
                    {
                        try
                        {
                            await GetRemoteConfigurationAsync();
                        }
                        catch (Exception e)
                        {
                            MobileCenterLog.Error(LogTag, "Could not run tests.", e);
                        }
                    });
                }
            }
        }

        private async Task GetRemoteConfigurationAsync()
        {
            // TODO handle network state, requires huge refactoring to reuse ingestion logic here...
            var httpNetworkAdapter = new HttpNetworkAdapter();

            // TODO manage cancel on disable
            var request = await httpNetworkAdapter.SendAsync($"{_configurationUrl}/{ConfigurationFileName}", HttpMethod.Get, Headers, "", new CancellationTokenSource().Token);

            // Parse configuration
            _configuration = JObject.Parse(request);

            // Prepare random weighted selection
            if (_configuration["e"] is JArray endpoints)
            {
                var totalWeight = 0;
                var weightedEndpoints = new List<JObject>(endpoints.Count);
                foreach (var jToken in endpoints)
                {
                    var endpoint = (JObject)jToken;
                    var weight = endpoint["w"].Value<int>();
                    if (weight > 0)
                    {
                        totalWeight += weight;
                        endpoint.Add("cumulatedWeight", totalWeight);
                        weightedEndpoints.Add(endpoint);
                    }
                }

                // Select n endpoints randomly with respect to weight
                var testUrls = new List<TestUrl>();
                var random = new Random();
                var testCount = Math.Min(_configuration["n"].Value<int>(), weightedEndpoints.Count);
                for (var n = 0; n < testCount; n++)
                {
                    // Select random endpoint
                    var randomWeight = Math.Floor(random.NextDouble() * totalWeight);
                    JObject endpoint = null;
                    for (var i = 0; i < weightedEndpoints.Count; i++)
                    {
                        var weightedEndpoint = weightedEndpoints[i];
                        var cumulatedWeight = weightedEndpoint["cumulatedWeight"].Value<int>();
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
                            cumulatedWeight -= endpoint["w"].Value<int>();
                            weightedEndpoint["cumulatedWeight"] = cumulatedWeight;
                        }
                    }

                    // Update total weight since we removed the picked endpoint.
                    if (endpoint == null)
                    {
                        continue;
                    }
                    totalWeight -= endpoint["w"].Value<int>();
                    MobileCenterLog.Verbose(LogTag, "e=" + endpoint);

                    // Use endpoint to generate test urls.
                    var protocolSuffix = "";
                    var measurementType = endpoint["m"].Value<int>();
                    if ((measurementType & FlagHttps) > 0)
                    {
                        protocolSuffix = "s";
                    }
                    var requestId = endpoint["e"].Value<string>();

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
                        var uuid = Guid.NewGuid().ToString();
                        baseUrl = uuid + "." + domain;
                        requestId = domain.Equals("clo.footprintdns.com", StringComparison.OrdinalIgnoreCase) ? uuid : domain;
                    }

                    // Generate test urls.
                    var probeId = Guid.NewGuid().ToString();
                    var testUrl = $"http{protocolSuffix}://{baseUrl}/apc/{WarmUpImage}?{probeId}";
                    testUrls.Add(new TestUrl { Url = testUrl, RequestId = requestId, Object = WarmUpImage, Conn = "cold" });
                    MobileCenterLog.Verbose(LogTag, testUrl);
                    var testImage = (measurementType & FlagSeventeenk) > 0 ? SeventeenkImage : WarmUpImage;
                    probeId = Guid.NewGuid().ToString();
                    testUrl = $"http{protocolSuffix}://{baseUrl}/apc/{testImage}?{probeId}";
                    testUrls.Add(new TestUrl { Url = testUrl, RequestId = requestId, Object = testImage, Conn = "warm" });
                    MobileCenterLog.Verbose(LogTag, testUrl);
                }
            }
            else
            {
                throw new MobileCenterException("Invalid remote configuration for Rum.");
            }
        }

        #endregion
    }
}
