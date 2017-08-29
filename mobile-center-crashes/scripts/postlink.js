const rnpmlink = require('mobile-center-link-scripts');
const npmPackages = require('./../package.json');

return rnpmlink.ios.initMobileCenterConfig().then(() => {
    const prompt = npmPackages.rnpm.params[0];
    prompt.message = prompt.message.replace(/Android/, 'iOS');

    return rnpmlink.inquirer.prompt(prompt);
}).then((answer) => {
    const code = answer.whenToSendCrashes === 'ALWAYS_SEND' ?
        '  [RNCrashes registerWithCrashDelegate: [[RNCrashesDelegateAlwaysSend alloc] init]];  // Initialize Mobile Center crashes' :
        '  [RNCrashes register];  // Initialize Mobile Center crashes';
    return rnpmlink.ios.initInAppDelegate('#import <RNCrashes/RNCrashes.h>', code, /.*\[RNCrashes register.*/g);
}).then((file) => {
    console.log(`Added code to initialize iOS Crashes SDK in ${file}`);
    return rnpmlink.ios.addPodDeps([
        { pod: 'MobileCenter/Crashes', version: '0.11.2' },
        { pod: 'RNMobileCenterShared', version: '0.9.0' } // in case people don't link mobile-center (core)
    ]).catch((e) => {
        console.log(`
            Could not install dependencies using CocoaPods.
            Please refer to the documentation to install dependencies manually.

            Error Reason - ${e.message}
        `);
        return Promise.resolve();
    });
});
