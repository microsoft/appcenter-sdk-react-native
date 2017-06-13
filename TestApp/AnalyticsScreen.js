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
import SharedStyles from './SharedStyles';

export default class AnalyticsScreen extends React.Component {
  constructor() {
    super();
    this.state = {
      analyticsEnabled: false
    };
  }

  async componentDidMount() {
    let status = "";
    const component = this;

    const analyticsEnabled = await Analytics.isEnabled();
    component.setState({analyticsEnabled: analyticsEnabled});
  }

  async toggleEnabled() {
    await Analytics.setEnabled(! this.state.analyticsEnabled);

    const analyticsEnabled = await Analytics.isEnabled();
    this.setState({analyticsEnabled: analyticsEnabled});
  }

  render() {
    return (
      <View style={SharedStyles.container}>
        <ScrollView >
          <Text style={SharedStyles.heading}>
            Test Analytics
          </Text>

          <Text style={SharedStyles.enabledText}>
            Analytics enabled: {this.state.analyticsEnabled ? "yes" : "no"}
          </Text>
          <TouchableOpacity onPress={this.toggleEnabled.bind(this)}>
            <Text style={SharedStyles.toggleEnabled}>
              toggle
            </Text>
          </TouchableOpacity>

          <TouchableOpacity onPress={() => Analytics.trackEvent("Button press", { page: "Home page" })}>
            <Text style={SharedStyles.button}>
              Track Event
            </Text>
          </TouchableOpacity>

          <TouchableOpacity onPress={() => Analytics.trackEvent("Button press", data)}>
            <Text style={SharedStyles.button}>
              Track Event badly (Don't do this, only strings are supported)
            </Text>
          </TouchableOpacity>

        </ScrollView>
      </View>
    );
  }
}
