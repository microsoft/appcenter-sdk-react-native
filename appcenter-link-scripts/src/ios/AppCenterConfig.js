const fs = require('fs');
const path = require('path');

const plist = require('plist');
const xcode = require('xcode');
const glob = require('glob');
const debug = require('debug')('appcenter-link:ios:AppCenterConfigPlist');

const AppCenterConfigPlist = function (plistPath, pbxprojPath) {
    this.plistPath = plistPath;
    this.pbxprojPath = pbxprojPath;

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

    return addConfigToProject(this.plistPath, this.pbxprojPath);
};

function addConfigToProject(appcenterConfigPlistPath, pbxprojPath) {
    return new Promise(((resolve, reject) => {
        debug(`Trying to add ${appcenterConfigPlistPath} to XCode project`);

        if (!fs.existsSync(pbxprojPath)) {
            reject(new Error(`
                Could not locate the xcode project to add AppCenter-Config.plist file to. 
                Expected project.pbxproj path: ${pbxprojPath}`));
            return;
        }

        debug(`Adding ${appcenterConfigPlistPath} to  ${pbxprojPath}`);

        const project = xcode.project(pbxprojPath);

        project.parse((err) => {
            if (err) {
                reject(err);
                return;
            }
            try {
                const relativeFilePath = path.relative(path.resolve(pbxprojPath, '../..'), appcenterConfigPlistPath);
                const plistPbxFile = project.addFile(relativeFilePath, project.getFirstProject().firstProject.mainGroup);
                if (plistPbxFile === null) {
                    debug(`Looks like ${appcenterConfigPlistPath} was already added to ${pbxprojPath}`);
                    resolve(appcenterConfigPlistPath);
                    return;
                }
                plistPbxFile.uuid = project.generateUuid();
                plistPbxFile.target = project.getFirstTarget().uuid;
                project.addToPbxBuildFileSection(plistPbxFile);
                project.addToPbxResourcesBuildPhase(plistPbxFile);

                fs.writeFileSync(pbxprojPath, project.writeSync());
                debug(`Added ${appcenterConfigPlistPath} to ${pbxprojPath}`);
            } catch (e) {
                reject(e);
            }
            resolve(appcenterConfigPlistPath);
        });
    }));
}

AppCenterConfigPlist.searchForFile = function (iosProjectConfig) {
    const iosProjectFolderName = iosProjectConfig.projectName.replace('.xcodeproj', '');
    const iosProjectSourceDirectory = iosProjectConfig.sourceDir;

    const AppCenterConfigPaths = glob.sync('**/AppCenter-Config.plist', {
        ignore: ['node_modules/**', '**/build/**'],
        cwd: iosProjectSourceDirectory || process.cwd()
    });

    if (AppCenterConfigPaths.length > 1) {
        debug(AppCenterConfigPaths);
        throw new Error(`Found more than one AppCenter-Config.plist in this project and hence, could not write App Secret.
            Please add "AppSecret" to the correct AppCenter-Config.plist file
            AppCenter-config.plist found at ${AppCenterConfigPaths}
        `);
    } else if (AppCenterConfigPaths.length === 1) {
        return path.resolve(iosProjectSourceDirectory || process.cwd(), AppCenterConfigPaths[0]);
    } else {
        return path.resolve(iosProjectSourceDirectory, iosProjectFolderName, 'AppCenter-Config.plist');
    }
};

module.exports = AppCenterConfigPlist;
