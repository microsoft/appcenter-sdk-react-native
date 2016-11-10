var rnpmlink = require('mobilecenter-link-scripts');
var package = require('./../package.json');

return rnpmlink.ios.initMobileCenterConfig().then(function(file) {
    console.log('App Secret for iOS written to ' + file);

    var prompt = package.rnpm.params[0];
    prompt.message = prompt.message.replace(/Android/, 'iOS');

    return rnpmlink.inquirer.prompt(prompt);
    
}).then(function(answer) {
    var code = answer.whenToEnableAnalytics === 'ALWAYS_SEND' ?
        '[RNAnalytics registerWithInitiallyEnabled:true];'
        : '[RNAnalytics registerWithInitiallyEnabled:false];'
    return rnpmlink.ios.initInAppDelegate('#import "RNAnalytics.h"', code);
}).then(function(file) {
    console.log('Added code to initialize iOS Crashes SDK in ' + file);
    return rnpmlink.ios.addPodDeps([
        { pod: 'MobileCenter', podspec: 'https://mobilecentersdkdev.blob.core.windows.net/sdk/MobileCenter.podspec' },
        { pod: 'RNMobileCenter', podspec: '../../RNMobileCenter.podspec' }
    ]).catch(function(e) {
        console.log(`
            Could not install dependencies using CocoaPods. 
            Please refer the documentation to install dependencies manually. 

            Error Reason - ${e.message}
        `)
        return Promise.resolve();
    })
});