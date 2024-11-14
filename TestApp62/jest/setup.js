// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

import mockAsyncStorage from '@react-native-community/async-storage/jest/async-storage-mock';

// https://github.com/expo/expo/issues/1705#issuecomment-385338032
require('stacktrace-parser');

const React = require('react');
const ReactNative = require('react-native');

// Switch is mocked to fix the issue: https://github.com/callstack/react-native-testing-library/issues/329
const Switch = function (props) {
  const [value, setValue] = React.useState(props.value);

  return (
    <ReactNative.TouchableOpacity
      onPress={() => {
        props.onValueChange(!value);
        setValue(!value);
      }}
      testID={props.testID}
    >
      <ReactNative.Text>{value.toString()}</ReactNative.Text>
    </ReactNative.TouchableOpacity>
  );
};

Object.defineProperty(ReactNative, 'Switch', {
  get() {
    return Switch;
  }
});

jest.mock('@react-native-community/async-storage', () => mockAsyncStorage);

// Mock the native image picker library
jest.mock('react-native-image-picker', () => null);

// Jest fake timers are used to fix the issue: https://github.com/facebook/jest/issues/6434
beforeEach(() => {
  jest.useFakeTimers();
});

// Running all pending timers and switching to real timers using Jest
afterEach(() => {
  jest.runOnlyPendingTimers();
  jest.useRealTimers();
});
