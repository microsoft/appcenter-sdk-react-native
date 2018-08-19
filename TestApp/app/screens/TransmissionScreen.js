import React, { Component } from 'react';
import { Image, View, Picker, SectionList, Text, TouchableOpacity } from 'react-native';

import Analytics from 'appcenter-analytics';

import SharedStyles from '../SharedStyles';

const targetTokens = [
  {
    name: 'Target Token 1',
    value: 'c86c1b0383d149f6969b80462b250e62-e3c516ac-ae36-4776-b3eb-9c21116a756c-7045'
  },
  {
    name: 'Target Token 2 (Child of 1)',
    value: '739fadd014d642809473cdde9d1177d1-4477e206-0087-4d70-b810-229652426c89-7219'
  },
  {
    name: 'Target Token 3 (Child of 2)',
    value: '518cb8157cb743be9f7a921a46fda15d-5c9111b6-2c0f-417e-95f9-2241235db0b6-6776'
  }
];

export default class TransmissionScreen extends Component {
  static navigationOptions = {
    tabBarIcon: () => <Image style={{ width: 24, height: 24 }} source={require('../assets/fuel.png')} />
  }

  state = {
    targetToken: targetTokens[0].value
  }

  render() {
    const pickerRenderItem = ({ item: { valueChanged, tokens } }) => (
      <Picker selectedValue={this.state.targetToken} onValueChange={valueChanged}>
        { tokens.map(token => <Picker.Item key={token.value} label={token.name} value={token.value} />) }
      </Picker>
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
              title: 'Select transmission target',
              data: [
                {
                  valueChanged: value => this.setState({ targetToken: value }),
                  tokens: targetTokens
                },
              ],
              renderItem: pickerRenderItem
            },
            {
              title: 'Actions',
              data: [
                {
                  title: 'Track Event with properties',
                  action: async () => {
                    const transmissionTarget = await Analytics.getTransmissionTarget(this.state.targetToken);
                    transmissionTarget.trackEvent('event_for_transmission_target', { page: 'Transmission screen' });
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
