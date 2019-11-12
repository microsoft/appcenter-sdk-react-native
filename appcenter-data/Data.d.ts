// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

export enum TimeToLive {
    INFINITE = -1,
    NO_CACHE = 0,
    DEFAULT = -1
}

export enum DefaultPartitions {
    USER_DOCUMENTS = 'user',
    APP_DOCUMENTS = 'readonly'
}

export class ReadOptions {
    timeToLive: number | TimeToLive;
    constructor(timeToLive: number | TimeToLive);
}

export class WriteOptions {
    timeToLive: number | TimeToLive;
    constructor(timeToLive: number | TimeToLive);
}

export interface DocumentWrapper {
    deserializedValue: object | null;
    eTag: string | null;
    id: string;
    isFromDeviceCache: boolean;
    jsonValue: string | null;
    lastUpdatedDate: Date | null;
    partition: string | null;
}

export interface PaginatedDocuments {
    currentPage: Page;
    hasNextPage: () => Promise<boolean>;
    getNextPage: () => Promise<Page>;
    close: () => void;
}

export interface Page {
    items: DocumentWrapper[];
}

export interface RemoteOperationListener {
    onRemoteOperationCompleted?: (remoteOperationCompletedData: RemoteOperationCompletedData) => void;
}

export interface DataSyncError {
    message: string;
}

export interface RemoteOperationCompletedData {
    id?: string;
    eTag?: string;
    partition?: string;
    operation: string;
    error?: DataSyncError;
}

export function isEnabled(): Promise<boolean>;
export function setEnabled(enabled: boolean): Promise<void>;
export function setRemoteOperationListener(listener: RemoteOperationListener): Promise<void>;
export function read(documentId: string, partition: DefaultPartitions, readOptions?: ReadOptions): Promise<DocumentWrapper>;
export function list(partition: DefaultPartitions, readOptions?: ReadOptions): Promise<PaginatedDocuments>;
export function create(documentId: string, document: object, partition: DefaultPartitions, writeOptions?: WriteOptions): Promise<DocumentWrapper>;
export function remove(documentId: string, partition: DefaultPartitions, writeOptions?: WriteOptions): Promise<DocumentWrapper>;
export function replace(documentId: string, document: object, partition: DefaultPartitions, writeOptions?: WriteOptions): Promise<DocumentWrapper>;
