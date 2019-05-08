// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

const RNFS = jest.mock('react-native-fs');
RNFS.DocumentDirectoryPath = jest.fn();
RNFS.readFile = jest.fn();
RNFS.writeFile = jest.fn();
export default RNFS;
