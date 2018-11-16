const rnpmlink = require('appcenter-link-scripts');
const inquirer = require('inquirer');

// Configure Android first.
let promise = null;
if (rnpmlink.android.checkIfAndroidDirectoryExists()) {
    console.log('Configuring AppCenter Analytics for Android');
    promise = rnpmlink.android.initAppCenterConfig()
        .then(() =>
            inquirer.prompt([{
                type: 'list',
                name: 'whenToEnableAnalytics',
                message: 'For the Android app, should user tracking be enabled automatically?',
                choices: [
                    {
                        name: 'Enable Automatically',
                        value: 'ALWAYS_SEND'
                    },
                    {
                        name: 'Enable in JavaScript',
                        value: 'ENABLE_IN_JS'
                    }]
            }])
        ).then((androidAnswer) => {
            rnpmlink.android.patchStrings('appCenterAnalytics_whenToEnableAnalytics',
                androidAnswer.whenToEnableAnalytics);
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
        .then(() =>
            inquirer.prompt([{
                type: 'list',
                name: 'whenToEnableAnalytics',
                message: 'For the iOS app, should user tracking be enabled automatically?',
                choices: [
                    {
                        name: 'Enable Automatically',
                        value: 'ALWAYS_SEND'
                    },
                    {
                        name: 'Enable in JavaScript',
                        value: 'ENABLE_IN_JS'
                    }]
            }])
        )
        .then((iosAnswer) => {
            const code = iosAnswer.whenToEnableAnalytics === 'ALWAYS_SEND' ?
                '  [AppCenterReactNativeAnalytics registerWithInitiallyEnabled:true];  // Initialize AppCenter analytics' :
                '  [AppCenterReactNativeAnalytics registerWithInitiallyEnabled:false];  // Initialize AppCenter analytics';
            return rnpmlink.ios.initInAppDelegate('#import <AppCenterReactNativeAnalytics/AppCenterReactNativeAnalytics.h>', code, /.*\[AppCenterReactNativeAnalytics register.*/g);
        })
        .then((file) => {
            console.log(`Added code to initialize iOS Analytics SDK in ${file}`);
            return rnpmlink.ios.addPodDeps(
                [
                    { pod: 'AppCenter/Analytics', version: '1.11.0' },
                    { pod: 'AppCenterReactNativeShared', version: '1.10.0' } // in case people don't link appcenter (core)
                ],
                { platform: 'ios', version: '9.0' }
            );
        })
        .catch((e) => {
            console.error(`Could not configure AppCenter Analytics for iOS. Error Reason - ${e.message}`);
            return Promise.resolve();
        });
}
return promise;
