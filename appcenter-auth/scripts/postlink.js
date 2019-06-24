// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

const rnpmlink = require('appcenter-link-scripts');

// Configure Android first.
let promise = null;
if (rnpmlink.android.checkIfAndroidDirectoryExists()) {
    console.log('Configuring AppCenter Auth for Android');
    promise = rnpmlink.android.initAppCenterConfig()
        .then(() => {
            rnpmlink.android.removeAndroidDuplicateLinks();
        }).catch((e) => {
            console.error(`Could not configure AppCenter Auth for Android. Error Reason - ${e.message}`);
            return Promise.resolve();
        });
} else {
    promise = Promise.resolve();
}

// Then iOS even if Android failed.
if (rnpmlink.ios.checkIfAppDelegateExists()) {
    promise
        .then(() => {
            console.log('Configuring AppCenter Auth for iOS');
            return rnpmlink.ios.initAppCenterConfig();
        })
        .then(() => {
            const code = '[AppCenterReactNativeAuth register];  // Initialize AppCenter auth';
            return rnpmlink.ios.initInAppDelegate('#import <AppCenterReactNativeAuth/AppCenterReactNativeAuth.h>', code, /.*\[AppCenterReactNativeAuth register.*/g);
        })
        .then((file) => {
            console.log(`Added code to initialize iOS Auth SDK in ${file}`);
            return rnpmlink.ios.addPodDeps(
                [
                    { pod: 'AppCenter/Auth', version: '2.1.0' },
                    { pod: 'AppCenterReactNativeShared', version: '2.1.0' } // in case people don't link appcenter (core)
                ],
                { platform: 'ios', version: '9.0' }
            );
        })
        .catch((e) => {
            console.error(`Could not configure AppCenter Auth for iOS. Error Reason - ${e.message}`);
            promise = Promise.resolve();
        });
}

return promise;
