import React, { Component } from 'react';
import { Image, View, Text, Switch, SectionList, TouchableOpacity, NativeModules, Platform } from 'react-native';
import Toast from 'react-native-simple-toast';

import AppCenter, { CustomProperties } from 'appcenter';
import Push from 'appcenter-push';

import SharedStyles from '../SharedStyles';
import DialsTabBarIcon from '../assets/dials.png';

const SecretStringHelper = NativeModules.TestAppSecretStringHelper;

const SecretStrings = {
  ios: {
    appSecret: 'e59c0968-b7e3-474d-85ad-6dcfaffb8bf5',
    target: 'target=c10075a08d114205b3d67118c0028cf5-70b2d0e7-e693-4fe0-be1f-a1e9801dcf12-6906'
  },
  android: {
    appSecret: '32fcfc69-d576-41dc-8d49-4be159e3d7b2',
    target: 'target=4dacd24d0b1b42db9894926d0db2f4c7-39311d37-fb55-479c-b7b6-9893b53d0186-7306'
  }
};

SecretStrings.ios.both = `appsecret=${SecretStrings.ios.appSecret};${SecretStrings.ios.target}`;
SecretStrings.android.both = `appsecret=${SecretStrings.android.appSecret};${SecretStrings.android.target}`;

export default class AppCenterScreen extends Component {
  static navigationOptions = {
    tabBarIcon: () => <Image style={{ width: 24, height: 24 }} source={DialsTabBarIcon} />
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
    Toast.show('Scheduled custom properties log. Please check verbose logs.');
  }

  async configureStartup(secretString) {
    await SecretStringHelper.configureStartup(secretString);
    Toast.show('Startup mode updated. Please kill the application and relaunch again.');
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
                    const pushEnabled = await Push.isEnabled();
                    this.setState({ appCenterEnabled, pushEnabled });
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
            {
              title: 'Change Startup Mode',
              data: [
                {
                  title: 'AppCenter target only',
                  action: () => this.configureStartup(SecretStrings[Platform.OS].appSecret)
                },
                {
                  title: 'OneCollector target only',
                  action: () => this.configureStartup(SecretStrings[Platform.OS].target)
                },
                {
                  title: 'Both targets',
                  action: () => this.configureStartup(SecretStrings[Platform.OS].both)
                },
                {
                  title: 'No default target',
                  action: () => this.configureStartup(null)
                },
              ],
              renderItem: actionRenderItem
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
