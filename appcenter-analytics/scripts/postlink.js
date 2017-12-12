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
                console.log(`Could not determine when to enable AppCenter analytics. Error Reason - ${e.message}`);
                return Promise.reject();
            });
    })
    .then((answer) => {
        const code = answer.whenToEnableAnalytics === 'ALWAYS_SEND' ?
            '  [AppCenterReactNativeAnalytics registerWithInitiallyEnabled:true];  // Initialize AppCenter analytics' :
            '  [AppCenterReactNativeAnalytics registerWithInitiallyEnabled:false];  // Initialize AppCenter analytics';
        return rnpmlink.ios.initInAppDelegate('#import <AppCenterReactNativeAnalytics/AppCenterReactNativeAnalytics.h>', code, /.*\[AppCenterReactNativeAnalytics register.*/g)
            .catch((e) => {
                console.log(`Could not initialize AppCenter analytics in AppDelegate. Error Reason - ${e.message}`);
                return Promise.reject();
            });
    })
    .then((file) => {
        console.log(`Added code to initialize iOS Analytics SDK in ${file}`);
        return rnpmlink.ios.addPodDeps([
            { pod: 'AppCenter/Analytics', version: '1.1.0' },
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
