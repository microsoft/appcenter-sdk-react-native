export interface PushNotification {
    message: string | null;
    title: string | null;
    customProperties?: { [name: string]: string };
}

export interface PushListener {
    onPushNotificationReceived?: (pushNotification: PushNotification) => void;
}

export function isEnabled(): Promise<boolean>;
export function setEnabled(enabled: boolean): Promise<void>;
export function setListener(pushListener?: PushListener): Promise<void>;