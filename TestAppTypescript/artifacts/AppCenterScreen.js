var __awaiter = (this && this.__awaiter) || function (thisArg, _arguments, P, generator) {
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
        function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
        function step(result) { result.done ? resolve(result.value) : new P(function (resolve) { resolve(result.value); }).then(fulfilled, rejected); }
        step((generator = generator.apply(thisArg, _arguments || [])).next());
    });
};
import React from 'react';
import { StyleSheet, Text, View, ScrollView, TouchableOpacity } from 'react-native';
import AppCenter, { CustomProperties } from 'appcenter';
import SharedStyles from './SharedStyles';
export default class AppCenterScreen extends React.Component {
    constructor(props) {
        super(props);
        this.state = {
            appCenterEnabled: false,
            installId: 'uninitialized',
            logLevel: -1
        };
        this.toggleEnabled = this.toggleEnabled.bind(this);
        this.toggleLogging = this.toggleLogging.bind(this);
        this.setCustomProperties = this.setCustomProperties.bind(this);
    }
    componentDidMount() {
        return __awaiter(this, void 0, void 0, function* () {
            const component = this;
            const appCenterEnabled = yield AppCenter.isEnabled();
            component.setState({ appCenterEnabled });
            const installId = yield AppCenter.getInstallId();
            component.setState({ installId });
            const logLevel = yield AppCenter.getLogLevel();
            component.setState({ logLevel });
        });
    }
    toggleEnabled() {
        return __awaiter(this, void 0, void 0, function* () {
            yield AppCenter.setEnabled(!this.state.appCenterEnabled);
            const appCenterEnabled = yield AppCenter.isEnabled();
            this.setState({ appCenterEnabled });
        });
    }
    toggleLogging() {
        return __awaiter(this, void 0, void 0, function* () {
            let logLevel = yield AppCenter.getLogLevel();
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
            yield AppCenter.setLogLevel(logLevel);
            this.setState({ logLevel });
        });
    }
    setCustomProperties() {
        return __awaiter(this, void 0, void 0, function* () {
            const properties = new CustomProperties()
                .set('pi', 3.14)
                .clear('old')
                .set('color', 'blue')
                .set('optin', true)
                .set('optout', false)
                .set('score', 7)
                .set('now', new Date());
            yield AppCenter.setCustomProperties(properties);
        });
    }
    render() {
        return (<View style={SharedStyles.container}>
                <ScrollView>
                    <Text style={SharedStyles.heading}>
                        Test AppCenter
          </Text>

                    <Text style={SharedStyles.enabledText}>
                        AppCenter enabled: {this.state.appCenterEnabled ? 'yes' : 'no'}
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
            </View>);
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
//# sourceMappingURL=AppCenterScreen.js.map