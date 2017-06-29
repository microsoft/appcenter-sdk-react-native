let ReactNative = require('react-native');
let RNPush = require("react-native").NativeModules.RNPush;

const pushNotificationReceivedEvent = "MobileCenterPushNotificationReceived";


let Push = {
    // async - returns a Promise
    setEnabled(enabled) {
        return RNPush.setEnabled(enabled);
    },

    // async - returns a Promise
    isEnabled() {
        return RNPush.isEnabled();
    },

    setEventListener(listenerMap) {
        ReactNative.DeviceEventEmitter.removeAllListeners(pushNotificationReceivedEvent);
        if (listenerMap && listenerMap.pushNotificationReceived) {
            ReactNative.DeviceEventEmitter.addListener(pushNotificationReceivedEvent, listenerMap.pushNotificationReceived);

            return RNPush.sendAndClearInitialNotification();
        }
    }
};

module.exports = Push;
