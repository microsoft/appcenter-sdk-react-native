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
import Push from "appcenter-push";
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
        <Button title="Test Push" onPress={() => navigate("Push")} />
        <Button
          title="Test Other AppCenter APIs"
          onPress={() => navigate("AppCenter")}
        />
      </View>
    );
  }
}

Push.setListener({
  onPushNotificationReceived(pushNotification) {
    let message = pushNotification.message;
    let title = pushNotification.title;
    if (title === null) {
      title = "";
    }

    if (message === null) {
      message = "";
    } else {
      message += "\n";
    }

    // Any custom name/value pairs added in the portal are in customProperties
    if (
      pushNotification.customProperties &&
      Object.keys(pushNotification.customProperties).length > 0
    ) {
      message += `\nCustom properties:\n${JSON.stringify(
        pushNotification.customProperties
      )}`;
    }

    if (AppState.currentState === "active") {
      Alert.alert(title, message);
    } else {
      // Sometimes the push callback is received shortly before the app is fully active in the foreground.
      // In this case you'll want to save off the notification info and wait until the app is fully shown
      // in the foreground before displaying any UI. You could use AppState.addEventListener to be notified
      // when the app is fully in the foreground.

      // Showing an alert when not in the "active" state seems to work on iOS; for Android, we show a toast
      // message instead
      if (Platform.OS === "android") {
        ToastAndroid.show(
          `Notification while inactive:\n${message}`,
          ToastAndroid.LONG
        );
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
