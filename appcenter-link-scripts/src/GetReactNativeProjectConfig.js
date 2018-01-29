

const getReactNativeCliConfig = require('../../react-native/local-cli/core/index');

const GetReactNativeProjectConfig = function () {
    return getReactNativeCliConfig().getProjectConfig();
};

module.exports = GetReactNativeProjectConfig;
