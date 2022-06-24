const path = require('path');

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
