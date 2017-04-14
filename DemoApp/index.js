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

export default class DemoApp extends Component {
  constructor() {
    super();
    this.state = {
      lastSessionStatus: "",
      sendStatus: ""
    };
  }

  async componentDidMount() {
    let status = "";
    const component = this;

    const crashedInLastSession = await Crashes.hasCrashedInLastSession();

    status += `Crashed: ${crashedInLastSession ? "yes" : "no"}\n\n`;
    component.setState({lastSessionStatus: status});

    if (crashedInLastSession) {
      const crashReport = await Crashes.lastSessionCrashReport()

      status += JSON.stringify(crashReport, null, 4);
      component.setState({lastSessionStatus: status});
    }
  }
  
  jsCrash() {
    var foo = new FooClass();
    foo.method1();
  }
  nativeCrash() {
    NativeModules.TestCrash.crash();
  }

  sendCrashes() {
    const component = this;
    Crashes.process(function (reports, send) {
      let status = "";

      if (reports.length === 0) {
        status += `Nothing to send\n`;
        component.setState({sendStatus: status});
        return;
      }

      Crashes.addEventListener({
        willSendCrash: function () {
          status += `Will send crash\n`;
          component.setState({sendStatus: status});
        },
        didSendCrash: function () {
          status += `Did send crash\n`;
          component.setState({sendStatus: status});
        },
        failedSendingCrash: function () {
          status += `Failed sending crash\n`;
          component.setState({sendStatus: status});
        }
      });

      let crashes = "";
      for (const report of reports) {
        if (crashes.length > 0) {
          crashes += "\n\n";
        }
        crashes += report.exceptionReason;
      }

      Alert.alert(
        `Send ${reports.length} crash(es)?`,
        crashes,
        [
          {text: 'Send', onPress: () => send(true) },
          {text: 'Ignore', onPress: () => send(false), style: 'cancel'},
        ]
      );
    });
  }

  render() {
    return (
      <View style={styles.container}>
        <ScrollView >
          <Text style={styles.welcome}>
            Welcome to Mobile Center!
          </Text>

          <TouchableOpacity onPress={this.jsCrash.bind(this)}>
            <Text style={styles.button}>
              Crash JavaScript
            </Text>
          </TouchableOpacity>
          <TouchableOpacity onPress={this.nativeCrash.bind(this)}>
            <Text style={styles.button}>
              Crash native code
            </Text>
          </TouchableOpacity>

          <TouchableOpacity onPress={this.sendCrashes.bind(this)}>
            <Text style={styles.button}>
              Send crashes
            </Text>
          </TouchableOpacity>
          <Text style={styles.lastSessionInfo}>
            {this.state.sendStatus}
          </Text>
    
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
  welcome: {
    fontSize: 20,
    textAlign: 'center',
    margin: 10,
  },
  container: {
    flex: 1,
    justifyContent: 'center',
    alignItems: 'center',
    backgroundColor: '#F5FCFF',
  },
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

AppRegistry.registerComponent('DemoApp', () => DemoApp);
