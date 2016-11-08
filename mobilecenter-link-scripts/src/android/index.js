var fs = require('fs');

var glob = require('glob');
var inquirer = require('inquirer');

var MobileCenterConfig = require('./MobileCenterConfig');
module.exports = {
    initMobileCenterConfig: function () {
        var config = new MobileCenterConfig(MobileCenterConfig.searchForFile());
        return inquirer.prompt([{
            type: 'input',
            default: config.get('app_secret'),
            message: 'What is the Android App Secret?',
            name: 'app_secret',
        }]).then(function (answers) {
            config.set('app_secret', answers['app_secret']);
            return config.save();
        });
    }
};
