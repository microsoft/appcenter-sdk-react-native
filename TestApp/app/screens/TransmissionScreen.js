import React, { Component } from 'react';
import { Image, View, SectionList, Text, TextInput, TouchableOpacity, Switch } from 'react-native';
import ModalSelector from 'react-native-modal-selector';
import Toast from 'react-native-simple-toast';

import AppCenter from 'appcenter';
import Analytics from 'appcenter-analytics';

import PropertiesConfiguratorView from '../components/PropertiesConfiguratorView';

import SharedStyles from '../SharedStyles';
import TransmissionTabBarIcon from '../assets/fuel.png';

import Constants from '../Constants';

const targetTokens = Constants.targetTokens;

export default class TransmissionScreen extends Component {
  static navigationOptions = {
    tabBarIcon: () => <Image style={{ width: 24, height: 24 }} source={TransmissionTabBarIcon} />
  }

  standardProperties = targetTokens.reduce((map, el) => {
    map[el.key] = {
      appName: '',
      appVersion: '',
      appLocale: ''
    };
    return map;
  }, {});

  customProperties = targetTokens.reduce((map, el) => {
    map[el.key] = [];
    return map;
  }, {});

  transmissionTargets = {};

  state = {
    targetToken: targetTokens[0],
    showProperties: true,
    standardProperties: this.standardProperties[targetTokens[0].key],
    customProperties: this.customProperties[targetTokens[0].key],
    deviceIdEnabled: {},
    targetEnabled: true
  }

  async componentWillMount() {
    await AppCenter.startFromLibrary(Analytics);
    await this.createTargetsFromTokens(0, Analytics);
  }

  async createTargetsFromTokens(index, parentTarget) {
    if (index >= targetTokens.length) {
      return;
    }
    const targetToken = targetTokens[index].key;
    const transmissionTarget = await parentTarget.getTransmissionTarget(targetToken);
    this.transmissionTargets[targetToken] = transmissionTarget;
    await this.createTargetsFromTokens(++index, transmissionTarget);
  }

  async setStandardProperty(key, value) {
    if (value === '') {
      value = null;
    }
    const targetToken = this.state.targetToken.key;
    const transmissionTarget = this.transmissionTargets[targetToken];
    switch (key) {
      case 'appName':
        await transmissionTarget.propertyConfigurator.setAppName(value);
        break;
      case 'appVersion':
        await transmissionTarget.propertyConfigurator.setAppVersion(value);
        break;
      case 'appLocale':
        await transmissionTarget.propertyConfigurator.setAppLocale(value);
        break;
      default:
        throw new Error(`Unexpected key=${key}`);
    }
    this.setState((state) => {
      state.standardProperties[key] = value;
      this.standardProperties[targetToken] = state.standardProperties;
      return state;
    });
  }

  async addProperty(property) {
    const target = this.transmissionTargets[this.state.targetToken.key];
    await target.propertyConfigurator.setEventProperty(property.name, property.value);
    this.setState((state) => {
      state.customProperties.push(property);
      this.customProperties[this.state.targetToken.key] = state.customProperties;
      return state;
    });
  }

  async removeProperty(propertyName) {
    const target = this.transmissionTargets[this.state.targetToken.key];
    await target.propertyConfigurator.removeEventProperty(propertyName);
    this.setState((state) => {
      state.customProperties = state.customProperties.filter(item => item.name !== propertyName);
      this.customProperties[this.state.targetToken.key] = state.customProperties;
      return state;
    });
  }

  async replaceProperty(oldPropertyName, newProperty) {
    const target = this.transmissionTargets[this.state.targetToken.key];
    await target.propertyConfigurator.removeEventProperty(oldPropertyName);
    await target.propertyConfigurator.setEventProperty(newProperty.name, newProperty.value);
    this.setState((state) => {
      const index = state.customProperties.findIndex(el => el.name === oldPropertyName);
      state.customProperties[index] = newProperty;
      this.customProperties[this.state.targetToken.key] = state.customProperties;
      return state;
    });
  }

  render() {
    const pickerRenderItem = ({ item: { title, valueChanged, tokens } }) => (
      <ModalSelector
        data={tokens}
        initValue={title}
        onChange={valueChanged}
        style={SharedStyles.modalSelector}
        selectTextStyle={SharedStyles.itemButton}
      />
    );

    // After trying to fix the next line lint warning, the code was harder to read and format, disable it once.
    // eslint-disable-next-line object-curly-newline
    const settingsRenderItem = ({ item: { title, disabled, value, onChange } }) => (
      <View style={SharedStyles.item}>
        <Text style={SharedStyles.itemTitle}>{title}</Text>
        <Switch disabled={disabled} value={value} onValueChange={onChange} />
      </View>
    );

    const actionRenderItem = ({ item: { title, action } }) => (
      <TouchableOpacity style={SharedStyles.item} onPress={action}>
        <Text style={SharedStyles.itemButton}>{title}</Text>
      </TouchableOpacity>
    );

    const standardPropertiesRenderItem = ({ item: { title, key, onChange } }) => (
      <View style={SharedStyles.item}>
        <Text style={SharedStyles.itemTitle}>{title}</Text>
        <TextInput style={SharedStyles.itemInput} onChangeText={onChange}>{this.state.standardProperties[key]}</TextInput>
      </View>
    );

    const customPropertiesRenderItem = () => (
      <PropertiesConfiguratorView
        onPropertyAdded={() => {
          const nextItem = this.state.customProperties.length + 1;
          this.addProperty({ name: `key${nextItem}`, value: `value${nextItem}` });
        }}
        onPropertyRemoved={propertyName => this.removeProperty(propertyName)}
        onPropertyChanged={(oldPropertyName, newProperty) => this.replaceProperty(oldPropertyName, newProperty)}
        properties={this.state.customProperties}
        allowChanges={this.state.showProperties}
      />
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
              title: 'Select transmission target',
              data: [
                {
                  title: this.state.targetToken.label,
                  valueChanged: async (option) => {
                    const transmissionTarget = this.transmissionTargets[option.key];
                    const targetEnabled = transmissionTarget ? await transmissionTarget.isEnabled() : false;
                    this.setState({
                      targetToken: option,
                      showProperties: !!option.key,
                      standardProperties: this.standardProperties[option.key],
                      customProperties: this.customProperties[option.key],
                      targetEnabled
                    });
                  },
                  tokens: targetTokens
                },
              ],
              renderItem: pickerRenderItem
            },
            {
              title: 'Settings',
              data: [
                {
                  title: 'Device ID Enabled',
                  disabled: !!this.state.deviceIdEnabled[this.state.targetToken.key],
                  value: !!this.state.deviceIdEnabled[this.state.targetToken.key],
                  onChange: async () => {
                    const transmissionTarget = this.transmissionTargets[this.state.targetToken.key];
                    if (transmissionTarget) {
                      this.setState({ deviceIdEnabled: { ...this.state.deviceIdEnabled, [this.state.targetToken.key]: true } });
                      transmissionTarget.propertyConfigurator.collectDeviceId();
                    }
                  },
                },
                {
                  title: 'Transmission Target Enabled',
                  value: this.state.targetEnabled,
                  onChange: async (value) => {
                    const transmissionTarget = this.transmissionTargets[this.state.targetToken.key];
                    if (transmissionTarget) {
                      await transmissionTarget.setEnabled(value);
                      const targetEnabled = await transmissionTarget.isEnabled();
                      this.setState({ targetEnabled });
                    }
                  }
                }
              ],
              renderItem: settingsRenderItem
            },
            {
              title: 'Actions',
              data: [
                {
                  title: 'Track event without properties',
                  action: () => {
                    const transmissionTarget = this.transmissionTargets[this.state.targetToken.key];
                    if (transmissionTarget) {
                      const eventName = 'EventWithoutPropertiesFromTarget';
                      transmissionTarget.trackEvent(eventName);
                      showEventToast(eventName);
                    }
                  }
                },
                {
                  title: 'Track event with properties',
                  action: () => {
                    const transmissionTarget = this.transmissionTargets[this.state.targetToken.key];
                    if (transmissionTarget) {
                      const eventName = 'EventWithPropertiesFromTarget';
                      transmissionTarget.trackEvent(eventName, { property1: '100', property2: '200' });
                      showEventToast(eventName);
                    }
                  }
                }
              ],
              renderItem: actionRenderItem
            },
            {
              title: 'Standard Properties',
              data: [
                {
                  title: 'App Name',
                  key: 'appName',
                  onChange: appName => this.setStandardProperty('appName', appName)
                },
                {
                  title: 'App Version',
                  key: 'appVersion',
                  onChange: appVersion => this.setStandardProperty('appVersion', appVersion)
                },
                {
                  title: 'App Locale',
                  key: 'appLocale',
                  onChange: appLocale => this.setStandardProperty('appLocale', appLocale)
                },
              ],
              renderItem: standardPropertiesRenderItem
            },
            {
              title: 'Properties',
              data: [{}],
              renderItem: customPropertiesRenderItem
            },
          ]}
        />
      </View>
    );
  }
}
