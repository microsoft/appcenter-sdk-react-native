const rnpmlink = require('appcenter-link-scripts');

console.log('\nConfiguring AppCenter Analytics');

return rnpmlink.android.checkIfAndroidDirectoryExists()
    .then(() => {
        rnpmlink.android.initAppCenterConfig()
            .catch((e) => {
                console.log(`Could not create AppCenter config file. Error Reason - ${e.message}`);
                return Promise.reject();
            });
    })
    .catch((err) => {
        console.log('An error occurred while checking for the android project directory', err);
        return Promise.resolve();
    });
