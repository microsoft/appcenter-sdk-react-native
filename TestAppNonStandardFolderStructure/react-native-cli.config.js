'use strict';

const fs = require('fs');
const path = require('path');



const blacklist = require('metro/src/blacklist');
const minimist = require('minimist');
const getRNPMConfig = function (folder) { return require(path.join(folder, './package.json')).rnpm || {}; };




const currentWorkingDirectory = process.cwd();


const androidConfigHelper = require(path.join(currentWorkingDirectory, 'node_modules', 'react-native', 'local-cli', 'core', 'android', 'index.js'));
const iosConfigHelper = require(path.join(currentWorkingDirectory, 'node_modules', 'react-native', 'local-cli', 'core', 'ios', 'index.js'));
const windowsConfigHelper = require(path.join(currentWorkingDirectory, 'node_modules', 'react-native', 'local-cli', 'core', 'windows', 'index.js'));

const findAssets = require(path.join(currentWorkingDirectory, 'node_modules', 'react-native', 'local-cli', 'core', 'findAssets.js'));

const wrapCommandsBase = require(path.join(currentWorkingDirectory, 'node_modules', 'react-native', 'local-cli', 'core', 'wrapCommands.js'));
const wrapCommands = function(commandToWrap) {
    const cliArgs = minimist(process.argv.slice(2));
    if(cliArgs.config != null) {
        commandToWrap += ` --config ${cliArgs.config}`;
    }

    return wrapCommandsBase(commandToWrap);
}


module.exports = {
    getProjectConfig() {

        const rnpm = getRNPMConfig(currentWorkingDirectory);

        let projectFolder = path.resolve(currentWorkingDirectory, 'NestedProjectFolder');

        if (!fs.existsSync(projectFolder)) {
            throw new Error(`Can't find the NestedProjectFolder/ project folder`);
        }

        // resolve projects relative to the NestedProjectFolder, 
        // assets relative to the current working directory where the package.json should be located...
        const projectConfig = Object.assign({}, rnpm, {
            android: androidConfigHelper.projectConfig(projectFolder, rnpm.android || {}),
            ios: iosConfigHelper.projectConfig(projectFolder, rnpm.ios || {}),
            windows: windowsConfigHelper.projectConfig(projectFolder, rnpm.windows || {}),
            assets: findAssets(currentWorkingDirectory, rnpm.assets),
        });

        //console.log('Project Config: ', projectConfig);

        return projectConfig;
    },

    getDependencyConfig(packageName) {
        const folder = path.join(process.cwd(), 'node_modules', packageName);
        const rnpm = getRNPMConfig(
          path.join(process.cwd(), 'node_modules', packageName)
        );
    
        return Object.assign({}, rnpm, {
          ios: iosConfigHelper.dependencyConfig(folder, rnpm.ios || {}),
          android: androidConfigHelper.dependencyConfig(folder, rnpm.android || {}),
          windows: windowsConfigHelper.dependencyConfig(folder, rnpm.windows || {}),
          assets: findAssets(folder, rnpm.assets),
          commands: wrapCommands(rnpm.commands),
          params: rnpm.params || [],
        });
    }
};


