const fs = require('fs');

const mockFileContent = `
const Crashes = jest.mock('appcenter-crashes');
Crashes.generateTestCrash = jest.fn();
Crashes.hasCrashedInLastSession = jest.fn();
Crashes.lastSessionCrashReport = jest.fn();
Crashes.isEnabled = jest.fn();
Crashes.setEnabled = jest.fn();
Crashes.notifyUserConfirmation = jest.fn();
Crashes.setListener = jest.fn();
export default Crashes;
`;

// Create mock file for Jest
if (process.env.INIT_CWD !== undefined) {
    const mocksDirectory = `${process.env.INIT_CWD}/__mocks__`;
    const mockFileName = 'appcenter-crashes.js';
    if (!fs.existsSync(`${mocksDirectory}/${mockFileName}`)) {
        if (!fs.existsSync(mocksDirectory)) {
            fs.mkdirSync(mocksDirectory);
        }
        fs.writeFileSync(`${mocksDirectory}/${mockFileName}`, mockFileContent);
    }
}
