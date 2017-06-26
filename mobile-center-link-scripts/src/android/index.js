var fs = require('fs');

var glob = require('glob');
var inquirer = require('inquirer');

var MobileCenterConfig = require('./MobileCenterConfig');
module.exports = {
    initMobileCenterConfig: function (alwaysPromptForAppSecret) {
        var config = new MobileCenterConfig(MobileCenterConfig.searchForFile());
        var currentAppSecret = config.get('app_secret');

        // We always prompt for the app secret for the mobile-center package. For other packages, we only prompt for
        // it if not already set. That way we minimize prompts in the normal getting started scenario, but still allow
        // updating the app secret for the app, if the user really wants, by them redo'ing the link on mobile-center
        if (currentAppSecret && !alwaysPromptForAppSecret)
            return Promise.resolve(null);

        return inquirer.prompt([{
            type: 'input',
            default: currentAppSecret,
            message: 'What is the Android App Secret?',
            name: 'app_secret',
        }]).then(function (answers) {
            config.set('app_secret', answers['app_secret']);
            var file = config.save();
            console.log('App Secret for Android written to ' + file);
            return file;
        });
    }
};
