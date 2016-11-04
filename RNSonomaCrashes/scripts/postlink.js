var rnpmlink = require('sonoma-react-native-link-scripts');

return rnpmlink.ios.initSonomaConfig().then(function(file) {
    console.log('App Secret for iOS written to ' + file);
    return rnpmlink.inquirer.prompt({
        type: 'list',
        name: 'whenToSendiOSCrashes',
        message: 'For the iOS App, should crashes be sent automatically, or processed in javascript before being sent ?',
        choices: [
            {
                "name": "Automatically",
                "value": "ALWAYS_SEND"
            },
            {
                "name": "Processed in JavaScript by user",
                "value": "ASK_JAVASCRIPT"
            }
        ]
    });
}).then(function(answer) {
    var code = answer.whenToSendiOSCrashes === 'ALWAYS_SEND' ?
        '[RNSonomaCrashses registerWithCrashDelegate: [[RNSonomaCrashesDelegateAlwaysSend alloc] init]]'
        : '[RNSonomaCrashes register]'
    return rnpmlink.ios.initInAppDelegate('#import "RNSonomaCrashes.h"', code);
}).then(function(file) {
    console.log('Added code to initialize iOS Crashes SDK in ' + file);
    return rnpmlink.ios.addPodDeps([
        { pod: 'Sonoma', podspec: 'https://download.hockeyapp.net/sonoma/ios/Sonoma.podspec' },
        { pod: 'RNSonomaCore', podspec: '../../RNSonomaCore.podspec' }
    ]).catch(function(e) {
        console.log(`
            Could not install dependencies using CocoaPods. 
            Please refer the documentation to install dependencies manually. 

            Error Reason - ${e.message}
        `)
        return Promise.resolve();
    })
});