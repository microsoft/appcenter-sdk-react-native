// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

import { NativeModules } from 'react-native';
import Data from '../Data';

describe('App Center Data tests', () => {
  test('isEnabled is called', async () => {
    const spy = jest.spyOn(NativeModules.AppCenterReactNativeData, 'isEnabled');
    await Data.isEnabled();
    expect(spy).toHaveBeenCalled();
  });

  test('setEnabled is called', async () => {
    const enabled = true;
    const spy = jest.spyOn(NativeModules.AppCenterReactNativeData, 'setEnabled');
    await Data.setEnabled(enabled);
    expect(spy).toHaveBeenCalled();
  });

  test('read is called', async () => {
    const documentId = "Test documentId";
    const partition = Data.DefaultPartitions.USER_DOCUMENTS;
    const readOptions = new Data.ReadOptions(Data.TimeToLive.DEFAULT);
    const spy = jest.spyOn(NativeModules.AppCenterReactNativeData, 'read');
    await Data.read(documentId, partition, readOptions);
    expect(spy).toHaveBeenCalledWith(documentId, partition, readOptions);
  })
});
