import React, { Component } from 'react';
import { Text, Image } from 'react-native';

export default class TransmissionScreen extends Component {
    static navigationOptions = {

        // eslint-disable-next-line global-require
        tabBarIcon: () => <Image style={{ width: 24, height: 24 }} source={require('../assets/fuel.png')} />
    }

    render() {
        return <Text style={{ padding: 30 }}>Not implemented yet.</Text>;
    }
}
