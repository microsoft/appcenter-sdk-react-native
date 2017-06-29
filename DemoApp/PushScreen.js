/**
 * Sample React Native App
 * https://github.com/facebook/react-native
 * @flow
 */

import React, { Component } from 'react';
import {
  Alert,
  AppRegistry,
  StyleSheet,
  Text,
  View,
  ScrollView,
  TouchableOpacity,
  NativeModules
} from 'react-native';

import Push from 'mobile-center-push';
import SharedStyles from './SharedStyles';

export default class PushScreen extends React.Component {
  constructor() {
    super();
    this.state = {
      pushEnabled: false
    };
  }

  async componentDidMount() {
    const component = this;

    const pushEnabled = await Push.isEnabled();
    component.setState({pushEnabled: pushEnabled});
  }

  async toggleEnabled() {
    await Push.setEnabled(! this.state.pushEnabled);

    const pushEnabled = await Push.isEnabled();
    this.setState({pushEnabled: pushEnabled});
  }

  render() {
    return (
      <View style={SharedStyles.container}>
        <ScrollView >
          <Text style={SharedStyles.heading}>
            Test Push
          </Text>

          <Text style={SharedStyles.enabledText}>
            Push enabled: {this.state.pushEnabled ? "yes" : "no"}
          </Text>
          <TouchableOpacity onPress={this.toggleEnabled.bind(this)}>
            <Text style={SharedStyles.toggleEnabled}>
              toggle
            </Text>
          </TouchableOpacity>

        </ScrollView>
      </View>
    );
  }
}
