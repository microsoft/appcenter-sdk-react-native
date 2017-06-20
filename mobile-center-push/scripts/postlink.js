var rnpmlink = require('mobile-center-link-scripts');
var package = require('./../package.json');

return rnpmlink.ios.initMobileCenterConfig().then(function (file) {
    console.log('App Secret for iOS written to ' + file);

    var code = '  [RNPush registerAndEnable];  // Initialize Mobile Center push';
    return rnpmlink.ios.initInAppDelegate('#import <RNPush/RNPush.h>', code);
}).then(function (file) {
    console.log('Added code to initialize iOS Push SDK in ' + file);
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
