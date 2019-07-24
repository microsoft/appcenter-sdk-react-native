// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

jest.mock('appcenter-auth', () => ({
  isEnabled: jest.fn(),
  setEnabled: jest.fn(),
  signIn: jest.fn(),
  signOut: jest.fn()
}));
