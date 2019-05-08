// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

import { NativeModules } from 'react-native';
import Analytics from '../Analytics';

describe('App Center Analytics tests', () => {
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

  test('trackEvent with null properties are sanitized', async () => {
    const eventName = 'Test event';
    const spy = jest.spyOn(NativeModules.AppCenterReactNativeAnalytics, 'trackEvent');
    await Analytics.trackEvent(eventName);
    expect(spy).toHaveBeenCalledWith(eventName, {});
  });

  test('trackEvent with invalid properties throws error', async () => {
    const eventName = 'Test event';
    const eventProperties = { invalid: { prop: 1 } };
    expect(() => Analytics.trackEvent(eventName, eventProperties))
      .toThrowError('Properties cannot be serialized. Object must only contain strings');
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

  test('setTransmissionTargetAppName is called', async () => {
    const appName = 'Test app';
    const token = 'Test token';
    const propertyConfigurator = new Analytics.PropertyConfigurator({ targetToken: token });
    const spy = jest.spyOn(NativeModules.AppCenterReactNativeAnalytics, 'setTransmissionTargetAppName');
    await propertyConfigurator.setAppName(appName);
    expect(spy).toHaveBeenCalledWith(appName, token);
  });

  test('setTransmissionTargetAppVersion is called', async () => {
    const appVersion = '1.0';
    const token = 'Test token';
    const propertyConfigurator = new Analytics.PropertyConfigurator({ targetToken: token });
    const spy = jest.spyOn(NativeModules.AppCenterReactNativeAnalytics, 'setTransmissionTargetAppVersion');
    await propertyConfigurator.setAppVersion(appVersion);
    expect(spy).toHaveBeenCalledWith(appVersion, token);
  });

  test('setTransmissionTargetAppLocale is called', async () => {
    const appLocale = 'Test locale';
    const token = 'Test token';
    const propertyConfigurator = new Analytics.PropertyConfigurator({ targetToken: token });
    const spy = jest.spyOn(NativeModules.AppCenterReactNativeAnalytics, 'setTransmissionTargetAppLocale');
    await propertyConfigurator.setAppLocale(appLocale);
    expect(spy).toHaveBeenCalledWith(appLocale, token);
  });

  test('setTransmissionTargetEventProperty is called', async () => {
    const key = 'Test key';
    const value = 'Test value';
    const token = 'Test token';
    const propertyConfigurator = new Analytics.PropertyConfigurator({ targetToken: token });
    const spy = jest.spyOn(NativeModules.AppCenterReactNativeAnalytics, 'setTransmissionTargetEventProperty');
    await propertyConfigurator.setEventProperty(key, value);
    expect(spy).toHaveBeenCalledWith(key, value, token);
  });

  test('removeTransmissionTargetEventProperty is called', async () => {
    const key = 'Test key';
    const token = 'Test token';
    const propertyConfigurator = new Analytics.PropertyConfigurator({ targetToken: token });
    const spy = jest.spyOn(NativeModules.AppCenterReactNativeAnalytics, 'removeTransmissionTargetEventProperty');
    await propertyConfigurator.removeEventProperty(key);
    expect(spy).toHaveBeenCalledWith(key, token);
  });

  test('collectTransmissionTargetDeviceId is called', async () => {
    const token = 'Test token';
    const propertyConfigurator = new Analytics.PropertyConfigurator({ targetToken: token });
    const spy = jest.spyOn(NativeModules.AppCenterReactNativeAnalytics, 'collectTransmissionTargetDeviceId');
    await propertyConfigurator.collectDeviceId();
    expect(spy).toHaveBeenCalledWith(token);
  });

  test('isTransmissionTargetEnabled is called', async () => {
    const token = 'Test token';
    const transmissionTarget = new Analytics.TransmissionTarget(token);
    const spy = jest.spyOn(NativeModules.AppCenterReactNativeAnalytics, 'isTransmissionTargetEnabled');
    await transmissionTarget.isEnabled();
    expect(spy).toHaveBeenCalledWith(token);
  });

  test('setTransmissionTargetEnabled is called', async () => {
    const enabled = true;
    const token = 'Test token';
    const transmissionTarget = new Analytics.TransmissionTarget(token);
    const spy = jest.spyOn(NativeModules.AppCenterReactNativeAnalytics, 'setTransmissionTargetEnabled');
    await transmissionTarget.setEnabled(enabled);
    expect(spy).toHaveBeenCalledWith(enabled, token);
  });

  test('trackTransmissionTargetEvent is called', async () => {
    const token = 'Test token';
    const eventName = 'Test event';
    const eventProperties = { string: 'value', number: 1, boolean: true };
    const expectedProperties = { string: 'value', number: '1', boolean: 'true' };
    const transmissionTarget = new Analytics.TransmissionTarget(token);
    const spy = jest.spyOn(NativeModules.AppCenterReactNativeAnalytics, 'trackTransmissionTargetEvent');
    await transmissionTarget.trackEvent(eventName, eventProperties);
    expect(spy).toHaveBeenCalledWith(eventName, expectedProperties, token);
  });

  test('getChildTransmissionTarget is called', async () => {
    const token = 'Test token';
    const childToken = 'Test child token';
    const transmissionTarget = new Analytics.TransmissionTarget(token);
    const spy = jest.spyOn(NativeModules.AppCenterReactNativeAnalytics, 'getChildTransmissionTarget');
    await transmissionTarget.getTransmissionTarget(childToken);
    expect(spy).toHaveBeenCalledWith(childToken, token);
  });
});
