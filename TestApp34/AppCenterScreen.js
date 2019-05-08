// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

/**
 * Sample React Native App
 * https://github.com/facebook/react-native
 * @flow
 */

import React from 'react';
import {
  StyleSheet,
  Text,
  View,
  ScrollView,
  TouchableOpacity
} from 'react-native';

import AppCenter, { CustomProperties } from 'appcenter';
import SharedStyles from './SharedStyles';

export default class Screen extends React.Component {
  constructor() {
    super();
    this.state = {
      appCenterEnabled: false,
      installId: 'uninitialized',
      logLevel: -1   // default to something invalid; shouldn't show in UI
    };
    this.toggleEnabled = this.toggleEnabled.bind(this);
    this.toggleLogging = this.toggleLogging.bind(this);
    this.setCustomProperties = this.setCustomProperties.bind(this);
  }

  async componentDidMount() {
    const component = this;

    const appCenterEnabled = await AppCenter.isEnabled();
    component.setState({ appCenterEnabled });

    const installId = await AppCenter.getInstallId();
    component.setState({ installId });

    const logLevel = await AppCenter.getLogLevel();
    component.setState({ logLevel });
  }

  async toggleEnabled() {
    await AppCenter.setEnabled(!this.state.appCenterEnabled);

    const appCenterEnabled = await AppCenter.isEnabled();
    this.setState({ appCenterEnabled });
  }

  async toggleLogging() {
    let logLevel = await AppCenter.getLogLevel();
    switch (logLevel) {
      case AppCenter.LogLevelAssert:
        logLevel = AppCenter.LogLevelNone;
        break;

      case AppCenter.LogLevelNone:
        logLevel = AppCenter.LogLevelVerbose;
        break;

      default:
        logLevel++;
    }
    await AppCenter.setLogLevel(logLevel);
    this.setState({ logLevel });
  }

  async setCustomProperties() {
    const properties = new CustomProperties()
      .set('pi', 3.14)
      .clear('old')
      .set('color', 'blue')
      .set('optin', true)
      .set('optout', false)
      .set('score', 7)
      .set('now', new Date());
    await AppCenter.setCustomProperties(properties);
  }

  render() {
    return (
      <View style={SharedStyles.container}>
        <ScrollView >
          <Text style={SharedStyles.heading}>
            Test AppCenter
          </Text>

          <Text style={SharedStyles.enabledText}>
            App Center enabled: {this.state.appCenterEnabled ? 'yes' : 'no'}
          </Text>
          <TouchableOpacity onPress={this.toggleEnabled}>
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
          <TouchableOpacity onPress={this.toggleLogging}>
            <Text style={SharedStyles.toggleEnabled}>
              Change log level
            </Text>
          </TouchableOpacity>

          <TouchableOpacity onPress={this.setCustomProperties}>
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
