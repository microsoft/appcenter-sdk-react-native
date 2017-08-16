using Microsoft.Azure.Mobile.Channel;
using Microsoft.Azure.Mobile.Utils;

namespace Microsoft.Azure.Mobile.Test.Windows
{
    public class TestMobileCenterService : MobileCenterService
    {
        private readonly IApplicationSettings _settings;

        public TestMobileCenterService(IApplicationSettings settings)
        {
            _settings = settings;
        }

        public TestMobileCenterService()
        {
        }

        protected override IApplicationSettings ApplicationSettings => _settings ?? base.ApplicationSettings;

        protected override string ChannelName => "test_service";
        public override string ServiceName => "TestService";

        public IChannelGroup PublicChannelGroup => ChannelGroup;
        public IChannel PublicChannel => Channel;
        public string PublicEnabledPreferenceKey => EnabledPreferenceKey;
        public bool PublicIsInactive => IsInactive;
        public string PublicChannelName => ChannelName;
    }
}
