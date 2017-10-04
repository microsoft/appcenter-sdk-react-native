const rnpmlink = require('mobile-center-link-scripts');

return rnpmlink.ios.checkIfAppDelegateExists()
    .then(() => rnpmlink.ios.initMobileCenterConfig().catch((e) => {
        console.log(`Could not create or update Mobile Center config file (MobileCenter-Config.plist). Error Reason - ${e.message}`);
        return Promise.reject();
    }))
    .then(() => {
        const code = '  [RNPush register];  // Initialize Mobile Center push';
        return rnpmlink.ios.initInAppDelegate('#import <RNPush/RNPush.h>', code)
            .catch((e) => {
                console.log(`Could not initialize Mobile Center push in AppDelegate. Error Reason - ${e.message}`);
                return Promise.reject();
            });
    })
    .then((file) => {
        console.log(`Added code to initialize iOS Push SDK in ${file}`);
        return rnpmlink.ios.addPodDeps([
            { pod: 'MobileCenter/Push', version: '0.13.0' },
            { pod: 'RNMobileCenterShared', version: '0.10.0' } // in case people don't link mobile-center (core)
        ]).catch((e) => {
            console.log(`
            Could not install dependencies using CocoaPods.
            Please refer to the documentation to install dependencies manually.

            Error Reason - ${e.message}
        `);
            return Promise.reject();
        });
    })
    .catch(() => Promise.resolve());
