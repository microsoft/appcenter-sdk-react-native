export function trackEvent(
  eventName: string,
  properties?: { [name: string]: string | number | boolean }
): Promise<string | null>;
export function isEnabled(): Promise<boolean | string>;
export function setEnabled(enabled: boolean): Promise<string | null>;
