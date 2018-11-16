const rnpmlink = require('appcenter-link-scripts');
const inquirer = require('inquirer');

// Configure Android first.
let promise = null;
if (rnpmlink.android.checkIfAndroidDirectoryExists()) {
    console.log('Configuring AppCenter Crashes for Android');
    promise = rnpmlink.android.initAppCenterConfig()
        .then(() =>
            inquirer.prompt([{
                type: 'list',
                name: 'whenToSendCrashes',
                message: 'For the Android app, should crashes be sent automatically or processed in JavaScript before being sent?',
                choices: [
                    {
                        name: 'Automatically',
                        value: 'ALWAYS_SEND'
                    },
                    {
                        name: 'Processed in JavaScript by user',
                        value: 'ASK_JAVASCRIPT'
                    }]
            }])
        ).then((androidAnswer) => {
            rnpmlink.android.patchStrings('appCenterCrashes_whenToSendCrashes',
                androidAnswer.whenToSendCrashes);
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
        .then(() =>
            inquirer.prompt([{
                type: 'list',
                name: 'whenToSendCrashes',
                message: 'For the iOS app, should crashes be sent automatically or processed in JavaScript before being sent?',
                choices: [
                    {
                        name: 'Automatically',
                        value: 'ALWAYS_SEND'
                    },
                    {
                        name: 'Processed in JavaScript by user',
                        value: 'ASK_JAVASCRIPT'
                    }]
            }])
        )
        .then((iosAnswer) => {
            const code = iosAnswer.whenToSendCrashes === 'ALWAYS_SEND' ?
                '  [AppCenterReactNativeCrashes registerWithAutomaticProcessing];  // Initialize AppCenter crashes' :
                '  [AppCenterReactNativeCrashes register];  // Initialize AppCenter crashes';
            return rnpmlink.ios.initInAppDelegate('#import <AppCenterReactNativeCrashes/AppCenterReactNativeCrashes.h>', code, /.*\[AppCenterReactNativeCrashes register.*/g);
        })
        .then((file) => {
            console.log(`Added code to initialize iOS Crashes SDK in ${file}`);
            return rnpmlink.ios.addPodDeps(
                [
                    { pod: 'AppCenter/Crashes', version: '1.11.0' },
                    { pod: 'AppCenterReactNativeShared', version: '1.10.0' } // in case people don't link appcenter (core)
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
