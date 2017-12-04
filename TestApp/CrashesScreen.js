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
  TextInput,
  ScrollView,
  AsyncStorage,
  TouchableOpacity
} from 'react-native';

import { DialogComponent } from 'react-native-dialog-component';
import Crashes from 'appcenter-crashes';
import { FooClass } from './js/FooClass';
import SharedStyles from './SharedStyles';

const TEXT_ATTACHMENT_KEY = 'TEXT_ATTACHMENT_KEY';
const BINARY_ATTACHMENT_KEY = 'BINARY_ATTACHMENT_KEY';

export default class CrashesScreen extends Component {
  constructor() {
    super();
    this.state = {
      crashesEnabled: false,
      lastSessionStatus: '',
      sendStatus: '',
      textAttachment:''
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

    const textAttachment = await AsyncStorage.getItem(TEXT_ATTACHMENT_KEY);
    component.setState({textAttachment: textAttachment});
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

  async saveAttachmentValue(key, value) {
    await AsyncStorage.setItem(key,value)
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
          <TouchableOpacity onPress={() => {this.dialogComponent.show()}}>
            <Text style={styles.button}>
              Set text attachmet
            </Text>
          </TouchableOpacity>
          <Text style={SharedStyles.enabledText}>{'Current value:'}{this.state.textAttachment}</Text>
          <Text style={styles.lastSessionHeader}>Last session:</Text>
          <Text style={styles.lastSessionInfo}>
            {this.state.lastSessionStatus}
          </Text>
        </ScrollView>
        <DialogComponent 
        ref={(dialogComponent) => { this.dialogComponent = dialogComponent; }}
        width={0.9}>
          <View>
            <TextInput
            style={{height: 40, borderColor: 'gray', borderWidth: 1, margin: 8}}
            onChangeText = {(text) => this.setState({ textAttachment: text })}/>
            <View style={{flex: 1, flexDirection: 'row', justifyContent: 'space-between',}}>
              <TouchableOpacity
                style={{height:50}}
                onPress={() => {
                    this.saveAttachmentValue(TEXT_ATTACHMENT_KEY, this.state.textAttachment);
                    this.dialogComponent.dismiss();
                  }}>
                <Text style={styles.button}>
                  Save
                </Text>
              </TouchableOpacity>
              <TouchableOpacity
                style={{height:50}}
                onPress={() => {this.dialogComponent.dismiss()}}>
                <Text style={styles.button}>
                  Cancel
                </Text>
              </TouchableOpacity>
            </View>
          </View>
        </DialogComponent>
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
