const fs = require('fs');
const path = require('path');

const projectDirectory = path.resolve(__dirname, '..', '..', '..');
const setupFileName = `.${path.sep}node_modules${path.sep}appcenter${path.sep}test${path.sep}appCenterMock.js`;

// Check if package.json has jest as dependency
let packageJson = '';
const packageJsonFile = path.join(`${projectDirectory}`, 'package.json');
try {
    packageJson = JSON.parse(fs.readFileSync(packageJsonFile, 'utf8'));
    if (!Object.prototype.hasOwnProperty.call(packageJson.devDependencies, 'jest') &&
        !Object.prototype.hasOwnProperty.call(packageJson.dependencies, 'jest')) {
        return;
    }
} catch (e) {
    console.log('Could not read package.json file');
    return;
}

// Add setup file for module
if (Object.prototype.hasOwnProperty.call(packageJson, 'jest')) {
    if (packageJson.jest.setupFiles === undefined) {
        packageJson.jest.setupFiles = [setupFileName];
    } else if (packageJson.jest.setupFiles.indexOf(setupFileName) === -1) {
        packageJson.jest.setupFiles.push(setupFileName);
    }
    fs.writeFileSync(packageJsonFile, JSON.stringify(packageJson));
}
