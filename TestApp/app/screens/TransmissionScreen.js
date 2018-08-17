import React, { Component } from 'react';
import { View, Image } from 'react-native';

export default class TransmissionScreen extends Component {
    static navigationOptions = {
      tabBarIcon: () => <Image style={{ width: 24, height: 24 }} source={require('../assets/fuel.png')} />
    }

    render() {
        return <View />;
    }
}
