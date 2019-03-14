const rnpmlink = require('appcenter-link-scripts');

// Configure Android first.
let promise = null;
if (rnpmlink.android.checkIfAndroidDirectoryExists()) {
    console.log('Configuring AppCenter Analytics for Android');
    try {
        rnpmlink.android.removeAndroidDuplicateLinks()
        promise = Promise.resolve();
    } catch (e) {
        console.error(`Could not configure AppCenter for Android. Error Reason - ${e.message}`);
        promise = Promise.resolve();
    }
} else {
    promise = Promise.resolve();
}

// Then iOS even if Android failed.
if (rnpmlink.ios.checkIfAppDelegateExists()) {
    promise
        .then(() => {
            const code = '  [AppCenterReactNative register];  // Initialize AppCenter ';
            return rnpmlink.ios.initInAppDelegate('#import <AppCenterReactNative/AppCenterReactNative.h>', code);
        })
        .then((file) => {
            console.log(`Added code to initialize iOS AppCenter SDK in ${file}`);
            return rnpmlink.ios.addPodDeps(
                [{ pod: 'AppCenterReactNativeShared', version: '1.12.2' }],
                { platform: 'ios', version: '9.0' }
            );
        })
        .catch((e) => {
            console.error(`Could not configure AppCenter for iOS. Error Reason - ${e.message}`);
            return Promise.resolve();
        });
}
return promise;
