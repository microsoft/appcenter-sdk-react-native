var fs = require('fs');
var path = require('path');
var childProcess = require('child_process');

var glob = require('glob');
var which = require('which').sync;
var debug = require('debug')('sonoma-link:ios:PodFile');

var PodFile = function (file) {
    debug(`PodFile located at ${file}`);
    this.file = file;
    this.fileContents = fs.readFileSync(file, 'utf-8');
};

PodFile.prototype.addPodLine = function (pod, podspec, version) {
    if (this.fileContents.match(new RegExp(`pod\\s+'${pod}'`))) {
        debug(`${pod} already present in ${this.file}`);
        return;
    }
    var line = `pod '${pod}'`;
    if (podspec) {
        line = `${line}, :podspec => '${podspec}'`;
    } else if (version) {
        line = `${line}, '~> ${version}'`;
    }
    var pattern = this.fileContents.match(/end[\r\n\s]*$/)[0];
    this.fileContents = this.fileContents.replace(pattern, `${line}\n${pattern}`);
    debug(`${line} ===> ${this.file}`);
};

PodFile.prototype.save = function () {
    fs.writeFileSync(this.file, this.fileContents);
    debug(`Saved ${this.file}`);
    return this.file;
};

PodFile.prototype.install = function () {
    var cwd = path.dirname(this.file)
    return new Promise(function (resolve, reject) {
        debug(`Installing pods in ${this.file}`);
        var process = childProcess.spawn('pod', ['install'], {
            cwd: cwd,
            stdio: 'inherit'
        });

        process.on('close', resolve);
        process.on('error', reject);
    });
};

PodFile.searchForFile = function (cwd) {
    var podFilePaths = glob.sync(path.join(cwd, 'PodFile'), { ignore: "node_modules/**" });
    if (podFilePaths.length === 1) {
        return podFilePaths[0];
    } else if (podFilePaths.length === 0) {
        debug(`No podfile found in ${cwd}`);
        childProcess.execSync('pod init', { cwd: cwd });
        return path.join(cwd, 'PodFile');
    }
};

PodFile.isCocoaPodsInstalled = function () {
    try {
        return which('pod');
    } catch (e) {
        debug(`Error with CocoaPods - ${e}`);
        return false;
    }
};

module.exports = PodFile;