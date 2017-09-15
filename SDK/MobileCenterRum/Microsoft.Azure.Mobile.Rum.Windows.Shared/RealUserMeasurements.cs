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

        // Rum configuration endpoint
        private const string ConfigurationEndpoint = "https://rumconfig.trafficmanager.net";

        // JSON configuration file name
        private const string ConfigurationFileName = "rumConfig.js";

        // Empty headers to use for HTTP
        private readonly IDictionary<string, string> Headers = new Dictionary<string, string>();

        private static readonly object PushLock = new object();

        private static RealUserMeasurements _instanceField;

        public static RealUserMeasurements Instance
        {
            get
            {
                lock (PushLock)
                {
                    return _instanceField ?? (_instanceField = new RealUserMeasurements());
                }
            }
            set
            {
                lock (PushLock)
                {
                    _instanceField = value;
                }
            }
        }

        static Task<bool> PlatformIsEnabledAsync()
        {
            lock (PushLock)
            {
                return Task.FromResult(Instance.InstanceEnabled);
            }
        }

        static Task PlatformSetEnabledAsync(bool enabled)
        {
            lock (PushLock)
            {
                Instance.InstanceEnabled = enabled;
                return Task.FromResult(default(object));
            }
        }

        static void PlatformSetRumKey(string rumKey)
        {
        }

        static void PlatformSetConfigurationUrl(string url)
        {
            Instance.InstanceSetConfigurationUrl(url);
        }

        #endregion

        #region instance

        private string _configurationUrl = ConfigurationEndpoint;

        private JObject _configuration;

        public override bool InstanceEnabled
        {
            get
            {
                return base.InstanceEnabled;
            }

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
            var _httpNetworkAdapter = new HttpNetworkAdapter();

            // TODO manage cancel on disable
            var request = await _httpNetworkAdapter.SendAsync($"{_configurationUrl}/{ConfigurationFileName}", HttpMethod.Get, Headers, "", new CancellationTokenSource().Token);

            // Parse configuration
            _configuration = JObject.Parse(request);

            // Prepare random weighted selection
            var endpoints = _configuration["e"] as JArray;
            var totalWeight = 0;
            var weightedEndpoints = new List<JObject>(endpoints.Count);
            foreach (JObject endpoint in endpoints)
            {
                var weight = endpoint["w"].Value<Int32>();
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
            var testCount = Math.Min(_configuration["n"].Value<Int32>(), weightedEndpoints.Count);
            for (int n = 0; n < testCount; n++)
            {
                // Select random endpoint
                var randomWeight = Math.Floor(random.NextDouble() * totalWeight);
                JObject endpoint = null;
                for (int i = 0; i < weightedEndpoints.Count; i++)
                {
                    var weightedEndpoint = weightedEndpoints[i];
                    var cumulatedWeight = weightedEndpoint["cumulatedWeight"].Value<Int32>();
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
                        cumulatedWeight -= endpoint["w"].Value<Int32>();
                        weightedEndpoint["cumulatedWeight"] = cumulatedWeight;
                    }
                }

                // Update total weight since we removed the picked endpoint.
                totalWeight -= endpoint["w"].Value<Int32>();
                MobileCenterLog.Verbose(LogTag, "e=" + endpoint);
            }
        }

        #endregion
    }
}
