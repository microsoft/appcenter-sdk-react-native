import React, { Component } from 'react';
import { Image, View, SectionList, Text, TouchableOpacity } from 'react-native';
import ModalSelector from 'react-native-modal-selector';
import Toast from 'react-native-simple-toast';

import AppCenter from 'appcenter';
import Analytics from 'appcenter-analytics';

import PropertiesConfiguratorView from '../components/PropertiesConfiguratorView'

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

  state = {
    targetToken: targetTokens[0],
    showProperties: true,
    properties: this.targetTokensProperties[targetTokens[0].key]
  }

  async componentWillMount() {
    await AppCenter.startFromLibrary(Analytics);
  }

  async addProperty(property) {
    const target = await Analytics.getTransmissionTarget(this.state.targetToken.key);
    //TODO: Add property to target
    this.setState(state => {
      state.properties.push(property)
      return state;
    });
  }

  async removeProperty(propertyName) {
    const target = await Analytics.getTransmissionTarget(this.state.targetToken.key);
    //TODO: Remove property from target
    this.setState(state => {
      state.properties = state.properties.filter((item) => item.name !== propertyName);
      return state;
    });
  }

  async replaceProperty(oldPropertyName, newProperty) {
    const target = await Analytics.getTransmissionTarget(this.state.targetToken.key);
    //TODO: Replace property from target
    this.setState(state => {
      let index = state.properties.findIndex((el, index, array) => el.name === oldPropertyName);
      state.properties[index] = newProperty;
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
    const propertiesRenderItem = () => (
      <PropertiesConfiguratorView
      onPropertyAdded={() => {
        let nextItem = this.state.properties.length + 1;
        this.addProperty({name: "key" + nextItem, value: "value" + nextItem})}
      }
      onPropertyRemoved={(propertyName) => this.removeProperty(propertyName)}
      onPropertyChanged={(oldPropertyName, newProperty) => this.replaceProperty(oldPropertyName, newProperty)}
      properties={this.state.properties}
      allowChanges={this.state.showProperties}/>
    );
    const showEventToast = eventName => Toast.show(`Scheduled event '${eventName}'.`);

    return (
      <View style={SharedStyles.container}>
        <SectionList
          renderItem={({ item }) => <Text style={[SharedStyles.item, SharedStyles.title]}>{item}</Text>}
          renderSectionHeader={({ section: { title } }) => <Text style={SharedStyles.header}>{title}</Text>}
          keyExtractor={(item, index) => item + index}
          sections={[
            {
              title: 'Select transmission target',
              data: [
                {
                  title: this.state.targetToken.label,
                  valueChanged: option => {
                    this.setState({ 
                      targetToken: option,
                      showProperties: !!option.key,
                      properties: this.targetTokensProperties[option.key]})
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
                  action: async () => {
                    const transmissionTarget = await Analytics.getTransmissionTarget(this.state.targetToken.key);
                    if (transmissionTarget) {
                      const eventName = 'EventWithoutPropertiesFromTarget';
                      transmissionTarget.trackEvent(eventName);
                      showEventToast(eventName);
                    }
                  }
                },
                {
                  title: 'Track event with properties',
                  action: async () => {
                    const transmissionTarget = await Analytics.getTransmissionTarget(this.state.targetToken.key);
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
              title: 'Properties',
              data: [{}],
              renderItem: propertiesRenderItem
            }
          ]}
        />
      </View>
    );
  }
}
