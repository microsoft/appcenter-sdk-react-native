// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

const rnpmlink = require('appcenter-link-scripts');

// Configure Android first.
let promise;
if (rnpmlink.android.checkIfAndroidDirectoryExists()) {
    console.log('Configuring AppCenter Analytics for Android');
    promise = rnpmlink.android.initAppCenterConfig()
        .then(() => {
            rnpmlink.android.patchStrings('appCenterAnalytics_whenToEnableAnalytics', 'ALWAYS_SEND');
            rnpmlink.android.removeAndroidDuplicateLinks();
        }).catch((e) => {
            console.error(`Could not configure AppCenter Analytics for Android. Error Reason - ${e.message}`);
            return Promise.resolve();
        });
} else {
    promise = Promise.resolve();
}

// Then iOS even if Android failed.
if (rnpmlink.ios.checkIfAppDelegateExists()) {
    promise
        .then(() => {
            console.log('Configuring AppCenter Analytics for iOS');
            return rnpmlink.ios.initAppCenterConfig();
        })
        .then(() => {
            const code = '[AppCenterReactNativeAnalytics registerWithInitiallyEnabled:true];  // Initialize AppCenter analytics';
            return rnpmlink.ios.initInAppDelegate('#import <AppCenterReactNativeAnalytics/AppCenterReactNativeAnalytics.h>', code, /.*\[AppCenterReactNativeAnalytics register.*/g);
        })
        .then((file) => {
            console.log(`Added code to initialize iOS Analytics SDK in ${file}`);
            return rnpmlink.ios.addPodDeps(
                [
                    { pod: 'AppCenter/Analytics', version: '2.5.0' },
                    { pod: 'AppCenterReactNativeShared', version: '2.5.0' } // in case people don't link appcenter (core)
                ],
                { platform: 'ios', version: '9.0' }
            );
        })
        .catch((e) => {
            console.error(`Could not configure AppCenter Analytics for iOS. Error Reason - ${e.message}`);
            return Promise.resolve();
        });
}
