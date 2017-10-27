const ReactNative = require('react-native');
const RNPush = ReactNative.NativeModules.RNPush;

const pushEventEmitter = new ReactNative.NativeEventEmitter(RNPush);

const pushNotificationReceivedEvent = 'MobileCenterPushNotificationReceived';


const Push = {
    // async - returns a Promise
    setEnabled(enabled) {
        return RNPush.setEnabled(enabled);
    },

    // async - returns a Promise
    isEnabled() {
        return RNPush.isEnabled();
    },

    // async - returns a Promise
    setListener(listenerMap) {
        pushEventEmitter.removeAllListeners(pushNotificationReceivedEvent);
        if (listenerMap && listenerMap.onPushNotificationReceived) {
            pushEventEmitter.addListener(pushNotificationReceivedEvent, listenerMap.onPushNotificationReceived);

            return RNPush.sendAndClearInitialNotification();
        }
        return Promise.resolve();
    }
};

module.exports = Push;
