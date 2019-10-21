// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

const fs = require('fs');
const os = require('os');
const path = require('path');
const childProcess = require('child_process');

const glob = require('glob');
const which = require('which').sync;
const debug = require('debug')('appcenter-link:ios:Podfile');

const Podfile = function (file) {
    debug(`Podfile located at ${file}`);
    this.file = file;
    this.fileContents = fs.readFileSync(file, 'utf-8');
};

Podfile.prototype.eraseOldLines = function () {
    this.fileContents = this.fileContents.replace(new RegExp("pod 'AppCenter'.*"), '');
};

Podfile.prototype.addPodLine = function (pod, version) {
    debug(`addPodLine pod=${pod} version=${version}`);
    const line = `pod '${pod}', '~> ${version}'`;
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
    const pattern = this.getTargetSectionPattern().match;
    this.fileContents = this.fileContents.replace(pattern, `${pattern}\n  ${line}`);
    debug(`${line} ===> ${this.file}`);
};

Podfile.prototype.addMinimumDeploymentTarget = function (platform, version) {
    try {
        const isGlobalPlatformDefined = this.isGlobalPlatformDefined(platform);
        if (!isGlobalPlatformDefined) {
            const isTargetPlatformDefined = this.isTargetPlatformDefined(platform);
            if (!isTargetPlatformDefined) {
                this.addPlatformToTarget(platform, version);
            }
        }
    } catch (e) {
        const line = `platform :${platform}, '${version}'`;
        console.log(`Could not automatically add line: ${line} to your Podfile. Please make sure that this line is present.`, e);
    }
};

Podfile.prototype.addPlatformToTarget = function (platform, version) {
    const line = `platform :${platform}, '${version}'`;
    const sectionStart = this.getTargetSectionPattern();
    const keywordMatch = this.nextKeyword(this.fileContents, sectionStart.index + 1);
    const newLineIndex = this.fileContents.lastIndexOf(os.EOL, keywordMatch.index);
    const part1 = this.fileContents.slice(0, newLineIndex);
    const part2 = this.fileContents.slice(newLineIndex);
    const indent = '  ';
    this.fileContents = `${part1}\n${indent}${line}${part2}`;
};

Podfile.prototype.isGlobalPlatformDefined = function (platform) {
    const platformPattern = new RegExp(`platform\\s+:${platform}`, 'g');
    const platformMatches = this.fileContents.match(platformPattern);
    if (!platformMatches) {
        return false;
    }
    const globalRanges = this.scopeRanges(this.fileContents);
    let platformIndex = 0;
    for (let match = 0; match < platformMatches.length; match++) {
        platformIndex = this.fileContents.indexOf(platformMatches[match], platformIndex + 1);
        if (this.isInScope(platformIndex, globalRanges) && this.isLineActive(this.fileContents, platformIndex)) {
            return true;
        }
    }
    return false;
};

Podfile.prototype.isTargetPlatformDefined = function (platform) {
    const platformPattern = new RegExp(`platform\\s+:${platform}`, 'g');
    const patternIndex = this.getTargetSectionPattern().index;
    const startSection = this.startOfSection(patternIndex);
    const endSection = this.endOfSection(patternIndex + 'target'.length);
    const sectionContent = this.fileContents.substring(startSection, endSection);
    const scopeRanges = this.scopeRanges(sectionContent);
    const platformMatches = sectionContent.match(platformPattern);
    if (!platformMatches) {
        return false;
    }
    let platformIndex = 0;
    for (let match = 0; match < platformMatches.length; match++) {
        platformIndex = sectionContent.indexOf(platformMatches[match], platformIndex + 1);
        if (this.isInScope(platformIndex, scopeRanges) && this.isLineActive(sectionContent, platformIndex)) {
            return true;
        }
    }
    return false;
};

Podfile.prototype.getTargetSectionPattern = function () {
    const projectDirectory = path.resolve(__dirname, '..', '..', '..', '..');
    const projectName = projectDirectory.substr(projectDirectory.lastIndexOf(path.sep) + 1);
    const reg = RegExp(`target\\s+'${projectName}'\\s+do`);
    const targetPatterns = this.fileContents.match(reg);
    if (targetPatterns === null) {
        debug(`Could not find target ${reg}`);
        const patterns = this.fileContents.match(/# Pods for .*/);
        if (patterns === null) {
            throw new Error(`
        Error: Could not find a "# Pods for" comment in your Podfile. Please add a "# Pods for AppCenter" line
        in ${this.file}, inside
        the "target" section, then rerun the react-native link. AppCenter pods will be added below the comment.
        `);
        }
        const sectionStart = this.startOfSection(patterns.index);
        const targetSection = this.fileContents.substring(sectionStart, this.fileContents.indexOf(os.EOL, sectionStart));
        return { match: targetSection, index: sectionStart };
    }
    return { match: targetPatterns[0], index: targetPatterns.index };
};

Podfile.prototype.isInScope = function (index, scopeRanges) {
    for (let scope = 0; scope < scopeRanges.length; scope++) {
        if (index > scopeRanges[scope].start && index < scopeRanges[scope].end) {
            return true;
        }
    }
    return false;
};

Podfile.prototype.isLineActive = function (content, index) {
    if (index < 0) {
        return false;
    }
    const commentSymbol = '#';
    const newlineSymbol = '\n';
    let diff = 1;
    let previousCharacter = content.charAt(index - (diff++));
    while (previousCharacter !== commentSymbol && previousCharacter !== newlineSymbol) {
        previousCharacter = content.charAt(index - (diff++));
    }
    return previousCharacter !== commentSymbol;
};

Podfile.prototype.scopeRanges = function (content) {
    const targetKeyword = 'target';

    const result = [];
    const targetsStack = [];
    const currentRange = { start: 0, end: 0 };
    let currentOffset = 0;

    let keywordMatch = this.nextKeyword(content, currentOffset);
    while (keywordMatch.index >= 0) {
        if (keywordMatch.keyword === targetKeyword) {
            currentRange.end = keywordMatch.index - 1;
            if (targetsStack.length === 0) {
                result.push({ ...currentRange });
            }
            targetsStack.push(keywordMatch.index);
        } else {
            targetsStack.pop();
            if (targetsStack.length === 0) {
                currentRange.start = keywordMatch.index + keywordMatch.keyword.length;
            }
        }
        currentOffset = keywordMatch.index + keywordMatch.keyword.length;
        keywordMatch = this.nextKeyword(content, currentOffset);
    }
    currentRange.end = content.length;
    result.push({ ...currentRange });
    return result;
};

Podfile.prototype.sectionBoundary = function (content, position, keyword) {
    const targetKeyword = 'target';
    let reverse = false;
    let direction = 1;
    if (keyword === targetKeyword) {
        reverse = true;
        direction = -1;
    }
    const keywordStack = [];
    let keywordMatch = this.nextKeyword(content, position, reverse);
    while (keywordMatch.index >= 0) {
        if (keywordMatch.keyword !== keyword) {
            keywordStack.push(keywordMatch.keyword);
        } else if (keywordStack.length === 0) {
            break;
        } else {
            keywordStack.pop();
        }
        keywordMatch = this.nextKeyword(content, keywordMatch.index + (keywordMatch.keyword.length * direction), reverse);
    }
    return keywordMatch.index;
 };

Podfile.prototype.endOfSection = function (position) {
    const endKeyword = 'end';
    return this.sectionBoundary(this.fileContents, position - endKeyword.length, endKeyword);
};

Podfile.prototype.startOfSection = function (position) {
    const targetKeyword = 'target';
    return this.sectionBoundary(this.fileContents, position + targetKeyword.length, targetKeyword);
};

Podfile.prototype.nextKeyword = function (content, offset, reverse = false) {
    const targetKeyword = 'target';
    const endKeyword = 'end';

    const targetIndex = reverse
        ? content.lastIndexOf(targetKeyword, offset)
        : content.indexOf(targetKeyword, offset);
    const endIndex = reverse
        ? content.lastIndexOf(endKeyword, offset)
        : content.indexOf(endKeyword, offset);

    if (reverse) {
        return targetIndex < endIndex
            ? { keyword: endKeyword, index: endIndex }
            : { keyword: targetKeyword, index: targetIndex };
    }
    if (targetIndex < 0) {
        return { keyword: endKeyword, index: endIndex };
    }
    return targetIndex < endIndex
        ? { keyword: targetKeyword, index: targetIndex }
        : { keyword: endKeyword, index: endIndex };
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
