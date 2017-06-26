var rnpmlink = require('mobile-center-link-scripts');

console.log('Configuring Mobile Center Analytics');

return rnpmlink.android.initMobileCenterConfig(false).then(function (file) {
});
