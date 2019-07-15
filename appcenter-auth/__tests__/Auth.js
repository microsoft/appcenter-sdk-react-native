// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

import { NativeModules } from 'react-native';
import Auth from '../Auth';

describe('App Center Auth tests', () => {
  test('isEnabled is called', async () => {
    const spy = jest.spyOn(NativeModules.AppCenterReactNativeAuth, 'isEnabled');
    await Auth.isEnabled();
    expect(spy).toHaveBeenCalled();
  });

  test('setEnabled is called', async () => {
    const enabled = true;
    const spy = jest.spyOn(NativeModules.AppCenterReactNativeAuth, 'setEnabled');
    await Auth.setEnabled(enabled);
    expect(spy).toHaveBeenCalled();
  });

  test('signIn is called', async () => {
    const spy = jest.spyOn(NativeModules.AppCenterReactNativeAuth, 'signIn');
    await Auth.signIn();
    expect(spy).toHaveBeenCalled();
  });

  test('signOut is called', async () => {
    const spy = jest.spyOn(NativeModules.AppCenterReactNativeAuth, 'signOut');
    await Auth.signOut();
    expect(spy).toHaveBeenCalled();
  });
});
