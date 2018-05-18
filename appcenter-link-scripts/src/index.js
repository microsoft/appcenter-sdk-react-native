const android = require('./android');
const ios = require('./ios');
const mock = require('./SetupAppCenterMock');
const inquirer = require('inquirer');

module.exports = {
    android,
    ios,
    mock,
    inquirer
};
