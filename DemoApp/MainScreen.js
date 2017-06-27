/**
 * Sample React Native App
 * https://github.com/facebook/react-native
 * @flow
 */

import React, { Component } from 'react';
import { Button, View } from 'react-native';
import SharedStyles from './SharedStyles';

export default class MainScreen extends Component {
  static navigationOptions = {
    title: 'DemoApp',
  };

  constructor() {
    super();
    this.state = {
      lastSessionStatus: "",
      sendStatus: ""
    };
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

