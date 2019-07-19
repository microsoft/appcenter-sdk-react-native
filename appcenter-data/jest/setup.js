// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

jest.mock('NativeModules', () => {
  /* Sat 01 Jan 2000 12:00:00 AM UTC */
  const testTimestamp = 946684800000;

  return {
    AppCenterReactNativeData: {
      isEnabled: jest.fn(),
      setEnabled: jest.fn(),
      read: jest.fn(() => Promise.resolve({ lastUpdatedDate: testTimestamp })),
      list: jest.fn(() => {
        const result = {
          currentPage: [],
          paginatedDocumentsId: 'test_paginated_documents_id'
        };
        return Promise.resolve(result);
      }),
      create: jest.fn(() => Promise.resolve({ lastUpdatedDate: testTimestamp })),
      remove: jest.fn(() => Promise.resolve({ lastUpdatedDate: testTimestamp })),
      replace: jest.fn(() => Promise.resolve({ lastUpdatedDate: testTimestamp }))
    }
  };
});
