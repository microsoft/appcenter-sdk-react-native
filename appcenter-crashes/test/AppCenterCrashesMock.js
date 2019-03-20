// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

jest.mock('appcenter-crashes', () => ({
    generateTestCrash: jest.fn(),
    hasCrashedInLastSession: jest.fn(),
    lastSessionCrashReport: jest.fn(),
    isEnabled: jest.fn(),
    setEnabled: jest.fn(),
    notifyUserConfirmation: jest.fn(),
    setListener: jest.fn(),
}));
