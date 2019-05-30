// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

const rnpmlink = require('appcenter-link-scripts');

console.log('Please enter your Android and iOS app secrets below.');
console.log('For more information: https://docs.microsoft.com/en-us/appcenter/sdk/getting-started/react-native');

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
return promise;
