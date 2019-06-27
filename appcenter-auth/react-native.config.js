module.exports = {
    dependency: {
      platforms: {
        ios: {},
        android: {
          packageInstance: "new AppCenterReactNativeAuthPackage(MainApplication.this)"
        },
      },
      assets: [],
      hooks: {
        postlink: "./node_modules/appcenter-auth/scripts/postlink"
      }
    },
  };