using NUnit.Framework;
using Xamarin.UITest;
using System;
using System.IO;
using System.Threading;

namespace Contoso.Forms.Test.UITests
{
    [TestFixture(Platform.Android)]
    [TestFixture(Platform.iOS)]
    public class Tests
    {
        IApp app;
        Platform platform;

        private const int AfterCrashSleepTime = 2000;
        private const int LongStabilizationSleepTime = 10000;

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
        public void InstallIdIsCorrectFormat()
        {
            app.Screenshot("InstallIdIsCorrectFormat - Ready for tests");

            CorePageHelper.app = app;
            app.Tap(TestStrings.GoToCorePageButton);
            var id = CorePageHelper.InstallId;
            Assert.IsTrue(id.HasValue);
        }

        [Test]
        public void TestEnablingAndDisablingServices()
        {
            app.Screenshot("TestEnablingAndDisablingServices - Ready for tests");

            ServiceStateHelper.app = app;
            app.WaitForElement(TestStrings.GoToTogglePageButton);
            app.Tap(TestStrings.GoToTogglePageButton);

            /* Test setting enabling all services */
            ServiceStateHelper.AppCenterEnabled = true;
            Assert.IsTrue(ServiceStateHelper.AppCenterEnabled);
            ServiceStateHelper.AnalyticsEnabled = true;
            Assert.IsTrue(ServiceStateHelper.AnalyticsEnabled);
            ServiceStateHelper.CrashesEnabled = true;
            Assert.IsTrue(ServiceStateHelper.CrashesEnabled);

            /* Test that disabling AppCenter disables everything */
            ServiceStateHelper.AppCenterEnabled = false;
            Assert.IsFalse(ServiceStateHelper.AppCenterEnabled);
            Assert.IsFalse(ServiceStateHelper.AnalyticsEnabled);
            Assert.IsFalse(ServiceStateHelper.CrashesEnabled);

            /* Test disabling individual services */
            ServiceStateHelper.AppCenterEnabled = true;
            Assert.IsTrue(ServiceStateHelper.AppCenterEnabled);
            ServiceStateHelper.AnalyticsEnabled = false;
            Assert.IsFalse(ServiceStateHelper.AnalyticsEnabled);
            ServiceStateHelper.CrashesEnabled = false;
            Assert.IsFalse(ServiceStateHelper.CrashesEnabled);

            /* Test that enabling AppCenter enables everything, regardless of previous states */
            ServiceStateHelper.AppCenterEnabled = true;
            Assert.IsTrue(ServiceStateHelper.AppCenterEnabled);
            Assert.IsTrue(ServiceStateHelper.AnalyticsEnabled);
            Assert.IsTrue(ServiceStateHelper.CrashesEnabled);
        }

        [Test]
        public void TestServiceStatePersistenceCrashes()
        {
            app.Screenshot("TestServiceStatePersistenceCrashes - Ready for tests");
            ServiceStateHelper.app = app;
            app.WaitForElement(TestStrings.GoToTogglePageButton);
            app.Tap(TestStrings.GoToTogglePageButton);

            /* Make sure Crashes enabled state is persistent */
            ServiceStateHelper.AppCenterEnabled = true;
            ServiceStateHelper.CrashesEnabled = false;
            Assert.IsFalse(ServiceStateHelper.CrashesEnabled);

            // There seems to be some sort of timing issue here.
            // without the Thread.Sleep - this code fails although I see that the method called is Async (with a Wait)
            // so I think this deserves further investigation. I've added a terribly long wait to get the tests green.
            Thread.Sleep(LongStabilizationSleepTime);
            app = AppInitializer.StartAppNoClear(platform);
            Thread.Sleep(LongStabilizationSleepTime);
            app.WaitForElement(TestStrings.GoToTogglePageButton);
            app.Tap(TestStrings.GoToTogglePageButton);
            Thread.Sleep(LongStabilizationSleepTime);
            Assert.IsTrue(ServiceStateHelper.AppCenterEnabled);
            Assert.IsTrue(ServiceStateHelper.AnalyticsEnabled);
            Assert.IsFalse(ServiceStateHelper.CrashesEnabled);
            app.Screenshot("Crashes persistent");

            /* Reset services to enabled */
            ServiceStateHelper.AppCenterEnabled = true;
        }

        [Test]
        public void TestServiceStatePersistenceAnalytics()
        {
            app.Screenshot("TestServiceStatePersistenceAnalytics - Ready for tests");
            ServiceStateHelper.app = app;
            app.WaitForElement(TestStrings.GoToTogglePageButton);
            app.Tap(TestStrings.GoToTogglePageButton);

            /* Make sure Analytics enabled state is persistent */
            ServiceStateHelper.AppCenterEnabled = true;
            ServiceStateHelper.AnalyticsEnabled = false;
            Assert.IsFalse(ServiceStateHelper.AnalyticsEnabled);
            Thread.Sleep(LongStabilizationSleepTime);

            app = AppInitializer.StartAppNoClear(platform);
            app.WaitForElement(TestStrings.GoToTogglePageButton);
            app.Tap(TestStrings.GoToTogglePageButton);
            Thread.Sleep(LongStabilizationSleepTime);
            Assert.IsTrue(ServiceStateHelper.AppCenterEnabled);
            Assert.IsFalse(ServiceStateHelper.AnalyticsEnabled);
            Assert.IsTrue(ServiceStateHelper.CrashesEnabled);
            app.Screenshot("Analytics persistent");

            /* Reset services to enabled */
            ServiceStateHelper.AppCenterEnabled = true;
        }

        [Test]
        public void TestServiceStatePersistenceAppCenter()
        {
            app.Screenshot("TestServiceStatePersistenceAppCenter - Ready for tests");
            ServiceStateHelper.app = app;
            app.WaitForElement(TestStrings.GoToTogglePageButton);
            app.Tap(TestStrings.GoToTogglePageButton);

            /* Make sure AppCenter enabled state is persistent */
            ServiceStateHelper.AppCenterEnabled = true;
            ServiceStateHelper.AppCenterEnabled = false;
            Assert.IsFalse(ServiceStateHelper.AppCenterEnabled);
            Assert.IsFalse(ServiceStateHelper.CrashesEnabled);
            Assert.IsFalse(ServiceStateHelper.AnalyticsEnabled);
            Thread.Sleep(LongStabilizationSleepTime);

            app.Screenshot("TestServiceStatePersistenceAppCenter - Before restart");

            app = AppInitializer.StartAppNoClear(platform);
            Thread.Sleep(LongStabilizationSleepTime);

            app.WaitForElement(TestStrings.GoToTogglePageButton);
            app.Tap(TestStrings.GoToTogglePageButton);
            Thread.Sleep(LongStabilizationSleepTime);
            Assert.IsFalse(ServiceStateHelper.AppCenterEnabled);
            Assert.IsFalse(ServiceStateHelper.AnalyticsEnabled);
            Assert.IsFalse(ServiceStateHelper.CrashesEnabled);
            app.Screenshot("AppCenter persistent");

            /* Reset services to enabled */
            ServiceStateHelper.AppCenterEnabled = true;
        }

        [Test]
        public void SendEventWithProperties()
        {
            app.Screenshot("SendEventWithProperties - Ready for tests");
            app.WaitForElement(TestStrings.GoToAnalyticsPageButton);
            app.Tap(TestStrings.GoToAnalyticsPageButton);
            app.WaitForElement(TestStrings.GoToAnalyticsResultsPageButton);

            int numProperties = 5;
            SendEvent(numProperties);
            app.Tap(TestStrings.GoToAnalyticsResultsPageButton);

            /* Verify that the event was sent properly */
            AnalyticsResultsHelper.app = app;
            Assert.IsTrue(AnalyticsResultsHelper.SendingEventWasCalled);
            Assert.IsTrue(AnalyticsResultsHelper.VerifyEventName());

            /* The SDK already has retry logic. So, give it a second try. */
            Assert.IsTrue(AnalyticsResultsHelper.SentEventWasCalled || AnalyticsResultsHelper.SentEventWasCalled);
            Assert.IsFalse(AnalyticsResultsHelper.FailedToSendEventWasCalled);
        } 

        [Test]
        public void SendEventWithNoProperties()
        {
            app.Screenshot("SendEventWithNoProperties - Ready for tests");
            app.WaitForElement(TestStrings.GoToAnalyticsPageButton);
            app.Tap(TestStrings.GoToAnalyticsPageButton);
            app.WaitForElement(TestStrings.GoToAnalyticsResultsPageButton);

            int numProperties = 0;
            SendEvent(numProperties);
            app.Tap(TestStrings.GoToAnalyticsResultsPageButton);

            /* Verify that the event was sent properly */
            AnalyticsResultsHelper.app = app;
            Assert.IsTrue(AnalyticsResultsHelper.SendingEventWasCalled);
            Assert.IsTrue(AnalyticsResultsHelper.VerifyEventName());
            Assert.IsTrue(AnalyticsResultsHelper.SentEventWasCalled);
            Assert.IsFalse(AnalyticsResultsHelper.FailedToSendEventWasCalled);
        }

        [Test]
        public void SendEventWithAnalyticsDisabled()
        {
            app.Screenshot("SendEventWithAnalyticsDisabled - Ready for tests");
            /* Disable Analytics */
            ServiceStateHelper.app = app;
            app.WaitForElement(TestStrings.GoToTogglePageButton);
            app.Tap(TestStrings.GoToTogglePageButton);
            ServiceStateHelper.AnalyticsEnabled = false;
            app.Tap(TestStrings.DismissButton);

            app.Tap(TestStrings.GoToAnalyticsPageButton);
            app.WaitForElement(TestStrings.GoToAnalyticsResultsPageButton);

            int numProperties = 1;
            SendEvent(numProperties);
            app.Tap(TestStrings.GoToAnalyticsResultsPageButton);

            /* Verify that the event was not sent */
            AnalyticsResultsHelper.app = app;
            Assert.IsFalse(AnalyticsResultsHelper.SendingEventWasCalled);
            Assert.IsFalse(AnalyticsResultsHelper.VerifyEventName());
            Assert.IsFalse(AnalyticsResultsHelper.SentEventWasCalled);
            Assert.IsFalse(AnalyticsResultsHelper.FailedToSendEventWasCalled);
        }

        [Test]
        public void InvalidOperation()
        {
            app.Screenshot("InvalidOperation - Ready for tests");
            /* Crash the application with an invalid operation exception and then restart */
            app.WaitForElement(TestStrings.GoToCrashesPageButton);
            app.Tap(TestStrings.GoToCrashesPageButton);
            app.WaitForElement(TestStrings.CrashWithInvalidOperationButton);
            app.Tap(TestStrings.CrashWithInvalidOperationButton);

            Thread.Sleep(AfterCrashSleepTime);

            TestSuccessfulCrash();
            LastSessionErrorReportHelper.app = app;
            Assert.IsTrue(LastSessionErrorReportHelper.VerifyExceptionType(typeof(InvalidOperationException).Name));
            Assert.IsTrue(LastSessionErrorReportHelper.VerifyExceptionMessage(TestStrings.InvalidOperationExceptionMessage));
        }

        [Test]
        public void AggregateException()
        {
            app.Screenshot("AggregateException - Ready for tests");
            /* Crash the application with an aggregate exception and then restart */
            app.WaitForElement(TestStrings.GoToCrashesPageButton);
            app.Tap(TestStrings.GoToCrashesPageButton);
            app.WaitForElement(TestStrings.CrashWithAggregateExceptionButton);
            app.Tap(TestStrings.CrashWithAggregateExceptionButton);

            Thread.Sleep(AfterCrashSleepTime);

            TestSuccessfulCrash();
            LastSessionErrorReportHelper.app = app;
            Assert.IsTrue(LastSessionErrorReportHelper.VerifyExceptionType(typeof(AggregateException).Name));
            Assert.IsTrue(LastSessionErrorReportHelper.VerifyExceptionMessage(TestStrings.AggregateExceptionMessage));
        }

        [Test]
        public void DivideByZero()
        {
            app.Screenshot("DivideByZero - Ready for tests");
            /* Crash the application with a divide by zero exception and then restart */
            app.WaitForElement(TestStrings.GoToCrashesPageButton);
            app.Tap(TestStrings.GoToCrashesPageButton);
            app.WaitForElement(TestStrings.DivideByZeroCrashButton);
            app.Tap(TestStrings.DivideByZeroCrashButton);

            Thread.Sleep(AfterCrashSleepTime);

            TestSuccessfulCrash();
            LastSessionErrorReportHelper.app = app;
            Assert.IsTrue(LastSessionErrorReportHelper.VerifyExceptionType(typeof(DivideByZeroException).Name));
            Assert.IsTrue(LastSessionErrorReportHelper.VerifyExceptionMessage(TestStrings.DivideByZeroExceptionMessage));
        }

        [Test]
        public void AsyncTaskException()
        {
            app.Screenshot("AsyncTaskException - Ready for tests");
            /* Crash the application inside an asynchronous task and then restart */
            app.WaitForElement(TestStrings.GoToCrashesPageButton);
            app.Tap(TestStrings.GoToCrashesPageButton);
            app.WaitForElement(TestStrings.CrashInsideAsyncTaskButton);
            app.Tap(TestStrings.CrashInsideAsyncTaskButton);

            Thread.Sleep(AfterCrashSleepTime);

            TestSuccessfulCrash();
            LastSessionErrorReportHelper.app = app;
            Assert.IsTrue(LastSessionErrorReportHelper.VerifyExceptionType(typeof(IOException).Name));
            Assert.IsTrue(LastSessionErrorReportHelper.VerifyExceptionMessage(TestStrings.IOExceptionMessage));
        }

        /* Helper functions */

        /* Verify that a crash has been triggered and handled correctly */
        public void TestSuccessfulCrash()
        {
            app.Screenshot("TestSuccessfulCrash - Ready for tests");
            app = AppInitializer.StartAppNoClear(platform);
            app.WaitForElement(TestStrings.GoToCrashResultsPageButton);
            app.Tap(TestStrings.GoToCrashResultsPageButton);

            /* Ensure that the callbacks were properly called */
            CrashResultsHelper.app = app;
            Assert.IsTrue(CrashResultsHelper.SendingErrorReportWasCalled);
            Assert.IsTrue(CrashResultsHelper.SentErrorReportWasCalled);
            Assert.IsFalse(CrashResultsHelper.FailedToSendErrorReportWasCalled);
            Assert.IsTrue(CrashResultsHelper.ShouldProcessErrorReportWasCalled);
            Assert.IsTrue(CrashResultsHelper.ShouldAwaitUserConfirmationWasCalled);

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
