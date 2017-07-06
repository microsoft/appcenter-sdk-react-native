/**
 * Sample React Native App
 * https://github.com/facebook/react-native
 * @flow
 */

import React, { Component } from 'react';
import { AppState, Alert, Button, Text, View, ToastAndroid } from 'react-native';
import SharedStyles from './SharedStyles';
import Push from 'mobile-center-push';


export default class MainScreen extends Component {
  static navigationOptions = {
    title: 'DemoApp',
  };

  constructor() {
    super();
  }

  render() {
    const { navigate } = this.props.navigation;

    return (
      <View style={SharedStyles.container}>

        <Button
          title="Test Crashes"
          onPress={() =>
            navigate('Crashes')
          }
        />

        <Button
          title="Test Analytics"
          onPress={() =>
            navigate('Analytics')
          }
        />

        <Button
          title="Test Push"
          onPress={() =>
            navigate('Push')
          }
        />

         <Button
          title="Test Mobile Center"
          onPress={() =>
            navigate('MobileCenter')
          }
        />

      </View>
    );
  }
}

Push.setEventListener({
  pushNotificationReceived: function (pushNotification) {
    let message = pushNotification.message;
    let title = pushNotification.title;

    if (message === null || message === undefined) {
      // Android messages received in the background don't include a message. On Android, that fact can be used to
      // check if the message was received in the background or foreground. For iOS the message is always present.
      title = "Android background"
      message = "<empty>"
    }

    // Any custom name/value pairs added in the portal are in customProperties
    if (pushNotification.customProperties && Object.keys(pushNotification.customProperties).length > 0) {
      message += '\nCustom properties:\n' + JSON.stringify(pushNotification.customProperties);
    }

    if (AppState.currentState === 'active') {
      Alert.alert(title, message);
    }
    else {
      // Sometimes the push callback is received shortly before the app is fully active in the foreground.
      // In this case you'll want to save off the notification info and wait until the app is fully shown
      // in the foreground before displaying any UI. You could use AppState.addEventListener to be notified
      // when the app is fully in the foreground.

      // Showing an alert when not in the "active" state seems to work on iOS; for Android, we show a toast
      // message instead
      if (Platform.OS === "android") {
        ToastAndroid.show('Notification while inactive:\n' + message, ToastAndroid.LONG);
      }
      else {
        Alert.alert(title, message);
      }
    }
  }
});
