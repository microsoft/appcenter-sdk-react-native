const fs = require('fs');
const path = require('path');
const childProcess = require('child_process');

const glob = require('glob');
const which = require('which').sync;
const debug = require('debug')('mobile-center-link:ios:Podfile');

const Podfile = function (file) {
    debug(`Podfile located at ${file}`);
    this.file = file;
    this.fileContents = fs.readFileSync(file, 'utf-8');
};

Podfile.prototype.eraseOldLines = function () {
    this.fileContents = this.fileContents.replace(new RegExp("pod 'MobileCenter'.*"), '');
};

Podfile.prototype.addPodLine = function (pod, podspec, version) {
    debug(`addPodLine pod=${pod} podspec=${podspec} version=${version}`);
    let line = `pod '${pod}'`;
    if (podspec) {
        line = `${line}, :podspec => '${podspec}'`;
    } else if (version) {
        line = `${line}, '~> ${version}'`;
    }
    const podLinePattern = new RegExp(`pod\\s+'${pod}'.*`);
    const existingLines = this.fileContents.match(podLinePattern);
    if (existingLines) {
        const existingLine = existingLines[0];
        if (existingLine.match(':path')) {
            debug(`${pod} set to specific path, not updating it.`);
            return;
        }
        debug(`${pod} already present in ${this.file}: ${existingLine}, updating it`);
        this.fileContents = this.fileContents.replace(podLinePattern, line);
        debug(`Replace ${existingLine} by ${line}`);
        return;
    }
    const patterns = this.fileContents.match(/# Pods for .*/);
    if (patterns === null) {
        throw new Error(
    `
    Error: Could not find a "# Pods for" comment in your Podfile. Please add a "# Pods for Mobile Center" line
    in ${this.file}, inside
    the "target" section, then rerun the react-native link. Mobile Center pods will be added below the comment.
    `);
    }
    const pattern = patterns[0];
    this.fileContents = this.fileContents.replace(pattern, `${pattern}\n  ${line}`);
    debug(`${line} ===> ${this.file}`);
};

Podfile.prototype.save = function () {
    fs.writeFileSync(this.file, this.fileContents);
    debug(`Saved ${this.file}`);
    return this.file;
};

Podfile.prototype.install = function () {
    const cwd = path.dirname(this.file);
    return new Promise((resolve, reject) => {
        debug(`Installing pods in ${this.file}`);
        const process = childProcess.spawn('pod', ['install'], {
            cwd,
            stdio: 'inherit'
        });

        process.on('close', resolve);
        process.on('error', reject);
    });
};

Podfile.searchForFile = function (cwd) {
    const podFilePaths = glob.sync(path.join(cwd, 'Podfile'), { ignore: 'node_modules/**' });
    if (podFilePaths.length > 1) {
        debug(podFilePaths);
        throw new Error('Found more than one Podfile in this project');
    } else if (podFilePaths.length === 1) {
        return podFilePaths[0];
    } else {
        debug(`No podfile found in ${cwd}`);
        childProcess.execSync('pod init', { cwd });
        // Remove tests sub-specification or it breaks the project
        const podfile = path.join(cwd, 'Podfile');
        const contents = fs.readFileSync(podfile, { encoding: 'utf-8' });
        const subspecRegex = /target '[^']*Tests' do[\s\S]*?end/m;
        fs.writeFileSync(podfile, contents.replace(subspecRegex, ''));
        return podfile;
    }
};

Podfile.isCocoaPodsInstalled = function () {
    try {
        return which('pod');
    } catch (e) {
        debug(`Error with CocoaPods - ${e}`);
        return false;
    }
};

module.exports = Podfile;
