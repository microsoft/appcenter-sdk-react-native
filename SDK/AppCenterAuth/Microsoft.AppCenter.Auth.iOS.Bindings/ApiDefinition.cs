using System;

using ObjCRuntime;
using Foundation;
using UIKit;

namespace NativeLibrary
{
    // @interface MSIdentity : MSService
    [BaseType(typeof(NSObject))]
    interface MSIdentity
    {
        // +(void)setEnabled:(BOOL)isEnabled;
        [Static]
        [Export("setEnabled:")]
        void SetEnabled(bool isEnabled);

        // +(BOOL)isEnabled;
        [Static]
        [Export("isEnabled")]
        bool IsEnabled();

        // + (void) signInWithCompletionHandler:(MSSignInCompletionHandler _Nullable) completionHandler;
        [Static]
        [Export("signInWithCompletionHandler:")]
        void SignIn(MSSignInCompletionHandler completionHandler);

        // + (void) signOut;
        [Static]
        [Export("signOut")]
        void SignOut();
    }

    // typedef void (^MSSignInCompletionHandler)(MSUserInformation* _Nullable userInformation, NSError * _Nullable error);
    delegate void MSSignInCompletionHandler([NullAllowed] MSUserInformation userInformation, [NullAllowed] NSError error);

    [BaseType(typeof(NSObject))]
    interface MSUserInformation
    {
        // @property(nonatomic, copy) NSString *accountId;
        [Export("accountId")]
        string AccountId { get; set; }
    }
}
