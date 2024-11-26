// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

import { Alert, LogBox, Image } from 'react-native';
import { NavigationContainer } from '@react-navigation/native';
import { createBottomTabNavigator } from '@react-navigation/bottom-tabs';

import Crashes, { UserConfirmation } from 'appcenter-crashes';

import AppCenterScreen from './screens/AppCenterScreen';
import TransmissionScreen from './screens/TransmissionScreen';
import AnalyticsScreen from './screens/AnalyticsScreen';
import CrashesScreen from './screens/CrashesScreen';
import AttachmentsProvider from './AttachmentsProvider';

import AnalyticsTabBarIcon from './assets/analytics.png';
import DialsTabBarIcon from './assets/dials.png';
import CrashesTabBarIcon from './assets/crashes.png';
import TransmissionTabBarIcon from './assets/fuel.png';

const Tab = createBottomTabNavigator();

const App = () => {
  return (
    <NavigationContainer>
      <Tab.Navigator>
        <Tab.Screen
          name="AppCenter"
          component={AppCenterScreen}
          options={{
            tabBarLabel: 'AppCenter',
            tabBarIcon: () => (
              <Image style={{ width: 24, height: 24 }} source={DialsTabBarIcon} />
            ),
          }}
        />
        <Tab.Screen
          name="Analytics"
          component={AnalyticsScreen}
          options={{
            tabBarLabel: 'Analytics',
            tabBarIcon: () => (
              <Image style={{ width: 24, height: 24 }} source={AnalyticsTabBarIcon} />
            ),
          }}
        />
        <Tab.Screen
          name="Transmission"
          component={TransmissionScreen}
          options={{
            tabBarLabel: 'Transmission',
            tabBarIcon: () => (
              <Image style={{ width: 24, height: 24 }} source={TransmissionTabBarIcon} />
            ),
          }}
        />
        <Tab.Screen
          name="Crashes"
          component={CrashesScreen}
          options={{
            tabBarLabel: 'Crashes',
            tabBarIcon: () => (
              <Image style={{ width: 24, height: 24 }} source={CrashesTabBarIcon} />
            ),
          }}
        />
      </Tab.Navigator>
    </NavigationContainer>
  );
};

export default App;

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
  },

  onSendingSucceeded() {
    console.log('Did send crash. onSendingSucceeded is invoked.');
  },

  onSendingFailed() {
    console.log('Failed sending crash. onSendingFailed is invoked.');
  }
});
