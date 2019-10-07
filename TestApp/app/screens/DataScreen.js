// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

import React, { Component } from 'react';
import { Alert, Image, View, Text, TextInput, Switch, SectionList, Modal, TouchableOpacity, Picker, ActivityIndicator } from 'react-native';
import ModalSelector from 'react-native-modal-selector';
import Data from 'appcenter-data';

import { DataDocumentListView } from './DataDocumentListView';

import SharedStyles from '../SharedStyles';
import DataTabBarIcon from '../assets/data.png';

export default class DataScreen extends Component {
  static navigationOptions = {
    tabBarIcon: () => <Image style={{ width: 24, height: 24 }} source={DataTabBarIcon} />,
    tabBarOnPress: ({ defaultHandler, navigation }) => {
      // Allow consequent presses to refresh the screen.
      const refreshData = navigation.getParam('refreshData');
      if (refreshData) {
        refreshData();
      }
      defaultHandler();
    }
  }

  // Screen's state.
  state = {
    dataEnabled: false,
    createDocModalVisible: false,
    canCreateDocument: false,
    docTtl: 60,
    docId: '',
    docType: '',
    docKey: '',
    docValue: '',
    partition: Data.DefaultPartitions.APP_DOCUMENTS,
    documents: [],
    loadingData: true
  }

  async componentDidMount() {
    // Sync the module toggle.
    await this.refreshToggle();

    // Add a way to refresh the screen when the tab is pressed.
    this.props.navigation.setParams({
      refreshData: this.refreshToggle.bind(this)
    });
    const documents = await this.listDocuments(this.state.partition);
    this.setState({ documents, loadingData: false });
  }

  async refreshToggle() {
    const dataEnabled = await Data.isEnabled();
    this.setState({ dataEnabled });
  }

  setCreateDocModalVisible(visible) {
    this.setState({ createDocModalVisible: visible });
  }

  async listDocuments(partition) {
    const documents = [];
    try {
      let page = await Data.list(partition);
      page.currentPage.items.forEach((item) => {
        documents.push(item);
      });

      /* eslint-disable no-await-in-loop */
      while (await page.hasNextPage()) {
        page = await page.getNextPage();
        page.currentPage.items.forEach((item) => {
          documents.push(item);
        });
      }
      /* eslint-enable no-await-in-loop */

      if (partition === Data.DefaultPartitions.USER_DOCUMENTS) {
        this.setState({ canCreateDocument: true });
      } else {
        this.setState({ canCreateDocument: false });
      }
      return documents;
    } catch (err) {
      if (partition === Data.DefaultPartitions.USER_DOCUMENTS) {
        this.setState({ canCreateDocument: false });
      }
      Alert.alert('Unable to list user partition.', err.message);
      console.log(err);
    }
    return documents;
  }

  render() {
    const switchRenderItem = ({ item: { title, value, toggle } }) => (
      <View style={SharedStyles.item}>
        <Text style={SharedStyles.itemTitle}>{title}</Text>
        <Switch value={this.state[value]} onValueChange={toggle} />
      </View>
    );

    const DocTtlRenderItem = ({ item: { title } }) => (
      <View style={{ flexDirection: 'row', alignItems: 'center' }}>
        <View style={{ flex: 0.25 }}>
          <Text style={SharedStyles.itemTitle}>{title}</Text>
        </View>
        <View style={{ flex: 0.75 }}>
          <Picker
            selectedValue={this.state.docTtl}
            style={{ flex: 1 }}
            onValueChange={itemValue =>
              this.setState({ docTtl: itemValue })
            }
          >
            <Picker.Item label="Infinite(Default)" value={Data.TimeToLive.DEFAULT} />
            <Picker.Item label="No Cache" value={Data.TimeToLive.NO_CACHE} />
            <Picker.Item label="2 seconds" value={2} />
          </Picker>
        </View>
      </View>
    );

    const DocTypeRenderItem = ({ item: { title } }) => (
      <View style={{ flexDirection: 'row', alignItems: 'center' }}>
        <View style={{ flex: 0.25 }}>
          <Text style={SharedStyles.itemTitle}>{title}</Text>
        </View>
        <View style={{ flex: 0.75 }}>
          <Picker
            selectedValue={this.state.docType}
            style={{ flex: 1 }}
            onValueChange={itemValue =>
              this.setState({ docType: itemValue })
            }
          >
            <Picker.Item label="String" value="string" />
            <Picker.Item label="Boolean" value="bool" />
            <Picker.Item label="Long" value="long" />
            <Picker.Item label="Double" value="double" />
            <Picker.Item label="Datetime" value="datetime" />
          </Picker>
        </View>
      </View>
    );

    const DocIdRenderItem = ({ item: { title } }) => (
      <View style={{ flexDirection: 'row', alignItems: 'center' }}>
        <View style={{ flex: 0.25 }}>
          <Text style={SharedStyles.itemTitle}>{title}</Text>
        </View>
        <View style={{ flex: 0.75 }}>
          <TextInput
            onChangeText={docId => this.setState({ docId })}
            value={this.state.docId}
          />
        </View>
      </View>
    );

    const DocValueRenderItem = ({ item: { title } }) => (
      <View style={{ flexDirection: 'row', alignItems: 'center' }}>
        <View style={{ flex: 0.25 }}>
          <Text style={SharedStyles.itemTitle}>{title}</Text>
        </View>
        <View style={{ flex: 0.75 }}>
          <TextInput
            onChangeText={docValue => this.setState({ docValue })}
            value={this.state.docValue}
          />
        </View>
      </View>
    );

    const DocKeyRenderItem = ({ item: { title } }) => (
      <View style={{ flexDirection: 'row', alignItems: 'center' }}>
        <View style={{ flex: 0.25 }}>
          <Text style={SharedStyles.itemTitle}>{title}</Text>
        </View>
        <View style={{ flex: 0.75 }}>
          <TextInput
            onChangeText={docKey => this.setState({ docKey })}
            value={this.state.docKey}
          />
        </View>
      </View>
    );

   const actionRenderItem = ({ item: { title, action } }) => (
     <TouchableOpacity disabled={this.state.partition === Data.DefaultPartitions.APP_DOCUMENTS} style={SharedStyles.item} onPress={action}>
       <Text style={SharedStyles.itemButton}>{title}</Text>
       <Modal
         animationType="slide"
         transparent={false}
         visible={this.state.createDocModalVisible && this.state.canCreateDocument}
       >
         <View style={{ marginTop: 22 }}>
           <SectionList
             renderItem={({ item }) => <Text style={[SharedStyles.item, SharedStyles.itemTitle]}>{item}</Text>}
             keyExtractor={(item, index) => item + index}
             sections={[
                  {
                    data: [
                      {
                        title: 'Time to Live',
                        value: 'docTtl'
                      },
                    ],
                    renderItem: DocTtlRenderItem
                  },
                  {
                    data: [
                      {
                        title: 'ID',
                        value: 'docId'
                      },
                    ],
                    renderItem: DocIdRenderItem
                  },
                  {
                    data: [
                      {
                        title: 'Key',
                        value: 'docKey'
                      },
                    ],
                    renderItem: DocKeyRenderItem
                  },
                  {
                    data: [
                      {
                        title: 'Type',
                        value: 'docType'
                      },
                    ],
                    renderItem: DocTypeRenderItem
                  },
                  {
                    data: [
                      {
                        title: 'Value',
                        value: 'docValue'
                      },
                    ],
                    renderItem: DocValueRenderItem
                  },
                ]}
           />
           <View style={{ flexDirection: 'row', alignItems: 'center' }}>
             <View style={{ flex: 0.5 }}>
               <TouchableOpacity
                 onPress={async () => {
                    const createResult =
                            await Data.create(
                              this.state.docId,
                              this.state.value,
                              Data.DefaultPartitions.USER_DOCUMENTS,
                              new Data.WriteOptions(this.state.docTtl)
                            );
                    console.log('Successful create', createResult);
                    this.setCreateDocModalVisible(!this.state.createDocModalVisible);
                  }}
               >
                 <Text style={[SharedStyles.itemButton]}>Create</Text>
               </TouchableOpacity>
             </View>
             <View style={{ flex: 0.5 }}>
               <TouchableOpacity
                 onPress={() => {
                      this.setCreateDocModalVisible(!this.state.createDocModalVisible);
                    }}
               >
                 <Text style={[SharedStyles.itemButton]}>Cancel</Text>
               </TouchableOpacity>
             </View>
           </View>
         </View>
       </Modal>
     </TouchableOpacity>
    );

    const partitionPicker = ({ item: { onChange } }) => (
      <ModalSelector
        data={[
          {
            label: 'Application documents',
            key: Data.DefaultPartitions.APP_DOCUMENTS
          },
          {
            label: 'User documents',
            key: Data.DefaultPartitions.USER_DOCUMENTS
          }
        ]}
        selectedKey={this.state.partition}
        onChange={onChange}
        style={SharedStyles.modalSelector}
      />
    );

    const documentsViewer = ({ item: { onDocumentRemoved } }) => (
      <View>
        <ActivityIndicator size="large" color="red" animating={this.state.loadingData} />
        <DataDocumentListView
          items={this.state.documents}
          onDocumentRemoved={onDocumentRemoved}
        />
      </View>
    );

    return (
      <View style={SharedStyles.container}>
        <SectionList
          renderItem={({ item }) => <Text style={[SharedStyles.item, SharedStyles.itemTitle]}>{item}</Text>}
          renderSectionHeader={({ section: { title } }) => <Text style={SharedStyles.header}>{title}</Text>}
          keyExtractor={(item, index) => item + index}
          sections={[
            {
              title: 'Settings',
              data: [
                {
                  title: 'Data Enabled',
                  value: 'dataEnabled',
                  toggle: async () => {
                    await Data.setEnabled(!this.state.dataEnabled);
                    const dataEnabled = await Data.isEnabled();
                    this.setState({ dataEnabled });
                  }
                },
              ],
              renderItem: switchRenderItem
            },
            {
              title: 'Actions',
              data: [
                {
                  title: 'Create a new document',
                  value: 'createNewDocument',
                  action: async () => {
                    this.setCreateDocModalVisible(this.state.dataEnabled &&
                      this.state.canCreateDocument);
                  }
                },
              ],
              renderItem: actionRenderItem
            },
            {
              title: 'Partition',
              value: 'partition',
              data: [
                {
                  onChange: async (option) => {
                    this.setState({ loadingData: true });
                    const documents = await this.listDocuments(option.key);
                    this.setState({ partition: option.key, documents, loadingData: false });
                  }
                }
              ],
              renderItem: partitionPicker
            },
            {
              title: 'Documents',
              data: [
                {
                  onDocumentRemoved: async (documentId) => {
                    await Data.remove(documentId, this.state.partition);
                    const newDocuments = [];
                    this.state.documents.forEach((document) => {
                      if (document.id !== documentId) {
                        newDocuments.push(document);
                      }
                    });
                    this.setState({ documents: newDocuments });
                  }
                }
              ],
              renderItem: documentsViewer
            }
          ]}
        />
      </View>
    );
  }
}
