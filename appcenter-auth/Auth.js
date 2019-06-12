// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

const ReactNative = require('react-native');

const { AppCenterReactNativeAuth } = ReactNative.NativeModules;

const Auth = {
    isEnabled() {
        return AppCenterReactNativeAuth.isEnabled();
    },
    setEnabled(enabled) {
        return AppCenterReactNativeAuth.setEnabled(enabled);
    },
    signIn() {
        return AppCenterReactNativeAuth.signIn();
    },
    signOut() {
        AppCenterReactNativeAuth.signOut();
    }
};

module.exports = Auth;
