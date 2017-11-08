const ReactNative = require('react-native');

const AppCenterReactNativePush = ReactNative.NativeModules.AppCenterReactNativePush;

const pushEventEmitter = new ReactNative.NativeEventEmitter(AppCenterReactNativePush);

const pushNotificationReceivedEvent = 'AppCenterPushNotificationReceived';


const Push = {
    // async - returns a Promise
    setEnabled(enabled) {
        return AppCenterReactNativePush.setEnabled(enabled);
    },

    // async - returns a Promise
    isEnabled() {
        return AppCenterReactNativePush.isEnabled();
    },

    // async - returns a Promise
    setListener(listenerMap) {
        pushEventEmitter.removeAllListeners(pushNotificationReceivedEvent);
        if (listenerMap && listenerMap.onPushNotificationReceived) {
            pushEventEmitter.addListener(pushNotificationReceivedEvent, listenerMap.onPushNotificationReceived);

            return AppCenterReactNativePush.sendAndClearInitialNotification();
        }
        return Promise.resolve();
    }
};

module.exports = Push;
