

require('../../react-native/setupBabel')();

const reactNativeCliConfig = require('../../react-native/local-cli/core/index');

const GetReactNativeProjectConfig = function () {
    return reactNativeCliConfig.getProjectConfig();
};

module.exports = GetReactNativeProjectConfig;
