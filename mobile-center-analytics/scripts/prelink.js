const rnpmlink = require('mobile-center-link-scripts');

console.log('Configuring Mobile Center Analytics');

return rnpmlink.android.checkIfAndroidDirectoryExists()
    .then(() => {
        rnpmlink.android.initMobileCenterConfig()
            .catch((e) => {
                console.log(`Could not create mobile center config file. Error Reason - ${e.message}`);
                return Promise.reject();
            });
    })
    .catch(() => Promise.resolve());
