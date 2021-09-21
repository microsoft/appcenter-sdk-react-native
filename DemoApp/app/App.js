// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

import { Alert, YellowBox } from 'react-native';
import { createBottomTabNavigator, createAppContainer } from 'react-navigation';
import Toast from 'react-native-simple-toast';

import Crashes, { UserConfirmation } from 'appcenter-crashes';

import AppCenterScreen from './screens/AppCenterScreen';
import TransmissionScreen from './screens/TransmissionScreen';
import AnalyticsScreen from './screens/AnalyticsScreen';
import CrashesScreen from './screens/CrashesScreen';
import AttachmentsProvider from './AttachmentsProvider';

YellowBox.ignoreWarnings(['Remote debugger']);

const TabNavigator = createBottomTabNavigator(
  {
    AppCenter: AppCenterScreen,
    Analytics: AnalyticsScreen,
    Transmission: TransmissionScreen,
    Crashes: CrashesScreen
  },
  {
    tabBarOptions: {
      activeBackgroundColor: 'white',
      inactiveBackgroundColor: 'white',
    },
  }
);

export default createAppContainer(TabNavigator);

Crashes.setListener({
  shouldProcess(report) {
    console.log(`Should process report with id: ${report.id}'`);
    return true;
  },

  shouldAwaitUserConfirmation() {
    console.log('Should await user confirmation');
    Alert.alert(
      'One or more crashes were detected, do you want to report them?',
      null,
      [
        { text: 'Yes, and ask me again if it occurs again.', onPress: () => Crashes.notifyUserConfirmation(UserConfirmation.SEND) },
        { text: 'Yes, always send them', onPress: () => Crashes.notifyUserConfirmation(UserConfirmation.ALWAYS_SEND) },
        { text: 'Don\'t send at this time', onPress: () => Crashes.notifyUserConfirmation(UserConfirmation.DONT_SEND) },
      ]
    );
    return true;
  },

  getErrorAttachments(report) {
    console.log(`Get error attachments for report with id: ${report.id}'`);
    return AttachmentsProvider.getErrorAttachments();
  },

  onBeforeSending() {
    console.log('Will send crash. onBeforeSending is invoked.');
    Toast.show('Sending crashes...');
  },

  onSendingSucceeded() {
    console.log('Did send crash. onSendingSucceeded is invoked.');
    Toast.show('Sending crashes succeeded.');
  },

  onSendingFailed() {
    console.log('Failed sending crash. onSendingFailed is invoked.');
    Toast.show('Sending crashes failed, please check verbose logs.');
  }
});
