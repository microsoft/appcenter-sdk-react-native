var __awaiter = (this && this.__awaiter) || function (thisArg, _arguments, P, generator) {
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
        function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
        function step(result) { result.done ? resolve(result.value) : new P(function (resolve) { resolve(result.value); }).then(fulfilled, rejected); }
        step((generator = generator.apply(thisArg, _arguments || [])).next());
    });
};
import React from 'react';
import { Text, View, ScrollView, TouchableOpacity } from 'react-native';
import Analytics from 'appcenter-analytics';
import SharedStyles from './SharedStyles';
export default class AnalyticsScreen extends React.Component {
    constructor(props) {
        super(props);
        this.state = {
            analyticsEnabled: false
        };
        this.toggleEnabled = this.toggleEnabled.bind(this);
    }
    componentDidMount() {
        return __awaiter(this, void 0, void 0, function* () {
            const component = this;
            const analyticsEnabled = yield Analytics.isEnabled();
            component.setState({ analyticsEnabled });
        });
    }
    toggleEnabled() {
        return __awaiter(this, void 0, void 0, function* () {
            yield Analytics.setEnabled(!this.state.analyticsEnabled);
            const analyticsEnabled = yield Analytics.isEnabled();
            this.setState({ analyticsEnabled });
        });
    }
    render() {
        return (<View style={SharedStyles.container}>
                <ScrollView>
                    <Text style={SharedStyles.heading}>
                        Test Analytics
          </Text>

                    <Text style={SharedStyles.enabledText}>
                        Analytics enabled: {this.state.analyticsEnabled ? 'yes' : 'no'}
                    </Text>
                    <TouchableOpacity onPress={this.toggleEnabled}>
                        <Text style={SharedStyles.toggleEnabled}>
                            toggle
            </Text>
                    </TouchableOpacity>

                    <TouchableOpacity onPress={() => Analytics.trackEvent('Button press', { page: 'Home page' })}>
                        <Text style={SharedStyles.button}>
                            Track Event
            </Text>
                    </TouchableOpacity>

                    <TouchableOpacity onPress={() => Analytics.trackEvent('Event without properties')}>
                        <Text style={SharedStyles.button}>
                            Track Event without properties
            </Text>
                    </TouchableOpacity>

                    <TouchableOpacity onPress={() => Analytics.trackEvent('Button press', { propertyValueTooLong: '12345678901234567890123456789012345678901234567890123456789012345' })}>
                        <Text style={SharedStyles.button}>
                            Track Event - event property value truncated after 64 characters
            </Text>
                    </TouchableOpacity>
                    <TouchableOpacity onPress={() => Analytics.trackEvent('Button press', undefined)}>
                        <Text style={SharedStyles.button}>
                            Track Event badly (Do not do this, only strings are supported)
            </Text>
                    </TouchableOpacity>

                </ScrollView>
            </View>);
    }
}
//# sourceMappingURL=AnalyticsScreen.js.map