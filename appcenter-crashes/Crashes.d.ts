export const enum UserConfirmation {
  DONT_SEND = 0,
  SEND = 1,
  ALWAYS_SEND = 2
}

export interface ListenerMap {
  onBeforeSending?: () => void;
  onSendingSucceeded?: () => void;
  onSendingFailed?: () => void;
  getErrorAttachments?: (report) => Promise<[TextAttachment, FileAttachment]>;
}

interface TextAttachment {
  text: string;
  fileName: string;
}

interface FileAttachment {
  data: string;
  fileName: string;
  contentType: string;
}

export interface ErrorAttachmentLog {
  attachmentWithText(text: string, fileName: string): TextAttachment;
  attachmentWithBinary(
    data: object,
    fileName: string,
    contentType: string
  ): FileAttachment;
}

export default interface Crashes {
  generateTestCrash(): Promise<void>;
  hasCrashedInLastSession(): Promise<boolean>;
  lastSessionCrashReport(): Promise<X>;
  isEnabled(): Promise<boolean>;
  setEnabled(shouldEnable: boolean): Promise<void>;
  notifyUserConfirmation(userConfirmation: UserConfirmation): void;
  setListener(listenerMap: ListenerMap): Promise<void>;
};
