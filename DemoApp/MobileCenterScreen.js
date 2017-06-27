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

import MobileCenter from 'mobile-center';
import SharedStyles from './SharedStyles';

export default class MobileCenterScreen extends React.Component {
  constructor() {
    super();
    this.state = {
      logLevel: 0
    };
  }

  async componentDidMount() {
    let status = "";
    const component = this;

    const logLevel = await MobileCenter.getLogLevel();
    component.setState({logLevel: logLevel});
  }

  async toggleVerboseLogging() {
    const logLevel = await MobileCenter.getLogLevel();
    const newLogLEvel = logLevel === MobileCenter.LogLevelWarning ? MobileCenter.LogLevelVerbose : MobileCenter.LogLevelWarning;
    await MobileCenter.setLogLevel(newLogLEvel); //just for testing
    this.setState({logLevel: newLogLEvel});
  }

  async setCustomProperties() {
    let properties = {
        'color': 'white',
        'number': 7
    };

    MobileCenter.setCustomProperties(properties);
  }

  render() {
    return (
      <View style={SharedStyles.container}>
        <ScrollView >
          <Text style={SharedStyles.heading}>
            Test MobileCenter
          </Text>

          <Text style={SharedStyles.enabledText}>
            Log level: {this.state.logLevel}
          </Text>
          <TouchableOpacity onPress={this.toggleVerboseLogging.bind(this)}>
            <Text style={SharedStyles.toggleEnabled}>
              toggle verbose
            </Text>
          </TouchableOpacity>

          <TouchableOpacity onPress={this.setCustomProperties.bind(this)}>
            <Text style={SharedStyles.toggleEnabled}>
              Set Custom Properties
            </Text>
          </TouchableOpacity>

        </ScrollView>
      </View>
    );
  }
}
