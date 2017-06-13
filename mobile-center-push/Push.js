let ReactNative = require('react-native');
let RNPush = require("react-native").NativeModules.RNPush;

let Push = {
    // async - returns a Promise
    setEnabled(enabled) {
        return RNPush.setEnabled(enabled);
    },

    // async - returns a Promise
    isEnabled() {
        return RNPush.isEnabled();
    },
};

module.exports = Push;
