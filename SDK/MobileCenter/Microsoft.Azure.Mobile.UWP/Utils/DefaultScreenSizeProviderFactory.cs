namespace Microsoft.AAppCenterUtils
{
    public class DefaultScreenSizeProviderFactory : IScreenSizeProviderFactory
    {
        public IScreenSizeProvider CreateScreenSizeProvider()
        {
            return new DefaultScreenSizeProvider();
        }
    }
}
