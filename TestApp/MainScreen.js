/**
 * Sample React Native App
 * https://github.com/facebook/react-native
 * @flow
 */

import React, { Component } from 'react';
import { AppState, Alert, Button, View, Platform, ToastAndroid, Text } from 'react-native';
import MobileCenter from 'mobile-center';
import Crashes, { UserConfirmation, ErrorAttachmentLog } from 'mobile-center-crashes';
import Push from 'mobile-center-push';
import SharedStyles from './SharedStyles';

export default class MainScreen extends Component {
  constructor() {
    super();
    this.state = {
      wrapperSdkVersion: MobileCenter.getSdkVersion()
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
        <Button title="Test Other Mobile Center APIs" onPress={() => navigate('MobileCenter')} />
      </View>
    );
  }
}

Push.setEventListener({
  pushNotificationReceived(pushNotification) {
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

Crashes.setEventListener({
  shouldProcess(report) {
    console.log('Should process report with id: ' + report["id"] + '\n');
    return true;
  },

  shouldAwaitUserConfirmation() {
    console.log('Should await user confirmation\n');
    Alert.alert(
      'One or more crashes were detected, do you want to report them?',
      null,
      [
        { text: 'Yes, and ask me again if it occurs again.', onPress: () => Crashes.notifyUserConfirmation(UserConfirmation.Send) },
        { text: 'Yes, always send them', onPress: () => Crashes.notifyUserConfirmation(UserConfirmation.AlwaysSend) },
        { text: 'Don\'t send at this time', onPress: () => Crashes.notifyUserConfirmation(UserConfirmation.DontSend) },
      ]
    );
    return true;
  },

  getErrorAttachments(report) {
    console.log('Get error attachments for report with id: ' + report["id"] + '\n');
    return [
      ErrorAttachmentLog.attachmentWithText("hello", "hello.txt"),
      ErrorAttachmentLog.attachmentWithBinary(testIcon, "icon.png", "image/png")
    ];
  },

  willSendCrash() {
    console.log('Will send crash\n');
  },

  didSendCrash() {
    console.log('Did send crash\n');
  },

  failedSendingCrash() {
    console.log('Failed sending crash\n');
  }
});

const testIcon = `iVBORw0KGgoAAAANSUhEUgAAABAAAAAQCAMAAAAoLQ9TAAAABGdBTUEAALGP
C/xhBQAAACBjSFJNAAB6JgAAgIQAAPoAAACA6AAAdTAAAOpgAAA6mAAAF3Cc
ulE8AAAA1VBMVEXLLmPLLWPLLWLMMWXLLGLMMmbcdJftt8nYY4vKLGHSSXfp
qL799fj////oobnVVYDUUX3LL2TccpX12OL88fXrsMT56O7NNWjhhaT56O3S
SHfTT3z56e777vPcc5bQQXH22+Tuvc7sssX++vv66/DuvM3sssbYZIv22uT7
7vLvvs79+PrUUH3OOmzjjqr66u/99vj23OXZZo3YYIn89Pf++fv22uPYYorX
YIjZaI767PHuusz99/nbb5TPQHDqqsD55+3ggqL55ez11+HRSHfUUn7TT3vg
lpRpAAAAAWJLR0QN9rRh9QAAAJpJREFUGNNjYMAKGJmYmZD5LKxs7BxMDJws
UD4nFzcPLx8LA7+AIJjPKiQsIirGJy4hKSwFUsMpLSMrJ6+gqKTMqyLACRRg
klflUVPX4NXU0lbRAQkwMOnqiegbGBoZmyAJaJqamVtABYBaDNgtDXmtrG0g
AkBDNW3tFFRFTaGGgqyVtXfgE3d0cnZhQXYYk6ubIA6nY3oOGQAAubQPeKPu
sH8AAAAldEVYdGRhdGU6Y3JlYXRlADIwMTctMDctMjhUMDM6NDE6MTUrMDI6
MDAk+3aMAAAAJXRFWHRkYXRlOm1vZGlmeQAyMDE3LTA3LTI4VDAzOjQxOjE1
KzAyOjAwVabOMAAAAABJRU5ErkJggg==`;
