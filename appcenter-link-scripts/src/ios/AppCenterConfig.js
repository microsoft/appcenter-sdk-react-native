// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

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

    return addConfigToProject(this.plistPath);
};

function addConfigToProject(file) {
    return new Promise(((resolve, reject) => {
        debug(`Trying to add ${file} to XCode project`);

        const globString = 'ios/*.xcodeproj/project.pbxproj';
        const projectPaths = glob.sync(globString, { ignore: 'node_modules/**' });

        if (projectPaths.length !== 1) {
            reject(new Error(`
                Could not locate the xcode project to add AppCenter-Config.plist file to. 
                Looked in paths - 
                ${JSON.stringify(projectPaths)}`));
            return;
        }

        const projectPath = projectPaths[0];
        debug(`Adding ${file} to  ${projectPath}`);

        const project = xcode.project(projectPath);

        project.parse((err) => {
            if (err) {
                reject(err);
                return;
            }
            try {
                const relativeFilePath = path.relative(path.resolve(projectPath, '../..'), file);
                const plistPbxFile = project.addFile(relativeFilePath, project.getFirstProject().firstProject.mainGroup);
                if (plistPbxFile === null) {
                    debug(`Looks like ${file} was already added to ${projectPath}`);
                    resolve(file);
                    return;
                }
                plistPbxFile.uuid = project.generateUuid();
                plistPbxFile.target = project.getFirstTarget().uuid;
                project.addToPbxBuildFileSection(plistPbxFile);
                project.addToPbxResourcesBuildPhase(plistPbxFile);

                fs.writeFileSync(projectPath, project.writeSync());
                debug(`Added ${file} to ${projectPath}`);
            } catch (e) {
                reject(e);
            }
            resolve(file);
        });
    }));
}

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
