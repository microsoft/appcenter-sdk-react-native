const fs = require('fs');
const path = require('path');

const plist = require('plist');
const xcode = require('xcode');
const glob = require('glob');
const debug = require('debug')('appcenter-link:ios:AppCenterConfigPlist');



const PbxProject = function (pbxProjectPath) {
    this.pbxProjectPath = pbxProjectPath;
    this.pbxProjectContents = fs.readFileSync(pbxProjectPath, 'utf8');
    debug('Read contents of', pbxProjectPath);
};

PbxProject.prototype.updateFrameworkSearchPaths = function (podsInstallPath) {
    const sourceRoot = path.resolve(path.dirname(this.pbxProjectPath), '..');
    const absolutePodsInstallPath = path.resolve(podsInstallPath);
    const relativePodsInstallPath = path.relative(sourceRoot, absolutePodsInstallPath);
    const frameworkSearchPath = path.join('$(SRCROOT)', relativePodsInstallPath);

    const FRAMEWORK_SEARCH_PATHS_REPLACEMENT_REGEX = /(FRAMEWORK_SEARCH_PATHS = \()(\r?\n\s*)("\$\(SRCROOT\)\/\.\.\/\.\.\/\.\.\/ios\/Pods\/\*\*",)(\r?\n\s*)("\$\(inherited\)",)(\r?\n\s*)\);/g;
    const replacementString = `$1$2$3$4$5$4"${frameworkSearchPath}",$6);`;

    this.pbxProjectContents = this.pbxProjectContents.replace(FRAMEWORK_SEARCH_PATHS_REPLACEMENT_REGEX, replacementString)
};

PbxProject.prototype.save = function () {
    fs.writeFileSync(this.pbxProjectPath, this.pbxProjectContents);
    debug(`Saved updated pbxproject in ${this.pbxProjectPath}`);

    return this.pbxProjectPath;
};

module.exports = PbxProject;
