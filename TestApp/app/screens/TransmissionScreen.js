import React, { Component } from 'react';
import { Image, View, SectionList, Text, TextInput, TouchableOpacity } from 'react-native';
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

  targetTokensProperties = targetTokens.reduce((map, el) => {
    map[el.key] = [];
    return map;
  }, {});

  transmissionTargets = {};

  state = {
    targetToken: targetTokens[0],
    showProperties: true,
    properties: this.targetTokensProperties[targetTokens[0].key]
  }

  async componentWillMount() {
    await AppCenter.startFromLibrary(Analytics);
    this.createTargetsFromTokens(targetTokens, 0, null);
  }

  async createTargetsFromTokens(tokens, index, parentTarget) {
    if (index > tokens.length) {
      return;
    }
    let transmissionTarget = parentTarget;
    const token = tokens[index];
    if (token.key) {
      if (transmissionTarget) {
        transmissionTarget = await transmissionTarget.getTransmissionTarget(token.key);
        this.transmissionTargets[token.key] = transmissionTarget;
      } else {
        transmissionTarget = await Analytics.getTransmissionTarget(token.key);
        this.transmissionTargets[token.key] = transmissionTarget;
      }
      await this.createTargetsFromTokens(tokens, ++index, transmissionTarget);
    }
  }

  async addProperty(property) {
    const target = this.transmissionTargets[this.state.targetToken.key];
    await target.propertyConfigurator.setEventProperty(property.name, property.value);
    this.setState((state) => {
      state.properties.push(property);
      this.targetTokensProperties[this.state.targetToken.key] = state.properties;
      return state;
    });
  }

  async removeProperty(propertyName) {
    const target = this.transmissionTargets[this.state.targetToken.key];
    await target.propertyConfigurator.removeEventProperty(propertyName);
    this.setState((state) => {
      state.properties = state.properties.filter(item => item.name !== propertyName);
      this.targetTokensProperties[this.state.targetToken.key] = state.properties;
      return state;
    });
  }

  async replaceProperty(oldPropertyName, newProperty) {
    const target = this.transmissionTargets[this.state.targetToken.key];
    await target.propertyConfigurator.removeEventProperty(oldPropertyName);
    await target.propertyConfigurator.setEventProperty(newProperty.name, newProperty.value);
    this.setState((state) => {
      const index = state.properties.findIndex(el => el.name === oldPropertyName);
      state.properties[index] = newProperty;
      this.targetTokensProperties[this.state.targetToken.key] = state.properties;
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
    const actionRenderItem = ({ item: { title, action } }) => (
      <TouchableOpacity style={SharedStyles.item} onPress={action}>
        <Text style={SharedStyles.itemButton}>{title}</Text>
      </TouchableOpacity>
    );
    const standardPropertiesRenderItem = ({ item: { title, value, onChange } }) => (
      <View style={SharedStyles.item}>
        <Text style={SharedStyles.itemTitle}>{title}</Text>
        <TextInput style={SharedStyles.itemInput} onChangeText={onChange}>{value}</TextInput>
      </View>
    );
    const propertiesRenderItem = () => (
      <PropertiesConfiguratorView
        onPropertyAdded={() => {
          const nextItem = this.state.properties.length + 1;
          this.addProperty({ name: `key${nextItem}`, value: `value${nextItem}` });
        }}
        onPropertyRemoved={propertyName => this.removeProperty(propertyName)}
        onPropertyChanged={(oldPropertyName, newProperty) => this.replaceProperty(oldPropertyName, newProperty)}
        properties={this.state.properties}
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
                  valueChanged: (option) => {
                    this.setState({
                      targetToken: option,
                      showProperties: !!option.key,
                      properties: this.targetTokensProperties[option.key]
                    });
                  },
                  tokens: targetTokens
                },
              ],
              renderItem: pickerRenderItem
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
                  value: '',
                  onChange: (appName) => {
                    const transmissionTarget = this.transmissionTargets[this.state.targetToken.key];
                    if (transmissionTarget) {
                      transmissionTarget.propertyConfigurator.setAppName(appName);
                    }
                  }
                },
                {
                  title: 'App Version',
                  value: '',
                  onChange: (appVersion) => {
                    const transmissionTarget = this.transmissionTargets[this.state.targetToken.key];
                    if (transmissionTarget) {
                      transmissionTarget.propertyConfigurator.setAppVersion(appVersion);
                    }
                  }
                },
                {
                  title: 'App Locale',
                  value: '',
                  onChange: (appLocale) => {
                    const transmissionTarget = this.transmissionTargets[this.state.targetToken.key];
                    if (transmissionTarget) {
                      transmissionTarget.propertyConfigurator.setAppLocale(appLocale);
                    }
                  }
                },
              ],
              renderItem: standardPropertiesRenderItem
            },
            {
              title: 'Properties',
              data: [{}],
              renderItem: propertiesRenderItem
            },
          ]}
        />
      </View>
    );
  }
}
