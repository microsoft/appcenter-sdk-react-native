import React, { Component } from 'react';
import { Image, View, Text, Switch, SectionList, TouchableOpacity } from 'react-native';
import Toast from 'react-native-simple-toast';

import Analytics from 'appcenter-analytics';

import SharedStyles from '../SharedStyles';

export default class AnalyticsScreen extends Component {
  static navigationOptions = {
    tabBarIcon: () => <Image style={{ width: 24, height: 24 }} source={require('../assets/analytics.png')} />
  }

  state = {
    analyticsEnabled: false
  }

  async componentWillMount() {
    const analyticsEnabled = await Analytics.isEnabled();
    this.setState({ analyticsEnabled });
  }

  render() {
    const switchRenderItem = ({ item: { title, value, toggle } }) => (
      <View style={SharedStyles.item}>
        <Text style={SharedStyles.title}>{title}</Text>
        <Switch value={this.state[value]} onValueChange={toggle} />
      </View>
    );

    const actionRenderItem = ({ item: { title, action } }) => (
      <TouchableOpacity style={SharedStyles.item} onPress={action}>
        <Text style={SharedStyles.itemButton}>{title}</Text>
      </TouchableOpacity>
    );

    const showEventToast = () => Toast.show('Scheduled event log. Please check verbose logs.');

    return (
      <View style={SharedStyles.container}>
        <SectionList
          renderItem={({ item }) => <Text style={[SharedStyles.item, SharedStyles.title]}>{item}</Text>}
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
                  title: 'Track Event',
                  action: () => {
                    Analytics.trackEvent('Event without properties');
                    showEventToast();
                  }
                },
                {
                  title: 'Track Event with properties',
                  action: () => {
                    Analytics.trackEvent('Button press', { page: 'Home page' });
                    showEventToast();
                  }
                },
                {
                  title: 'Track Event with long property value',
                  action: () => {
                    Analytics.trackEvent('Button press', { propertyValueTooLong: '12345678901234567890123456789012345678901234567890123456789012345' });
                    showEventToast();
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
