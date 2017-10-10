/**
 * Sample React Native App
 * https://github.com/facebook/react-native
 * @flow
 */

import React, { Component } from 'react';
import {
  Alert,
  StyleSheet,
  Text,
  View,
  ScrollView,
  TouchableOpacity
} from 'react-native';

import Crashes from 'mobile-center-crashes';
import ErrorAttachmentLog from 'mobile-center-crashes';
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
    this.sendCrashes();
    
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

  sendCrashes() {
    const component = this;
    Crashes.setEventListener({
      shouldProcess(report) {
        console.log('Should process report with id: ' + report["id"] + '\n');       
        return true;
      },

      shouldAwaitUserConfirmation() {
        console.log('Should await user confirmation\n');
        return false;
      },

      getErrorAttachments(report) {        
        console.log('Get error attachments for report with id: ' + report["id"] + '\n');                
        return [
          ErrorAttachmentLog.attachmentWithText("hello", "hello.txt"),
          ErrorAttachmentLog.attachmentWithBinay("base64string", "icon.png", "image/png")
        ];
      },

      willSendCrash() {
        console.log('Will send crash\n');
      },

      didSendCrash() {
        console.log('Did send crash\n');
      },

      failedSendingCrash() {
        console.log('Failed sending crash\n');
      }
    });
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

const testIcon = `iVBORw0KGgoAAAANSUhEUgAAABAAAAAQCAMAAAAoLQ9TAAAABGdBTUEAALGP
C/xhBQAAACBjSFJNAAB6JgAAgIQAAPoAAACA6AAAdTAAAOpgAAA6mAAAF3Cc
ulE8AAAA1VBMVEXLLmPLLWPLLWLMMWXLLGLMMmbcdJftt8nYY4vKLGHSSXfp
qL799fj////oobnVVYDUUX3LL2TccpX12OL88fXrsMT56O7NNWjhhaT56O3S
SHfTT3z56e777vPcc5bQQXH22+Tuvc7sssX++vv66/DuvM3sssbYZIv22uT7
7vLvvs79+PrUUH3OOmzjjqr66u/99vj23OXZZo3YYIn89Pf++fv22uPYYorX
YIjZaI767PHuusz99/nbb5TPQHDqqsD55+3ggqL55ez11+HRSHfUUn7TT3vg
lpRpAAAAAWJLR0QN9rRh9QAAAJpJREFUGNNjYMAKGJmYmZD5LKxs7BxMDJws
UD4nFzcPLx8LA7+AIJjPKiQsIirGJy4hKSwFUsMpLSMrJ6+gqKTMqyLACRRg
klflUVPX4NXU0lbRAQkwMOnqiegbGBoZmyAJaJqamVtABYBaDNgtDXmtrG0g
AkBDNW3tFFRFTaGGgqyVtXfgE3d0cnZhQXYYk6ubIA6nY3oOGQAAubQPeKPu
sH8AAAAldEVYdGRhdGU6Y3JlYXRlADIwMTctMDctMjhUMDM6NDE6MTUrMDI6
MDAk+3aMAAAAJXRFWHRkYXRlOm1vZGlmeQAyMDE3LTA3LTI4VDAzOjQxOjE1
KzAyOjAwVabOMAAAAABJRU5ErkJggg==`;
