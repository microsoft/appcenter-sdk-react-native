const fs = require('fs');
const path = require('path');

const projectDirectory = path.resolve(__dirname, '..', '..', '..');
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

// Check if package.json has jest as dependency
const packageJsonFile = path.join(`${projectDirectory}`, 'package.json');
try {
    const packageJson = JSON.parse(fs.readFileSync(packageJsonFile, 'utf8'));
    if (!Object.prototype.hasOwnProperty.call(packageJson.devDependencies, 'jest')) {
        return;
    }
} catch (e) {
    console.log('Could not read package.json file');
    return;
}

// Create mock file for Jest
const mocksDirectory = `${projectDirectory}/__mocks__`;
const mockFileName = 'appcenter-crashes.js';
if (!fs.existsSync(`${mocksDirectory}/${mockFileName}`)) {
    if (!fs.existsSync(mocksDirectory)) {
        fs.mkdirSync(mocksDirectory);
    }
    fs.writeFileSync(`${mocksDirectory}/${mockFileName}`, mockFileContent);
}
