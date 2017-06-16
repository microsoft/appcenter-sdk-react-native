let ReactNative = require('react-native');
let RNMobileCenter = require("react-native").NativeModules.RNMobileCenter;

let MobileCenter = {
    // async - returns a Promise
    async setCustomProperties(properties) {
        return RNMobileCenter.setCustomProperties(properties);
    },

    async getLogLevel() {
        return RNMobileCenter.getLogLevel();
    },

    async setLogLevel(logLevel){
        return RNMobileCenter.setLogLevel(logLevel);
    }
};

module.exports = MobileCenter;
