/**
 * Sample React Native App
 * https://github.com/facebook/react-native
 * @flow
 */

import React, { Component } from 'react';
import { Alert, Button, View } from 'react-native';
import SharedStyles from './SharedStyles';
import Push from 'mobile-center-push';

export default class MainScreen extends Component {
  static navigationOptions = {
    title: 'TestApp',
  };

  constructor() {
    super();
    this.state = {
      lastSessionStatus: "",
      sendStatus: ""
    };
  }

  async componentDidMount() {
    const component = this;

    Push.setEventListener({
      pushNotificationReceived: function (pushNotification) {
        let msg =  pushNotification.message;
        if (pushNotification.customProperties && Object.keys(pushNotification.customProperties).length > 0) {
          msg += ' with custom properties:\n';
          msg += JSON.stringify(pushNotification.customProperties);
        }
        Alert.alert(
          pushNotification.title,
          msg
        );
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

