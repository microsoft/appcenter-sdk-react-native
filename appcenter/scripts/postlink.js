const rnpmlink = require('appcenter-link-scripts');

// Configure Android first.
let promise = null;
if (rnpmlink.android.checkIfAndroidDirectoryExists()) {
    console.log('Configuring AppCenter Analytics for Android');
    promise = rnpmlink.android.initAppCenterConfig()
        .then(() => {
            rnpmlink.android.removeAndroidDuplicateLinks();
        }).catch((e) => {
            console.log(`Could not configure AppCenter for Android. Error Reason - ${e.message}`);
            return Promise.resolve();
        });
} else {
    promise = Promise.resolve();
}

// Then iOS even if Android failed.
if (rnpmlink.ios.checkIfAppDelegateExists()) {
    promise
        .then(() => rnpmlink.ios.initAppCenterConfig().catch((e) => {
            console.log(`Could not create or update AppCenter config file (AppCenter-Config.plist). Error Reason - ${e.message}`);
            return Promise.reject();
        }))
        .then(() => {
            const code = '  [AppCenterReactNative register];  // Initialize AppCenter ';
            return rnpmlink.ios.initInAppDelegate('#import <AppCenterReactNative/AppCenterReactNative.h>', code)
                .catch((e) => {
                    console.log(`Could not initialize AppCenter in AppDelegate. Error Reason - ${e.message}`);
                    return Promise.reject();
                });
        })
        .then((file) => {
            console.log(`Added code to initialize iOS AppCenter SDK in ${file}`);
            return rnpmlink.ios.addPodDeps(
                [{ pod: 'AppCenterReactNativeShared', version: '1.3.0' }],
                { platform: 'ios', version: '9.0' }
            ).catch((e) => {
                console.log(`
                    Could not install dependencies using CocoaPods.
                    Please refer to the documentation to install dependencies manually.

                    Error Reason - ${e.message}
                `);
                return Promise.reject();
            });
        });
}
return promise;
