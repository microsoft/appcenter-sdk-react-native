const path = require('path');

module.exports = {
  dependency: {
    platforms: {
      ios: {
        podspecPath: path.join(__dirname, 'ios', 'appcenter-push.podspec')
      },
      android: {
        packageInstance: 'new AppCenterReactNativePushPackage(getApplication())'
      }
    }
  }
};
