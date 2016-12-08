using System;
using System.IO;
using System.Linq;
using NUnit.Framework;
using Xamarin.UITest;
using Xamarin.UITest.Queries;
using System.Diagnostics;

namespace Contoso.Forms.Test.UITests
{
    [TestFixture(Platform.Android)]
    [TestFixture(Platform.iOS)]
    public class Tests
    {
        IApp app;
        Platform platform;

        public Tests(Platform platform)
        {
            this.platform = platform;
        }

        [SetUp]
        public void BeforeEachTest()
        {
            app = AppInitializer.StartApp(platform);
        }

        [Test]
        public void TestEnablingAndDisablingServices()
        {
            ServiceStateHelper.app = app;
            ServiceStateHelper.platform = platform;

            app.Tap(TestStrings.GoToTogglePageButton);

            /* Test setting enabling all services */
            ServiceStateHelper.MobileCenterEnabled = true;
            Assert.IsTrue(ServiceStateHelper.MobileCenterEnabled);
            ServiceStateHelper.AnalyticsEnabled = true;
            Assert.IsTrue(ServiceStateHelper.AnalyticsEnabled);
            ServiceStateHelper.CrashesEnabled = true;
            Assert.IsTrue(ServiceStateHelper.CrashesEnabled);

            /* Test that disabling MobileCenter disables everything */
            ServiceStateHelper.MobileCenterEnabled = false;
            Assert.IsFalse(ServiceStateHelper.MobileCenterEnabled);
            Assert.IsFalse(ServiceStateHelper.AnalyticsEnabled);
            Assert.IsFalse(ServiceStateHelper.CrashesEnabled);

            /* Test disabling individual services */
            ServiceStateHelper.MobileCenterEnabled = true;
            Assert.IsTrue(ServiceStateHelper.MobileCenterEnabled);
            ServiceStateHelper.AnalyticsEnabled = false;
            Assert.IsFalse(ServiceStateHelper.AnalyticsEnabled);
            ServiceStateHelper.CrashesEnabled = false;
            Assert.IsFalse(ServiceStateHelper.CrashesEnabled);

            /* Test that enabling MobileCenter enabling everything, regardless of previous states */
            ServiceStateHelper.MobileCenterEnabled = true;
            Assert.IsTrue(ServiceStateHelper.MobileCenterEnabled);
            Assert.IsTrue(ServiceStateHelper.AnalyticsEnabled);
            Assert.IsTrue(ServiceStateHelper.CrashesEnabled);
        }

        [Test]
        public void SendEvents()
        {
            app.Tap(TestStrings.GoToAnalyticsPageButton);
            app.Tap(TestStrings.SendEventButton);
            app.Tap(TestStrings.AddPropertyButton);
            app.Tap(TestStrings.AddPropertyButton);
            app.Tap(TestStrings.AddPropertyButton);
            app.Tap(TestStrings.AddPropertyButton);
            app.Tap(TestStrings.AddPropertyButton);
            app.Tap(TestStrings.SendEventButton);
            //TODO some kind of verification
        }

        [Test]
        public void DivideByZero()
        {
            app.Tap(TestStrings.GoToCrashesPageButton);
            app.Tap(TestStrings.DivideByZeroCrashButton);
            app = AppInitializer.StartApp(platform);

            app.Tap(TestStrings.GoToCrashResultsPageButton);

            CrashResultsHelper.app = app;
            CrashResultsHelper.platform = platform;
            Assert.IsTrue(CrashResultsHelper.SendingErrorReportWasCalled);
            Assert.IsTrue(CrashResultsHelper.SentErrorReportWasCalled);
            Assert.IsFalse(CrashResultsHelper.FailedToSendErrorReportWasCalled);
            Assert.IsTrue(CrashResultsHelper.ShouldProcessErrorReportWasCalled);
            Assert.IsTrue(CrashResultsHelper.ShouldAwaitUserConfirmationWasCalled);
            Assert.IsTrue(CrashResultsHelper.GetErrorAttachmentWasCalled);
        }
    }
}
