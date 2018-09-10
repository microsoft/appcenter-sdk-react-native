const DeviceInfo = jest.mock('react-native-device-info');
DeviceInfo.getUniqueID = jest.fn();
export default DeviceInfo;
