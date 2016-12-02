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
        public void UnitTestA()
        {
            Process.GetCurrentProcess().BeginOutputReadLine();
            app.Tap(c => c.Marked("UnitTestACell"));
            app = AppInitializer.StartApp(platform);
            app.Query(c => c.Marked("UnitTestACell"));
            string output = Process.GetCurrentProcess().StandardOutput.ToString();
            Debug.WriteLine(output);
        }
    }
}
