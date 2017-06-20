var rnpmlink = require('mobile-center-link-scripts');
var package = require('./../package.json');

rnpmlink.ios.initInAppDelegate('#import <RNMobileCenter/RNMobileCenter.h>', "");
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



