namespace Microsoft.AppCenterUtils
{
    public class DefaultScreenSizeProviderFactory : IScreenSizeProviderFactory
    {
        public IScreenSizeProvider CreateScreenSizeProvider()
        {
            return new DefaultScreenSizeProvider();
        }
    }
}
