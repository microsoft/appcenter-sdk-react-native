const rnpmlink = require('mobile-center-link-scripts');

return rnpmlink.ios.checkIfAppDelegateExists()
.then(() => {
    return rnpmlink.ios.initMobileCenterConfig()
        .catch((e) => {
            console.log(`Could not create mobile center config file. Error Reason - ${e.message}`);
            return Promise.reject();
        });
}).then(() => {
    const code = '  [RNPush register];  // Initialize Mobile Center push';
    return rnpmlink.ios.initInAppDelegate('#import <RNPush/RNPush.h>', code)
        .catch((e) => {
            console.log(`Could not initialize Mobile Center push in AppDelegate. Error Reason - ${e.message}`);
            return Promise.reject();
        });
}).then((file) => {
    console.log(`Added code to initialize iOS Push SDK in ${file}`);
    return rnpmlink.ios.addPodDeps([
        { pod: 'MobileCenter/Push', version: '0.12.1' },
        { pod: 'RNMobileCenterShared', version: '0.9.0' } // in case people don't link mobile-center (core)
    ]).catch((e) => {
        console.log(`
            Could not install dependencies using CocoaPods.
            Please refer to the documentation to install dependencies manually.
            Error Reason - ${e.message}
        `);
        return Promise.reject();
    });
}).catch(() => Promise.resolve());
