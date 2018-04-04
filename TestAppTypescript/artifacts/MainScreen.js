var __awaiter = (this && this.__awaiter) || function (thisArg, _arguments, P, generator) {
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
        function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
        function step(result) { result.done ? resolve(result.value) : new P(function (resolve) { resolve(result.value); }).then(fulfilled, rejected); }
        step((generator = generator.apply(thisArg, _arguments || [])).next());
    });
};
import React, { Component } from 'react';
import SharedStyles from './SharedStyles';
import { AppState, Alert, Button, View, Platform, ToastAndroid, Text } from 'react-native';
import AppCenter from 'appcenter';
import Crashes, { ErrorAttachmentLog } from 'appcenter-crashes';
import Push from 'appcenter-push';
import AttachmentsProvider from './AttachmentsProvider';
export default class MainScreen extends Component {
    constructor(props) {
        super(props);
        this.state = {
            wrapperSdkVersion: AppCenter.getSdkVersion()
        };
    }
    render() {
        const { navigate } = this.props.navigation;
        return (<View style={SharedStyles.container}>
        <Text style={SharedStyles.heading}>
          React Native SDK version {this.state.wrapperSdkVersion}
        </Text>
        <Button title="Test Crashes" onPress={() => navigate('Crashes')}/>
        <Button title="Test Analytics" onPress={() => navigate('Analytics')}/>
        <Button title="Test Push" onPress={() => navigate('Push')}/>
        <Button title="Test Other AppCenter APIs" onPress={() => navigate('AppCenter')}/>
      </View>);
    }
}
MainScreen.navigationOptions = {
    title: 'TestApp',
};
Push.setListener({
    onPushNotificationReceived(pushNotification) {
        let message = pushNotification.message;
        let title = pushNotification.title;
        if (title === undefined || title === null) {
            title = "";
        }
        if (message === null || message === undefined) {
            title = 'Android background';
            message = '<empty>';
        }
        if (pushNotification.customProperties && Object.keys(pushNotification.customProperties).length > 0) {
            message += `\nCustom properties:\n${JSON.stringify(pushNotification.customProperties)}`;
        }
        if (AppState.currentState === 'active') {
            Alert.alert(title, message);
        }
        else {
            if (Platform.OS === 'android') {
                ToastAndroid.show(`Notification while inactive:\n${message}`, ToastAndroid.LONG);
            }
            Alert.alert(title, message);
        }
    }
});
Crashes.setListener({
    shouldProcess(report) {
        console.log(`Should process report with id: ${report.id}'`);
        return true;
    },
    shouldAwaitUserConfirmation() {
        console.log('Should await user confirmation');
        Alert.alert('One or more crashes were detected, do you want to report them?', undefined, [
            { text: 'Yes, and ask me again if it occurs again.', onPress: () => Crashes.notifyUserConfirmation(1) },
            { text: 'Yes, always send them', onPress: () => Crashes.notifyUserConfirmation(2) },
            { text: 'Don\'t send at this time', onPress: () => Crashes.notifyUserConfirmation(0) },
        ]);
        return true;
    },
    getErrorAttachments(report) {
        console.log(`Get error attachments for report with id: ${report.id}'`);
        return (() => __awaiter(this, void 0, void 0, function* () {
            const [textAttachment, binaryAttachment, binaryName, binaryType] = yield Promise.all([
                AttachmentsProvider.getTextAttachment(),
                AttachmentsProvider.getBinaryAttachment(),
                AttachmentsProvider.getBinaryName(),
                AttachmentsProvider.getBinaryType(),
            ]);
            return [ErrorAttachmentLog.attachmentWithText(textAttachment, 'hello.txt'),
                ErrorAttachmentLog.attachmentWithBinary(binaryAttachment, binaryName, binaryType)];
        }))();
    },
    onBeforeSending() {
        console.log('Will send crash. onBeforeSending is invoked.');
    },
    onSendingSucceeded() {
        console.log('Did send crash. onSendingSucceeded is invoked.');
    },
    onSendingFailed() {
        console.log('Failed sending crash. onSendingFailed is invoked.');
    }
});
//# sourceMappingURL=MainScreen.js.map