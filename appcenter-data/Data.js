// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

const ReactNative = require('react-native');

const { AppCenterReactNativeData } = ReactNative.NativeModules;

const TimeToLive = {
    INFINITE: -1,
    NO_CACHE: 0,
    DEFAULT: 86400
};

const DefaultPartitions = {
    USER_DOCUMENTS: 'user',
    APP_DOCUMENTS: 'readonly'
};

const Data = {
    TimeToLive,
    DefaultPartitions,
    read,
    create,
    remove
};

/**
 * Read a document.
 *
 * @param {string} documentId - The CosmosDB document id.
 * @param {string} partition - The CosmosDB partition key.
 * @param {object} readOptions - Cache read options when the operation is done offline.
 * @return {Promise} Future asynchronous operation with result being the document with metadata.
 * If the operation fails, the promise is rejected with an exception containing the details of the error.
 */
function read(documentId, partition, readOptions) {
    if (readOptions === undefined) {
        readOptions = new Data.ReadOptions(TimeToLive.DEFAULT);
    }
    return AppCenterReactNativeData.read(documentId, partition, readOptions).then(convertTimestampToDate);
}

/**
 * Create a document.
 *
 * @param {string} documentId - The CosmosDB document id.
 * @param {object} document - The document.
 * @param {string} partition - The CosmosDB partition key.
 * @param {object} writeOptions - Cache write options when the operation is done offline.
 * @return {Promise} Future asynchronous operation with result being the document with metadata.
 * If the operation fails, the promise is rejected with an exception containing the details of the error.
 */
function create(documentId, document, partition, writeOptions) {
    if (writeOptions === undefined) {
        writeOptions = new Data.WriteOptions(TimeToLive.DEFAULT);
    }
    return AppCenterReactNativeData.create(documentId, document, partition, writeOptions).then(convertTimestampToDate);
}

/**
 * Delete a document.
 * @param {string} documentId - The CosmosDB document id.
 * @param {string} partition - The CosmosDB partition key.
 * @param {object} writeOptions - Cache write options when the operation is done offline.
 */
function remove(documentId, partition, writeOptions) {
    if (!writeOptions) {
        writeOptions = new Data.WriteOptions(TimeToLive.DEFAULT);
    }
    return AppCenterReactNativeData.remove(documentId, partition, writeOptions);
}

Data.ReadOptions = class {
    constructor(timeToLive) {
        this.timeToLive = timeToLive;
    }
};

Data.WriteOptions = class {
    constructor(timeToLive) {
        this.timeToLive = timeToLive;
    }
};

function convertTimestampToDate(result) {
    if (!result || !result.lastUpdatedDate) {
        return result;
    }
    // Create a new `Date` object from timestamp as milliseconds.
    result.lastUpdatedDate = new Date(result.lastUpdatedDate);
    return result;
}

module.exports = Data;
