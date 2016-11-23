var fs = require('fs');
var debug = require('debug')('mobile-center-link:ios:AppDelegate');

var AppDelegate = function (file) {
    this.appDelegatePath = file;
    this.appDelegateContents = fs.readFileSync(this.appDelegatePath, 'utf-8');
    debug('Read contents for appDelegate from ', file);
};

AppDelegate.prototype.addHeader = function (header) {
    if (this.appDelegateContents.indexOf(header) === -1) {
        var pattern = '#import "AppDelegate.h"';
        this.appDelegateContents = this.appDelegateContents.replace(pattern, `${pattern}\n${header}\n`);
        debug('Added header', header, 'to file', this.appDelegatePath);
    } else {
        debug(`"${header}"" header already added to AppDelegate.m`);
    }
};

AppDelegate.prototype.addInitCode = function (code) {
    if (this.appDelegateContents.indexOf(code) === -1) {
        var pattern = this.appDelegateContents.match(/NSURL \*jsCodeLocation;\r*\n/)[0];
        this.appDelegateContents = this.appDelegateContents.replace(pattern, `${pattern}\n${code}\n`);
        debug('Added code', code, 'to file', this.appDelegatePath);
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