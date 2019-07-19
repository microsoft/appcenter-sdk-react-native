// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

jest.mock('NativeModules', () => ({
  AppCenterReactNativeData: {
    isEnabled: jest.fn(),
    setEnabled: jest.fn(),
    read: jest.fn((res) => {
      return new Promise((resolve, reject) => {
        resolve();
      })
    }),
    list: jest.fn(),
    create: jest.fn(),
    remove: jest.fn(),
    replace: jest.fn()
  }
}));