let RNSonomaAnalytics = require("react-native").NativeModules.RNSonomaAnalytics;

module.exports = {
    async trackEvent(eventName, properties) {
        await RNSonomaAnalytics.trackEvent(eventName, sanitizeProperties(properties));
    },

    async trackPage(pageName, properties) {
        await RNSonomaAnalytics.trackPage(pageName, sanitizeProperties(properties));
    }
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
            throw new Error("");
        }
    }

    return result;
}