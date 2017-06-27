var fs = require('fs');

var glob = require('glob');
var inquirer = require('inquirer');

var MobileCenterConfig = require('./MobileCenterConfig');
module.exports = {
    initMobileCenterConfig: function (alwaysPromptForAppSecret) {
        var config = new MobileCenterConfig(MobileCenterConfig.searchForFile());
        var currentAppSecret = config.get('app_secret');

        // If an app secret is already set, don't prompt again, instead give the user instructions on how they can change it themselves
        // if they want
        if (currentAppSecret) {
            console.log(`Android App Secret is '${currentAppSecret}' set in ${config.MobileCenterConfigPath}`);
            return Promise.resolve(null);
        }

        return inquirer.prompt([{
            type: 'input',
            default: currentAppSecret,
            message: 'What is the Android App Secret?',
            name: 'app_secret',
        }]).then(function (answers) {
            config.set('app_secret', answers['app_secret']);
            var file = config.save();
            console.log(`App Secret for Android written to ${file}`);
            return file;
        });
    }
};
