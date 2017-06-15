let ReactNative = require('react-native');
let RNMobileCenter = require("react-native").NativeModules.RNMobileCenterWrapper;

let MobileCenter = {
    // async - returns a Promise
    setCustomProperties(properties) {
        return RNMobileCenter.setCustomProperties(properties);
    }
};

module.exports = MobileCenter;
