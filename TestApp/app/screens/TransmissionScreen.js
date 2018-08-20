import React, { Component } from 'react';
import { Text, Image } from 'react-native';
import fuelTabBarIcon from '../assets/fuel.png';

export default class TransmissionScreen extends Component {
    static navigationOptions = {
        tabBarIcon: () => <Image style={{ width: 24, height: 24 }} source={fuelTabBarIcon} />
    }

    render() {
        return <Text style={{ padding: 30 }}>Not implemented yet.</Text>;
    }
}
