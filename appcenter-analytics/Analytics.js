const ReactNative = require('react-native');

const { AppCenterReactNativeAnalytics } = ReactNative.NativeModules;

const Analytics = {
    bindingType: ReactNative.Platform.select({
        ios: 'MSAnalytics',
        android: 'com.microsoft.appcenter.analytics.Analytics',
    }),

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
        return new Promise((resolve) => {
            AppCenterReactNativeAnalytics.getTransmissionTarget(targetToken)
                .then((token) => {
                    if (!token) {
                        resolve(null);
                    } else {
                        resolve(new Analytics.TransmissionTarget(token));
                    }
                });
        });
    },
};

Analytics.TransmissionTarget = class {
    constructor(targetToken) {
        this.targetToken = targetToken;
    }

    // async - returns a Promise
    trackEvent(eventName, properties) {
        return AppCenterReactNativeAnalytics.trackTransmissionTargetEvent(eventName, sanitizeProperties(properties), this.targetToken);
    }
};

module.exports = Analytics;

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
