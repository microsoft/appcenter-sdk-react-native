var fs = require('fs');
var path = require('path');

var debug = require('debug')('sonoma-link:ios:index');
var glob = require('glob');
var inquirer = require('inquirer');

// Assumption - react-native link is always called from the top of hte project
// As indicated in https://github.com/facebook/react-native/blob/4082a546495c5d9f4c6fd1b0c2f64e9bc7a88bc7/local-cli/link/getProjectDependencies.js#L7
var pjson = require(path.join(process.cwd(), './package.json'));

var SonomaConfig = require('./SonomaConfig');
var AppDelegate = require('./AppDelegate');
var PodFile = require('./Podfile');

var appDelegatePaths = glob.sync("**/AppDelegate.m", { ignore: "node_modules/**" });
var appDelegatePath = findFileByAppName(appDelegatePaths, pjson ? pjson.name : null) || appDelegatePaths[0];
debug('AppDelegate.m path - ' + appDelegatePath);

try {
    fs.accessSync(appDelegatePath, fs.F_OK);
} catch (e) {
    debug('Could not fine AppDelegate.m at ', appDelegatePath);
    throw Error(`
        Could not find AppDelegate.m file for this project, so could not add the framework for iOS
        You may have to add the framework manually. 
        Looked in ${pjson.name} the files - ${appDelegatePaths}
    `);
}

module.exports = {
    initSonomaConfig: function(file) {
        var sonomaConfig = new SonomaConfig(SonomaConfig.searchForFile(path.dirname(appDelegatePath)));
        return inquirer.prompt([{
            type: 'input',
            default: sonomaConfig.get('AppSecret'),
            message: 'What is the iOS App Secret?',
            name: 'AppSecret',
        }]).then(function(answers) {
            try {
                sonomaConfig.set('AppSecret', answers['AppSecret']);
                return sonomaConfig.save();
            } catch (e) {
                debug('Could not save SonomaConfig', e);
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
        if (!PodFile.isCocoaPodsInstalled()) {
            return Promise.reject(new Error('Could not find "pod" command. Is CocoaPods installed ? '));
        }
        var podFile = new PodFile(PodFile.searchForFile(path.resolve(path.dirname(appDelegatePath), '..')));
        pods.forEach(function(pod) {
            podFile.addPodLine(pod.pod, pod.podspec, pod.version);
        });
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