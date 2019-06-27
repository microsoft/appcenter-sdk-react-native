module.exports = {
  dependency: {
    platforms: {
      ios: {},
      android: {
        packageInstance: 'new AppCenterReactNativeAnalyticsPackage(getApplication(), getResources().getString(R.string.appCenterAnalytics_whenToEnableAnalytics))'
      },
    },
    assets: [],
    hooks: {
      postlink: './node_modules/appcenter-analytics/scripts/postlink'
    }
  },
};