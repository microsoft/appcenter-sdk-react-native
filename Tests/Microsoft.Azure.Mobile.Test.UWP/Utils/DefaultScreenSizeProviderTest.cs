using Microsoft.Azure.Mobile.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Threading.Tasks;
using Windows.Graphics.Display;

namespace Microsoft.Azure.Mobile.Test.UWP.Utils
{
    [TestClass]
    public class DefaultScreenSizeProviderTest
    {
        /// <summary>
        /// Verify screen size changed event is raised on screen size change
        /// </summary>
        [TestMethod]
        public void VerifyScreenSizeChangedEventIsRaised()
        {
            var screenSizeChanged = false;
            var screenSizeProvider = new DefaultScreenSizeProvider();
            screenSizeProvider.ScreenSizeChanged += (sender, args) => { screenSizeChanged = true; };

            screenSizeProvider.UpdateDisplayInformation(1, 1).Wait();
            
            Assert.IsTrue(screenSizeChanged);
        }

        /// <summary>
        /// Verify new screen size is set after screen size change 
        /// </summary>
        [TestMethod]
        public void VerifyNewScreenSizeIsSet()
        {
            var newScreenHeight = 1;
            var newScreenWidth = 1;
            var screenSizeProvider = new DefaultScreenSizeProvider();

            screenSizeProvider.UpdateDisplayInformation(newScreenHeight, newScreenWidth).Wait();

            Assert.AreEqual(screenSizeProvider.Height, newScreenHeight);
            Assert.AreEqual(screenSizeProvider.Width, newScreenWidth);
        }
    }
}
