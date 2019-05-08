// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

jest.mock('NativeModules', () => ({
  AppCenterReactNativeAnalytics: {
    isEnabled: jest.fn(),
    trackEvent: jest.fn(),
    setEnabled: jest.fn(),
    getTransmissionTarget: jest.fn(() => new Promise(resolve => resolve())),
    setTransmissionTargetAppName: jest.fn(),
    setTransmissionTargetAppVersion: jest.fn(),
    setTransmissionTargetAppLocale: jest.fn(),
    setTransmissionTargetEventProperty: jest.fn(),
    removeTransmissionTargetEventProperty: jest.fn(),
    collectTransmissionTargetDeviceId: jest.fn(),
    isTransmissionTargetEnabled: jest.fn(),
    setTransmissionTargetEnabled: jest.fn(),
    trackTransmissionTargetEvent: jest.fn(),
    getChildTransmissionTarget: jest.fn(() => new Promise(resolve => resolve()))
  }
}));
