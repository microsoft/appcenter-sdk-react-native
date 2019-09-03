// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

import React, { Component } from 'react';
import { Image, View, Text, Switch, SectionList, ActivityIndicator } from 'react-native';
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
      const refreshScreen = navigation.getParam('refreshData');
      if (refreshScreen) {
        refreshScreen();
      }
      defaultHandler();
    }
  }

  // Screen's state.
  state = {
    dataEnabled: false,
    partition: Data.DefaultPartitions.APP_DOCUMENTS,
    documents: [],
    loadingData: true
  }

  async componentDidMount() {
    // Sync the module toggle.
    await this.refreshToggle();

    // Add a way to refresh the screen when the tab is pressed.
    this.props.navigation.setParams({
      refreshScreen: this.refreshToggle.bind(this)
    });
    const documents = await this.listDocuments(this.state.partition);
    this.setState({ documents, loadingData: false });
  }

  async refreshToggle() {
    const dataEnabled = await Data.isEnabled();
    this.setState({ dataEnabled });
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
      return documents;
    } catch (err) {
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
