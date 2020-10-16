// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

/**
 * Sample React Native App
 * https://github.com/facebook/react-native
 * @flow
 */

import React, { Component } from 'react';
import { AppState, Alert, View, Platform, ToastAndroid, Text } from 'react-native';
import AppCenter from 'appcenter';
import Crashes, { UserConfirmation, ErrorAttachmentLog } from 'appcenter-crashes';
import SharedStyles from './SharedStyles';

export default class MainScreen extends Component {
  constructor() {
    super();
    this.state = {
      wrapperSdkVersion: AppCenter.getSdkVersion()
    };
  }

  static navigationOptions = {
    title: 'TestApp34',
  };

  render() {
    const { navigate } = this.props.navigation;

    return (
      <View style={SharedStyles.container}>
        <Text style={SharedStyles.heading}>
          React Native SDK version {this.state.wrapperSdkVersion}
        </Text>
        <Text style={SharedStyles.button} onPress={() => navigate('Crashes')}>Test Crashes</Text>
        <Text style={SharedStyles.button} onPress={() => navigate('Analytics')}>Test Analytics</Text>
        <Text style={SharedStyles.button} onPress={() => navigate('AppCenter')}>Test Other App Center APIs</Text>
      </View>
    );
  }
}

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
    return [
      ErrorAttachmentLog.attachmentWithText('hello', 'hello.txt'),
      ErrorAttachmentLog.attachmentWithBinary(testIcon, 'icon.png', 'image/png')
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
