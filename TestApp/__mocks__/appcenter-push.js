const Push = jest.mock('appcenter-push');
Push.isEnabled = jest.fn();
Push.setEnabled = jest.fn();
Push.setListener = jest.fn();
export default Push;