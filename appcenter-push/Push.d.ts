export interface PushNotification {
    message?: string | null;
    title?: string | null;
    customProperties?: { [name: string]: stirng };
}

export interface ListenerMap {
    onPushNotificationReceived?: (pushNotification: PushNotification) => void;
}

export function isEnabled(): Promise<boolean>;
export function setEnabled(enabled: boolean): Promise<void>;
export function setListener(listenerMap?: ListenerMap): Promise<void>;