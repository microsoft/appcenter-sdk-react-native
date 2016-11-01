let RNSonomaAnalytics = require("react-native").NativeModules.RNSonomaAnalytics;

module.exports = {
    async trackEvent(eventName, properties) {
        await RNSonomaAnalytics.trackEvent(eventName, sanitizeProperties(properties));
    },

    /*
    // TODO: Uncomment this once the underlying SDK supports the functionality
    async trackPage(pageName, properties) {
        await RNSonomaAnalytics.trackPage(pageName, sanitizeProperties(properties));
    }
    */
};

function sanitizeProperties(props) {
    // Only string:string mappings are supported currently.

    try {
        JSON.stringify(props);
        return props;
    } catch (e) {
        throw new Error("Properties cannot be serialized. Object must only contain strings");
    }
}