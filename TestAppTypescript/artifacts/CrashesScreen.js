var __awaiter = (this && this.__awaiter) || function (thisArg, _arguments, P, generator) {
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
        function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
        function step(result) { result.done ? resolve(result.value) : new P(function (resolve) { resolve(result.value); }).then(fulfilled, rejected); }
        step((generator = generator.apply(thisArg, _arguments || [])).next());
    });
};
import React, { Component } from 'react';
import { StyleSheet, Text, View, TextInput, ScrollView, TouchableOpacity } from 'react-native';
import { DialogComponent } from 'react-native-dialog-component';
import ImagePicker from 'react-native-image-picker';
import Crashes from 'appcenter-crashes';
import SharedStyles from './SharedStyles';
import AttachmentsProvider from './AttachmentsProvider';
export default class CrashesScreen extends Component {
    constructor(props) {
        super(props);
        this.state = {
            crashesEnabled: false,
            lastSessionStatus: '',
            textAttachment: '',
            binaryAttachment: ''
        };
        this.toggleEnabled = this.toggleEnabled.bind(this);
        this.jsCrash = this.jsCrash.bind(this);
        this.nativeCrash = this.nativeCrash.bind(this);
        this.showFilePicker = this.showFilePicker.bind(this);
        this.dialogComponent = null;
    }
    componentDidMount() {
        return __awaiter(this, void 0, void 0, function* () {
            let status = '';
            const component = this;
            const crashesEnabled = yield Crashes.isEnabled();
            component.setState({ crashesEnabled });
            const crashedInLastSession = yield Crashes.hasCrashedInLastSession();
            status += `Crashed: ${crashedInLastSession ? 'yes' : 'no'}\n\n`;
            component.setState({ lastSessionStatus: status });
            if (crashedInLastSession) {
                const crashReport = yield Crashes.lastSessionCrashReport();
                status += JSON.stringify(crashReport, null, 4);
                component.setState({ lastSessionStatus: status });
            }
            const textAttachmentValue = yield AttachmentsProvider.getTextAttachment();
            component.setState({ textAttachment: textAttachmentValue });
            const binaryAttachmentValue = yield AttachmentsProvider.getBinaryAttachmentInfo();
            component.setState({ binaryAttachment: binaryAttachmentValue });
        });
    }
    toggleEnabled() {
        return __awaiter(this, void 0, void 0, function* () {
            yield Crashes.setEnabled(!this.state.crashesEnabled);
            const crashesEnabled = yield Crashes.isEnabled();
            this.setState({ crashesEnabled });
        });
    }
    jsCrash() {
        this.props.fooMethod();
    }
    nativeCrash() {
        Crashes.generateTestCrash();
    }
    render() {
        return (<View style={SharedStyles.container}>
                <ScrollView>
                    <Text style={SharedStyles.heading}>
                        Test Crashes
          </Text>
                    <Text style={SharedStyles.enabledText}>
                        Crashes enabled: {this.state.crashesEnabled ? 'yes' : 'no'}
                    </Text>
                    <TouchableOpacity onPress={this.toggleEnabled}>
                        <Text style={SharedStyles.toggleEnabled}>
                            toggle
            </Text>
                    </TouchableOpacity>
                    <TouchableOpacity onPress={this.jsCrash}>
                        <Text style={styles.button}>
                            Crash JavaScript
            </Text>
                    </TouchableOpacity>
                    <TouchableOpacity onPress={this.nativeCrash}>
                        <Text style={styles.button}>
                            Crash native code
            </Text>
                    </TouchableOpacity>
                    <TouchableOpacity onPress={() => { this.dialogComponent.show(); }}>
                        <Text style={styles.button}>
                            Set text error attachment
            </Text>
                    </TouchableOpacity>
                    <Text style={SharedStyles.enabledText}>{'Current value:'}{this.state.textAttachment}</Text>
                    <TouchableOpacity onPress={this.showFilePicker}>
                        <Text style={styles.button}>
                            Select image as binary error attachment
            </Text>
                    </TouchableOpacity>
                    <Text style={SharedStyles.enabledText}>{'Current value:'}{this.state.binaryAttachment}</Text>
                    <Text style={styles.lastSessionHeader}>Last session:</Text>
                    <Text style={styles.lastSessionInfo}>
                        {this.state.lastSessionStatus}
                    </Text>
                </ScrollView>
                {this.getTextAttachmentDialog()}
            </View>);
    }
    getTextAttachmentDialog() {
        return (<DialogComponent ref={(dialogComponent) => { this.dialogComponent = dialogComponent; }} width={0.9}>
                <View>
                    <TextInput style={SharedStyles.dialogInput} onChangeText={text => this.setState({ textAttachment: text })}/>
                    <View style={SharedStyles.dialogButtonContainer}>
                        <TouchableOpacity style={SharedStyles.dialogButton} onPress={() => {
            AttachmentsProvider.saveTextAttachment(this.state.textAttachment);
            this.dialogComponent.dismiss();
        }}>
                            <Text style={styles.button}>
                                Save
              </Text>
                        </TouchableOpacity>
                        <TouchableOpacity style={SharedStyles.dialogButton} onPress={() => { this.dialogComponent.dismiss(); }}>
                            <Text style={styles.button}>
                                Cancel
              </Text>
                        </TouchableOpacity>
                    </View>
                </View>
            </DialogComponent>);
    }
    showFilePicker() {
        ImagePicker.showImagePicker({}, (response) => __awaiter(this, void 0, void 0, function* () {
            if (response.didCancel) {
                console.log('User cancelled image picker');
            }
            else if (response.error) {
                console.log('ImagePicker Error: ', response.error);
            }
            else {
                AttachmentsProvider.saveBinaryAttachment(getFileName(response), response.data, getFileType(response), getFileSize(response));
                const binaryAttachmentValue = yield AttachmentsProvider.getBinaryAttachmentInfo();
                this.setState({ binaryAttachment: binaryAttachmentValue });
            }
        }));
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
const styles = StyleSheet.create({
    button: {
        color: '#4444FF',
        fontSize: 18,
        textAlign: 'center',
        margin: 10,
    },
    lastSessionHeader: {
        fontSize: 20,
        textAlign: 'center',
        marginTop: 30
    },
    lastSessionInfo: {
        fontSize: 14,
        textAlign: 'center',
    },
});
//# sourceMappingURL=CrashesScreen.js.map