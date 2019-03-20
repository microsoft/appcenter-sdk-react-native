// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

const fs = require('fs');
const path = require('path');

const mkdirp = require('mkdirp');
const glob = require('glob');
const debug = require('debug')('appcenter-link:android:AppCenterConfig');

/**
 * Class to get and set values in AppCenter Config file for Android - aka appcenter-config.json
 */
const AppCenterConfig = function (file) {
    this.AppCenterConfigPath = file;
    this.AppCenterConfig = {};
    try {
        this.AppCenterConfig = JSON.parse(fs.readFileSync(this.AppCenterConfigPath), 'utf-8');
        debug('Read contents from ', file);
    } catch (e) {
        debug('Could not find ', e.message);
    }
};

AppCenterConfig.prototype.get = function (key) {
    return this.AppCenterConfig[key];
};

AppCenterConfig.prototype.set = function (key, value) {
    this.AppCenterConfig[key] = value;
};

AppCenterConfig.prototype.save = function () {
    try {
        mkdirp.sync(path.dirname(this.AppCenterConfigPath));
    } catch (e) {
        debug(e.message);
    }
    fs.writeFileSync(this.AppCenterConfigPath, JSON.stringify(this.AppCenterConfig, null, 4));
    debug(`Saved App secret to ${this.AppCenterConfigPath}`);
    return this.AppCenterConfigPath;
};

AppCenterConfig.searchForFile = function (cwd) {
    const AppCenterConfigPaths = glob.sync('**/appcenter-config.json', {
        ignore: ['node_modules/**', '**/build/**'],
        cwd: cwd || process.cwd()
    });
    if (AppCenterConfigPaths.length > 1) {
        debug(AppCenterConfigPaths);
        throw new Error(`Found more than one appcenter-config.json in this project and hence could not write App Secret.
            Please add "app_secret" to the correct appcenter-config.json file
            appcenter-config.json found at ${AppCenterConfigPaths}
        `);
    } else if (AppCenterConfigPaths.length === 1) {
        return AppCenterConfigPaths[0];
    } else {
        return path.join('android', 'app', 'src', 'main', 'assets', 'appcenter-config.json');
    }
};

module.exports = AppCenterConfig;
