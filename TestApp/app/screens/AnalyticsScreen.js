// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

import React, { Component } from 'react';
import { Image, View, Text, Switch, SectionList, NativeModules, TouchableOpacity } from 'react-native';

import Analytics from 'appcenter-analytics';

import SharedStyles from '../SharedStyles';

export default class AnalyticsScreen extends Component {

  state = {
    analyticsEnabled: false,
    isManualSessionEnabled: false
  }

  async componentDidMount() {
    NativeModules.TestAppNative.getManualSessionTrackerState().then((isEnabled) => {
      const isManualSessionEnabled = isEnabled === 1;
      this.setState({ isManualSessionEnabled });
    });
    await this.refreshToggle();

    const unsubscribe = this.props.navigation.addListener('tabPress', (e) => {
      this.refreshToggle();
    });
  
    return unsubscribe;
  }

  async refreshToggle() {
    const analyticsEnabled = await Analytics.isEnabled();
    this.setState({ analyticsEnabled });
  }

  render() {
    const switchRenderItem = ({ item: { title, value, toggle } }) => (
      <View style={SharedStyles.item}>
        <Text style={SharedStyles.itemTitle}>{title}</Text>
        <Switch value={this.state[value]} onValueChange={toggle} />
      </View>
    );

    const actionRenderItem = ({ item: { title, action } }) => (
      <TouchableOpacity style={SharedStyles.item} onPress={action}>
        <Text style={SharedStyles.itemButton}>{title}</Text>
      </TouchableOpacity>
    );

    return (
      <View style={SharedStyles.container}>
        <SectionList
          renderItem={({ item }) => <Text style={[SharedStyles.item, SharedStyles.itemTitle]}>{item}</Text>}
          renderSectionHeader={({ section: { title } }) => <Text style={SharedStyles.header}>{title}</Text>}
          keyExtractor={(item, index) => item + index}
          sections={[
            {
              title: 'Settings',
              data: [
                {
                  title: 'Analytics Enabled',
                  value: 'analyticsEnabled',
                  toggle: async () => {
                    await Analytics.setEnabled(!this.state.analyticsEnabled);
                    const analyticsEnabled = await Analytics.isEnabled();
                    this.setState({ analyticsEnabled });
                  }
                },
                {
                  title: 'Manual Session Tracking Enabled',
                  value: 'isManualSessionEnabled',
                  toggle: async () => {
                    const isManualSessionEnabled = !this.state.isManualSessionEnabled;
                    await NativeModules.TestAppNative.saveManualSessionTrackerState(isManualSessionEnabled);
                    this.setState({ isManualSessionEnabled });
                  }
                },
              ],
              renderItem: switchRenderItem
            },
            {
              title: 'Actions',
              data: [
                {
                  title: 'Track event without properties',
                  action: () => {
                    const eventName = 'EventWithoutProperties';
                    Analytics.trackEvent(eventName);
                    console.log(`Scheduled event '${eventName}'.`);
                  }
                },
                {
                  title: 'Track event with properties',
                  action: () => {
                    const eventName = 'EventWithProperties';
                    Analytics.trackEvent(eventName, { property1: '100', property2: '200' });
                    console.log(`Scheduled event '${eventName}'.`);
                  }
                },
                {
                  title: 'Track event with long property value',
                  action: () => {
                    const eventName = 'EventWithLongProperties';
                    Analytics.trackEvent(eventName, { propertyValueTooLong: '12345678901234567890123456789012345678901234567890123456789012345' });
                    console.log(`Scheduled event '${eventName}'.`);
                  }
                },
                {
                  title: 'Start session',
                  action: () => {
                    Analytics.startSession();
                  }
                },
              ],
              renderItem: actionRenderItem
            },
          ]}
        />
      </View>
    );
  }
}
