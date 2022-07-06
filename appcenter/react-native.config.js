module.exports = {
  dependency: {
    platforms: {
      ios: {},
      android: {
        packageInstance: 'new AppCenterReactNativePackage(getApplication())'
      }
    }
  }
};
