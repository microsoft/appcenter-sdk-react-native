// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

export const enum LogLevel {
    VERBOSE = 2,
    DEBUG = 3,
    INFO = 4,
    WARNING = 5,
    ERROR = 6,
    ASSERT = 7,
    NONE = 99
}

/**
 * @deprecated Use `LogLevel` instead.
 */
export const enum AppCenterLogLevel {
    VERBOSE = 2,
    DEBUG = 3,
    INFO = 4,
    WARNING = 5,
    ERROR = 6,
    ASSERT = 7,
    NONE = 99
}

export class CustomProperties {
    set(key: string, value: string | number | boolean | Date);
    clear(key: string);
}

export function setLogLevel(logLevel: LogLevel | AppCenterLogLevel): Promise<void>;
export function getLogLevel(): Promise<LogLevel | AppCenterLogLevel>;
export function getSdkVersion(): string;
export function isEnabled(): Promise<boolean>;
export function setEnabled(enabled: boolean): Promise<void>;
export function setUserId(userId: string | null): Promise<void>;
export function getInstallId(): Promise<string>;
export function setCustomProperties(properties: CustomProperties);
