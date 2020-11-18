// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

import 'react-native';
import React from 'react';
import Index from '../index.js';

// Note: test renderer must be required after react-native.
import renderer from 'react-test-renderer';

// Jest fake timers are used to fix the issue: https://github.com/facebook/jest/issues/6434
beforeEach(() => {
  jest.useFakeTimers()
});

// Running all pending timers and switching to real timers using Jest
afterEach(() => {
  jest.runOnlyPendingTimers()
  jest.useRealTimers()
});

it('renders correctly', async () => {
  const tree = renderer.create(
    <Index />
  );
});
