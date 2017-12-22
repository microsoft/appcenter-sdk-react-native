const debug = require('debug')('appcenter-link:ios:AppDelegateSwift');
const glob = require('glob');
const fs = require('fs');
const AppDelegate = require('./AppDelegate');

const bridgingHeaderPaths = glob.sync('**/*-Bridging-Header.h', { ignore: 'node_modules/**' });

const AppDelegateSwift = function (file) {
    AppDelegate.call(this, file);
};

AppDelegateSwift.prototype = Object.create(AppDelegate.prototype);
AppDelegateSwift.prototype.constructor = AppDelegateSwift;

AppDelegateSwift.prototype.addHeader = function (header) {
    try {
        fs.accessSync(bridgingHeaderPaths[0], fs.F_OK);
        let bridgingHeaderContents = fs.readFileSync(bridgingHeaderPaths[0], 'utf-8');
        if (bridgingHeaderContents.indexOf(header) === -1) {
            bridgingHeaderContents += '\n';
            bridgingHeaderContents += header;
            fs.writeFileSync(bridgingHeaderPaths[0], bridgingHeaderContents);
            debug('Added header', header, 'to file', bridgingHeaderPaths[0]);
        } else {
            debug(`"${header}"" header already added to bridging header`);
        }
    } catch (e) {
        console.log(e);
        throw Error(`
            Could not access bridging header. Please check that it exists and that you have rights to read and write into it.
        `);
    }
};

AppDelegateSwift.prototype.addInitCode = function (code, oldCodeRegExp) {
    const errorMessage = `
        Could not find method "application:didFinishLaunchingWithOptions:" in file AppDelegate.swift.
        Update AppDelegate.swift so that the method is present, as we match on it and insert '${code}' after for AppCenter SDK integration.
    `;
    AppDelegate.prototype.addInitCode.call(this, code, oldCodeRegExp, /didFinishLaunchingWithOptions{1}.*\s*\{/, errorMessage);
};

module.exports = AppDelegateSwift;
