// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

jest.mock('appcenter-analytics', () => ({
    trackEvent: jest.fn(),
    isEnabled: jest.fn(),
    setEnabled: jest.fn()
}));
