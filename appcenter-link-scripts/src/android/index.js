const fs = require('fs');

const inquirer = require('inquirer');
const debug = require('debug')('appcenter-link:android:index');

const AppCenterConfig = require('./AppCenterConfig');
const GetReactNativeProjectConfig = require('../GetReactNativeProjectConfig');


const getAndroidDirectory = function () {
    return GetReactNativeProjectConfig().android.folder;
};


module.exports = {
    checkIfAndroidDirectoryExists() {
        try {
            const androidProjectDirectory = getAndroidDirectory() || './android';

            if (fs.statSync(androidProjectDirectory).isDirectory()) {
                return Promise.resolve();
            }
        } catch (e) {
            debug('Could not find android project directory in your application.');
        }
        return Promise.reject();
    },

    initAppCenterConfig() {
        const androidProjectDirectory = getAndroidDirectory();

        const config = new AppCenterConfig(AppCenterConfig.searchForFile(androidProjectDirectory));
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
