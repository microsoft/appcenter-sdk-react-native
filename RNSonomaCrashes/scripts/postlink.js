var rnpmlink = require('sonoma-react-native-link-scripts');
var package = require('./../package.json');

return rnpmlink.ios.initSonomaConfig().then(function (file) {
    console.log('App Secret for iOS written to ' + file);
    var prompt = package.rnpm.params[0];
    prompt.message = prompt.message.replace(/Android/, 'iOS');

    return rnpmlink.inquirer.prompt(prompt);
}).then(function (answer) {
    var code = answer.whenToSendCrashes === 'ALWAYS_SEND' ?
        '[RNSonomaCrashses registerWithCrashDelegate: [[RNSonomaCrashesDelegateAlwaysSend alloc] init]]'
        : '[RNSonomaCrashes register]'
    return rnpmlink.ios.initInAppDelegate('#import "RNSonomaCrashes.h"', code);
}).then(function (file) {
    console.log('Added code to initialize iOS Crashes SDK in ' + file);
    return rnpmlink.ios.addPodDeps([
        { pod: 'Sonoma', podspec: 'https://download.hockeyapp.net/sonoma/ios/Sonoma.podspec' },
        { pod: 'RNSonomaCore', podspec: '../../RNSonomaCore.podspec' }
    ]).catch(function (e) {
        console.log(`
            Could not install dependencies using CocoaPods. 
            Please refer the documentation to install dependencies manually. 

            Error Reason - ${e.message}
        `)
        return Promise.resolve();
    })
});