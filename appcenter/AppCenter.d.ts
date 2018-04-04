export const enum AppCenterLogLevel {
    LogLevelVerbose = 2,
    LogLevelDebug = 3,
    LogLevelInfo = 4,
    LogLevelWarning = 5,
    LogLevelError = 6,
    LogLevelAssert = 7,
    LogLevelNone = 99
}

export function setLogLevel(logLevel: AppCenterLogLevel): Promise<void>;
export function getLogLevel(): Promise<AppCenterLogLevel>;
export function getSdkVersion(): string;
export function isEnabled(): Promise<boolean>;
export function setEnabled(enabled: boolean): Promise<void>;
export function getInstallId(): Promise<string>;
export function setCustomProperties(properties: { [key: string]: string });
export const LogLevelVerbose: AppCenterLogLevel.LogLevelVerbose;
export const LogLevelDebug: AppCenterLogLevel.LogLevelDebug;
export const LogLevelInfo: AppCenterLogLevel.LogLevelInfo;
export const LogLevelWarning: AppCenterLogLevel.LogLevelWarning;
export const LogLevelError: AppCenterLogLevel.LogLevelError;
export const LogLevelAssert: AppCenterLogLevel.LogLevelAssert;
export const LogLevelNone: AppCenterLogLevel.LogLevelNone;
export class CustomProperties {
    set(key: string, value: string);
    clear(key: string);
}