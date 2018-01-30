const fs = require('fs');
const path = require('path');

const debug = require('debug')('appcenter-link:ios:index');
const inquirer = require('inquirer');

const AppCenterConfig = require('./AppCenterConfig');
const AppDelegate = require('./AppDelegate');
const PodFile = require('./PodFile');

const GetReactNativeProjectConfig = require('../GetReactNativeProjectConfig');

const iosProjectConfig = GetReactNativeProjectConfig().ios;

const appDelegatePath = AppDelegate.searchForFile(iosProjectConfig);
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

    initAppCenterConfig() {
        const config = new AppCenterConfig(AppCenterConfig.searchForFile(iosProjectConfig), iosProjectConfig.pbxprojPath);
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
        if (!PodFile.isCocoaPodsInstalled()) {
            return Promise.reject(new Error('Could not find "pod" command. Is CocoaPods installed?'));
        }
        try {
            const podfilePath = iosProjectConfig.podfile || path.join(iosProjectConfig.projectPath, '..', 'Podfile');
            PodFile.initializePodfileIfNecessary(podfilePath);

            const podFile = new PodFile(podfilePath);
            pods.forEach((pod) => {
                podFile.addPodLine(pod.pod, pod.podspec, pod.version);
            });
            podFile.eraseOldLines();
            podFile.save();
            return Promise.resolve(podFile.install());
        } catch (e) {
            debug('Could not add pod dependencies', e);
            return Promise.reject(e);
        }
    }
};
