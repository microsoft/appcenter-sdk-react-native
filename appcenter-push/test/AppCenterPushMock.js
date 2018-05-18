jest.mock('appcenter-push', () => ({
    isEnabled: jest.fn(),
    setEnabled: jest.fn(),
    setListener: jest.fn()
}));
