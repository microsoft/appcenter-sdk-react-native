var rnpmlink = require('mobile-center-link-scripts');
var package = require('./../package.json');

var prompt = package.rnpm.params[0];
prompt.message = prompt.message.replace(/Android/, 'iOS');
rnpmlink.inquirer.prompt(prompt)
.then(function (answer) {
    var code = answer.whenToSendCrashes === 'ALWAYS_SEND' ?
        '  [RNCrashes registerWithCrashDelegate: [[RNCrashesDelegateAlwaysSend alloc] init]];  // Initialize Mobile Center crashes' :
        '  [RNCrashes register];  // Initialize Mobile Center crashes'
    return rnpmlink.ios.initInAppDelegate('#import <RNCrashes/RNCrashes.h>', code);
}).then(function (file) {
    console.log('Added code to initialize iOS Crashes SDK in ' + file);
    return rnpmlink.ios.addPodDeps([
        { pod: 'MobileCenter', version: '0.10.1' },
        { pod: 'RNMobileCenterShared', version: '0.6.0' }
    ]).catch(function (e) {
        console.log(`
            Could not install dependencies using CocoaPods.
            Please refer to the documentation to install dependencies manually.

            Error Reason - ${e.message}
        `)
        return Promise.resolve();
    })
});
