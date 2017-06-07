var rnpmlink = require('mobile-center-link-scripts');
var package = require('./../package.json');

return rnpmlink.ios.initMobileCenterConfig().then(function (file) {
    console.log('App Secret for iOS written to ' + file);

    var prompt = package.rnpm.params[0];
    prompt.message = prompt.message.replace(/Android/, 'iOS');

    return rnpmlink.inquirer.prompt(prompt);
}).then(function (answer) {
    var code = answer.whenToEnablePush === 'ALWAYS_SEND' ?
        '  [RNPush registerWithInitiallyEnabled:true];  // Initialize Mobile Center push' :
        '  [RNPush registerWithInitiallyEnabled:false];  // Initialize Mobile Center push'
    return rnpmlink.ios.initInAppDelegate('#import <RNPush/RNPush.h>', code);
}).then(function (file) {
    console.log('Added code to initialize iOS Crashes SDK in ' + file);
    return rnpmlink.ios.addPodDeps([
        { pod: 'MobileCenter', version: '0.9.0' },
        { pod: 'RNMobileCenter', version: '0.5.0' }
    ]).catch(function (e) {
        console.log(`
            Could not install dependencies using CocoaPods.
            Please refer the documentation to install dependencies manually.

            Error Reason - ${e.message}
        `)
        return Promise.resolve();
    })
});
