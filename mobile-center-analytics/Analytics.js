let RNAnalytics = require("react-native").NativeModules.RNAnalytics;

module.exports = {
    // async - returns a Promise
    trackEvent(eventName, properties) {
        return RNAnalytics.trackEvent(eventName, sanitizeProperties(properties));
    },

    // async - returns a Promise
    isEnabled() {
        return RNAnalytics.isEnabled();
    },

    // async - returns a Promise
    setEnabled(enabled) {
        return RNAnalytics.setEnabled(enabled);
    }

    /*
    // TODO: Uncomment this once the underlying SDK supports the functionality
    async trackPage(pageName, properties) {
        await RNAnalytics.trackPage(pageName, sanitizeProperties(properties));
    }
    */
};

function sanitizeProperties(props) {
    // Only string:string mappings are supported currently.

    var result = {};

    for(let i in props) {
        switch (typeof props[i]) {
        case "string":
        case "number":
        case "boolean":
            result[i] = ""+props[i];
            break;
        case "undefined":
            break;
            default:
            throw new Error("Properties cannot be serialized. Object must only contain strings");
        }
    }

    return result;
}