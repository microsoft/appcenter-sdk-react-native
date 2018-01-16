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

    const frameworkSearchPathsPattern = /(\s*)(FRAMEWORK_SEARCH_PATHS = )(\(|")([\s\S]+?)(";|\);)/g;

    const replacer = (match, leadingWhitespace, startString, parenthesisOrQuote, contents, endString, offset) => {
        let isArray = parenthesisOrQuote === '(';

        let existingSearchPaths;
        if(isArray) {
            existingSearchPaths = contents.split('\n').map(searchPath => searchPath.trim());
        } else {
            existingSearchPaths = [`"${contents}"`];
        }

        if(existingSearchPaths.indexOf(pathToAdd) > -1) {
            // path already in search paths, nothing to replace...
            return match;
        }

        existingSearchPaths.push(pathToAdd);

        let searchPathLineStart = '\n' + leadingWhitespace + '\t';
        let searchPaths = existingSearchPaths.join(searchPathLineStart);

        return `${leadingWhitespace}FRAMEWORK_SEARCH_PATHS = (${searchPathLineStart}${searchPaths}\n${leadingWhitespace});`;
    };

    this.pbxProjectContents = this.pbxProjectContents.replace(frameworkSearchPathsPattern, replacer);
};

PbxProject.prototype.save = function () {
    fs.writeFileSync(this.pbxProjectPath, this.pbxProjectContents);
    debug(`Saved updated pbxproject in ${this.pbxProjectPath}`);

    return this.pbxProjectPath;
};

module.exports = PbxProject;
