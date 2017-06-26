var rnpmlink = require('mobile-center-link-scripts');

console.log('Configuring Mobile Center');

return rnpmlink.android.initMobileCenterConfig(true).then(function (file) {
});
