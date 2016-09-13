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
  TouchableOpacity,
  View
} from 'react-native';

import SonomaAnalytics from 'react-native-sonoma-analytics';
import SonomaErrorReporting from 'react-native-sonoma-error-reporting';

class SonomaDemoApp extends Component {
  async componentDidMount() {
    if (await SonomaErrorReporting.hasCrashedInLastSession()) {
      let lastSessionCrashDetails = await SonomaErrorReporting.getLastSessionCrashDetails();
      Alert.alert(
        'Unhandled exception:',
        lastSessionCrashDetails.exception.split('\n')[0],
        [
          {text: 'Send crash', onPress: () => SonomaErrorReporting.sendCrashes() },
          {text: 'Ignore crash', onPress: () => SonomaErrorReporting.ignoreCrashes(), style: 'cancel'},
        ]
      );
    }
  }

  render() {
    return (
      <View style={styles.container}>
        <Text style={styles.welcome}>
          Welcome to Sonoma!
        </Text>

        <TouchableOpacity onPress={() => {
          if (__DEV__) {
            SonomaErrorReporting.generateTestCrash();
          } else {
            undefined.property;
          }
        }}>
          <Text style={styles.button}>
            Test Crash
          </Text>
        </TouchableOpacity>

        <TouchableOpacity onPress={() => SonomaAnalytics.trackEvent("Button press", { page: "Home page" })}>
          <Text style={styles.button}>
            Track Event
          </Text>
        </TouchableOpacity>
      </View>
    );
  }
}

const styles = StyleSheet.create({
  container: {
    flex: 1,
    justifyContent: 'center',
    alignItems: 'center',
    backgroundColor: '#F5FCFF',
  },
  welcome: {
    fontSize: 20,
    textAlign: 'center',
    margin: 10,
  },
  button: {
    color: '#4444FF',
    fontSize: 18,
    textAlign: 'center',
    margin: 10,
  },
});

AppRegistry.registerComponent('SonomaDemoApp', () => SonomaDemoApp);
