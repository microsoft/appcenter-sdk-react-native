module.exports = {
    dependency: {
      platforms: {
        ios: {},
        android: {
          packageInstance: "new AppCenterReactNativePackage(MainApplication.this)"
        },
      },
      assets: [],
      hooks: {
        postlink: "./node_modules/appcenter/scripts/postlink"
      }
    },
  };