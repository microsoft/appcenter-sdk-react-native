let ReactNative = require('react-native');
let RNMobileCenter = require("react-native").NativeModules.RNMobileCenter;

let MobileCenter = {
    // async - returns a Promise
    async setEnabled(enabled) {
        return RNMobileCenter.setEnabled(enabled);
    },

    async getLogLevel() {
        return RNMobileCenter.getLogLevel();
    },

    async setLogLevel(logLevel){
        return RNMobileCenter.setLogLevel(logLevel);
    },

    async setCustomProperties(properties) {
        return RNMobileCenter.setCustomProperties(properties);
    }
};

module.exports = MobileCenter;
