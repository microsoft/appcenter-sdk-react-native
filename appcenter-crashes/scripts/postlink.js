const rnpmlink = require('appcenter-link-scripts');
const npmPackages = require('./../package.json');

return rnpmlink.ios.checkIfAppDelegateExists()
    .then(() => rnpmlink.ios.initAppCenterConfig().catch((e) => {
        console.log(`Could not create or update AppCenter config file (AppCenter-Config.plist). Error Reason - ${e.message}`);
        return Promise.reject();
    }))
    .then(() => {
        const prompt = npmPackages.rnpm.params[0];
        prompt.message = prompt.message.replace(/Android/, 'iOS');
        return rnpmlink.inquirer.prompt(prompt)
            .catch((e) => {
                console.log(`Could not determine when to send AppCenter crashes. Error Reason - ${e.message}`);
                return Promise.reject();
            });
    })
    .then((answer) => {
        const code = answer.whenToSendCrashes === 'ALWAYS_SEND' ?
            [
                '  [AppCenterReactNativeCrashes registerWithAutomaticProcessing];  // Initialize AppCenter crashes',
                '    AppCenterReactNativeCrashes.registerWithAutomaticProcessing()  // Initialize AppCenter crashes'
            ] :
            [
                '  [AppCenterReactNativeCrashes register];  // Initialize AppCenter crashes',
                '    AppCenterReactNativeCrashes.register()  // Initialize AppCenter crashes'
            ];
        const oldCodeRegExp = [
            /.*\[AppCenterReactNativeCrashes register.*/g,
            /.*\[AppCenterReactNativeCrashes.register.*/g
        ];
        return rnpmlink.ios.initInAppDelegate('#import <AppCenterReactNativeCrashes/AppCenterReactNativeCrashes.h>', code, oldCodeRegExp)
            .catch((e) => {
                console.log(`Could not initialize AppCenter crashes in AppDelegate. Error Reason - ${e.message}`);
                return Promise.reject();
            });
    })
    .then((file) => {
        console.log(`Added code to initialize iOS Crashes SDK in ${file}`);
        return rnpmlink.ios.addPodDeps([
            { pod: 'AppCenter/Crashes', version: '1.1.0' },
            { pod: 'AppCenterReactNativeShared', version: '1.1.0' } // in case people don't link appcenter (core)
        ]).catch((e) => {
            console.log(`
            Could not install dependencies using CocoaPods.
            Please refer to the documentation to install dependencies manually.

            Error Reason - ${e.message}
        `);
            return Promise.reject();
        });
    })
    .catch(() => Promise.resolve());
