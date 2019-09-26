module.exports = {
  dependency: {
    platforms: {
      ios: {
        podspecPath: path.join(__dirname, 'ios', 'appcenter-data.podspec')
      },
      android: {
        packageInstance: 'new AppCenterReactNativeDataPackage(getApplication())'
      }
    }
  }
};
