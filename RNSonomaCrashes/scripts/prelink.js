var rnpmlink = require('sonoma-react-native-link-scripts');

return rnpmlink.android.initSonomaConfig().then(function (file) {
    console.log('App Secret for Android written to ', file);
});