// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

import React, { Component } from 'react';
import { Image, View, Text, TextInput, Switch, SectionList, NativeModules, Platform } from 'react-native';
import AsyncStorage from '@react-native-community/async-storage';
import ModalSelector from 'react-native-modal-selector';
import Toast from 'react-native-simple-toast';
import AppCenter from 'appcenter';
import SharedStyles from '../SharedStyles';
import DialsTabBarIcon from '../assets/dials.png';

const USER_ID_KEY = 'USER_ID_KEY';

const SecretStrings = {
  ios: {
    appSecret: '{RN_IOS_PROD}',
    target: 'target={RN_IOS_TARGET_TOKEN_PROD}'
  },
  android: {
    appSecret: '{RN_ANDROID_PROD}',
    target: 'target={RN_ANDROID_TARGET_TOKEN_PROD}'
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
    networkRequestsAllowed: true,
    installId: '',
    sdkVersion: AppCenter.getSdkVersion(),
    startupMode: StartupModes[0],
    userId: ''
  }

  async componentDidMount() {
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

    await AppCenter.setLogLevel(AppCenter.LogLevel.VERBOSE);
  }

  async refreshUI() {
    const networkRequestsAllowed = await AppCenter.isNetworkRequestsAllowed();
    this.setState({ networkRequestsAllowed });

    const appCenterEnabled = await AppCenter.isEnabled();
    this.setState({ appCenterEnabled });

    const installId = await AppCenter.getInstallId();
    this.setState({ installId });
  }

  async configureStartup(secretString, startAutomatically) {
    await NativeModules.DemoAppNative.configureStartup(secretString, startAutomatically);
    Toast.show('Relaunch app for changes to be applied.');
  }

  async selectStartup() {
    switch (this.state.startupMode.key) {
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
        throw new Error(`Unexpected startup type=${this.state.startupMode.key}`);
    }
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

    const startupModeRenderItem = ({ item: { startupModes } }) => (
      <ModalSelector
        data={startupModes}
        initValue={this.state.startupMode.label}
        style={SharedStyles.modalSelector}
        selectTextStyle={SharedStyles.itemButton}
        onChange={async ({ key }) => {
            await AsyncStorage.setItem(STARTUP_MODE, key);
            this.setState({ startupMode: startupModes.filter(m => m.key === key)[0] }, this.selectStartup);
          }
        }
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
                    this.setState({ appCenterEnabled });
                  }
                },
              ],
              renderItem: switchRenderItem
            },
            {
              title: 'Network requests allowed',
              data: [
                {
                  title: 'Network requests allowed',
                  value: 'networkRequestsAllowed',
                  toggle: async () => {
                    await AppCenter.setNetworkRequestsAllowed(!this.state.networkRequestsAllowed);
                    const networkRequestsAllowed = await AppCenter.isNetworkRequestsAllowed();
                    this.setState({ networkRequestsAllowed });
                  },
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
              renderItem: startupModeRenderItem
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
