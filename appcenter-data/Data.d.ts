// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

export const enum TimeToLive {
    INFINITE = -1,
    NO_CACHE = 0,
    DEFAULT = -1
}

export const enum DefaultPartitions {
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
    deserializedValue: object;
    eTag: string;
    id: string;
    isFromDeviceCache: boolean;
    jsonValue: string;
    lastUpdatedDate: Date;
    partition: string;
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

export function isEnabled(): Promise<boolean>;
export function setEnabled(enabled: boolean): Promise<void>;
export function read(documentId: string, partition: DefaultPartitions, readOptions?: ReadOptions): Promise<DocumentWrapper>;
export function list(partition: DefaultPartitions): Promise<PaginatedDocuments>;
export function create(documentId: string, document: object, partition: DefaultPartitions, writeOptions?: WriteOptions): Promise<DocumentWrapper>;
export function remove(documentId: string, partition: DefaultPartitions, writeOptions?: WriteOptions): Promise<DocumentWrapper>;
export function replace(documentId: string, document: object, partition: DefaultPartitions, writeOptions?: WriteOptions): Promise<DocumentWrapper>;
