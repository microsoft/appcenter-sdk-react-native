module.exports = {
    dependency: {
      platforms: {
        ios: {},
        android: {
          packageInstance: "new AppCenterReactNativePushPackage(getApplication())"
        },
      },
      assets: [],
      hooks: {
        postlink: "./node_modules/appcenter-push/scripts/postlink"
      }
    },
  };