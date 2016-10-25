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
import SonomaCrashes from 'react-native-sonoma-crashes';

var data = {};
data.x = "x value";
data.y = 12;
data.null = null;
data.undefined = undefined;

class SonomaDemoApp extends Component {
  async componentDidMount() {
    if (SonomaCrashes.hasCrashedInLastSession) {
      SonomaCrashes.process(function (reports, send) {
        SonomaCrashes.addEventListener({
          willSendCrash: function () {
            console.log("WILL SEND CRASH");
            console.log(arguments);
          },
          didSendCrash: function () {
            console.log("DID SEND CRASH");
            console.log(arguments);
          },
          failedSendingCrash: function () {
            console.log("FAILED SENDING CRASH");
            console.log(arguments);
          }
        });
        Alert.alert(
        'Unhandled exception:',
        reports[0].exceptionReason,
        [
          {text: 'Send crash', onPress: () => send(true,{}) },
          {text: 'Ignore crash', onPress: () => send(false,{}), style: 'cancel'},
        ]
      );
      });
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
            SonomaCrashes.generateTestCrash();
          } else {
            undefined.property;
          }
        }}>
          <Text style={styles.button}>
            Test Crash
          </Text>
        </TouchableOpacity>

        <TouchableOpacity onPress={() => {
            undefined.functionCall();
        }}>
          <Text style={styles.button}>
            Test JS Crash
          </Text>
        </TouchableOpacity>

        <TouchableOpacity onPress={() => SonomaAnalytics.trackEvent("Button press", { page: "Home page" })}>
          <Text style={styles.button}>
            Track Event
          </Text>
        </TouchableOpacity>

      <TouchableOpacity onPress={() => SonomaAnalytics.trackEvent("Button press", data)}>
          <Text style={styles.button}>
            Track Event badly
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
