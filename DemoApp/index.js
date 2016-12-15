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

import Analytics from 'mobile-center-analytics';
import Crashes from 'mobile-center-crashes';
import {FooClass} from './js/FooClass';

var data = {};
data.x = "x value";
data.y = 12;
data.null = null;
data["undefd"] = undefined;
data["recursive"] = data;
data["function"] = function () { console.log(arguments);};

class DemoApp extends Component {
  async componentDidMount() {
    if (Crashes.hasCrashedInLastSession) {
      Crashes.process(function (reports, send) {
        Crashes.addEventListener({
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
          Welcome to Mobile Center!
        </Text>

        <TouchableOpacity onPress={() => {
          if (__DEV__) {
            Crashes.generateTestCrash();
          } else {
            undefined.property;
          } }}>

          <Text style={styles.button}>
            Test Crash
          </Text>
        </TouchableOpacity>

        <TouchableOpacity onPress={() => { this.jsCrash(); }}>
          <Text style={styles.button}>
            Test JavaScript Crash
          </Text>
        </TouchableOpacity>

        <TouchableOpacity onPress={() => Analytics.trackEvent("Button press", { page: "Home page" })}>
          <Text style={styles.button}>
            Track Event
          </Text>
        </TouchableOpacity>

      <TouchableOpacity onPress={() => Analytics.trackEvent("Button press", data)}>
          <Text style={styles.button}>
            Track Event badly (Don't do this, only strings are supported)
          </Text>
        </TouchableOpacity>
      </View>
    );
  }

  jsCrash() {
    this.sourceOfCrashFunction();
  }

  sourceOfCrashFunction() {
    throw new Error('This is a JavaScript crash message');
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

AppRegistry.registerComponent('DemoApp', () => DemoApp);
