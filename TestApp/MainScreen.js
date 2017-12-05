/**
 * Sample React Native App
 * https://github.com/facebook/react-native
 * @flow
 */

import React, { Component } from 'react';
import { AppState, Alert, Button, View, Platform, ToastAndroid, Text } from 'react-native';
import AppCenter from 'appcenter';
import Crashes, { UserConfirmation, ErrorAttachmentLog } from 'appcenter-crashes';
import Push from 'appcenter-push';
import SharedStyles from './SharedStyles';
import AttachmentsProvider from './AttachmentsProvider';

export default class MainScreen extends Component {
  constructor() {
    super();
    this.state = {
      wrapperSdkVersion: AppCenter.getSdkVersion()
    };
  }

  static navigationOptions = {
    title: 'TestApp',
  };

  render() {
    const { navigate } = this.props.navigation;
    return (
      <View style={SharedStyles.container}>
        <Text style={SharedStyles.heading}>
          React Native SDK version {this.state.wrapperSdkVersion}
        </Text>
        <Button title="Test Crashes" onPress={() => navigate('Crashes')} />
        <Button title="Test Analytics" onPress={() => navigate('Analytics')} />
        <Button title="Test Push" onPress={() => navigate('Push')} />
        <Button title="Test Other AppCenter APIs" onPress={() => navigate('AppCenter')} />
      </View>
    );
  }
}

Push.setListener({
  onPushNotificationReceived(pushNotification) {
    let message = pushNotification.message;
    let title = pushNotification.title;

    if (message === null || message === undefined) {
      // Android messages received in the background don't include a message. On Android, that fact can be used to
      // check if the message was received in the background or foreground. For iOS the message is always present.
      title = 'Android background';
      message = '<empty>';
    }

    // Any custom name/value pairs added in the portal are in customProperties
    if (pushNotification.customProperties && Object.keys(pushNotification.customProperties).length > 0) {
      message += `\nCustom properties:\n${JSON.stringify(pushNotification.customProperties)}`;
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

  async getErrorAttachments(report) {
    console.log(`Get error attachments for report with id: ${report.id}'`);
    let textAttachment = await AttachmentsProvider.getTextAttachment();
    let binaryAttachment = await AttachmentsProvider.getBinaryAttachment();
    let binaryName = await AttachmentsProvider.getBinaryName(); 
    let binaryType = await AttachmentsProvider.getBinaryType(); 
    return [
      ErrorAttachmentLog.attachmentWithText(textAttachment, 'hello.txt'),
      ErrorAttachmentLog.attachmentWithBinary(binaryAttachment, binaryName, binaryType)
    ];
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
