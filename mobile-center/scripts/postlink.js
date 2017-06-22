var rnpmlink = require('mobile-center-link-scripts');
var package = require('./../package.json');

return rnpmlink.ios.initMobileCenterConfig().then(function (file) {
    console.log('App Secret for iOS written to ' + file);

    return rnpmlink.ios.initInAppDelegate('#import <RNMobileCenter/RNMobileCenter.h>', "");
}).then(function(file){
    return rnpmlink.ios.addPodDeps([
        { pod: 'RNMobileCenterShared', version: '0.6.0' },
        { pod: 'MobileCenter', version: '0.10.1' }
    ]).catch(function (e) {
        console.log(`
            Could not install dependencies using CocoaPods.
            Please refer to the documentation to install dependencies manually.

            Error Reason - ${e.message}
        `)
        return Promise.resolve();
    });
});








