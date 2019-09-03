// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

import React, { Component } from 'react';
import { View, Text, Image, FlatList, TouchableOpacity, StyleSheet, Modal, SectionList } from 'react-native';
import Data from 'appcenter-data';
import RemoveIcon from '../assets/remove.png';
import SharedStyles from '../SharedStyles';

export class DataDocumentListView extends Component {
    state = {
        modalVisible: false,
        currentDocument: null
      };

    setModalVisible(visible, currentDocument) {
        this.setState({ modalVisible: visible, currentDocument });
    }

    render() {
        const renderDocumentItem = ({ item: { title, value } }) => (
          <View style={SharedStyles.item}>
            <Text style={SharedStyles.itemTitle}>{title}</Text>
            <Text style={SharedStyles.itemInput}>{value}</Text>
          </View>);

        return (
          <View style={styles.container}>
            <FlatList
              style={{ marginTop: 10 }}
              data={this.props.items}
              renderItem={({ item }) => (
                <View style={styles.documentsContainer}>
                  <TouchableOpacity
                    style={styles.documentText}
                    onPress={() => {
                            this.setModalVisible(true, item);
                          }}
                  >
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
            <Modal
              animationType="slide"
              transparent={false}
              visible={this.state.modalVisible}
              onRequestClose={() => {}}
            >
              <View style={SharedStyles.container}>
                <SectionList
                  renderItem={({ item }) => <Text style={[SharedStyles.item, SharedStyles.itemTitle]}>{item}</Text>}
                  keyExtractor={(item, index) => item + index}
                  sections={[
                            {
                                data: [{
                                    title: 'ID',
                                    value: this.state.currentDocument == null ? '' : this.state.currentDocument.id,
                                    }],
                                renderItem: renderDocumentItem
                            },
                            {
                                data: [
                                    {
                                    title: 'Partition',
                                    value: this.state.currentDocument == null ? '' : this.state.currentDocument.partition,
                                    },
                            ],
                            renderItem: renderDocumentItem
                            },
                            {
                                data: [
                                    {
                                    title: 'Date',
                                    value: this.state.currentDocument == null ? '' : this.state.currentDocument.lastUpdatedDate,
                                    },
                            ],
                            renderItem: renderDocumentItem
                            },
                            {
                                data: [
                                    {
                                    title: 'State',
                                    value: this.formatStateLabel()
                                    },
                            ],
                            renderItem: renderDocumentItem
                            },
                            {
                                data: [
                                    {
                                    title: 'Content',
                                    value: this.state.currentDocument == null ? '' : (this.state.currentDocument.jsonValue)
                                    },
                            ],
                            renderItem: renderDocumentItem
                            },
                            {
                                data: [{}],
                                renderItem: () => (
                                  <TouchableOpacity
                                    style={SharedStyles.item}
                                    onPress={() => {
                                        this.setModalVisible(!this.state.modalVisible);
                                        }}
                                  >
                                    <Text style={SharedStyles.itemButton}>Close</Text>
                                  </TouchableOpacity>)
                            },
                        ]}
                />
              </View>
            </Modal>
          </View>
        );
    }

    formatStateLabel() {
        if (this.state.currentDocument != null) {
            return this.state.currentDocument.isFromDeviceCache ? 'Local' : 'Remote';
        }
        return '';
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
