import { AppState, Alert, Platform, ToastAndroid } from 'react-native';
import { createBottomTabNavigator } from 'react-navigation';
import Toast from 'react-native-simple-toast';

import Crashes, { UserConfirmation, ErrorAttachmentLog } from 'appcenter-crashes';
import Push from 'appcenter-push';

import AppCenterScreen from './screens/AppCenterScreen';
import TransmissionScreen from './screens/TransmissionScreen';
import AnalyticsScreen from './screens/AnalyticsScreen';
import CrashesScreen from './screens/CrashesScreen';
import AttachmentsProvider from './AttachmentsProvider';

export default createBottomTabNavigator(
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

Push.setListener({
  onPushNotificationReceived(pushNotification) {
    let message = pushNotification.message;
    const title = pushNotification.title;

    // Message can be null on iOS silent push or Android background notifications.
    if (message === null) {
      message = '';
    } else {
      message += '\n';
    }

    // Any custom name/value pairs added in the portal are in customProperties
    if (pushNotification.customProperties && Object.keys(pushNotification.customProperties).length > 0) {
      message += `Custom properties:\n${JSON.stringify(pushNotification.customProperties)}`;
    }

    if (AppState.currentState === 'active') {
      Alert.alert(title, message);
    } else {
      // Sometimes the push callback is received shortly before the app is fully active in the foreground.
      // In this case you'll want to save off the notification info and wait until the app is fully shown
      // in the foreground before displaying any UI. You could use AppState.addEventListener to be notified
      // when the app is fully in the foreground.

      // Showing an alert when not in the "active" state seems to work on iOS; for Android, we show a toast
      // message instead
      if (Platform.OS === 'android') {
        ToastAndroid.show(`Notification while inactive:\n${message}`, ToastAndroid.LONG);
      }
      Alert.alert(title, message);
    }
  }
});

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
    return (async () => {
      const attachments = [];
      const [textAttachment, binaryAttachment, binaryName, binaryType] = await Promise.all([
        AttachmentsProvider.getTextAttachment(),
        AttachmentsProvider.getBinaryAttachment(),
        AttachmentsProvider.getBinaryName(),
        AttachmentsProvider.getBinaryType(),
      ]);
      if (textAttachment !== null) {
        attachments.push(ErrorAttachmentLog.attachmentWithText(textAttachment, 'hello.txt'));
      }
      if (binaryAttachment !== null && binaryName !== null && binaryType !== null) {
        attachments.push(ErrorAttachmentLog.attachmentWithBinary(binaryAttachment, binaryName, binaryType));
      }
      return attachments;
    })();
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
