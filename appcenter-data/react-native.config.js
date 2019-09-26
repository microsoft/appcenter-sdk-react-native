module.exports = {
  dependency: {
    platforms: {
      ios: {},
      android: {
        packageInstance: 'new AppCenterReactNativeDataPackage(getApplication())'
      }
    }
  }
};
