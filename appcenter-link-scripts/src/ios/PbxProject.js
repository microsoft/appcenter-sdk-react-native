const fs = require('fs');
const debug = require('debug')('appcenter-link:ios:AppCenterConfigPlist');


const PbxProject = function (pbxProjectPath) {
    this.pbxProjectPath = pbxProjectPath;
    this.pbxProjectContents = fs.readFileSync(pbxProjectPath, 'utf8');
    debug('Read contents of', pbxProjectPath);
};

PbxProject.prototype.updateFrameworkSearchPaths = function (searchPathToAdd) {
    if (!searchPathToAdd.startsWith('"')) {
        searchPathToAdd = `"${searchPathToAdd}`;
    }

    if (!searchPathToAdd.endsWith('"')) {
        searchPathToAdd += '"';
    }

    const frameworkSearchPathsPattern = /(\t*)(FRAMEWORK_SEARCH_PATHS = )(\(|")([\s\S]+?)(";|\);)/g;

    const replacer = function (match, leadingWhitespace, startString, parenthesisOrQuote, contents) {
        const isArray = parenthesisOrQuote === '(';

        let existingSearchPaths;
        if (isArray) {
            existingSearchPaths = contents.trim().split(',').map(searchPath => searchPath.trim()).filter(searchPath => searchPath !== '');
        } else {
            existingSearchPaths = [`"${contents.trim()}"`];
        }

        if (existingSearchPaths.indexOf(searchPathToAdd) > -1) {
            // path already in search paths, nothing to replace...
            return match;
        }

        existingSearchPaths.push(searchPathToAdd.trim());

        const searchPathSeparator = `,\n${leadingWhitespace}\t`;
        const searchPaths = existingSearchPaths.join(searchPathSeparator);

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
