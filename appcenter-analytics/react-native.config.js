const path = require('path');

module.exports = {
  dependency: {
    platforms: {
      ios: {
        podspecPath: path.join(__dirname, 'ios', 'appcenter-analytics.podspec')
      },
      android: {
        packageInstance: 'new AppCenterReactNativeAnalyticsPackage(getApplication(), getResources().getString(R.string.appCenterAnalytics_whenToEnableAnalytics))'
      }
    }
  }
};
