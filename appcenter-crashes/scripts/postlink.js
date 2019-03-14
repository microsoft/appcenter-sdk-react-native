const rnpmlink = require('appcenter-link-scripts');

// Configure Android first.
let promise = null;
if (rnpmlink.android.checkIfAndroidDirectoryExists()) {
    console.log('Configuring AppCenter Crashes for Android');
    try {
        rnpmlink.android.removeAndroidDuplicateLinks()
        promise = Promise.resolve();
    } catch (e) {
        console.error(`Could not configure AppCenter Crashes for Android. Error Reason - ${e.message}`);
        promise = Promise.resolve();
    }
} else {
    promise = Promise.resolve();
}

// Then iOS even if Android failed.
if (rnpmlink.ios.checkIfAppDelegateExists()) {
    promise
        .then(() => {
            const code = 
                '  [AppCenterReactNativeCrashes register];  // Initialize AppCenter crashes';
            return rnpmlink.ios.initInAppDelegate('#import <AppCenterReactNativeCrashes/AppCenterReactNativeCrashes.h>', code, /.*\[AppCenterReactNativeCrashes register.*/g);
        })
        .then((file) => {
            console.log(`Added code to initialize iOS Crashes SDK in ${file}`);
            return rnpmlink.ios.addPodDeps(
                [
                    { pod: 'AppCenter/Crashes', version: '1.13.2' },
                    { pod: 'AppCenterReactNativeShared', version: '1.12.2' } // in case people don't link appcenter (core)
                ],
                { platform: 'ios', version: '9.0' }
            );
        })
        .catch((e) => {
            console.error(`Could not configure AppCenter Crashes for iOS. Error Reason - ${e.message}`);
            return Promise.resolve();
        });
}
return promise;
