jest.mock('appcenter-analytics', () => ({
    trackEvent: jest.fn(),
    isEnabled: jest.fn(),
    setEnabled: jest.fn()
}));
