let RNSonomaAnalytics = require("react-native").NativeModules.RNSonomaAnalytics;

module.exports = {
    async trackEvent(eventName, properties) {
        await RNSonomaAnalytics.trackEvent(eventName, properties);
    },

    async trackPage(pageName, properties) {
        await RNSonomaAnalytics.trackPage(pageName, properties);
    }
};