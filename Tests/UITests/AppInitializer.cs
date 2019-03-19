// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Xamarin.UITest;
using Xamarin.UITest.Configuration;

namespace Contoso.Forms.Test.UITests
{
    public class AppInitializer
    {
        public static IApp StartApp(Platform platform)
        {
            if (platform == Platform.Android)
            {
                return ConfigureApp.Android.EnableLocalScreenshots().StartApp();
            }

            return ConfigureApp.iOS.EnableLocalScreenshots().StartApp();
        }

        public static IApp StartAppNoClear(Platform platform)
        {
            if (platform == Platform.Android)
            {
                return ConfigureApp.Android.EnableLocalScreenshots().StartApp(AppDataMode.DoNotClear);
            }
            return ConfigureApp.iOS.EnableLocalScreenshots().StartApp(AppDataMode.DoNotClear);
        }
    }
}
