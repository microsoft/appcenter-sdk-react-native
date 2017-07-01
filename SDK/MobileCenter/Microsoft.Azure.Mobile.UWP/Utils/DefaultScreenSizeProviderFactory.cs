namespace Microsoft.Azure.Mobile.Utils
{
    public class DefaultScreenSizeProviderFactory : IScreenSizeProviderFactory
    {
        public IScreenSizeProvider CreateScreenSizeProvider()
        {
            return new DefaultScreenSizeProvider();
        }
    }
}
