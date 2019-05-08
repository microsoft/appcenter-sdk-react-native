// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

const rnpmlink = require('appcenter-link-scripts');

// Configure Android first.
let promise;
if (rnpmlink.android.checkIfAndroidDirectoryExists()) {
    console.log('Configuring AppCenter Crashes for Android');
    promise = rnpmlink.android.initAppCenterConfig()
        .then(() => {
            rnpmlink.android.patchStrings('appCenterCrashes_whenToSendCrashes',
                'ALWAYS_SEND');
            rnpmlink.android.removeAndroidDuplicateLinks();
        }).catch((e) => {
            console.error(`Could not configure AppCenter Crashes for Android. Error Reason - ${e.message}`);
            return Promise.resolve();
        });
} else {
    promise = Promise.resolve();
}

// Then iOS even if Android failed.
if (rnpmlink.ios.checkIfAppDelegateExists()) {
    promise
        .then(() => {
            console.log('Configuring AppCenter Crashes for iOS');
            return rnpmlink.ios.initAppCenterConfig();
        })
        .then(() => {
            const code = '[AppCenterReactNativeCrashes registerWithAutomaticProcessing];  // Initialize AppCenter crashes';
            return rnpmlink.ios.initInAppDelegate('#import <AppCenterReactNativeCrashes/AppCenterReactNativeCrashes.h>', code, /.*\[AppCenterReactNativeCrashes register.*/g);
        })
        .then((file) => {
            console.log(`Added code to initialize iOS Crashes SDK in ${file}`);
            return rnpmlink.ios.addPodDeps(
                [
                    { pod: 'AppCenter/Crashes', version: '2.0.1' },
                    { pod: 'AppCenterReactNativeShared', version: '2.0.0' } // in case people don't link appcenter (core)
                ],
                { platform: 'ios', version: '9.0' }
            );
        })
        .catch((e) => {
            console.error(`Could not configure AppCenter Crashes for iOS. Error Reason - ${e.message}`);
            return Promise.resolve();
        });
}
