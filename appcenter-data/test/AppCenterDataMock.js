// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

jest.mock('appcenter-data', () => ({
    isEnabled: jest.fn(),
    setEnabled: jest.fn(),
    read: jest.fn(),
    list: jest.fn(),
    create: jest.fn(),
    remove: jest.fn(),
    replace: jest.fn()
}));
