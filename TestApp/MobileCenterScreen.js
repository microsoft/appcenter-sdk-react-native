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
      mobileCenterEnabled: false,
      installId: "uninitialized",
      logLevel: -1   // default to something invalid; shouldn't show in UI
    };
  }

  async componentDidMount() {
    let status = "";
    const component = this;

    const mobileCenterEnabled = await MobileCenter.isEnabled();
    component.setState({mobileCenterEnabled: mobileCenterEnabled});

    const installId = await MobileCenter.getInstallId();
    component.setState({installId: installId});

    const logLevel = await MobileCenter.getLogLevel();
    component.setState({logLevel: logLevel});
  }

  async toggleEnabled() {
    await MobileCenter.setEnabled(! this.state.mobileCenterEnabled);

    const mobileCenterEnabled = await MobileCenter.isEnabled();
    this.setState({mobileCenterEnabled: mobileCenterEnabled});
  }

  async toggleVerboseLogging() {
    const logLevel = await MobileCenter.getLogLevel();
    const newLogLEvel = logLevel === MobileCenter.LogLevelWarning ? MobileCenter.LogLevelVerbose : MobileCenter.LogLevelWarning;
    await MobileCenter.setLogLevel(newLogLEvel); //just for testing
    this.setState({logLevel: newLogLEvel});
  }

  async setCustomProperties() {
    let properties = {
        'color': 'red',
        'number': 2,
        'isEnabled': true,
        'MyCustomDate': new Date()
    };

    await MobileCenter.setCustomProperties(properties);
  }

  render() {
    return (
      <View style={SharedStyles.container}>
        <ScrollView >
          <Text style={SharedStyles.heading}>
            Test MobileCenter
          </Text>

          <Text style={SharedStyles.enabledText}>
            Mobile Center enabled: {this.state.mobileCenterEnabled ? "yes" : "no"}
          </Text>
          <TouchableOpacity onPress={this.toggleEnabled.bind(this)}>
            <Text style={SharedStyles.toggleEnabled}>
              toggle
            </Text>
          </TouchableOpacity>

          <Text style={styles.installIdHeader}>
            Install ID
          </Text>
          <Text style={styles.installId}>
            {this.state.installId}
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

const styles = StyleSheet.create({
  installIdHeader: {
    fontSize: 14,
    textAlign: 'center',
  },
  installId: {
    fontSize: 10,
    textAlign: 'center',
    marginBottom: 10
  }
});
