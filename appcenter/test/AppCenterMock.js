jest.mock('appcenter', () => ({
    getLogLevel: jest.fn(),
    setLogLevel: jest.fn(),
    getInstallId: jest.fn(),
    isEnabled: jest.fn(),
    setEnabled: jest.fn(),
    setCustomProperties: jest.fn(),
    getSdkVersion: jest.fn()
}));
