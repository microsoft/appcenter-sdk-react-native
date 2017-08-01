var fs = require('fs');
var path = require('path');

var debug = require('debug')('mobile-center-link:ios:index');
var glob = require('glob');
var inquirer = require('inquirer');

// Assumption - react-native link is always called from the top of hte project
// As indicated in https://github.com/facebook/react-native/blob/4082a546495c5d9f4c6fd1b0c2f64e9bc7a88bc7/local-cli/link/getProjectDependencies.js#L7
var pjson = require(path.join(process.cwd(), './package.json'));

var MobileCenterConfig = require('./MobileCenterConfig');
var AppDelegate = require('./AppDelegate');
var PodFile = require('./PodFile');

var appDelegatePaths = glob.sync("**/AppDelegate.m", { ignore: "node_modules/**" });
var appDelegatePath = findFileByAppName(appDelegatePaths, pjson ? pjson.name : null) || appDelegatePaths[0];
debug('AppDelegate.m path - ' + appDelegatePath);

try {
    fs.accessSync(appDelegatePath, fs.F_OK);
} catch (e) {
    debug('Could not fine AppDelegate.m at ', appDelegatePath);
    throw Error(`
        Could not find AppDelegate.m file for this project, so could not add the framework for iOS.
        You may have to add the framework manually.
    `);
}

module.exports = {
    initMobileCenterConfig: function() {
        var config = new MobileCenterConfig(MobileCenterConfig.searchForFile(path.dirname(appDelegatePath)));
        var currentAppSecret = config.get('AppSecret');

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
        }]).then(function(answers) {
            try {
                config.set('AppSecret', answers['AppSecret']);
                return config.save()
                    .then(function (file) {
                        console.log(`App Secret for iOS written to ${file}`);
                        return file;
                    });
            } catch (e) {
                debug('Could not save config', e);
                Promise.reject(e);
            }
        });
    },

    initInAppDelegate: function(header, initCode) {
        debug('Starting to write AppDelegate', appDelegatePath);
        try {
            var appDelegate = new AppDelegate(appDelegatePath);
            appDelegate.addHeader(header);
            appDelegate.addInitCode(initCode);
            return appDelegate.save();
        } catch (e) {
            debug('Could not change AppDelegate', e);
            return Promise.reject(e);
        }
    },

    addPodDeps: function(pods) {
        if (process.platform !== "darwin") {
            return Promise.reject(new Error("Since you are not running on a Mac, CocoaPods installation steps will be skipped."));
        }
        if (!PodFile.isCocoaPodsInstalled()) {
            return Promise.reject(new Error('Could not find "pod" command. Is CocoaPods installed?'));
        }
        var podFile = new PodFile(PodFile.searchForFile(path.resolve(path.dirname(appDelegatePath), '..')));
        pods.forEach(function(pod) {
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

    for (var i = 0; i < array.length; i++) {
        var path = array[i];
        if (path && path.indexOf(appName) !== -1) {
            return path;
        }
    }

    return null;
}
