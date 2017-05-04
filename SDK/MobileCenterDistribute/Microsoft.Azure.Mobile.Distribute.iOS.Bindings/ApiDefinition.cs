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

        // + (void)setDelegate:(id<MSDistributeDelegate>)delegate;
        [Static]
        [Export("setDelegate:")]
        void SetDelegate(MSDistributeDelegate distributeDelegate);

        // + (void)notifyUpdateAction:(MSUpdateAction)action;
        [Static]
        [Export("notifyUpdateAction:")]
        void NotifyUpdateAction(MSUpdateAction action);
    }

    // @protocol MSDistributeDelegate <NSObject>
    [Protocol, Model]
    [BaseType(typeof(NSObject))]
    interface MSDistributeDelegate
    {
        // @optional - (BOOL)distribute:(MSDistribute *)distribute releaseAvailableWithDetails:(MSReleaseDetails *)details;
        [Export("distribute:releaseAvailableWithDetails:")]
        bool OnReleaseAvailable(MSDistribute distribute, MSReleaseDetails details);
    }

    // @interface MSReleaseDetails : NSObject
    [BaseType(typeof(NSObject))]
    interface MSReleaseDetails
    {
        // @property(nonatomic, copy) NSNumber *id;
        [Export("id")]
        int Id { get; }

        // @property(nonatomic, copy) NSString *version;
        [Export("version")]
        string Version { get; }

        // @property(nonatomic, copy) NSString *shortVersion;
        [Export("shortVersion")]
        string ShortVersion { get; }

        // @property(nonatomic, copy) NSString *releaseNotes;
        [Export("releaseNotes")]
        string ReleaseNotes { get; }

        // @property(nonatomic) NSURL* releaseNotesUrl;
        [Export("releaseNotesUrl")]
        NSUrl ReleaseNotesUrl { get; }

        // @property(nonatomic) BOOL mandatoryUpdate;
        [Export("mandatoryUpdate")]
        bool MandatoryUpdate { get; }
    }
}