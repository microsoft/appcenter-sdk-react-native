const AppCenterReactNativeAnalytics = require('react-native').NativeModules.AppCenterReactNativeAnalytics;

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
    }
};

function sanitizeProperties(props) {
    // Only string:string mappings are supported currently.

    const result = {};
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
