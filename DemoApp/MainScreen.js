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

  async componentDidMount() {
    const component = this;

    Push.setEventListener({
      pushNotificationReceived: function (pushNotification) {
        const state = AppState.currentState;

        let msg = pushNotification.message;
        if (! msg) {
          msg = "<no message-Android backgnd?>"
        }
        if (pushNotification.customProperties && Object.keys(pushNotification.customProperties).length > 0) {
          msg += ' with custom properties:\n';
          msg += JSON.stringify(pushNotification.customProperties);
        }

        if (state === 'active') {
          Alert.alert(pushNotification.title, msg);
        }
        else {
          // This case should only happen on Android
          ToastAndroid.show('Background notification:\n' + msg, ToastAndroid.LONG);
        }
      }
    });
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

