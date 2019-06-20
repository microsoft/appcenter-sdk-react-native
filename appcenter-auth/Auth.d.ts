export interface UserInformation {
    accessToken: string;
    accountId: string;
    idToken: string;
}

export function isEnabled(): Promise<boolean>;
export function setEnabled(enabled: boolean): Promise<void>;
export function signIn(): Promise<UserInformation>;
export function signOut(): void;