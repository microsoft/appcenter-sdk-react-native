import React, { Component } from 'react';
import { Image, View, Text, TextInput, Switch, SectionList, TouchableOpacity } from 'react-native';
import { DialogComponent } from 'react-native-dialog-component';
import ImagePicker from 'react-native-image-picker';

import Crashes from 'appcenter-crashes';

import { FooClass } from '../../js/FooClass';
import AttachmentsProvider from '../AttachmentsProvider';
import SharedStyles from '../SharedStyles';

export default class CrashesScreen extends Component {
  static navigationOptions = {
    // eslint-disable-line global-require
    tabBarIcon: () => <Image style={{ width: 24, height: 24 }} source={require('../assets/crashes.png')} />
  }

  state = {
    crashesEnabled: false,
    lastSessionStatus: '',
    textAttachment: '',
    binaryAttachment: ''
  }

  async componentWillMount() {
    const crashesEnabled = await Crashes.isEnabled();
    this.setState({ crashesEnabled });

    const crashedInLastSession = await Crashes.hasCrashedInLastSession();
    const lastSessionStatus = crashedInLastSession ? 'Crashed' : 'OK';
    this.setState({ lastSessionStatus });

    const textAttachment = await AttachmentsProvider.getTextAttachment();
    this.setState({ textAttachment });

    const binaryAttachment = await AttachmentsProvider.getBinaryAttachmentInfo();
    this.setState({ binaryAttachment });
  }

  jsCrash() {
    const foo = new FooClass();
    foo.method1();
  }

  nativeCrash() {
    Crashes.generateTestCrash();
  }

  render() {
    const switchRenderItem = ({ item: { title, value, toggle } }) => (
      <View style={SharedStyles.item}>
        <Text style={SharedStyles.title}>{title}</Text>
        <Switch value={this.state[value]} onValueChange={toggle} />
      </View>
    );
    const valueRenderItem = ({ item: { title, value } }) => (
      <View style={SharedStyles.item}>
        <Text style={SharedStyles.title}>{title}</Text>
        <Text>{this.state[value]}</Text>
      </View>
    );
    const actionRenderItem = ({ item: { title, action } }) => (
      <TouchableOpacity style={SharedStyles.item} onPress={action}>
        <Text style={SharedStyles.itemButton}>{title}</Text>
      </TouchableOpacity>
    );

    return (
      <View style={SharedStyles.container}>
        <SectionList
          renderItem={({ item }) => <Text style={[SharedStyles.item, SharedStyles.title]}>{item}</Text>}
          renderSectionHeader={({ section: { title } }) => <Text style={SharedStyles.header}>{title}</Text>}
          keyExtractor={(item, index) => item + index}
          sections={[
            {
              title: 'Settings',
              data: [
                {
                  title: 'Crashes Enabled',
                  value: 'crashesEnabled',
                  toggle: async () => {
                    await Crashes.setEnabled(!this.state.crashesEnabled);
                    const crashesEnabled = await Crashes.isEnabled();
                    this.setState({ crashesEnabled });
                  }
                },
              ],
              renderItem: switchRenderItem
            },
            {
              title: 'Actions',
              data: [
                {
                  title: 'Crash JavaScript',
                  action: this.jsCrash
                },
                {
                  title: 'Crash native code',
                  action: this.nativeCrash
                },
                {
                  title: 'Set text error attachment',
                  action: () => { this.dialogComponent.show(); }
                },
                {
                  title: 'Select image as binary error attachment',
                  action: this.showFilePicker
                },
              ],
              renderItem: actionRenderItem
            },
            {
              title: 'Miscellaneous',
              data: [
                { title: 'Last session status', value: 'lastSessionStatus' },
                { title: 'Text attachment', value: 'textAttachment' },
                { title: 'Binary attachment', value: 'binaryAttachment' },
              ],
              renderItem: valueRenderItem
            },
          ]}
        />
        {this.getTextAttachmentDialog()}
      </View>
    );
  }


  getTextAttachmentDialog = () => (
    <DialogComponent ref={(dialogComponent) => { this.dialogComponent = dialogComponent; }} width={0.9}>
      <View>
        <TextInput style={SharedStyles.dialogInput} onChangeText={textAttachment => this.setState({ textAttachment })} />
        <View style={SharedStyles.dialogButtonContainer}>
          <TouchableOpacity
            style={SharedStyles.dialogButton}
            onPress={() => {
              AttachmentsProvider.saveTextAttachment(this.state.textAttachment);
              this.dialogComponent.dismiss();
            }}
          >
            <Text>Save</Text>
          </TouchableOpacity>
          <TouchableOpacity style={SharedStyles.dialogButton} onPress={() => { this.dialogComponent.dismiss(); }}>
            <Text>Cancel</Text>
          </TouchableOpacity>
        </View>
      </View>
    </DialogComponent>
  )

  showFilePicker = () => {
    ImagePicker.showImagePicker(null, async (response) => {
      if (response.didCancel) {
        console.log('User cancelled image picker');
      } else if (response.error) {
        console.log('ImagePicker Error: ', response.error);
      } else {
        AttachmentsProvider.saveBinaryAttachment(getFileName(response), response.data, getFileType(response), getFileSize(response));
        const binaryAttachmentValue = await AttachmentsProvider.getBinaryAttachmentInfo();
        this.setState({ binaryAttachment: binaryAttachmentValue });
      }
    });

    function getFileName(response) {
      let fileName = 'binary.jpeg';
      if (response.fileName) {
        fileName = response.fileName;
      }
      return fileName;
    }

    function getFileType(response) {
      let fileType = 'image/jpeg';
      if (response.type) {
        fileType = response.type;
      }
      return fileType;
    }

    function getFileSize(response) {
      const thresh = 1024;
      const units = ['KiB', 'MiB', 'GiB'];
      let fileSize = response.fileSize;
      if (Math.abs(fileSize) < thresh) {
          return `${fileSize} B`;
      }
      let u = -1;
      do {
        fileSize /= thresh;
        ++u;
      } while (Math.abs(fileSize) >= thresh && u < units.length - 1);
      return `${fileSize.toFixed(1)} ${units[u]}`;
    }
  }
}
