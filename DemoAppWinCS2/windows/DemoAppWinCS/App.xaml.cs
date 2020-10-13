using Microsoft.ReactNative;
using Windows.ApplicationModel.Activation;
using Microsoft.AppCenter.Crashes.ReactNative;


namespace DemoAppWinCS
{
	sealed partial class App : ReactApplication
    {
        public App()
        {
            MainComponentName = "DemoAppWinCS";

#if BUNDLE
            JavaScriptBundleFile = "index.windows";
            InstanceSettings.UseWebDebugger = false;
            InstanceSettings.UseFastRefresh = false;
#else
            JavaScriptMainModuleName = "index";
            InstanceSettings.UseWebDebugger = true;
            InstanceSettings.UseFastRefresh = true;
#endif

#if DEBUG
            InstanceSettings.EnableDeveloperMenu = true;
#else
            InstanceSettings.EnableDeveloperMenu = false;
#endif

            InstanceSettings.RedBoxHandler = new RedBoxHandler(Host);
			
            Microsoft.ReactNative.Managed.AutolinkedNativeModules.RegisterAutolinkedNativeModulePackages(PackageProviders); // Includes any autolinked modules
            
            PackageProviders.Add(new Microsoft.ReactNative.Managed.ReactPackageProvider()); // Includes any modules in this project
            PackageProviders.Add(new Microsoft.AppCenter.ReactNative.ReactPackageProvider());
            PackageProviders.Add(new Microsoft.AppCenter.Analytics.ReactNative.ReactPackageProvider());
            PackageProviders.Add(new Microsoft.AppCenter.Crashes.ReactNative.ReactPackageProvider());
            InitializeComponent();
        }
        /*
        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
            base.OnLaunched(e);
            AppCenterReactNativeShared.AppCenterReactNativeShared.configureAppCenter();
        }
        */
    }
}
