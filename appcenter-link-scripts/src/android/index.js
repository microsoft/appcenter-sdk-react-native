// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

const fs = require('fs');
const path = require('path');
const glob = require('glob');
const debug = require('debug')('appcenter-link:android:index');

const AppCenterConfig = require('./AppCenterConfig');

module.exports = {
    checkIfAndroidDirectoryExists() {
        try {
            if (fs.statSync('./android').isDirectory()) {
                return true;
            }
        } catch (e) {
            debug('Could not find /android directory in your application.');
        }
        return false;
    },

    initAppCenterConfig() {
        const config = new AppCenterConfig(AppCenterConfig.searchForFile());
        const currentAppSecret = config.get('app_secret');

        // If an app secret is already set, don't prompt again, instead give the user instructions on how they can change it themselves
        // if they want
        if (typeof (currentAppSecret) === 'string') {
            console.log(`Android App Secret is already set in ${config.AppCenterConfigPath}`);
            return Promise.resolve(null);
        }
        config.set('app_secret', 'YOUR_APP_SECRET');
        const file = config.save();
        console.log(`App Secret for Android written to ${file}`);
        return Promise.resolve(file);
    },

    patchStrings(key, value) {
        const stringsFile = path.join('android', 'app', 'src', 'main', 'res', 'values', 'strings.xml');
        let stringsXml = fs.readFileSync(stringsFile, 'utf-8');

        // If strings doesn't contain key, then insert default value
        if (stringsXml.indexOf(key) > 0) {
            return;
        }
        const pattern = new RegExp(`<string.*name="${key}".*>.*</string>`);
        const newValue = `<string name="${key}" moduleConfig="true" translatable="false">${value}</string>`;
        if (stringsXml.match(pattern)) {
            stringsXml = stringsXml.replace(pattern, newValue);
        } else {
            stringsXml = stringsXml.replace('\n</resources>', `\n    ${newValue}\n</resources>`);
        }
        fs.writeFileSync(stringsFile, stringsXml);
    },

    // Workaround for bug in react-native 0.53
    removeAndroidDuplicateLinks() {
        // Settings file
        let lines = {};
        const settingsPath = 'android/settings.gradle';
        let settingsContent = fs.readFileSync(settingsPath, 'utf-8');
        settingsContent.split('\n').forEach((line) => {
            if (lines[line]) {
                settingsContent = settingsContent.replace(line, '');
            }
            if (line.match(/^\s*(include|project).*appcenter.*$/)) {
                lines[line] = true;
            }
        });
        settingsContent = settingsContent.replace(/\n\n\n/g, '\n\n');
        fs.writeFileSync(settingsPath, settingsContent);

        // android build.gradle
        lines = {};
        const gradlePath = 'android/app/build.gradle';
        let gradleContent = fs.readFileSync(gradlePath, 'utf-8');
        gradleContent.split('\n').forEach((line) => {
            const match = line.match(/^\s*(compile|implementation) (project.*appcenter.*)$/);
            if (match) {
                if (lines[match[2]]) {
                    gradleContent = gradleContent.replace(line, '');
                } else {
                    lines[match[2]] = true;
                }
            }
        });
        gradleContent = gradleContent.replace(/\n\n\n/g, '\n\n');
        fs.writeFileSync(gradlePath, gradleContent);

        // MainApplication.java
        const appFiles = glob.sync('android/app/src/**/MainApplication.java', {
            ignore: ['node_modules/**', '**/build/**'],
            cwd: process.cwd()
        });
        if (appFiles.length > 0) {
            lines = {};
            const appFile = appFiles[0];
            let appContent = fs.readFileSync(appFile, 'utf-8');
            appContent.split('\n').forEach((line) => {
                const line2 = `${line},`;
                if (lines[line]) {
                    appContent = appContent.replace(line, '');
                }
                if (lines[line2]) {
                    appContent = appContent.replace(line2, '');
                }
                if (line.match(/^\s*(import.*appcenter|new AppCenterReactNative.*Package).*$/)) {
                    lines[line] = true;
                }
            });
            appContent = appContent.replace(/(import.*\n\n)\n/g, '$1');
            appContent = appContent.replace(/(new.*AppCenterReactNative.*Package.*\n)\n/g, '$1');
            fs.writeFileSync(appFile, appContent);
        }
    }
};
