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

PbxProject.prototype.updateFrameworkSearchPaths = function (pathToAdd) {
    if(!pathToAdd.startsWith("\"")) {
        pathToAdd = "\"" + pathToAdd;
    }

    if(!pathToAdd.endsWith("\"")) {
        pathToAdd = pathToAdd + "\"";
    }

    const frameworkSearchPathsPattern = /(\t*)(FRAMEWORK_SEARCH_PATHS = )(\(|")([\s\S]+?)(";|\);)/g;

    const replacer = function (match, leadingWhitespace, startString, parenthesisOrQuote, contents, endString) {
        let isArray = parenthesisOrQuote === '(';

        let existingSearchPaths;
        if(isArray) {
            existingSearchPaths = contents.trim().split(',').map(searchPath => searchPath.trim()).filter(searchPath => searchPath != '');
        } else {
            existingSearchPaths = [`"${contents.trim()}"`];
        }

        if(existingSearchPaths.indexOf(searchPathToAdd) > -1) {
            // path already in search paths, nothing to replace...
            return match;
        }


        existingSearchPaths.push(searchPathToAdd.trim());

        let searchPathSeparator = ',\n' + leadingWhitespace + '\t';
        let searchPaths = existingSearchPaths.join(searchPathSeparator);

        return `${leadingWhitespace}FRAMEWORK_SEARCH_PATHS = (\n${leadingWhitespace}\t${searchPaths}\n${leadingWhitespace});`;
    };

    this.pbxProjectContents = this.pbxProjectContents.replace(frameworkSearchPathsPattern, replacer);
};

PbxProject.prototype.save = function () {
    fs.writeFileSync(this.pbxProjectPath, this.pbxProjectContents);
    debug(`Saved updated pbxproject in ${this.pbxProjectPath}`);

    return this.pbxProjectPath;
};

module.exports = PbxProject;
