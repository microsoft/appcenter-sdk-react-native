// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

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
                return Guid.Parse(guidString);
            }
        }

        static string WaitForLabelToSayAnything(string labelName)
        {
            string text = "";
            app.WaitFor(() =>
            {
                AppResult[] results = app.Query(labelName);
                if (results.Length < 1)
                    return false;
                AppResult label = results[0];
                text = string.IsNullOrEmpty(label.Text) ? text : label.Text;
                return !string.IsNullOrEmpty(label.Text);
            }, timeout: TimeSpan.FromSeconds(5));
            return text;
        }
    }
}
