const rnpmlink = require('mobile-center-link-scripts');
const npmPackages = require('./../package.json');

return rnpmlink.ios.initMobileCenterConfig().then(() => {
    const prompt = npmPackages.rnpm.params[0];
    prompt.message = prompt.message.replace(/Android/, 'iOS');

    return rnpmlink.inquirer.prompt(prompt);
}).then((answer) => {
    const code = answer.whenToEnableAnalytics === 'ALWAYS_SEND' ?
        '  [RNAnalytics registerWithInitiallyEnabled:true];  // Initialize Mobile Center analytics' :
        '  [RNAnalytics registerWithInitiallyEnabled:false];  // Initialize Mobile Center analytics';
    return rnpmlink.ios.initInAppDelegate('#import <RNAnalytics/RNAnalytics.h>', code, /.*\[RNAnalytics register.*/g);
}).then((file) => {
    console.log(`Added code to initialize iOS Analytics SDK in ${file}`);
    return rnpmlink.ios.addPodDeps([
        { pod: 'MobileCenter/Analytics', version: '0.12.0' },
        { pod: 'RNMobileCenterShared', version: '0.9.0' } // in case people don't link mobile-center (core)
    ]).catch((e) => {
        console.log(`
            Could not install dependencies using CocoaPods.
            Please refer to the documentation to install dependencies manually.

            Error Reason - ${e.message}
        `);
        return Promise.resolve();
    });
});
