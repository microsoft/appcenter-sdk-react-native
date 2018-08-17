import React, { Component } from 'react';
import { Image, View, Text, Switch, SectionList, TouchableOpacity } from 'react-native';

import AppCenter, { CustomProperties } from 'appcenter';
import Push from 'appcenter-push';

import SharedStyles from '../SharedStyles';

export default class AppCenterScreen extends Component {
  static navigationOptions = {
    tabBarIcon: () => <Image style={{ width: 24, height: 24 }} source={require('../assets/dials.png')} />
  }

  state = {
    appCenterEnabled: false,
    pushEnabled: false,
    installId: '',
    sdkVersion: AppCenter.getSdkVersion()
  }

  async componentWillMount() {
    const appCenterEnabled = await AppCenter.isEnabled();
    this.setState({ appCenterEnabled });

    const pushEnabled = await Push.isEnabled();
    this.setState({ pushEnabled });

    const installId = await AppCenter.getInstallId();
    this.setState({ installId });
  }

  async setCustomProperties() {
    const properties = new CustomProperties()
      .set('pi', 3.14)
      .clear('old')
      .set('color', 'blue')
      .set('optin', true)
      .set('optout', false)
      .set('score', 7)
      .set('now', new Date());
    await AppCenter.setCustomProperties(properties);
  }

  render() {
    const switchRenderItem = ({ item: { title, value, toggle } }) => (
      <View style={SharedStyles.item}>
        <Text style={SharedStyles.title}>{title}</Text>
        <Switch value={this.state[value]} onValueChange={toggle} />
      </View>
    );
    const valueRenderItem = ({ item: { title, value } }) => (
      <View style={SharedStyles.item}>
        <Text style={SharedStyles.title}>{title}</Text>
        <Text>{this.state[value]}</Text>
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
          renderItem={({ item }) => <Text style={[SharedStyles.item, SharedStyles.title]}>{item}</Text>}
          renderSectionHeader={({ section: { title } }) => <Text style={SharedStyles.header}>{title}</Text>}
          keyExtractor={(item, index) => item + index}
          sections={[
            {
              title: 'Settings',
              data: [
                {
                  title: 'App Center Enabled',
                  value: 'appCenterEnabled',
                  toggle: async () => {
                    await AppCenter.setEnabled(!this.state.appCenterEnabled);
                    const appCenterEnabled = await AppCenter.isEnabled();
                    this.setState({ appCenterEnabled });
                  }
                },
                {
                  title: 'Push Enabled',
                  value: 'pushEnabled',
                  toggle: async () => {
                    await Push.setEnabled(!this.state.pushEnabled);
                    const pushEnabled = await Push.isEnabled();
                    this.setState({ pushEnabled });
                  }
                  },
              ],
              renderItem: switchRenderItem
            },

            // TODO: Implement set startup mode
            {
              title: 'Startup Mode',
              data: [
                'App Target Only',
                'Library Target Only',
                'Both Targets',
              ]
            },
            {
              title: 'Actions',
              data: [
                {
                  title: 'Set Custom Properties',
                  action: this.setCustomProperties
                },
              ],
              renderItem: actionRenderItem
            },
            {
              title: 'Miscellaneous',
              data: [
                { title: 'Install ID', value: 'installId' },
                { title: 'SDK Version', value: 'sdkVersion' },
              ],
              renderItem: valueRenderItem
            },
          ]}
        />
      </View>
    );
  }
}
