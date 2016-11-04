var fs = require('fs');
var path = require('path');

var mkdirp = require('mkdirp');
var glob = require('glob');
var debug = require('debug')('sonoma-link:android:SonomaConfig');

/**
 * Class to get and set values in Sonoma Config file for Android - aka sonoma-config.json
 */
var SonomaConfig = function (file) {
    this.sonomaConfigPath = file;
    this.sonomaConfig = {}
    try {
        this.sonomaConfig = JSON.parse(fs.readFileSync(this.sonomaConfigPath), 'utf-8');
        debug('Read contents from ', file);
    } catch (e) {
        debug('Could not find ', e.message);
    }
};

SonomaConfig.prototype.get = function (key) {
    return this.sonomaConfig[key];
};

SonomaConfig.prototype.set = function (key, value) {
    this.sonomaConfig[key] = value;
};

SonomaConfig.prototype.save = function () {
    try{
        mkdirp.sync(path.dirname(this.sonomaConfigPath));
    } catch(e){
        debug(e.message);
    }
    fs.writeFileSync(this.sonomaConfigPath, JSON.stringify(this.sonomaConfig, null, 4));
    debug(`Saved App secret to ${this.sonomaConfigPath}`);
    return this.sonomaConfigPath;
};

SonomaConfig.searchForFile = function (cwd) {
    var sonomaConfigPaths = glob.sync('**/sonoma-config.json', {
        ignore: ['node_modules/**', '**/build/**'],
        cwd: cwd || process.cwd()
    });
    if (sonomaConfigPaths.length > 1) {
        debug(sonomaConfigPaths);
        throw new Error(`Found more than one sonoma-config.json in this project and hence, could not write App Secret.
            Please add "app_secret" to the correct sonoma-config.json file
            sonoma-config.json found at ${sonomaConfigPaths}
        `);
    }
    else if (sonomaConfigPaths.length === 1) {
        return sonomaConfigPaths[0];
    } else {
        return path.join('android', 'app', 'src', 'main', 'assets', 'sonoma-config.json');
    }
};

module.exports = SonomaConfig;
