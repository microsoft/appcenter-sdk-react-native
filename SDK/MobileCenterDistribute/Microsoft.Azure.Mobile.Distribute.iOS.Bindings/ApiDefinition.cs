using Foundation;

namespace Microsoft.Azure.Mobile.Distribute.iOS.Bindings
{
    // @interface MSDistribute : MSService
    [BaseType(typeof(NSObject))]
    interface MSDistribute
    {
        // +(void)setEnabled:(BOOL)isEnabled;
        [Static]
        [Export("setEnabled:")]
        void SetEnabled(bool isEnabled);

        // +(BOOL)isEnabled;
        [Static]
        [Export("isEnabled")]
        bool IsEnabled();

        // + (void)setApiUrl:(NSString *)apiUrl;
        [Static]
        [Export("setApiUrl:")]
        void SetApiUrl(string apiUrl);

        // + (void)setInstallUrl:(NSString *)installUrl;
        [Static]
        [Export("setInstallUrl:")]
        void SetInstallUrl(string installUrl);

        // + (void)openUrl:(NSURL *)url;
        [Static]
        [Export("openUrl:")]
        void OpenUrl(NSUrl url);
    }
}
