import { NativeModules } from 'react-native';
import Analytics from '../Analytics';

describe('Native module methods are called', () => {
  test('isEnabled is called', async () => {
    const spy = jest.spyOn(NativeModules.AppCenterReactNativeAnalytics, 'isEnabled');
    await Analytics.isEnabled();
    expect(spy).toHaveBeenCalled();
  });

  test('trackEvent is called', async () => {
    const eventName = 'Test event';
    const eventProperties = { string: 'value', number: 1, boolean: true };
    const expectedProperties = { string: 'value', number: '1', boolean: 'true' };
    const spy = jest.spyOn(NativeModules.AppCenterReactNativeAnalytics, 'trackEvent');
    await Analytics.trackEvent(eventName, eventProperties);
    expect(spy).toHaveBeenCalledWith(eventName, expectedProperties);
  });

  test('setEnabled is called', async () => {
    const enabled = true;
    const spy = jest.spyOn(NativeModules.AppCenterReactNativeAnalytics, 'setEnabled');
    await Analytics.setEnabled(enabled);
    expect(spy).toHaveBeenCalledWith(enabled);
  });

  test('getTransmissionTarget is called', async () => {
    const token = 'Test token';
    const spy = jest.spyOn(NativeModules.AppCenterReactNativeAnalytics, 'getTransmissionTarget');
    await Analytics.getTransmissionTarget(token);
    expect(spy).toHaveBeenCalledWith(token);
  });
});
