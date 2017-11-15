const fs = require('fs');
const debug = require('debug')('appcenter-link:ios:AppDelegate');

const AppDelegate = function (file) {
    this.appDelegatePath = file;
    this.appDelegateContents = fs.readFileSync(this.appDelegatePath, 'utf-8');
    debug('Read contents for appDelegate from ', file);
};

AppDelegate.prototype.addHeader = function (header) {
    if (this.appDelegateContents.indexOf(header) === -1) {
        const match = this.appDelegateContents.match(/#import "AppDelegate.h"[ \t]*\r*\n/);
        if (match === null) {
            throw Error(`
        Could not find line '#import "AppDelegate.h"' in file AppDelegate.m.
        Update AppDelegate.m so that text is present, as we match on it and insert '${header}' after for AppCenter SDK integration.
`);
        }

        const existingLine = match[0];
        this.appDelegateContents = this.appDelegateContents.replace(existingLine, `${existingLine}${header}\n`);
        debug('Added header', header, 'to file', this.appDelegatePath);
    } else {
        debug(`"${header}"" header already added to AppDelegate.m`);
    }
};

AppDelegate.prototype.addInitCode = function (code, oldCodeRegExp) {
    const oldCodeMatches = oldCodeRegExp ? this.appDelegateContents.match(oldCodeRegExp) : null;
    if (oldCodeMatches && oldCodeMatches.length > 1) {
        for (let i = 1; i < oldCodeMatches.length; i++) {
            this.appDelegateContents = this.appDelegateContents.replace(oldCodeMatches[i], '');
            debug(`Removed duplicate ${oldCodeRegExp} lines from AppDelegate.m`);
        }
    }
    if (this.appDelegateContents.indexOf(code) === -1) {
        /* If new code not found but old code found, replace. */
        if (oldCodeMatches) {
            this.appDelegateContents = this.appDelegateContents.replace(oldCodeMatches[0], code);
            debug('Replaced code', code, 'to file', this.appDelegatePath);
        } else {
            const match = this.appDelegateContents.match(/NSURL \*jsCodeLocation;[ \t]*\r*\n/);
            if (match === null) {
                throw Error(`
        Could not find line "NSURL \*jsCodeLocation;" in file AppDelegate.m.
        Update AppDelegate.m so that text is present, as we match on it and insert '${code}' after for AppCenter SDK integration.
`);
            }

            const existingLine = match[0];
            this.appDelegateContents = this.appDelegateContents.replace(existingLine, `${existingLine}\n${code}\n`);
            debug('Added code', code, 'to file', this.appDelegatePath);
        }
    } else {
        debug(`Looks like ${code} is already added to AppDelegate.m`);
    }
};

AppDelegate.prototype.save = function () {
    fs.writeFileSync(this.appDelegatePath, this.appDelegateContents);
    debug('Saved appDelegate', this.appDelegatePath);
    return this.appDelegatePath;
};

module.exports = AppDelegate;
