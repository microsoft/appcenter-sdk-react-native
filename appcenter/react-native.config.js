module.exports = {
  dependency: {
    platforms: {
      ios: {},
      android: {
        packageInstance: 'new AppCenterReactNativePackage(getApplication())'
      },
    },
    assets: [],
    hooks: {
      postlink: './node_modules/appcenter/scripts/postlink'
    }
  },
};
