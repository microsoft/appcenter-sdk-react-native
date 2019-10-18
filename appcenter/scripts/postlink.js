// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

const rnpmlink = require('appcenter-link-scripts');

// Configure Android first.
let promise = null;
if (rnpmlink.android.checkIfAndroidDirectoryExists()) {
    console.log('Configuring AppCenter Analytics for Android');
    promise = rnpmlink.android.initAppCenterConfig()
        .then(() => {
            rnpmlink.android.removeAndroidDuplicateLinks();
        }).catch((e) => {
            console.error(`Could not configure AppCenter for Android. Error Reason - ${e.message}`);
            return Promise.resolve();
        });
} else {
    promise = Promise.resolve();
}

// Then iOS even if Android failed.
if (rnpmlink.ios.checkIfAppDelegateExists()) {
    promise
        .then(() => {
            console.log('Configuring AppCenter for iOS');
            return rnpmlink.ios.initAppCenterConfig();
        })
        .then(() => {
            const code = '[AppCenterReactNative register];  // Initialize AppCenter';
            return rnpmlink.ios.initInAppDelegate('#import <AppCenterReactNative/AppCenterReactNative.h>', code);
        })
        .then((file) => {
            console.log(`Added code to initialize iOS AppCenter SDK in ${file}`);
            return rnpmlink.ios.addPodDeps(
                [{ pod: 'AppCenterReactNativeShared', version: '2.5.0' }],
                { platform: 'ios', version: '9.0' }
            );
        })
        .catch((e) => {
            console.error(`Could not configure AppCenter for iOS. Error Reason - ${e.message}`);
            return Promise.resolve();
        });
}
return promise;
