jest.mock('NativeModules', () => ({
  AppCenterReactNativeAnalytics: {
    isEnabled: jest.fn(),
    trackEvent: jest.fn(),
    setEnabled: jest.fn(),
    getTransmissionTarget: jest.fn(() => new Promise(resolve => resolve()))
  }
}));
