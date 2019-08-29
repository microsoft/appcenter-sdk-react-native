// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

import React, { Component } from 'react';
import { Image, View, Text, Switch, SectionList } from 'react-native';
import ModalSelector from 'react-native-modal-selector';
import { DataDocumentListView } from '../components/DataDocumentListView';

import Data from 'appcenter-data';

import SharedStyles from '../SharedStyles';
import DataTabBarIcon from '../assets/data.png';

export default class DataScreen extends Component {
  static navigationOptions = {
    tabBarIcon: () => <Image style={{ width: 24, height: 24 }} source={DataTabBarIcon} />,
    tabBarOnPress: ({ defaultHandler, navigation }) => {
      // Allow consequent presses to refresh the screen.
      const refreshScreen = navigation.getParam('refreshData');
      if (refreshScreen) {
        refreshScreen();
      }
      defaultHandler();
    }
  }

  // List of supported partitions.
  static partitions = [
    {
      label: 'Application documents',
      key: Data.DefaultPartitions.APP_DOCUMENTS
    },
    {
      label: 'User documents',
      key: Data.DefaultPartitions.USER_DOCUMENTS
    }
  ]

  // Screen's state.
  state = {
    dataEnabled: false,
    partition: Data.DefaultPartitions.APP_DOCUMENTS,
    documents: []
  }

  async componentDidMount() {
    // Sync the module toggle.
    await this.refreshToggle();

    // Add a way to refresh the screen when the tab is pressed.
    this.props.navigation.setParams({
      refreshScreen: this.refreshToggle.bind(this)
    });
    this.setState({documents: await this.listDocuments(this.state.partition)})
  }

  async refreshToggle() {
    const dataEnabled = await Data.isEnabled();
    this.setState({ dataEnabled });
  }

  async listDocuments(partition) {
    let documents = [];
    hasNextPage = false;
    try {
      let page = await Data.list(partition);
      page.currentPage.items.forEach(item => {
        documents.push(item);
      });
      while (await page.hasNextPage()) {
        page = await page.getNextPage();
        page.currentPage.items.forEach(item => {
          documents.push(item);
        });
      }
      return documents;
    } catch (err) {
      console.log(err)
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

    const partitionPicker = ({ item: { onChange } }) => (
      <ModalSelector
        data={DataScreen.partitions}
        selectedKey={this.state.partition}
        onChange={onChange}
        style={SharedStyles.modalSelector}
      />
    );

    const documentsViewer = ({ item: { onDocumentRemoved } }) => (
      <DataDocumentListView 
        items={this.state.documents} 
        onDocumentRemoved = {onDocumentRemoved}
      />
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
              title: 'Partition',
              value: 'partition',
              data: [
                {
                  onChange: async (option) => {
                    let documents = await this.listDocuments(option.key)
                    this.setState({ partition: option.key, documents: documents });
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
                    console.log(documentId);
                    await Data.remove(documentId, this.state.partition);
                    let newDocuments = [];
                    this.state.documents.forEach(document=>{
                      if (document.id !== documentId) {
                        newDocuments.push(document);
                      }
                    });
                    this.setState({ documents: newDocuments});
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
