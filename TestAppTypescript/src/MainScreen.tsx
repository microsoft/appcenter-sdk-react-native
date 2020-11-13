import React, { Component } from "react";
import SharedStyles from "./SharedStyles";
import {
  AppState,
  Alert,
  Button,
  View,
  Platform,
  ToastAndroid,
  Text
} from "react-native";
import AppCenter from "appcenter";
import Crashes, {
  UserConfirmation,
  ErrorAttachmentLog
} from "appcenter-crashes";
import AttachmentsProvider from "./AttachmentsProvider";

export default class MainScreen extends Component<
  any,
  {
    wrapperSdkVersion: string;
  }
> {
  static navigationOptions = {
    title: "TestApp"
  };

  constructor(props: any) {
    super(props);
    this.state = {
      wrapperSdkVersion: AppCenter.getSdkVersion()
    };
  }

  render() {
    const { navigate } = this.props.navigation;

    return (
      <View style={SharedStyles.container}>
        <Text style={SharedStyles.heading}>
          React Native SDK version {this.state.wrapperSdkVersion}
        </Text>
        <Button title="Test Crashes" onPress={() => navigate("Crashes")} />
        <Button title="Test Analytics" onPress={() => navigate("Analytics")} />
        <Button
          title="Test Other AppCenter APIs"
          onPress={() => navigate("AppCenter")}
        />
      </View>
    );
  }
}

Crashes.setListener({
  shouldProcess(report) {
    console.log(`Should process report with id: ${report.id}'`);
    return true;
  },

  shouldAwaitUserConfirmation() {
    console.log("Should await user confirmation");
    Alert.alert(
      "One or more crashes were detected, do you want to report them?",
      undefined,
      [
        {
          text: "Yes, and ask me again if it occurs again.",
          onPress: () => Crashes.notifyUserConfirmation(UserConfirmation.SEND)
        },
        {
          text: "Yes, always send them",
          onPress: () =>
            Crashes.notifyUserConfirmation(UserConfirmation.ALWAYS_SEND)
        },
        {
          text: "Don't send at this time",
          onPress: () =>
            Crashes.notifyUserConfirmation(UserConfirmation.DONT_SEND)
        }
      ]
    );
    return true;
  },

  getErrorAttachments(report) {
    console.log(`Get error attachments for report with id: ${report.id}'`);
    return (async () => {
      const [
        textAttachment,
        binaryAttachment,
        binaryName,
        binaryType
      ] = await Promise.all([
        AttachmentsProvider.getTextAttachment(),
        AttachmentsProvider.getBinaryAttachment(),
        AttachmentsProvider.getBinaryName(),
        AttachmentsProvider.getBinaryType()
      ]);
      //TODO type of binary attachment is always string?
      return [
        ErrorAttachmentLog.attachmentWithText(textAttachment, "hello.txt"),
        ErrorAttachmentLog.attachmentWithBinary(
          binaryAttachment,
          binaryName,
          binaryType
        )
      ];
    })();
  },

  onBeforeSending() {
    console.log("Will send crash. onBeforeSending is invoked.");
  },

  onSendingSucceeded() {
    console.log("Did send crash. onSendingSucceeded is invoked.");
  },

  onSendingFailed() {
    console.log("Failed sending crash. onSendingFailed is invoked.");
  }
});
