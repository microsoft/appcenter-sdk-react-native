// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

import React, { Component } from 'react';
import { Image, View, Text, TextInput, Switch, SectionList, TouchableOpacity, NativeModules, Platform } from 'react-native';
import AsyncStorage from '@react-native-community/async-storage';
import ModalSelector from 'react-native-modal-selector';
import Toast from 'react-native-simple-toast';

import AppCenter, { CustomProperties } from 'appcenter';
import Auth from 'appcenter-auth';
import Push from 'appcenter-push';

import SharedStyles from '../SharedStyles';
import DialsTabBarIcon from '../assets/dials.png';

const USER_ID_KEY = 'USER_ID_KEY';

const SecretStrings = {
  ios: {
    appSecret: 'f5f84a76-6622-437a-9130-07b27d3c72e7',
    target: 'target=c10075a08d114205b3d67118c0028cf5-70b2d0e7-e693-4fe0-be1f-a1e9801dcf12-6906'
  },
  android: {
    appSecret: 'e65c7490-1f58-4e93-bb55-a2e11dac4368',
    target: 'target=4dacd24d0b1b42db9894926d0db2f4c7-39311d37-fb55-479c-b7b6-9893b53d0186-7306'
  }
};
SecretStrings.ios.both = `appsecret=${SecretStrings.ios.appSecret};${SecretStrings.ios.target}`;
SecretStrings.android.both = `appsecret=${SecretStrings.android.appSecret};${SecretStrings.android.target}`;

const STARTUP_MODE = 'STARTUP_MODE';

const StartupModes = [
  {
    label: 'AppCenter target only',
    key: 'APPCENTER'
  },
  {
    label: 'OneCollector target only',
    key: 'TARGET'
  },
  {
    label: 'Both targets',
    key: 'BOTH'
  },
  {
    label: 'No default target',
    key: 'NONE'
  },
  {
    label: 'Skip start (library only)',
    key: 'SKIP'
  }
];

export default class AppCenterScreen extends Component {
  static navigationOptions = {
    tabBarIcon: () => <Image style={{ width: 24, height: 24 }} source={DialsTabBarIcon} />,
    tabBarOnPress: ({ defaultHandler, navigation }) => {
      const refreshAppCenterScreen = navigation.getParam('refreshAppCenterScreen');

      // Initial press: the function is not defined yet so nothing to refresh.
      if (refreshAppCenterScreen) {
        refreshAppCenterScreen();
      }
      defaultHandler();
    }
  }

  state = {
    appCenterEnabled: false,
    pushEnabled: false,
    authEnabled: false,
    installId: '',
    sdkVersion: AppCenter.getSdkVersion(),
    startupMode: StartupModes[0],
    userId: '',
    accountId: '',
    authStatus: 'Authentication status unknown'
  }

  async componentWillMount() {
    await this.refreshUI();
    const startupModeKey = await AsyncStorage.getItem(STARTUP_MODE);
    for (let index = 0; index < StartupModes.length; index++) {
      const startupMode = StartupModes[index];
      if (startupMode.key === startupModeKey) {
        this.state.startupMode = startupMode;
        break;
      }
    }
    const userId = await AsyncStorage.getItem(USER_ID_KEY);
    if (userId !== null) {
      this.state.userId = userId;
      await AppCenter.setUserId(userId);
    }
    this.props.navigation.setParams({
      refreshAppCenterScreen: this.refreshUI.bind(this)
    });
  }

  async refreshUI() {
    const appCenterEnabled = await AppCenter.isEnabled();
    this.setState({ appCenterEnabled });

    const authEnabled = await Auth.isEnabled();
    this.setState({ authEnabled });

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

  async configureStartup(secretString, startAutomatically) {
    await NativeModules.DemoAppNative.configureStartup(secretString, startAutomatically);
    Toast.show('Relaunch app for changes to be applied.');
  }

  async selectStartup(key) {
    switch (key) {
      case 'APPCENTER':
        await this.configureStartup(SecretStrings[Platform.OS].appSecret, true);
        break;
      case 'TARGET':
        await this.configureStartup(SecretStrings[Platform.OS].target, true);
        break;
      case 'BOTH':
        await this.configureStartup(SecretStrings[Platform.OS].both, true);
        break;
      case 'NONE':
        await this.configureStartup(null, true);
        break;
      case 'SKIP':
        await this.configureStartup(null, false);
        break;
      default:
        throw new Error(`Unexpected startup type=${key}`);
    }
    await AsyncStorage.setItem(STARTUP_MODE, key);
  }

  render() {
    const switchRenderItem = ({ item: { title, value, toggle } }) => (
      <View style={SharedStyles.item}>
        <Text style={SharedStyles.itemTitle}>{title}</Text>
        <Switch value={this.state[value]} onValueChange={toggle} />
      </View>
    );

    // After trying to fix the next line lint warning, the code was harder to read and format, disable it once.
    // eslint-disable-next-line object-curly-newline
    const valueRenderItem = ({ item: { title, value, onChange, onSubmit } }) => (
      <View style={SharedStyles.item}>
        <Text style={SharedStyles.itemTitle}>{title}</Text>
        {onChange ? <TextInput style={SharedStyles.itemInput} onSubmitEditing={onSubmit} onChangeText={onChange}>{String(this.state[value])}</TextInput> : <Text>{String(this.state[value])}</Text>}
      </View>
    );

    const actionRenderItem = ({ item: { title, action } }) => (
      <TouchableOpacity style={SharedStyles.item} onPress={action}>
        <Text style={SharedStyles.itemButton}>{title}</Text>
      </TouchableOpacity>
    );

    const pickerRenderItem = ({ item: { startupModes } }) => (
      <ModalSelector
        data={startupModes}
        initValue={this.state.startupMode.label}
        style={SharedStyles.modalSelector}
        selectTextStyle={SharedStyles.itemButton}
        onChange={({ key }) => this.selectStartup(key)}
      />
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
                  title: 'App Center Enabled',
                  value: 'appCenterEnabled',
                  toggle: async () => {
                    await AppCenter.setEnabled(!this.state.appCenterEnabled);
                    const appCenterEnabled = await AppCenter.isEnabled();
                    const authEnabled = await Auth.isEnabled();
                    const pushEnabled = await Push.isEnabled();
                    this.setState({ appCenterEnabled, authEnabled, pushEnabled });
                  }
                },
                {
                  title: 'Auth Enabled',
                  value: 'authEnabled',
                  toggle: async () => {
                    await Auth.setEnabled(!this.state.authEnabled);
                    const authEnabled = await Auth.isEnabled();
                    this.setState({ authEnabled, accountId: '', authStatus: 'User is not authenticated' });
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
                  startupModes: StartupModes
                }
              ],
              renderItem: pickerRenderItem
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
              title: 'Auth',
              data: [
                {
                  title: 'Sign In',
                  action: async () => {
                    try {
                      const result = await Auth.signIn();
                      this.setState({ accountId: result.accountId, authStatus: 'User is authenticated' });
                    } catch (e) {
                      console.log(e);
                    }
                  }
                },
                {
                  title: 'Sign Out',
                  action: () => {
                    Auth.signOut();
                    this.setState({ accountId: '', authStatus: 'User is not authenticated' });
                  }
                },
              ],
              renderItem: actionRenderItem
            },
            {
              title: 'Miscellaneous',
              data: [
                { title: 'Install ID', value: 'installId' },
                { title: 'SDK Version', value: 'sdkVersion' },
                {
                  title: 'User ID',
                  value: 'userId',
                  onChange: async (userId) => {
                    this.setState({ userId });
                  },
                  onSubmit: async () => {
                    // We use empty text in UI to delete userID (null for AppCenter API).
                    const userId = this.state.userId.length === 0 ? null : this.state.userId;
                    if (userId !== null) {
                      await AsyncStorage.setItem(USER_ID_KEY, userId);
                    } else {
                      await AsyncStorage.removeItem(USER_ID_KEY);
                    }
                    await AppCenter.setUserId(userId);
                  }
                },
                {
                  title: 'Account ID',
                  value: 'accountId',
                  onChange: async (accountId) => {
                    this.setState({ accountId });
                  }
                },
                {
                  title: 'Auth Status',
                  value: 'authStatus',
                  onChange: async (authStatus) => {
                    this.setState({ authStatus });
                  }
                }
              ],
              renderItem: valueRenderItem
            }
          ]}
        />
      </View>
    );
  }
}
