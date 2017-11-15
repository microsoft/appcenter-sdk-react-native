const rnpmlink = require('appcenter-link-scripts');

console.log('Configuring AppCenter Analytics');

return rnpmlink.android.checkIfAndroidDirectoryExists()
    .then(() => {
        rnpmlink.android.initConfig()
            .catch((e) => {
                console.log(`Could not create AppCenter config file. Error Reason - ${e.message}`);
                return Promise.reject();
            });
    })
    .catch(() => Promise.resolve());
