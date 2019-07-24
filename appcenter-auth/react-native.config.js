module.exports = {
  dependency: {
    platforms: {
      ios: {},
      android: {
        packageInstance: 'new AppCenterReactNativeAuthPackage(getApplication())'
      }
    }
  }
};
