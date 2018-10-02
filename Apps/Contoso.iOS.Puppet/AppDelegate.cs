using System;
using Foundation;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Microsoft.AppCenter.Distribute;
using Microsoft.AppCenter.Push;
using UIKit;

namespace Contoso.iOS.Puppet
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the
    // User Interface of the application, as well as listening (and optionally responding) to application events from iOS.
    [Register("AppDelegate")]
    public class AppDelegate : UIApplicationDelegate
    {
        // class-level declarations

        public override UIWindow Window
        {
            get;
            set;
        }

        public override bool FinishedLaunching(UIApplication application, NSDictionary launchOptions)
        {
            // Override point for customization after application launch.
            // If not required for your application you can safely delete this method

            AppCenter.LogLevel = LogLevel.Verbose;
            AppCenter.SetLogUrl("https://in-integration.dev.avalanch.es");
            Distribute.SetInstallUrl("http://install.appcenter-int.trafficmanager.net");
            Distribute.SetApiUrl("https://appcenter-int.trafficmanager.net/api/v0.1");
            Distribute.DontCheckForUpdatesInDebug();
            AppCenter.Start("e94aaff4-e80d-4fee-9a5f-a84eb6e688fc", typeof(Analytics), typeof(Crashes), typeof(Distribute), typeof(Push));
            try
            {
                ThrowAnException();
            }
            catch (Exception e)
            {
                AppCenterLog.Verbose("THETAG", "THEMESSAGE", e);
            }

            Analytics.SetEnabledAsync(true);
            System.Diagnostics.Debug.WriteLine("ANALYTICS: " + Analytics.IsEnabledAsync().Result);
            return true;
        }

        private void ThrowAnException()
        {
            throw new Exception();
        }

        public override void OnResignActivation(UIApplication application)
        {
            // Invoked when the application is about to move from active to inactive state.
            // This can occur for certain types of temporary interruptions (such as an incoming phone call or SMS message) 
            // or when the user quits the application and it begins the transition to the background state.
            // Games should use this method to pause the game.
        }

        public override void DidEnterBackground(UIApplication application)
        {
            // Use this method to release shared resources, save user data, invalidate timers and store the application state.
            // If your application supports background exection this method is called instead of WillTerminate when the user quits.
        }

        public override void WillEnterForeground(UIApplication application)
        {
            // Called as part of the transiton from background to active state.
            // Here you can undo many of the changes made on entering the background.
        }

        public override void OnActivated(UIApplication application)
        {
            // Restart any tasks that were paused (or not yet started) while the application was inactive. 
            // If the application was previously in the background, optionally refresh the user interface.
        }

        public override void WillTerminate(UIApplication application)
        {
            // Called when the application is about to terminate. Save data, if needed. See also DidEnterBackground.
        }
    }
}

