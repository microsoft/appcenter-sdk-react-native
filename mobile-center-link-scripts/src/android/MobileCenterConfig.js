var fs = require('fs');
var path = require('path');

var mkdirp = require('mkdirp');
var glob = require('glob');
var debug = require('debug')('mobilecenter-link:android:MobileCenterConfig');

/**
 * Class to get and set values in MobileCenter Config file for Android - aka mobilecenter-config.json
 */
var MobileCenterConfig = function (file) {
    this.MobileCenterConfigPath = file;
    this.MobileCenterConfig = {}
    try {
        this.MobileCenterConfig = JSON.parse(fs.readFileSync(this.MobileCenterConfigPath), 'utf-8');
        debug('Read contents from ', file);
    } catch (e) {
        debug('Could not find ', e.message);
    }
};

MobileCenterConfig.prototype.get = function (key) {
    return this.MobileCenterConfig[key];
};

MobileCenterConfig.prototype.set = function (key, value) {
    this.MobileCenterConfig[key] = value;
};

MobileCenterConfig.prototype.save = function () {
    try{
        mkdirp.sync(path.dirname(this.MobileCenterConfigPath));
    } catch(e){
        debug(e.message);
    }
    fs.writeFileSync(this.MobileCenterConfigPath, JSON.stringify(this.MobileCenterConfig, null, 4));
    debug(`Saved App secret to ${this.MobileCenterConfigPath}`);
    return this.MobileCenterConfigPath;
};

MobileCenterConfig.searchForFile = function (cwd) {
    var MobileCenterConfigPaths = glob.sync('**/mobilecenter-config.json', {
        ignore: ['node_modules/**', '**/build/**'],
        cwd: cwd || process.cwd()
    });
    if (MobileCenterConfigPaths.length > 1) {
        debug(MobileCenterConfigPaths);
        throw new Error(`Found more than one mobilecenter-config.json in this project and hence, could not write App Secret.
            Please add "app_secret" to the correct mobilecenter-config.json file
            mobilecenter-config.json found at ${MobileCenterConfigPaths}
        `);
    }
    else if (MobileCenterConfigPaths.length === 1) {
        return MobileCenterConfigPaths[0];
    } else {
        return path.join('android', 'app', 'src', 'main', 'assets', 'mobilecenter-config.json');
    }
};

module.exports = MobileCenterConfig;
