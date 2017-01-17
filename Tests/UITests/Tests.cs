using NUnit.Framework;
using Xamarin.UITest;
using System;
using System.IO;

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

            /* Test that enabling MobileCenter enables everything, regardless of previous states */
            ServiceStateHelper.MobileCenterEnabled = true;
            Assert.IsTrue(ServiceStateHelper.MobileCenterEnabled);
            Assert.IsTrue(ServiceStateHelper.AnalyticsEnabled);
            Assert.IsTrue(ServiceStateHelper.CrashesEnabled);
        }

        [Test]
        public void TestServiceStatePersistence()
        {
            ServiceStateHelper.app = app;
            app.Tap(TestStrings.GoToTogglePageButton);

            /* Make sure Crashes enabled state is persistent */
            ServiceStateHelper.MobileCenterEnabled = true;
            ServiceStateHelper.CrashesEnabled = false;
            Assert.IsFalse(ServiceStateHelper.CrashesEnabled);
            app = AppInitializer.StartAppNoClear(platform);
            app.Tap(TestStrings.GoToTogglePageButton);
            Assert.IsTrue(ServiceStateHelper.MobileCenterEnabled);
            Assert.IsTrue(ServiceStateHelper.AnalyticsEnabled);
            Assert.IsFalse(ServiceStateHelper.CrashesEnabled);

            /* Make sure Analytics enabled state is persistent */
            ServiceStateHelper.MobileCenterEnabled = true;
            ServiceStateHelper.AnalyticsEnabled = false;
            Assert.IsFalse(ServiceStateHelper.AnalyticsEnabled);
            app = AppInitializer.StartAppNoClear(platform);
            app.Tap(TestStrings.GoToTogglePageButton);
            Assert.IsTrue(ServiceStateHelper.MobileCenterEnabled);
            Assert.IsFalse(ServiceStateHelper.AnalyticsEnabled);
            Assert.IsTrue(ServiceStateHelper.CrashesEnabled);

            /* Make sure MobileCenter enabled state is persistent */
            ServiceStateHelper.MobileCenterEnabled = false;
            Assert.IsFalse(ServiceStateHelper.MobileCenterEnabled);
            Assert.IsFalse(ServiceStateHelper.CrashesEnabled);
            Assert.IsFalse(ServiceStateHelper.AnalyticsEnabled);
            app = AppInitializer.StartAppNoClear(platform);
            app.Tap(TestStrings.GoToTogglePageButton);
            Assert.IsFalse(ServiceStateHelper.MobileCenterEnabled);
            Assert.IsFalse(ServiceStateHelper.AnalyticsEnabled);
            Assert.IsFalse(ServiceStateHelper.CrashesEnabled);

            /* Reset services to enabled */
            ServiceStateHelper.MobileCenterEnabled = true;
        }


        [Test]
        public void SendEventWithProperties()
        {
            app.Tap(TestStrings.GoToAnalyticsPageButton);
            int numProperties = 5;
            SendEvent(numProperties);
            app.Tap(TestStrings.GoToAnalyticsResultsPageButton);

            /* Verify that the event was sent properly */
            AnalyticsResultsHelper.app = app;
            Assert.IsTrue(AnalyticsResultsHelper.SendingEventWasCalled);
            Assert.IsTrue(AnalyticsResultsHelper.VerifyEventName());
            Assert.IsTrue(AnalyticsResultsHelper.VerifyNumProperties(numProperties));
            Assert.IsTrue(AnalyticsResultsHelper.SentEventWasCalled);
            Assert.IsFalse(AnalyticsResultsHelper.FailedToSendEventWasCalled);
        } 

        [Test]
        public void SendEventWithNoProperties()
        {
            app.Tap(TestStrings.GoToAnalyticsPageButton);
            int numProperties = 0;
            SendEvent(numProperties);
            app.Tap(TestStrings.GoToAnalyticsResultsPageButton);

            /* Verify that the event was sent properly */
            AnalyticsResultsHelper.app = app;
            Assert.IsTrue(AnalyticsResultsHelper.SendingEventWasCalled);
            Assert.IsTrue(AnalyticsResultsHelper.VerifyEventName());
            Assert.IsTrue(AnalyticsResultsHelper.VerifyNumProperties(numProperties));
            Assert.IsTrue(AnalyticsResultsHelper.SentEventWasCalled);
            Assert.IsFalse(AnalyticsResultsHelper.FailedToSendEventWasCalled);
        }

        [Test]
        public void SendEventWithAnalyticsDisabled()
        {
            /* Disable Analytics */
            ServiceStateHelper.app = app;
            app.Tap(TestStrings.GoToTogglePageButton);
            ServiceStateHelper.AnalyticsEnabled = false;
            app.Tap(TestStrings.DismissButton);

            app.Tap(TestStrings.GoToAnalyticsPageButton);
            int numProperties = 1;
            SendEvent(numProperties);
            app.Tap(TestStrings.GoToAnalyticsResultsPageButton);

            /* Verify that the event was not sent */
            AnalyticsResultsHelper.app = app;
            Assert.IsFalse(AnalyticsResultsHelper.SendingEventWasCalled);
            Assert.IsFalse(AnalyticsResultsHelper.VerifyEventName());
            Assert.IsFalse(AnalyticsResultsHelper.VerifyNumProperties(numProperties));
            Assert.IsFalse(AnalyticsResultsHelper.SentEventWasCalled);
            Assert.IsFalse(AnalyticsResultsHelper.FailedToSendEventWasCalled);
        }

        [Test]
        public void InvalidOperation()
        {
            /* Crash the application with an invalid operation exception and then restart */
            app.Tap(TestStrings.GoToCrashesPageButton);
            app.Tap(TestStrings.CrashWithInvalidOperationButton);
            TestSuccessfulCrash();
            LastSessionErrorReportHelper.app = app;
            Assert.IsTrue(LastSessionErrorReportHelper.VerifyExceptionType(typeof(InvalidOperationException).Name));
            Assert.IsTrue(LastSessionErrorReportHelper.VerifyExceptionMessage(TestStrings.InvalidOperationExceptionMessage));
        }

        [Test]
        public void AggregateException()
        {
            /* Crash the application with an aggregate exception and then restart */
            app.Tap(TestStrings.GoToCrashesPageButton);
            app.Tap(TestStrings.CrashWithAggregateExceptionButton);
            TestSuccessfulCrash();
            LastSessionErrorReportHelper.app = app;
            Assert.IsTrue(LastSessionErrorReportHelper.VerifyExceptionType(typeof(AggregateException).Name));
            Assert.IsTrue(LastSessionErrorReportHelper.VerifyExceptionMessage(TestStrings.AggregateExceptionMessage));
        }

        [Test]
        public void DivideByZero()
        {
            /* Crash the application with a divide by zero exception and then restart */
            app.Tap(TestStrings.GoToCrashesPageButton);
            app.Tap(TestStrings.DivideByZeroCrashButton);
            TestSuccessfulCrash();
            LastSessionErrorReportHelper.app = app;
            Assert.IsTrue(LastSessionErrorReportHelper.VerifyExceptionType(typeof(DivideByZeroException).Name));
            Assert.IsTrue(LastSessionErrorReportHelper.VerifyExceptionMessage(TestStrings.DivideByZeroExceptionMessage));
        }

        [Test]
        public void AsyncTaskException()
        {
            /* Crash the application inside an asynchronous task and then restart */
            app.Tap(TestStrings.GoToCrashesPageButton);
            app.Tap(TestStrings.CrashInsideAsyncTaskButton);
            TestSuccessfulCrash();
            LastSessionErrorReportHelper.app = app;
            Assert.IsTrue(LastSessionErrorReportHelper.VerifyExceptionType(typeof(IOException).Name));
            Assert.IsTrue(LastSessionErrorReportHelper.VerifyExceptionMessage(TestStrings.IOExceptionMessage));
        }

        /* Helper functions */

        /* Verify that a crash has been triggered and handled correctly */
        public void TestSuccessfulCrash()
        {
            app = AppInitializer.StartAppNoClear(platform);
            app.Tap(TestStrings.GoToCrashResultsPageButton);

            /* Ensure that the callbacks were properly called */
            CrashResultsHelper.app = app;
            Assert.IsTrue(CrashResultsHelper.SendingErrorReportWasCalled);
            Assert.IsTrue(CrashResultsHelper.SentErrorReportWasCalled);
            Assert.IsFalse(CrashResultsHelper.FailedToSendErrorReportWasCalled);
            Assert.IsTrue(CrashResultsHelper.ShouldProcessErrorReportWasCalled);
            Assert.IsTrue(CrashResultsHelper.ShouldAwaitUserConfirmationWasCalled);
            Assert.IsTrue(CrashResultsHelper.GetErrorAttachmentWasCalled);

            /* Verify that the error report is correct */
            LastSessionErrorReportHelper.app = app;
            app.Tap(TestStrings.ViewLastSessionErrorReportButton);
            Assert.IsTrue(LastSessionErrorReportHelper.DeviceReported);
            Assert.IsTrue(LastSessionErrorReportHelper.HasId);
            Assert.IsTrue(LastSessionErrorReportHelper.HasAppErrorTime);
            Assert.IsTrue(LastSessionErrorReportHelper.HasAppStartTime);

            if (platform == Platform.Android)
            {
                Assert.IsTrue(LastSessionErrorReportHelper.HasAndroidDetails);
                Assert.IsFalse(LastSessionErrorReportHelper.HasIosDetails);
            }
            else if (platform == Platform.iOS)
            {
                Assert.IsTrue(LastSessionErrorReportHelper.HasIosDetails);
                Assert.IsFalse(LastSessionErrorReportHelper.HasAndroidDetails);
            }
        }

        /* Send an event with numProperties properties */
        public void SendEvent(int numProperties)
        {
            /* Should be on Analytics page already */
            for (int i = 0; i < numProperties; ++i)
            {
                app.Tap(TestStrings.AddPropertyButton);
            }
            app.Tap(TestStrings.SendEventButton);
        }
    }
}
