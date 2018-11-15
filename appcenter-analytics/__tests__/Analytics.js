import { NativeModules } from 'react-native';
import Analytics from '../Analytics';

describe('Native module methods are called', () => {

  test('isEnabled is called', async () => {
    const spy = jest.spyOn(NativeModules.AppCenterReactNativeAnalytics, 'isEnabled');
    await Analytics.isEnabled();
    expect(spy).toHaveBeenCalled();
  });

  test('trackEvent is called', async () => {
    let eventName = "Test event";
    let eventProperties = {"string": "value", "number": 1, "boolean": true};
    let expectedProperties = {"string": "value", "number": "1", "boolean": "true"};
    const spy = jest.spyOn(NativeModules.AppCenterReactNativeAnalytics, 'trackEvent');
    await Analytics.trackEvent(eventName, eventProperties);
    expect(spy).toHaveBeenCalledWith(eventName, expectedProperties);
  });

  test('setEnabled is called', async () => {
    let enabled = true;
    const spy = jest.spyOn(NativeModules.AppCenterReactNativeAnalytics, 'setEnabled');
    await Analytics.setEnabled(enabled);
    expect(spy).toHaveBeenCalledWith(enabled);
  });

  test('getTransmissionTarget is called', async () => {
    let token = "Test token";
    const spy = jest.spyOn(NativeModules.AppCenterReactNativeAnalytics, 'getTransmissionTarget');
    await Analytics.getTransmissionTarget(token);
    expect(spy).toHaveBeenCalledWith(token);
  });

});
