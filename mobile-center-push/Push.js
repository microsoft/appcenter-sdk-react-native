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
    setEventListener(listenerMap) {
        pushEventEmitter.removeAllListeners(pushNotificationReceivedEvent);
        if (listenerMap && listenerMap.pushNotificationReceived) {
            pushEventEmitter.addListener(pushNotificationReceivedEvent, listenerMap.pushNotificationReceived);

            return RNPush.sendAndClearInitialNotification();
        }
        return Promise.resolve();
    }
};

module.exports = Push;
