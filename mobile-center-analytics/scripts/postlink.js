const rnpmlink = require('mobile-center-link-scripts');
const npmPackages = require('./../package.json');

return rnpmlink.ios.checkIfAppDelegateExists()
.then(() => {
    return initMobileCenterConfig()
        .catch((e) => {
            console.log(`Could not create mobile center config file. Error Reason - ${e.message}`);
            return Promise.reject();
        });
}).then(() => {
    const prompt = npmPackages.rnpm.params[0];
    prompt.message = prompt.message.replace(/Android/, 'iOS');
    return rnpmlink.inquirer.prompt(prompt)
        .catch((e) => {
            console.log(`Could not find answer of whenToEnableAnalytics. Error Reason - ${e.message}`);
            return Promise.reject();   
        });     
}).then((answer) => {
    const code = answer.whenToEnableAnalytics === 'ALWAYS_SEND' ?
        '  [RNAnalytics registerWithInitiallyEnabled:true];  // Initialize Mobile Center analytics' :
        '  [RNAnalytics registerWithInitiallyEnabled:false];  // Initialize Mobile Center analytics';
    return rnpmlink.ios.initInAppDelegate('#import <RNAnalytics/RNAnalytics.h>', code, /.*\[RNAnalytics register.*/g)
        .catch((e) => {
            console.log(`Could not initialize Mobile Center analytics in AppDelegate. Error Reason - ${e.message}`);
            return Promise.reject();
        });
}).then((file) => {
    console.log(`Added code to initialize iOS Analytics SDK in ${file}`);
    return rnpmlink.ios.addPodDeps([
        { pod: 'MobileCenter/Analytics', version: '0.12.1' },
        { pod: 'RNMobileCenterShared', version: '0.9.0' } // in case people don't link mobile-center (core)
    ]).catch((e) => {
        console.log(`
            Could not install dependencies using CocoaPods.
            Please refer to the documentation to install dependencies manually.

            Error Reason - ${e.message}
        `);
        return Promise.reject();
    });
}).catch(() => Promose.resolve());
