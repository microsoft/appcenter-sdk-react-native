module.exports = {
    dependency: {
      platforms: {
        ios: {},
        android: {
          packageInstance: "new AppCenterReactNativePushPackage(MainApplication.this)"
        },
      },
      assets: [],
      hooks: {
        postlink: "./node_modules/appcenter-push/scripts/postlink"
      }
    },
  };