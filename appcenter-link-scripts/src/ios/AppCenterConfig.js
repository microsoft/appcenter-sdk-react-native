const fs = require('fs');
const path = require('path');

const plist = require('plist');
const xcode = require('xcode');
const glob = require('glob');
const debug = require('debug')('appcenter-link:ios:AppCenterConfigPlist');

const AppCenterConfigPlist = function (plistPath) {
    this.plistPath = plistPath;
    try {
        const plistContents = fs.readFileSync(plistPath, 'utf8');
        this.parsedInfoPlist = plist.parse(plistContents);
        debug('Read contents of', plistPath);
    } catch (e) {
        debug(`Could not read contents of AppCenter-Config.plist - ${e.message}`);
        this.parsedInfoPlist = plist.parse(plist.build({}));
    }
};

AppCenterConfigPlist.prototype.get = function (key) {
    return this.parsedInfoPlist[key];
};

AppCenterConfigPlist.prototype.set = function (key, value) {
    this.parsedInfoPlist[key] = value;
};

AppCenterConfigPlist.prototype.save = function () {
    const plistContents = plist.build(this.parsedInfoPlist);
    fs.writeFileSync(this.plistPath, plistContents);
    debug(`Saved App Secret in ${this.plistPath}`);
};

AppCenterConfigPlist.searchForFile = function (cwd) {
    const configPaths = glob.sync(path.join(cwd, 'AppCenter-Config.plist').replace(/\\/g, '/'), {
        ignore: 'node_modules/**'
    });
    if (configPaths.length > 1) {
        debug(configPaths);
        throw new Error(`Found more than one AppCenter-Config.plist in this project and hence, could not write App Secret.
            Please add "AppSecret" to the correct AppCenter-Config.plist file
            AppCenter-config.plist found at ${configPaths}
        `);
    } else if (configPaths.length === 1) {
        return configPaths[0];
    } else {
        return path.join(cwd, 'AppCenter-Config.plist');
    }
};

module.exports = AppCenterConfigPlist;
