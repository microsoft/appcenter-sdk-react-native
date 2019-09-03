// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

import React, { Component } from 'react';
import { View, Text, Image, FlatList, TouchableOpacity, StyleSheet } from 'react-native';
import Data from 'appcenter-data';
import RemoveIcon from '../assets/remove.png';

export class DataDocumentListView extends Component {
    render() {
        return (
          <View style={styles.container}>
            <FlatList
              style={{ marginTop: 10 }}
              data={this.props.items}
              renderItem={({ item }) => (
                <View style={styles.documentsContainer}>
                  <TouchableOpacity style={styles.documentText}>
                    <Text>{item.id}</Text>
                  </TouchableOpacity>
                  <Divider />
                  {
                                item.partition === Data.DefaultPartitions.APP_DOCUMENTS ?
                                    null :
                                    (
                                      <TouchableOpacity onPress={() => this.props.onDocumentRemoved(item.id)}>
                                        <Image style={styles.removeIcon} source={RemoveIcon} />
                                      </TouchableOpacity>
                                    )

                            }
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
    propertiesList: {
        marginTop: 10
    },
    documentsContainer: {
        flex: 1,
        flexDirection: 'row',
        alignItems: 'center'
    },
    documentText: {
        flex: 1,
        padding: 5,
        height: 35,
        fontSize: 17,
        borderColor: 'black',
        borderWidth: 0.3
    },
    removeIcon: {
        height: 26,
        width: 26
    }
});
