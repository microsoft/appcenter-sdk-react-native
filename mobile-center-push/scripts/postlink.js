var rnpmlink = require('mobile-center-link-scripts');
var package = require('./../package.json');

return rnpmlink.ios.initMobileCenterConfig().then(function (file) {
    console.log('App Secret for iOS written to ' + file);

    var code = '  [RNPush registerAndEnable];  // Initialize Mobile Center push';
    return rnpmlink.ios.initInAppDelegate('#import <RNPush/RNPush.h>', code);
}).then(function (file) {
    console.log('Added code to initialize iOS Push SDK in ' + file);
    return rnpmlink.ios.addPodDeps([
        { pod: 'MobileCenter/MobileCenterPush', version: '0.9.0' },
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
