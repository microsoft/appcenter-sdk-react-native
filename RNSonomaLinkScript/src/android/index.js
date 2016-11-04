var fs = require('fs');

var glob = require('glob');
var inquirer = require('inquirer');

var SonomaConfig = require('./SonomaConfig');
module.exports = {
    initSonomaConfig: function () {
        var sonomaConfig = new SonomaConfig(SonomaConfig.searchForFile());
        return inquirer.prompt([{
            type: 'input',
            default: sonomaConfig.get('app_secret'),
            message: 'What is the Android App Secret?',
            name: 'app_secret',
        }]).then(function (answers) {
            sonomaConfig.set('app_secret', answers['app_secret']);
            return sonomaConfig.save();
        });
    }
};
