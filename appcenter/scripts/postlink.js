const rnpmlink = require('appcenter-link-scripts');

return rnpmlink.ios.checkIfAppDelegateExists()
    .then(() =>
        Promise.resolve().then(() =>
            rnpmlink.ios.initAppCenterConfig()
                .catch((e) => {
                    console.log(`Could not create or update AppCenter config file (AppCenter-Config.plist). Error Reason - ${e.message}`);
                    return Promise.reject();
                })
        )
        .then(() => {
            const code = '  [AppCenterReactNative register];  // Initialize AppCenter ';
            return rnpmlink.ios.initInAppDelegate('#import <AppCenterReactNative/AppCenterReactNative.h>', code)
                .catch((e) => {
                    console.log(`Could not initialize AppCenter in AppDelegate. Error Reason - ${e.message}`);
                    return Promise.reject();
                });
        })
        .then((file) => {
            console.log(`Added code to initialize iOS AppCenter SDK in ${file}`);
            return rnpmlink.ios.addPodDeps([
                { pod: 'AppCenterReactNativeShared', version: '1.1.0' }
            ]).catch((e) => {
                console.log(`
                Could not install dependencies using CocoaPods.
                Please refer to the documentation to install dependencies manually.

                Error Reason - ${e.message}
            `);
                return Promise.reject();
            });
        })
        .catch(() => Promise.resolve())
    )
    .catch((err) => {
        console.log('Could not locate the ios project directory; skipping AppCenter postlink steps for ios.');
        if(err) {
            console.log(err);
        }
        return Promise.resolve();
    });
