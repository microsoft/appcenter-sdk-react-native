module.exports = {
  dependency: {
    platforms: {
      ios: {},
      android: {
        packageInstance: 'new AppCenterReactNativeCrashesPackage(getApplication(), getResources().getString(R.string.appCenterCrashes_whenToSendCrashes))'
      }
    }
  }
};
