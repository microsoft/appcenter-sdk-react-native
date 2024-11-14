// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

import 'react-native';
import React from 'react';
import Index from '../index.js';

// Note: test renderer must be required after react-native.
import renderer from 'react-test-renderer';

it('renders correctly', async () => {
  const tree = renderer.create(
    <Index />
  );
});
