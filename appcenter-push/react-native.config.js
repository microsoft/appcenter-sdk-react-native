module.exports = {
  dependency: {
    platforms: {
      ios: {},
      android: {
        packageInstance: 'new AppCenterReactNativePushPackage(getApplication())'
      }
    }
  }
};
