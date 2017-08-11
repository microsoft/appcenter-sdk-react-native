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

import MobileCenter, { CustomProperties } from 'mobile-center';
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
    component.setState({ mobileCenterEnabled: mobileCenterEnabled });

    const installId = await MobileCenter.getInstallId();
    component.setState({ installId: installId });

    const logLevel = await MobileCenter.getLogLevel();
    component.setState({ logLevel: logLevel });
  }

  async toggleEnabled() {
    await MobileCenter.setEnabled(!this.state.mobileCenterEnabled);

    const mobileCenterEnabled = await MobileCenter.isEnabled();
    this.setState({ mobileCenterEnabled: mobileCenterEnabled });
  }

  async toggleLogging() {
    let logLevel = await MobileCenter.getLogLevel();
    switch (logLevel) {

      case MobileCenter.LogLevelAssert:
        logLevel = MobileCenter.LogLevelNone;
        break;

      case MobileCenter.LogLevelNone:
        logLevel = MobileCenter.LogLevelVerbose;
        break;

      default:
        logLevel++;
    }
    await MobileCenter.setLogLevel(logLevel);
    this.setState({ logLevel: logLevel });
  }

  async setCustomProperties() {
    const properties = new CustomProperties().
    set('pi', 3.14)
    .clear('old')
    .set('optin', true)
    .set('score', 7)
    .set('now', new Date());
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
          <TouchableOpacity onPress={this.toggleLogging.bind(this)}>
            <Text style={SharedStyles.toggleEnabled}>
              Change log level
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
