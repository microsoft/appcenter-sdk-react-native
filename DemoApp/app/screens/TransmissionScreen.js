import React, { Component } from 'react';
import { Image, View, SectionList, Text, TouchableOpacity } from 'react-native';
import ModalSelector from 'react-native-modal-selector';
import Toast from 'react-native-simple-toast';

import AppCenter from 'appcenter';
import Analytics from 'appcenter-analytics';

import SharedStyles from '../SharedStyles';
import TransmissionTabBarIcon from '../assets/fuel.png';

import Constants from '../Constants';

const targetTokens = Constants.targetTokens;

export default class TransmissionScreen extends Component {
  static navigationOptions = {
    tabBarIcon: () => <Image style={{ width: 24, height: 24 }} source={TransmissionTabBarIcon} />
  }

  state = {
    targetToken: targetTokens[0]
  }

  async componentWillMount() {
    await AppCenter.startFromLibrary(Analytics);
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
                  valueChanged: option => this.setState({ targetToken: option }),
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
          ]}
        />
      </View>
    );
  }
}
