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
export function getLogLevel(): Promise<AppCenterLog>;
export function getSdkVersion(): string;
export function isEnabled(): Promise<boolean>;
export function setEnabled(enabled: boolean): Promise<void>;
export function getInstallId(): Promise<string>;
export function setCustomProperties(properties: { [key: string]: string });