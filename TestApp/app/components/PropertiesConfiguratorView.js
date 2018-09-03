import React, { Component } from 'react';
import { View, Text, TextInput, Image, FlatList, TouchableOpacity, StyleSheet } from 'react-native';

import AddIcon from '../assets/add.png';
import RemoveIcon from '../assets/remove.png';

export default class PropertiesConfiguratorView extends Component {
  render() {
    return (
      <View style={styles.container}>
        <TouchableOpacity onPress={() => { if (this.props.allowChanges) this.props.onPropertyAdded(); }}>
          <View style={styles.addPropertyContainer}>
            <Image style={styles.addIcon} source={AddIcon} />
            <Divider />
            <Text>Add property</Text>
          </View>
        </TouchableOpacity>
        <FlatList
          style={{ marginTop: 10 }}
          data={this.props.properties}
          renderItem={({ item }) => (
            <View style={styles.propertyContainer}>
              <TouchableOpacity onPress={() => this.props.onPropertyRemoved(item.name)}>
                <Image style={styles.removeIcon} source={RemoveIcon} />
              </TouchableOpacity>
              <Divider />
              <TextInput style={styles.propertyInput} onChangeText={text => this.props.onPropertyChanged(item.name, { name: text, value: item.value })}>{item.name}</TextInput>
              <Divider />
              <TextInput style={styles.propertyInput} onChangeText={text => this.props.onPropertyChanged(item.name, { name: item.name, value: text })}>{item.value}</TextInput>
            </View>)
          }
          keyExtractor={(item, index) => index.toString()}
        />
      </View>
    );
  }
}

const Divider = () => <View style={{ width: 4 }} />;

const styles = StyleSheet.create({
  container: {
    backgroundColor: 'white',
    padding: 10,
    flexDirection: 'column'
  },
  addPropertyContainer: {
    flexDirection: 'row',
    alignItems: 'center'
  },
  propertiesList: {
    marginTop: 10
  },
  propertyContainer: {
    flex: 1,
    flexDirection: 'row',
    alignItems: 'center'
  },
  propertyInput: {
    flex: 0.5,
    padding: 5,
    height: 35,
    fontSize: 17,
    borderColor: 'grey',
    borderWidth: 0.3
  },
  addIcon: {
    height: 24,
    width: 24
  },
  removeIcon: {
    height: 26,
    width: 26
  }
});
