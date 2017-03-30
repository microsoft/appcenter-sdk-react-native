using System;
using Xamarin.UITest;
using Xamarin.UITest.Queries;

namespace Contoso.Forms.Test.UITests
{
    public static class CorePageHelper
    {
        public static IApp app;

        public static Guid? InstallId
        {
            get
            {
                string guidString = WaitForLabelToSayAnything(TestStrings.InstallIdLabel);
                try
                {
                    return Guid.Parse(guidString);
                }
                catch (FormatException)
                {
                    return null;
                }
            }
        }

        static string WaitForLabelToSayAnything(string labelName)
        {
            try
            {
                string text = "";
                app.WaitFor(() =>
                {
                    AppResult[] results = app.Query(labelName);
                    if (results.Length < 1)
                        return false;
                    AppResult label = results[0];
                    text = label.Text != "" ? label.Text : text;
                    return label.Text != "";
                }, timeout: TimeSpan.FromSeconds(5));
                return text;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
