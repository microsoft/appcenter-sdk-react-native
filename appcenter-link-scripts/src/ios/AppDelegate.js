const fs = require('fs');
const debug = require('debug')('appcenter-link:ios:AppDelegate');

const AppDelegate = function (file) {
    this.appDelegatePath = file;
    this.appDelegateContents = fs.readFileSync(this.appDelegatePath, 'utf-8');
    debug('Read contents for appDelegate from ', file);
};

AppDelegate.prototype.save = function () {
    fs.writeFileSync(this.appDelegatePath, this.appDelegateContents);
    debug('Saved appDelegate', this.appDelegatePath);
    return this.appDelegatePath;
};

module.exports = AppDelegate;
