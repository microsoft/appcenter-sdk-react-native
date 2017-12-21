const fs = require('fs');
const debug = require('debug')('appcenter-link:ios:AppDelegateSwift');

const AppDelegate = require('./AppDelegate');

const AppDelegateSwift = function (file) {
    AppDelegate.call(this, file);
};

AppDelegateSwift.prototype = Object.create(AppDelegate.prototype);
AppDelegateSwift.prototype.constructor = AppDelegateSwift;

AppDelegateSwift.prototype.addHeader = function (header) {
    console.log('Adding header to swift');
};

AppDelegate.prototype.addInitCode = function (code, oldCodeRegExp) {
    const oldCodeMatches = oldCodeRegExp ? this.appDelegateContents.match(oldCodeRegExp) : null;
    if (oldCodeMatches && oldCodeMatches.length > 1) {
        for (let i = 1; i < oldCodeMatches.length; i++) {
            this.appDelegateContents = this.appDelegateContents.replace(oldCodeMatches[i], '');
            debug(`Removed duplicate ${oldCodeRegExp} lines from AppDelegate.swift`);
        }
    }
    if (this.appDelegateContents.indexOf(code) === -1) {
        /* If new code not found but old code found, replace. */
        if (oldCodeMatches) {
            this.appDelegateContents = this.appDelegateContents.replace(oldCodeMatches[0], code);
            debug('Replaced code', code, 'to file', this.appDelegatePath);
        } else {
            const match = this.appDelegateContents.match(/didFinishLaunchingWithOptions{1}.*\{/);
            if (match === null) {
                throw Error(`
                    Could not find method "application:didFinishLaunchingWithOptions:" in file AppDelegate.swift.
                    Update AppDelegate.swift so that the method is present, as we match on it and insert '${code}' after for AppCenter SDK integration.
                `);
            }

            console.log('Match found = ', match[0]);

            const existingLine = match[0];
            this.appDelegateContents = this.appDelegateContents.replace(existingLine, `${existingLine}\n${code}\n`);
            debug('Added code', code, 'to file', this.appDelegatePath);
        }
    } else {
        debug(`Looks like ${code} is already added to AppDelegate.swift`);
    }
};

module.exports = AppDelegateSwift;
