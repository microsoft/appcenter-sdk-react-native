var rnpmlink = require('mobile-center-link-scripts');
var package = require('./../package.json');

return rnpmlink.ios.initMobileCenterConfig().then(function (file) {
    console.log('App Secret for iOS written to ' + file);

    var prompt = package.rnpm.params[0];
    prompt.message = prompt.message.replace(/Android/, 'iOS');

    return rnpmlink.inquirer.prompt(prompt);
}).then(function (answer) {
    var code = answer.whenToEnableAnalytics === 'ALWAYS_SEND' ?
        '  [RNAnalytics registerWithInitiallyEnabled:true];  // Initialize Mobile Center analytics' :
        '  [RNAnalytics registerWithInitiallyEnabled:false];  // Initialize Mobile Center analytics'
    return rnpmlink.ios.initInAppDelegate('#import <RNAnalytics/RNAnalytics.h>', code);
}).then(function (file) {
    console.log('Added code to initialize iOS Crashes SDK in ' + file);
    return rnpmlink.ios.addPodDeps([
        { pod: 'MobileCenter', version: '0.9.0' },
        { pod: 'RNMobileCenter', version: '0.5.0' }
    ]).catch(function (e) {
        console.log(`
            Could not install dependencies using CocoaPods.
            Please refer to the documentation to install dependencies manually.

            Error Reason - ${e.message}
        `)
        return Promise.resolve();
    })
});
