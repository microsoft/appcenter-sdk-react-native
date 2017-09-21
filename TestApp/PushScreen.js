/**
 * Sample React Native App
 * https://github.com/facebook/react-native
 * @flow
 */

import React from 'react';
import {
  Text,
  View,
  ScrollView,
  TouchableOpacity
} from 'react-native';

import Push from 'mobile-center-push';
import SharedStyles from './SharedStyles';

export default class PushScreen extends React.Component {
  constructor() {
    super();
    this.state = {
      pushEnabled: false
    };
    this.toggleEnabled = this.toggleEnabled.bind(this);
  }

  async componentDidMount() {
    const component = this;

    const pushEnabled = await Push.isEnabled();
    component.setState({ pushEnabled });
  }

  async toggleEnabled() {
    await Push.setEnabled(!this.state.pushEnabled);

    const pushEnabled = await Push.isEnabled();
    this.setState({ pushEnabled });
  }

  render() {
    return (
      <View style={SharedStyles.container}>
        <ScrollView >
          <Text style={SharedStyles.heading}>
            Test Push
          </Text>

          <Text style={SharedStyles.enabledText}>
            Push enabled: {this.state.pushEnabled ? 'yes' : 'no'}
          </Text>
          <TouchableOpacity onPress={this.toggleEnabled}>
            <Text style={SharedStyles.toggleEnabled}>
              toggle
            </Text>
          </TouchableOpacity>

        </ScrollView>
      </View>
    );
  }
}
