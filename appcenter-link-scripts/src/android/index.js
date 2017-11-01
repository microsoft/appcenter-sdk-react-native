const fs = require('fs');

const inquirer = require('inquirer');
const debug = require('debug')('appcenter-link:android:index');

const AppCenterConfig = require('./AppCenterConfig');

module.exports = {
    checkIfAndroidDirectoryExists() {
        try {
            if (fs.statSync('./android').isDirectory()) {
                return Promise.resolve();
            }
        } catch (e) {
            debug('Could not find /android directory in your application.');
        }
        return Promise.reject();
    },

    initAppCenterConfig() {
        const config = new AppCenterConfig(AppCenterConfig.searchForFile());
        const currentAppSecret = config.get('app_secret');

        // If an app secret is already set, don't prompt again, instead give the user instructions on how they can change it themselves
        // if they want
        if (currentAppSecret) {
            console.log(`Android App Secret is '${currentAppSecret}' set in ${config.AppCenterConfigPath}`);
            return Promise.resolve(null);
        }

        return inquirer.prompt([{
            type: 'input',
            default: currentAppSecret,
            message: 'What is the Android App Secret?',
            name: 'app_secret',
        }]).then((answers) => {
            config.set('app_secret', answers.app_secret);
            const file = config.save();
            console.log(`App Secret for Android written to ${file}`);
            return file;
        });
    }
};
