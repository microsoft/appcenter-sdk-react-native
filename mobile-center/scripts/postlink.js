var rnpmlink = require('mobile-center-link-scripts');
var package = require('./../package.json');

rnpmlink.ios.initInAppDelegate('#import <RNMobileCenterWrapper/RNMobileCenterWrapper.h>', "");
return rnpmlink.ios.addPodDeps([
        { pod: 'RNMobileCenter', version: '0.5.0' }
    ]).catch(function (e) {
        console.log(`
            Could not install dependencies using CocoaPods.
            Please refer to the documentation to install dependencies manually.

            Error Reason - ${e.message}
        `)
        return Promise.resolve();
    });



