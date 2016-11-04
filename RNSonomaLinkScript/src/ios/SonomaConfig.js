var fs = require('fs');
var path = require('path');

var plist = require('plist');
var xcode = require('xcode');
var glob = require('glob');
var debug = require('debug')('sonoma-link:ios:SonomaConfigPlist');

var SonomaConfigPlist = function (plistPath) {
    this.plistPath = plistPath;
    try {
        var plistContents = fs.readFileSync(plistPath, "utf8");
        this.parsedInfoPlist = plist.parse(plistContents);
        debug('Read contents of', plistPath);
    }
    catch (e) {
        debug(`Could not read contents of Sonoma-Config.plist - ${e.message}`);
        this.parsedInfoPlist = plist.parse(plist.build({}));
    }
};

SonomaConfigPlist.prototype.get = function (key) {
    return this.parsedInfoPlist[key];
};

SonomaConfigPlist.prototype.set = function (key, value) {
    this.parsedInfoPlist[key] = value;
};

SonomaConfigPlist.prototype.save = function (key, value) {
    var plistContents = plist.build(this.parsedInfoPlist);
    fs.writeFileSync(this.plistPath, plistContents);
    debug(`Saved App Secret in ${this.plistPath}`);

    return addConfigToProject(this.plistPath);
};

function addConfigToProject(file) {
    var pjson = require(path.join(process.cwd(), './package.json'));
    return new Promise(function (resolve, reject) {
        debug(`Trying to add ${file} to XCode project`);

        var globString = `**/${(pjson && pjson.name ? pjson.name : '*')}.xcodeproj/project.pbxproj`
        var projectPaths = glob.sync(globString, { ignore: 'node_modules/**' });

        if (projectPaths.length !== 1) {
            reject(new Error(`
                Could not locate the xcode project to add Sonoma-Config.plist file to. 
                Looked in paths - 
                ${JSON.stringify(projectPaths)}`
            ));
            return;
        }

        var projectPath = projectPaths[0];
        debug(`Adding ${file} to  ${projectPath}`);

        var project = xcode.project(projectPath);

        project.parse(function (err) {
            if (err) {
                reject(err);
                return;
            }
            try {
                var relativeFilePath = path.relative(path.resolve(projectPath, '../..'), file);
                var plistPbxFile = project.addFile(relativeFilePath, project.getFirstProject().firstProject.mainGroup);
                if (plistPbxFile === null) {
                    debug(`Looks like ${file} was already added to ${this.projectPath}`);
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
    });
}

SonomaConfigPlist.searchForFile = function (cwd) {
    var configPaths = glob.sync(path.join(cwd, 'Sonoma-Config.plist').replace(/\\/g, '/'), {
        ignore: "node_modules/**"
    });
    if (configPaths.length > 1) {
        debug(sonomaConfigPaths);
        throw new Error(`Found more than one Sonoma-Config.plist in this project and hence, could not write App Secret.
            Please add "AppSecret" to the correct Sonoma-Config.plist file
            sonoma-config.json found at ${sonomaConfigPaths}
        `);
    }
    else if (configPaths.length === 1) {
        return configPaths[0];
    } else if (configPaths.length === 0) {
        return path.join(cwd, "Sonoma-Config.plist");
    }
};

module.exports = SonomaConfigPlist;