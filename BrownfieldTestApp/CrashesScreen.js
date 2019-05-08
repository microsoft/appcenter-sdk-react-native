// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

/**
 * Sample React Native App
 * https://github.com/facebook/react-native
 * @flow
 */

import React, { Component } from 'react';
import {
  StyleSheet,
  Text,
  View,
  ScrollView,
  TouchableOpacity
} from 'react-native';

import Crashes from 'appcenter-crashes';
import { FooClass } from './js/FooClass';
import SharedStyles from './SharedStyles';

export default class CrashesScreen extends Component {
  constructor() {
    super();
    this.state = {
      crashesEnabled: false,
      lastSessionStatus: '',
      sendStatus: ''
    };
    this.toggleEnabled = this.toggleEnabled.bind(this);
    this.jsCrash = this.jsCrash.bind(this);
    this.nativeCrash = this.nativeCrash.bind(this);
  }

  async componentDidMount() {
    let status = '';
    const component = this;

    const crashesEnabled = await Crashes.isEnabled();
    component.setState({ crashesEnabled });

    const crashedInLastSession = await Crashes.hasCrashedInLastSession();

    status += `Crashed: ${crashedInLastSession ? 'yes' : 'no'}\n\n`;
    component.setState({ lastSessionStatus: status });

    if (crashedInLastSession) {
      const crashReport = await Crashes.lastSessionCrashReport();

      status += JSON.stringify(crashReport, null, 4);
      component.setState({ lastSessionStatus: status });
    }
  }

  async toggleEnabled() {
    await Crashes.setEnabled(!this.state.crashesEnabled);

    const crashesEnabled = await Crashes.isEnabled();
    this.setState({ crashesEnabled });
  }

  jsCrash() {
    const foo = new FooClass();
    foo.method1();
  }

  nativeCrash() {
    Crashes.generateTestCrash();
  }

  render() {
    return (
      <View style={SharedStyles.container}>
        <ScrollView>
          <Text style={SharedStyles.heading}>
            Test Crashes
          </Text>

          <Text style={SharedStyles.enabledText}>
            Crashes enabled: {this.state.crashesEnabled ? 'yes' : 'no'}
          </Text>
          <TouchableOpacity onPress={this.toggleEnabled}>
            <Text style={SharedStyles.toggleEnabled}>
              toggle
            </Text>
          </TouchableOpacity>

          <TouchableOpacity onPress={this.jsCrash}>
            <Text style={styles.button}>
              Crash JavaScript
            </Text>
          </TouchableOpacity>
          <TouchableOpacity onPress={this.nativeCrash}>
            <Text style={styles.button}>
              Crash native code
            </Text>
          </TouchableOpacity>
          <Text style={styles.lastSessionHeader}>Last session:</Text>
          <Text style={styles.lastSessionInfo}>
            {this.state.lastSessionStatus}
          </Text>
        </ScrollView>
      </View>
    );
  }
}


const styles = StyleSheet.create({
  button: {
    color: '#4444FF',
    fontSize: 18,
    textAlign: 'center',
    margin: 10,
  },
  lastSessionHeader: {
    fontSize: 20,
    textAlign: 'center',
    marginTop: 30
  },
  lastSessionInfo: {
    fontSize: 14,
    textAlign: 'center',
  },
});
