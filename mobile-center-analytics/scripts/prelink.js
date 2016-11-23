var rnpmlink = require('mobile-center-link-scripts');

return rnpmlink.android.initMobileCenterConfig().then(function (file) {
    console.log('App Secret for Android written to ', file);
});