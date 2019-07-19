// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

const ReactNative = require('react-native');

const { AppCenterReactNativeData } = ReactNative.NativeModules;

const TimeToLive = {
    INFINITE: -1,
    NO_CACHE: 0,
    DEFAULT: -1
};

const DefaultPartitions = {
    USER_DOCUMENTS: 'user',
    APP_DOCUMENTS: 'readonly'
};

const Data = {
    TimeToLive,
    DefaultPartitions,
    isEnabled,
    setEnabled,
    read,
    list,
    create,
    remove,
    replace
};

/**
 * Check whether Data service is enabled or not.
 *
 * @return Future asynchronous operation with result being true if enabled, false otherwise.
 */
function isEnabled() {
    return AppCenterReactNativeData.isEnabled();
}

/**
 * Enable or disable Data service.
 *
 * @param {boolean} enabled - true to enable, false to disable.
 * @return Future asynchronous operation with null result to monitor when the operation completes.
 */
function setEnabled(enabled) {
    return AppCenterReactNativeData.setEnabled(enabled);
}

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
 * Retrieve a paginated list of the documents in a partition.
 *
 * @param {string} partition - The CosmosDB partition key.
 * @return {Promise} Future asynchronous operation with result being the document list.
 * If the operation fails, the promise is rejected with an exception containing the details of the error.
 */
function list(partition) {
    return AppCenterReactNativeData.list(partition).then((result) => {
        const paginatedDocuments = {
            currentPage: result.currentPage,
            hasNextPage() {
                return AppCenterReactNativeData.hasNextPage(result.paginatedDocumentsId);
            },
            getNextPage() {
                return AppCenterReactNativeData.getNextPage(result.paginatedDocumentsId);
            },
            close() {
                AppCenterReactNativeData.close(result.paginatedDocumentsId);
            }
        };
        return paginatedDocuments;
    });
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
    return AppCenterReactNativeData.create(documentId, partition, document, writeOptions).then(convertTimestampToDate);
}

/**
 * Delete a document.
 *
 * @param {string} documentId - The CosmosDB document id.
 * @param {string} partition - The CosmosDB partition key.
 * @param {object} writeOptions - Cache write options when the operation is done offline.
 * @return {Promise} Future asynchronous operation with result being the document with metadata.
 * If the operation fails, the promise is rejected with an exception containing the details of the error.
 */
function remove(documentId, partition, writeOptions) {
    if (writeOptions === undefined) {
        writeOptions = new Data.WriteOptions(TimeToLive.DEFAULT);
    }
    return AppCenterReactNativeData.remove(documentId, partition, writeOptions).then(convertTimestampToDate);
}

/**
 * Replace a document.
 *
 * @param {string} documentId - The CosmosDB document id.
 * @param {object} document - The document.
 * @param {string} partition - The CosmosDB partition key.
 * @param {object} writeOptions - Cache write options when the operation is done offline.
 * @return {Promise} Future asynchronous operation with result being the document with metadata.
 * If the operation fails, the promise is rejected with an exception containing the details of the error.
 */
function replace(documentId, document, partition, writeOptions) {
    if (writeOptions === undefined) {
        writeOptions = new Data.WriteOptions(TimeToLive.DEFAULT);
    }
    return AppCenterReactNativeData.replace(documentId, partition, document, writeOptions).then(convertTimestampToDate);
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
    if (result && result.lastUpdatedDate) {
        // Create a new `Date` object from timestamp as milliseconds.
        result.lastUpdatedDate = new Date(result.lastUpdatedDate);
    }
    return result;
}

module.exports = Data;
