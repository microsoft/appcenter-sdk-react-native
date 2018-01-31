const fs = require('fs');
const path = require('path');
const minimist = require('minimist');

let reactNativeCliConfig;

const setupBabelPath = path.join(__dirname, '..', '..', 'react-native', 'setupBabel.js');
if(fs.existsSync(setupBabelPath)) {
    require('../../react-native/setupBabel')();

    reactNativeCliConfig = require('../../react-native/local-cli/core/index');
} else {
    // for older versions of react-native, e.g v 0.34
    require('../../react-native/packager/babelRegisterOnly')([
        /private-cli\/src/,
        /local-cli/,
        /react-packager\/src/
    ]);


    const RNConfig = require('../../react-native/local-cli/util/Config');
    const defaultConfig = require('../../react-native/local-cli/default.config');

    // Use a lightweight option parser to look up the CLI configuration file,
    // which we need to set up the parser for the other args and options
    let cliArgs = minimist(process.argv.slice(2));

    let cwd;
    let configPath;
    if (cliArgs.config != null) {
        cwd = process.cwd();
        configPath = cliArgs.config;
    } else {
        cwd = __dirname;
        configPath = RNConfig.findConfigPath(cwd);
    }

    reactNativeCliConfig = RNConfig.get(cwd, defaultConfig, configPath);
}

const GetReactNativeProjectConfig = function () {
    return reactNativeCliConfig.getProjectConfig();
};

module.exports = GetReactNativeProjectConfig;
