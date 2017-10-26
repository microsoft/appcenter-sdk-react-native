using Microsoft.AppCenterChannel;

namespace Microsoft.AppCenter.Test.Windows
{
    public class TestAppCenterService : AppCenterService
    {
        protected override string ChannelName => "test_service";
        public override string ServiceName => "TestService";

        public IChannelGroup PublicChannelGroup => ChannelGroup;
        public IChannel PublicChannel => Channel;
        public string PublicEnabledPreferenceKey => EnabledPreferenceKey;
        public bool PublicIsInactive => IsInactive;
        public string PublicChannelName => ChannelName;
    }
}
