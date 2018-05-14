
const Crashes = jest.mock('appcenter-crashes');
Crashes.generateTestCrash = jest.fn();
Crashes.hasCrashedInLastSession = jest.fn();
Crashes.lastSessionCrashReport = jest.fn();
Crashes.isEnabled = jest.fn();
Crashes.setEnabled = jest.fn();
Crashes.notifyUserConfirmation = jest.fn();
Crashes.setListener = jest.fn();
export default Crashes;
