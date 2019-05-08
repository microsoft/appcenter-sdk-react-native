// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

jest.mock('appcenter', () => ({
    getLogLevel: jest.fn(),
    setLogLevel: jest.fn(),
    getInstallId: jest.fn(),
    setUserId: jest.fn(),
    isEnabled: jest.fn(),
    setEnabled: jest.fn(),
    setCustomProperties: jest.fn(),
    getSdkVersion: jest.fn()
}));
