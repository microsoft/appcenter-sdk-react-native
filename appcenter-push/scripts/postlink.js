const rnpmlink = require('appcenter-link-scripts');

// Configure Android first.
let promise = null;
if (rnpmlink.android.checkIfAndroidDirectoryExists()) {
    console.log('Configuring AppCenter Analytics for Android');
    promise = rnpmlink.android.initAppCenterConfig()
        .then(() => {
            rnpmlink.android.removeAndroidDuplicateLinks();
        }).catch((e) => {
            console.error(`Could not configure AppCenter Push for Android. Error Reason - ${e.message}`);
            return Promise.resolve();
        });
} else {
    promise = Promise.resolve();
}

// Then iOS even if Android failed.
if (rnpmlink.ios.checkIfAppDelegateExists()) {
    promise
        .then(() => {
            console.log('Configuring AppCenter Push for iOS');
            return rnpmlink.ios.initAppCenterConfig();
        })
        .then(() => {
            const code = '  [AppCenterReactNativePush register];  // Initialize AppCenter push';
            return rnpmlink.ios.initInAppDelegate('#import <AppCenterReactNativePush/AppCenterReactNativePush.h>', code);
        })
        .then((file) => {
            console.log(`Added code to initialize iOS Push SDK in ${file}`);
            return rnpmlink.ios.addPodDeps(
                [
                    { pod: 'AppCenter/Push', version: '1.5.0' },
                    { pod: 'AppCenterReactNativeShared', version: '1.4.0' } // in case people don't link appcenter (core)
                ],
                { platform: 'ios', version: '9.0' }
            );
        })
        .catch((e) => {
            console.error(`Could not configure AppCenter Push for iOS. Error Reason - ${e.message}`);
            return Promise.resolve();
        });
}
return promise;
