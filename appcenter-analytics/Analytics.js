// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

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

Analytics.PropertyConfigurator = class {
    constructor(transmissionTarget) {
        this.transmissionTarget = transmissionTarget;
    }

    // async - returns a Promise
    setAppName(appName) {
        return AppCenterReactNativeAnalytics.setTransmissionTargetAppName(appName, this.transmissionTarget.targetToken);
    }

    // async - returns a Promise
    setAppVersion(appVersion) {
        return AppCenterReactNativeAnalytics.setTransmissionTargetAppVersion(appVersion, this.transmissionTarget.targetToken);
    }

    // async - returns a Promise
    setAppLocale(appLocale) {
        return AppCenterReactNativeAnalytics.setTransmissionTargetAppLocale(appLocale, this.transmissionTarget.targetToken);
    }

    // async - returns a Promise
    setEventProperty(key, value) {
        return AppCenterReactNativeAnalytics.setTransmissionTargetEventProperty(key, value, this.transmissionTarget.targetToken);
    }

    // async - returns a Promise
    removeEventProperty(key) {
        return AppCenterReactNativeAnalytics.removeTransmissionTargetEventProperty(key, this.transmissionTarget.targetToken);
    }

    // async - returns a Promise
    collectDeviceId() {
        return AppCenterReactNativeAnalytics.collectTransmissionTargetDeviceId(this.transmissionTarget.targetToken);
    }
};

Analytics.TransmissionTarget = class {
    constructor(targetToken) {
        this.targetToken = targetToken;
        this.propertyConfigurator = new Analytics.PropertyConfigurator(this);
    }

    // async - returns a Promise
    isEnabled() {
        return AppCenterReactNativeAnalytics.isTransmissionTargetEnabled(this.targetToken);
    }

    // async - returns a Promise
    setEnabled(enabled) {
        return AppCenterReactNativeAnalytics.setTransmissionTargetEnabled(enabled, this.targetToken);
    }

    // async - returns a Promise
    trackEvent(eventName, properties) {
        return AppCenterReactNativeAnalytics.trackTransmissionTargetEvent(eventName, sanitizeProperties(properties), this.targetToken);
    }

    // async - returns a Promise
    getTransmissionTarget(childToken) {
        return new Promise((resolve) => {
            AppCenterReactNativeAnalytics.getChildTransmissionTarget(childToken, this.targetToken)
                .then((token) => {
                    if (!token) {
                        resolve(null);
                    } else {
                        resolve(new Analytics.TransmissionTarget(token));
                    }
                });
        });
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
