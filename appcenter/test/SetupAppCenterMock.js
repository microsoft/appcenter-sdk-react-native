const fs = require('fs');
const path = require('path');

module.exports = {
    setupMock(moduleName, setupFileName) {
        const projectDirectory = path.resolve(__dirname, '..', '..', '..');
        const setupFileNamePath = `<rootDir>${path.sep}node_modules${path.sep}${moduleName}${path.sep}test${path.sep}${setupFileName}`;

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
                packageJson.jest.setupFiles = [setupFileNamePath];
            } else if (packageJson.jest.setupFiles.indexOf(setupFileNamePath) === -1) {
                packageJson.jest.setupFiles.push(setupFileNamePath);
            }
            fs.writeFileSync(packageJsonFile, JSON.stringify(packageJson, null, 2));
        }
    }
};
