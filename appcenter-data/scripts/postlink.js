// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

const rnpmlink = require('appcenter-link-scripts');

// Configure Android first.
let promise = null;
if (rnpmlink.android.checkIfAndroidDirectoryExists()) {
    console.log('Configuring AppCenter Data for Android');
    promise = rnpmlink.android.initAppCenterConfig()
        .then(() => {
            rnpmlink.android.removeAndroidDuplicateLinks();
        }).catch((e) => {
            console.error(`Could not configure AppCenter Data for Android. Error Reason - ${e.message}`);
            return Promise.resolve();
        });
} else {
    promise = Promise.resolve();
}

// Then iOS even if Android failed.
if (rnpmlink.ios.checkIfAppDelegateExists()) {
    promise
        .then(() => {
            console.log('Configuring AppCenter Data for iOS');
            return rnpmlink.ios.initAppCenterConfig();
        })
        .then(() => {
            const code = '[AppCenterReactNativeData register];  // Initialize AppCenter data';
            return rnpmlink.ios.initInAppDelegate('#import <AppCenterReactNativeData/AppCenterReactNativeData.h>', code, /.*\[AppCenterReactNativeData register.*/g);
        })
        .then((file) => {
            console.log(`Added code to initialize iOS Data SDK in ${file}`);
            return rnpmlink.ios.addPodDeps(
                [
                    { pod: 'AppCenter/Data', version: '2.2.0' },
                    { pod: 'AppCenterReactNativeShared', version: '2.2.0' } // in case people don't link appcenter (core)
                ],
                { platform: 'ios', version: '9.0' }
            );
        })
        .catch((e) => {
            console.error(`Could not configure AppCenter Data for iOS. Error Reason - ${e.message}`);
            promise = Promise.resolve();
        });
}
return promise;
