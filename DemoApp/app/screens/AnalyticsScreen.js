import React, { Component } from 'react';
import { Image, View, Text, Switch, SectionList, TouchableOpacity } from 'react-native';
import Toast from 'react-native-simple-toast';

import Analytics from 'appcenter-analytics';

import SharedStyles from '../SharedStyles';
import AnalyticsTabBarIcon from '../assets/analytics.png';

export default class AnalyticsScreen extends Component {
  static navigationOptions = {
    tabBarIcon: () => <Image style={{ width: 24, height: 24 }} source={AnalyticsTabBarIcon} />,
    tabBarOnPress: ({ defaultHandler, navigation }) => {
      const refreshAnalytics = navigation.getParam('refreshAnalytics');

      // Initial press: the function is not defined yet so nothing to refresh.
      if (refreshAnalytics) {
        refreshAnalytics();
      }
      defaultHandler();
    }
  }

  state = {
    analyticsEnabled: false
  }

  async componentWillMount() {
    await this.refreshToggle();

    this.props.navigation.setParams({
      refreshAnalytics: this.refreshToggle.bind(this)
    });
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

    const showEventToast = eventName => Toast.show(`Scheduled event '${eventName}'.`);

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
                    showEventToast(eventName);
                  }
                },
                {
                  title: 'Track event with properties',
                  action: () => {
                    const eventName = 'EventWithProperties';
                    Analytics.trackEvent(eventName, { property1: '100', property2: '200' });
                    showEventToast(eventName);
                  }
                },
                {
                  title: 'Track event with long property value',
                  action: () => {
                    const eventName = 'EventWithLongProperties';
                    Analytics.trackEvent(eventName, { propertyValueTooLong: '12345678901234567890123456789012345678901234567890123456789012345' });
                    showEventToast(eventName);
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
