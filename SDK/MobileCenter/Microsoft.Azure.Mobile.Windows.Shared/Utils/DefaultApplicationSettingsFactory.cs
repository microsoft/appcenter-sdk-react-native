namespace Microsoft.Azure.Mobile.Utils
{
    public class DefaultApplicationSettingsFactory : IApplicationSettingsFactory
    {
        public IApplicationSettings CreateApplicationSettings()
        {
            return new DefaultApplicationSettings();
        }
    }
}
