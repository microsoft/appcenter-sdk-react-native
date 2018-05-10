const fs = require('fs');

// Create mock file for Jest
const mocksDirectory = `${process.env.INIT_CWD}/__mocks__`;
const mockFileName = 'appcenter-crashes.js';
if(!fs.existsSync(`${mocksDirectory}/${mockFileName}`)) {
    if (!fs.existsSync(mocksDirectory)){
        fs.mkdirSync(mocksDirectory);
    }
    fs.writeFileSync(`${mocksDirectory}/${mockFileName}`, '');
}