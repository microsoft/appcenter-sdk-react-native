const fs = require('fs');

const mockFileContent = `
const Push = jest.mock('appcenter-push');
Push.isEnabled = jest.fn();
Push.setEnabled = jest.fn();
Push.setListener = jest.fn();
export default Push;
`;

// Create mock file for Jest
if (process.env.INIT_CWD !== undefined) {
    const mocksDirectory = `${process.env.INIT_CWD}/__mocks__`;
    const mockFileName = 'appcenter-push.js';
    if (!fs.existsSync(`${mocksDirectory}/${mockFileName}`)) {
        if (!fs.existsSync(mocksDirectory)) {
            fs.mkdirSync(mocksDirectory);
        }
        fs.writeFileSync(`${mocksDirectory}/${mockFileName}`, mockFileContent);
    }
}
