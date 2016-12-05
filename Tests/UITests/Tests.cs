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
        public void ToggleEnabledStates()
        {
            app.WaitForElement(c => c.Marked("ToggleMobileCenterEnabledButton"));
            app.WaitForElement(c => c.Marked("ToggleAnalyticsEnabledButton"));
            app.WaitForElement(c => c.Marked("ToggleCrashesEnabledButton"));

            app.Tap(c => c.Marked("ToggleMobileCenterEnabledButton"));
            app.Tap(c => c.Marked("ToggleAnalyticsEnabledButton"));
            app.Tap(c => c.Marked("ToggleCrashesEnabledButton"));
            //TODO some kind of verification (and more complicated sequence of events)
        }

        [Test]
        public void SendEvents()
        {
            app.WaitForElement(c => c.Marked("SendEventButton"));
            app.WaitForElement(c => c.Marked("AddPropertyButton"));

            app.Tap(c => c.Marked("SendEventButton"));
            app.Tap(c => c.Marked("AddPropertyButton"));
            app.Tap(c => c.Marked("AddPropertyButton"));
            app.Tap(c => c.Marked("AddPropertyButton"));
            app.Tap(c => c.Marked("AddPropertyButton"));
            app.Tap(c => c.Marked("AddPropertyButton"));
            app.Tap(c => c.Marked("SendEventButton"));
            //TODO some kind of verification
        }

        [Test]
        public void DivideByZero()
        {
            app.WaitForElement(c => c.Marked("DivideByZeroCrashButton"));

            app.Tap(c => c.Marked("DivideByZeroCrashButton"));
            app = AppInitializer.StartApp(platform);
            //TODO some kind of verification
        }
    }
}
