// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

import { NativeModules } from 'react-native';
import Data from '../Data';

describe('App Center Data isEnabled and setEnabled tests', () => {
  test('isEnabled is called', async () => {
    const spy = jest.spyOn(NativeModules.AppCenterReactNativeData, 'isEnabled');
    await Data.isEnabled();
    expect(spy).toHaveBeenCalled();
  });

  test('setEnabled is called', async () => {
    const enabled = true;
    const spy = jest.spyOn(NativeModules.AppCenterReactNativeData, 'setEnabled');
    const result = await Data.setEnabled(enabled);
    expect(spy).toHaveBeenCalled();
    expect(result).toBe(null);
  });
});

describe('App Center Data setRemoteOperationListener tests', () => {
  test('setRemoteOperationListener is called', async () => {
    const listener = {};
    const spy = jest.spyOn(NativeModules.AppCenterReactNativeData, 'setRemoteOperationListener');
    Data.setRemoteOperationListener(listener);
    expect(spy).toHaveBeenCalled();
  });
});

describe('App Center Data read operation tests', () => {
  test('read is called with readOptions and TimeToLive.INFINITE', async () => {
    const documentId = 'test_document_id';
    const partition = Data.DefaultPartitions.USER_DOCUMENTS;
    const readOptions = new Data.ReadOptions(Data.TimeToLive.INFINITE);
    const spy = jest.spyOn(NativeModules.AppCenterReactNativeData, 'read');
    await Data.read(documentId, partition, readOptions);
    expect(spy).toHaveBeenCalledWith(documentId, partition, readOptions);
  });

  test('read is called with readOptions and TimeToLive.NO_CACHE', async () => {
    const documentId = 'test_document_id';
    const partition = Data.DefaultPartitions.USER_DOCUMENTS;
    const readOptions = new Data.ReadOptions(Data.TimeToLive.NO_CACHE);
    const spy = jest.spyOn(NativeModules.AppCenterReactNativeData, 'read');
    await Data.read(documentId, partition, readOptions);
    expect(spy).toHaveBeenCalledWith(documentId, partition, readOptions);
  });

  test('read is called with readOptions and TimeToLive.DEFAULT', async () => {
    const documentId = 'test_document_id';
    const partition = Data.DefaultPartitions.USER_DOCUMENTS;
    const readOptions = new Data.ReadOptions(Data.TimeToLive.DEFAULT);
    const spy = jest.spyOn(NativeModules.AppCenterReactNativeData, 'read');
    await Data.read(documentId, partition, readOptions);
    expect(spy).toHaveBeenCalledWith(documentId, partition, readOptions);
  });

  test('read is called without readOptions', async () => {
    const documentId = 'test_document_id';
    const partition = Data.DefaultPartitions.USER_DOCUMENTS;
    const readOptions = new Data.ReadOptions(Data.TimeToLive.DEFAULT);
    const spy = jest.spyOn(NativeModules.AppCenterReactNativeData, 'read');
    await Data.read(documentId, partition);
    expect(spy).toHaveBeenCalledWith(documentId, partition, readOptions);
  });

  test('read is called with null readOptions', async () => {
    const documentId = 'test_document_id';
    const partition = Data.DefaultPartitions.USER_DOCUMENTS;
    const spy = jest.spyOn(NativeModules.AppCenterReactNativeData, 'read');
    await Data.read(documentId, partition, null);
    expect(spy).toHaveBeenCalledWith(documentId, partition, null);
  });

  test('read results contains lastUpdatedDate property', async () => {
    const documentId = 'test_document_id';
    const partition = Data.DefaultPartitions.USER_DOCUMENTS;
    const readOptions = new Data.ReadOptions(Data.TimeToLive.DEFAULT);
    const spy = jest.spyOn(NativeModules.AppCenterReactNativeData, 'read');
    const result = await Data.read(documentId, partition, readOptions);
    expect(spy).toHaveBeenCalledWith(documentId, partition, readOptions);
    expect(result).toHaveProperty('lastUpdatedDate', new Date(946684800000));
  });
});

describe('App Center Data list operation tests', () => {
  test('list is called ', async () => {
    const partition = Data.DefaultPartitions.USER_DOCUMENTS;
    const spy = jest.spyOn(NativeModules.AppCenterReactNativeData, 'list');
    const result = await Data.list(partition);
    console.log(result);
    expect(spy).toHaveBeenCalledWith(partition);
    expect(result).toHaveProperty('currentPage');
    expect(result).toHaveProperty('hasNextPage');
    expect(result).toHaveProperty('getNextPage');
    expect(result).toHaveProperty('close');
  });
});

describe('App Center Data create operation tests', () => {
  test('create is called with writeOptions and TimeToLive.INFINITE', async () => {
    const documentId = 'test_document_id';
    const document = {};
    const partition = Data.DefaultPartitions.USER_DOCUMENTS;
    const writeOptions = new Data.WriteOptions(Data.TimeToLive.INFINITE);
    const spy = jest.spyOn(NativeModules.AppCenterReactNativeData, 'create');
    await Data.create(documentId, document, partition, writeOptions);
    expect(spy).toHaveBeenCalledWith(documentId, partition, document, writeOptions);
  });

  test('create is called with writeOptions and TimeToLive.NO_CACHE', async () => {
    const documentId = 'test_document_id';
    const document = {};
    const partition = Data.DefaultPartitions.USER_DOCUMENTS;
    const writeOptions = new Data.WriteOptions(Data.TimeToLive.NO_CACHE);
    const spy = jest.spyOn(NativeModules.AppCenterReactNativeData, 'create');
    await Data.create(documentId, document, partition, writeOptions);
    expect(spy).toHaveBeenCalledWith(documentId, partition, document, writeOptions);
  });

  test('create is called with writeOptions and TimeToLive.DEFAULT', async () => {
    const documentId = 'test_document_id';
    const document = {};
    const partition = Data.DefaultPartitions.USER_DOCUMENTS;
    const writeOptions = new Data.WriteOptions(Data.TimeToLive.DEFAULT);
    const spy = jest.spyOn(NativeModules.AppCenterReactNativeData, 'create');
    await Data.create(documentId, document, partition, writeOptions);
    expect(spy).toHaveBeenCalledWith(documentId, partition, document, writeOptions);
  });

  test('create is called without writeOptions', async () => {
    const documentId = 'test_document_id';
    const document = {};
    const partition = Data.DefaultPartitions.USER_DOCUMENTS;
    const writeOptions = new Data.WriteOptions(Data.TimeToLive.DEFAULT);
    const spy = jest.spyOn(NativeModules.AppCenterReactNativeData, 'create');
    await Data.create(documentId, document, partition);
    expect(spy).toHaveBeenCalledWith(documentId, partition, document, writeOptions);
  });

  test('create is called with null writeOptions', async () => {
    const documentId = 'test_document_id';
    const document = {};
    const partition = Data.DefaultPartitions.USER_DOCUMENTS;
    const spy = jest.spyOn(NativeModules.AppCenterReactNativeData, 'create');
    await Data.create(documentId, document, partition, null);
    expect(spy).toHaveBeenCalledWith(documentId, partition, document, null);
  });

  test('create results contains lastUpdatedDate property', async () => {
    const documentId = 'test_document_id';
    const document = {};
    const partition = Data.DefaultPartitions.USER_DOCUMENTS;
    const spy = jest.spyOn(NativeModules.AppCenterReactNativeData, 'create');
    const result = await Data.create(documentId, partition, null);
    expect(spy).toHaveBeenCalledWith(documentId, partition, document, null);
    expect(result).toHaveProperty('lastUpdatedDate', new Date(946684800000));
  });
});

describe('App Center Data remove operation tests', () => {
  test('remove is called with writeOptions and TimeToLive.INFINITE', async () => {
    const documentId = 'test_document_id';
    const partition = Data.DefaultPartitions.USER_DOCUMENTS;
    const writeOptions = new Data.WriteOptions(Data.TimeToLive.INFINITE);
    const spy = jest.spyOn(NativeModules.AppCenterReactNativeData, 'remove');
    await Data.remove(documentId, partition, writeOptions);
    expect(spy).toHaveBeenCalledWith(documentId, partition, writeOptions);
  });

  test('remove is called with writeOptions and TimeToLive.NO_CACHE', async () => {
    const documentId = 'test_document_id';
    const partition = Data.DefaultPartitions.USER_DOCUMENTS;
    const writeOptions = new Data.WriteOptions(Data.TimeToLive.NO_CACHE);
    const spy = jest.spyOn(NativeModules.AppCenterReactNativeData, 'remove');
    await Data.remove(documentId, partition, writeOptions);
    expect(spy).toHaveBeenCalledWith(documentId, partition, writeOptions);
  });

  test('remove is called with writeOptions and TimeToLive.DEFAULT', async () => {
    const documentId = 'test_document_id';
    const partition = Data.DefaultPartitions.USER_DOCUMENTS;
    const writeOptions = new Data.WriteOptions(Data.TimeToLive.DEFAULT);
    const spy = jest.spyOn(NativeModules.AppCenterReactNativeData, 'remove');
    await Data.remove(documentId, partition, writeOptions);
    expect(spy).toHaveBeenCalledWith(documentId, partition, writeOptions);
  });

  test('remove is called without writeOptions', async () => {
    const documentId = 'test_document_id';
    const partition = Data.DefaultPartitions.USER_DOCUMENTS;
    const writeOptions = new Data.WriteOptions(Data.TimeToLive.DEFAULT);
    const spy = jest.spyOn(NativeModules.AppCenterReactNativeData, 'remove');
    await Data.remove(documentId, partition);
    expect(spy).toHaveBeenCalledWith(documentId, partition, writeOptions);
  });

  test('remove is called with null writeOptions', async () => {
    const documentId = 'test_document_id';
    const partition = Data.DefaultPartitions.USER_DOCUMENTS;
    const spy = jest.spyOn(NativeModules.AppCenterReactNativeData, 'remove');
    await Data.remove(documentId, partition, null);
    expect(spy).toHaveBeenCalledWith(documentId, partition, null);
  });

  test('remove results containts lastUpdatedDate property', async () => {
    const documentId = 'test_document_id';
    const partition = Data.DefaultPartitions.USER_DOCUMENTS;
    const writeOptions = new Data.WriteOptions(Data.TimeToLive.DEFAULT);
    const spy = jest.spyOn(NativeModules.AppCenterReactNativeData, 'remove');
    const result = await Data.remove(documentId, partition, writeOptions);
    expect(spy).toHaveBeenCalledWith(documentId, partition, writeOptions);
    expect(result).toHaveProperty('lastUpdatedDate', new Date(946684800000));
  });
});

describe('App Center Data replace operation tests', () => {
  test('replace is called with writeOptions and TimeToLive.INFINITE', async () => {
    const documentId = 'test_document_id';
    const document = {};
    const partition = Data.DefaultPartitions.USER_DOCUMENTS;
    const writeOptions = new Data.WriteOptions(Data.TimeToLive.INFINITE);
    const spy = jest.spyOn(NativeModules.AppCenterReactNativeData, 'replace');
    await Data.replace(documentId, document, partition, writeOptions);
    expect(spy).toHaveBeenCalledWith(documentId, partition, document, writeOptions);
  });

  test('replace is called with writeOptions and TimeToLive.NO_CACHE', async () => {
    const documentId = 'test_document_id';
    const document = {};
    const partition = Data.DefaultPartitions.USER_DOCUMENTS;
    const writeOptions = new Data.WriteOptions(Data.TimeToLive.NO_CACHE);
    const spy = jest.spyOn(NativeModules.AppCenterReactNativeData, 'replace');
    await Data.replace(documentId, document, partition, writeOptions);
    expect(spy).toHaveBeenCalledWith(documentId, partition, document, writeOptions);
  });

  test('replace is called with writeOptions and TimeToLive.DEFAULT', async () => {
    const documentId = 'test_document_id';
    const document = {};
    const partition = Data.DefaultPartitions.USER_DOCUMENTS;
    const writeOptions = new Data.WriteOptions(Data.TimeToLive.DEFAULT);
    const spy = jest.spyOn(NativeModules.AppCenterReactNativeData, 'replace');
    await Data.replace(documentId, document, partition, writeOptions);
    expect(spy).toHaveBeenCalledWith(documentId, partition, document, writeOptions);
  });

  test('replace is called without writeOptions', async () => {
    const documentId = 'test_document_id';
    const document = {};
    const partition = Data.DefaultPartitions.USER_DOCUMENTS;
    const writeOptions = new Data.WriteOptions(Data.TimeToLive.DEFAULT);
    const spy = jest.spyOn(NativeModules.AppCenterReactNativeData, 'replace');
    await Data.replace(documentId, document, partition);
    expect(spy).toHaveBeenCalledWith(documentId, partition, document, writeOptions);
  });

  test('replace is called with null writeOptions', async () => {
    const documentId = 'test_document_id';
    const document = {};
    const partition = Data.DefaultPartitions.USER_DOCUMENTS;
    const spy = jest.spyOn(NativeModules.AppCenterReactNativeData, 'replace');
    await Data.replace(documentId, document, partition, null);
    expect(spy).toHaveBeenCalledWith(documentId, partition, document, null);
  });

  test('replace results contains lastUpdatedDate property', async () => {
    const documentId = 'test_document_id';
    const document = {};
    const partition = Data.DefaultPartitions.USER_DOCUMENTS;
    const spy = jest.spyOn(NativeModules.AppCenterReactNativeData, 'replace');
    const result = await Data.replace(documentId, partition, null);
    expect(spy).toHaveBeenCalledWith(documentId, partition, document, null);
    expect(result).toHaveProperty('lastUpdatedDate', new Date(946684800000));
  });
});
