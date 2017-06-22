var rnpmlink = require('mobile-center-link-scripts');

console.log('Configuring Mobile Center App Secret...');
return rnpmlink.android.initMobileCenterConfig().then(function (file) {
    console.log('App Secret for Android written to ', file);
});
