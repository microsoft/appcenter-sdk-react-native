/**
 * Sample React Native App
 * https://github.com/facebook/react-native
 * @flow
 */

import React from 'react';
import {
  Text,
  View,
  ScrollView,
  TouchableOpacity
} from 'react-native';

import Analytics from 'mobile-center-analytics';
import SharedStyles from './SharedStyles';

export default class AnalyticsScreen extends React.Component {
  constructor() {
    super();
    this.state = {
      analyticsEnabled: false
    };
    this.toggleEnabled = this.toggleEnabled.bind(this);
  }

  async componentDidMount() {
    const component = this;

    const analyticsEnabled = await Analytics.isEnabled();
    component.setState({ analyticsEnabled });
  }

  async toggleEnabled() {
    await Analytics.setEnabled(!this.state.analyticsEnabled);

    const analyticsEnabled = await Analytics.isEnabled();
    this.setState({ analyticsEnabled });
  }

  /* eslint-disable no-undef */
  render() {
    return (
      <View style={SharedStyles.container}>
        <ScrollView >
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

          <TouchableOpacity onPress={() => Analytics.trackEvent('Button press', { propertyValueTooLong: '12345678901234567890123456789012345678901234567890123456789012345' })}>
            <Text style={SharedStyles.button}>
              Track Event - event property value truncated after 64 characters
            </Text>
          </TouchableOpacity>

          <TouchableOpacity onPress={() => Analytics.trackEvent('Button press', data)}>
            <Text style={SharedStyles.button}>
              Track Event badly (Do not do this, only strings are supported)
            </Text>
          </TouchableOpacity>

        </ScrollView>
      </View>
    );
  }
  /* eslint-enable no-undef */
}
