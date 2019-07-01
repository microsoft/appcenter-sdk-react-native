module.exports = {
  dependency: {
    platforms: {
      ios: {},
      android: {
        packageInstance: 'new AppCenterReactNativeAuthPackage(getApplication())'
      },
    },
    assets: [],
    hooks: {
      postlink: './node_modules/appcenter-auth/scripts/postlink'
    }
  },
};
