const rnpmlink = require('mobile-center-link-scripts');
const npmPackages = require('./../package.json');

return rnpmlink.ios.checkIfAppDelegateExists()
    .then(() => rnpmlink.ios.initMobileCenterConfig().catch((e) => {
        console.log(`Could not create or update Mobile Center config file (MobileCenter-Config.plist). Error Reason - ${e.message}`);
        return Promise.reject();
    }))
    .then(() => {
        const prompt = npmPackages.rnpm.params[0];
        prompt.message = prompt.message.replace(/Android/, 'iOS');
        return rnpmlink.inquirer.prompt(prompt)
            .catch((e) => {
                console.log(`Could not determine when to send Mobile Center crashes. Error Reason - ${e.message}`);
                return Promise.reject();
            });
    })
    .then((answer) => {
        const code = answer.whenToSendCrashes === 'ALWAYS_SEND' ?
            '  [RNCrashes registerWithCrashDelegate: [[RNCrashesDelegateAlwaysSend alloc] init]];  // Initialize Mobile Center crashes' :
            '  [RNCrashes register];  // Initialize Mobile Center crashes';
        return rnpmlink.ios.initInAppDelegate('#import <RNCrashes/RNCrashes.h>', code, /.*\[RNCrashes register.*/g)
            .catch((e) => {
                console.log(`Could not initialize Mobile Center crashes in AppDelegate. Error Reason - ${e.message}`);
                return Promise.reject();
            });
    })
    .then((file) => {
        console.log(`Added code to initialize iOS Crashes SDK in ${file}`);
        return rnpmlink.ios.addPodDeps([
            { pod: 'MobileCenter/Crashes', version: '0.13.0' },
            { pod: 'RNMobileCenterShared', version: '0.10.0' } // in case people don't link mobile-center (core)
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
