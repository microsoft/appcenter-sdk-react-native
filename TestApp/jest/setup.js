// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

// https://github.com/expo/expo/issues/1705#issuecomment-385338032
import mockAsyncStorage from '@react-native-community/async-storage/jest/async-storage-mock';

require('stacktrace-parser');

jest.mock('@react-native-community/async-storage', () => mockAsyncStorage);
