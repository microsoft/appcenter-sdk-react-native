// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

jest.mock('appcenter-push', () => ({
    isEnabled: jest.fn(),
    setEnabled: jest.fn(),
    setListener: jest.fn()
}));
