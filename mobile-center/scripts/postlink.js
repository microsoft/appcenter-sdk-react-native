var rnpmlink = require('mobile-center-link-scripts');
var package = require('./../package.json');

return rnpmlink.ios.initMobileCenterConfig().then(function (file) {
    var code = '  [RNMobileCenter register];  // Initialize Mobile Center '
    return rnpmlink.ios.initInAppDelegate('#import <RNMobileCenter/RNMobileCenter.h>', code);
}).then(function (file) {
    console.log('Added code to initialize iOS Mobile Center SDK in ' + file);
    return rnpmlink.ios.addPodDeps([
        { pod: 'RNMobileCenterShared', version: '0.8.1' }
    ]).catch(function (e) {
        console.log(`
            Could not install dependencies using CocoaPods.
            Please refer to the documentation to install dependencies manually.

            Error Reason - ${e.message}
        `)
        return Promise.resolve();
    })
});
