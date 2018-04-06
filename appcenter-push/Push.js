const ReactNative = require('react-native');

const { AppCenterReactNativePush } = ReactNative.NativeModules;

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
            pushEventEmitter.addListener(pushNotificationReceivedEvent, (pushNotification) => {
                // On Android it's null but on iOS nil maps to undefined, make it consistent.
                if (pushNotification.title === undefined) {
                    pushNotification.title = null;
                }
                if (pushNotification.message === undefined) {
                    pushNotification.message = null;
                }
                listenerMap.onPushNotificationReceived(pushNotification);
            });
            return AppCenterReactNativePush.sendAndClearInitialNotification();
        }
        return Promise.resolve();
    }
};

module.exports = Push;
