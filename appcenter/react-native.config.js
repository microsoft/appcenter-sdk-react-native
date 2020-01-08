const path = require('path');

module.exports = {
  dependency: {
    platforms: {
      ios: {
        podspecPath: path.join(__dirname, 'ios', 'appcenter-core.podspec')
      },
      android: {
        packageInstance: 'new AppCenterReactNativePackage(getApplication())'
      }
    }
  }
};
