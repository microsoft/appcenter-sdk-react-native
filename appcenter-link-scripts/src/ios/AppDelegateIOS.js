const debug = require('debug')('appcenter-link:ios:AppDelegateSwift');

const AppDelegate = require('./AppDelegate');

const AppDelegateIOS = function (file) {
    AppDelegate.call(this, file);
};

AppDelegateIOS.prototype = Object.create(AppDelegate.prototype);
AppDelegateIOS.prototype.constructor = AppDelegateIOS;

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

AppDelegateIOS.prototype.addInitCode = function (code, oldCodeRegExp) {
    const errorMessage = `
        Could not find line "NSURL \*jsCodeLocation;" in file AppDelegate.m.
        Update AppDelegate.m so that text is present, as we match on it and insert '${code}' after for AppCenter SDK integration.
    `;
    AppDelegate.prototype.addInitCode.call(this, code, oldCodeRegExp, /NSURL \*jsCodeLocation;[ \t]*\r*\n/, errorMessage);
};

module.exports = AppDelegateIOS;
