const fs = require('fs');
const path = require('path');

const debug = require('debug')('mobile-center-link:ios:index');
const glob = require('glob');
const inquirer = require('inquirer');

// Assumption - react-native link is always called from the top of the project
// As indicated in https://github.com/facebook/react-native/blob/4082a546495c5d9f4c6fd1b0c2f64e9bc7a88bc7/local-cli/link/getProjectDependencies.js#L7
const pjson = require(path.join(process.cwd(), './package.json'));

const MobileCenterConfig = require('./MobileCenterConfig');
const AppDelegate = require('./AppDelegate');
const PodFile = require('./PodFile');

const appDelegatePaths = glob.sync('**/AppDelegate.m', { ignore: 'node_modules/**' });
const appDelegatePath = findFileByAppName(appDelegatePaths, pjson ? pjson.name : null) || appDelegatePaths[0];
debug(`AppDelegate.m path - ${appDelegatePath}`);

module.exports = {
    checkIfAppDelegateExists() {
        try {
            fs.accessSync(appDelegatePath, fs.F_OK);
        } catch (e) {
            debug(`Could not find AppDelegate.m file at ${appDelegatePath}, so could not add the framework for iOS.`);
            return Promise.reject();
        }
        return Promise.resolve();
    },

    initMobileCenterConfig() {
        const config = new MobileCenterConfig(MobileCenterConfig.searchForFile(path.dirname(appDelegatePath)));
        const currentAppSecret = config.get('AppSecret');

        // If an app secret is already set, don't prompt again, instead give the user instructions on how they can change it themselves
        // if they want
        if (currentAppSecret) {
            console.log(`iOS App Secret is '${currentAppSecret}' set in ${config.plistPath}`);
            return Promise.resolve(null);
        }

        return inquirer.prompt([{
            type: 'input',
            default: currentAppSecret,
            message: 'What is the iOS App Secret?',
            name: 'AppSecret',
        }]).then((answers) => {
            try {
                config.set('AppSecret', answers.AppSecret);
                return config.save()
                    .then((file) => {
                        console.log(`App Secret for iOS written to ${file}`);
                        return file;
                    });
            } catch (e) {
                debug('Could not save config', e);
                return Promise.reject(e);
            }
        });
    },

    initInAppDelegate(header, initCode, oldInitCodeRegExp) {
        debug('Starting to write AppDelegate', appDelegatePath);
        try {
            const appDelegate = new AppDelegate(appDelegatePath);
            appDelegate.addHeader(header);
            appDelegate.addInitCode(initCode, oldInitCodeRegExp);
            return Promise.resolve(appDelegate.save());
        } catch (e) {
            debug('Could not change AppDelegate', e);
            return Promise.reject(e);
        }
    },

    addPodDeps(pods) {
        if (process.platform !== 'darwin') {
            return Promise.reject(new Error('Since you are not running on a Mac, CocoaPods installation steps will be skipped.'));
        }
        if (!PodFile.isCocoaPodsInstalled()) {
            return Promise.reject(new Error('Could not find "pod" command. Is CocoaPods installed?'));
        }
        const podFile = new PodFile(PodFile.searchForFile(path.resolve(path.dirname(appDelegatePath), '..')));
        pods.forEach((pod) => {
            podFile.addPodLine(pod.pod, pod.podspec, pod.version);
        });
        podFile.eraseOldLines();
        podFile.save();
        return podFile.install();
    }
};

// Helper that filters an array with AppDelegate.m paths for a path with the app name inside it
// Should cover nearly all cases
function findFileByAppName(array, appName) {
    if (array.length === 0 || !appName) return null;

    for (let i = 0; i < array.length; i++) {
        if (array[i] && array[i].indexOf(appName) !== -1) {
            return array[i];
        }
    }

    return null;
}
