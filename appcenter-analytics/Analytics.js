const ReactNative = require('react-native');

const { AppCenterReactNativeAnalytics } = ReactNative.NativeModules;

module.exports = {
    // async - returns a Promise
    trackEvent(eventName, properties) {
        return AppCenterReactNativeAnalytics.trackEvent(eventName, sanitizeProperties(properties));
    },

    // async - returns a Promise
    isEnabled() {
        return AppCenterReactNativeAnalytics.isEnabled();
    },

    // async - returns a Promise
    setEnabled(enabled) {
        return AppCenterReactNativeAnalytics.setEnabled(enabled);
    },

    // async - returns a Promise
    getTransmissionTarget(targetToken) {
        return new Promise(function(resolve, reject) {
            AppCenterReactNativeAnalytics.getTransmissionTarget(targetToken)
            .then(targetToken => {
                resolve(new TransmissionTarget(targetToken));
            })
            .catch(error => {
                reject(error);
            })
        })
    },

    TransmissionTarget: class {
        constructor(targetToken) {
            this._targetToken = targetToken;
        }

        // async - returns a Promise
        trackEvent(eventName, properties) {
            return AppCenterReactNativeAnalytics.trackEventForTransmissionTarget(this._targetToken, eventName, sanitizeProperties(properties));
        }
    }
};

function sanitizeProperties(props = null) {
    // Only string:string mappings are supported currently.

    const result = {};
    if (props === null) {
        return result;
    }
    Object.keys(props).forEach((key) => {
        switch (typeof props[key]) {
            case 'string':
            case 'number':
            case 'boolean':
                result[key] = `${props[key]}`;
                break;
            case 'undefined':
                break;
            default:
                throw new Error('Properties cannot be serialized. Object must only contain strings');
        }
    });
    return result;
}
