using Microsoft.Azure.Mobile.Utils;
using Moq;

namespace Microsoft.Azure.Mobile.Test.Utils
{
    public class MockApplicationSettingsFactory : IApplicationSettingsFactory
    {
        private readonly Mock<IApplicationSettings> _applicationSettingsMock;

        public MockApplicationSettingsFactory(Mock<IApplicationSettings> applicationSettingsMock)
        {
            _applicationSettingsMock = applicationSettingsMock;
        }

        public IApplicationSettings CreateApplicationSettings()
        {
            return _applicationSettingsMock.Object;
        }
    }
}
