# Mobile Center React-Native

Development repository for the Mobile Center SDK for React Native.

## Introduction

The Microsoft Mobile Center React Native SDK lets you add [Mobile Center](https://www.visualstudio.com/vs/mobile-center/) services to your
React Native application.

The SDK is currently in public preview. It provides a React Native friendly wrapper aound the native Mobile Center SDKs, supporting the
following services:

Analytics: Mobile Center Analytics helps you understand user behavior and customer engagement to improve your app. The SDK automatically
captures session count and device properties like model, OS Version etc. You can define your own custom events to measure things that matter
to your business. All the information captured is available in the Mobile Center portal for you to analyze the data.

Crashes: The Mobile Center SDK will automatically generate a crash log every time your app crashes. Both JavaScript exceptions and native
crashes are supported. The log is first written to the device's 
storage and when the user starts the app again, the crash report will be forwarded to Mobile Center. Collecting crashes works for both beta and
live apps, i.e. those submitted to Google Play or other app stores. Crash logs contain viable information for you to help resolve the issue.
The SDK gives you a lot of flexibility how to handle a crash log. As a developer you can collect and add additional information to the report
if you like.

To learn how to use this SDK, integrating it into your React Native app, see the [Mobile Center doc here.](https://docs.mobile.azure.com/sdk/React-Native/getting-started)

## To-do items:

### iOS

1. Error Reporting
  - Wait for notification from JS before sending crash
  - Send special JS error report if exception is from JS
  - Get last session crash details from JS
  - Implement "automatic" crash sending / set custom crash listener from JS
  - Send crash with text attachment
  - Send crash with binary attachment

### Android

1. Error Reporting
  - Send special JS error report if exception is from JS
  - Implement "automatic" crash sending / set custom crash listener from JS
  - Send crash with binary attachment

### JS

1. Error Reporting
  - Add global exception handler to capture JS unhandled exceptions send to server
2. Analytics
  - Figure out how auto page tracking should work for React Native
3. Type definitions (useful for VS Code IntelliSense)
4. Write tests

## Contributing

We're looking forward to your contributions via pull requests.

### Code of Conduct

This project has adopted the [Microsoft Open Source Code of Conduct](https://opensource.microsoft.com/codeofconduct/). For more information see the [Code of Conduct FAQ](https://opensource.microsoft.com/codeofconduct/faq/) or contact opencode@microsoft.com with any additional questions or comments.

### Contributor License

You must sign a [Contributor License Agreement](https://cla.microsoft.com/) before submitting your pull request. To complete the Contributor License Agreement (CLA), you will need to submit a request via the [form](https://cla.microsoft.com/) and then electronically sign the CLA when you receive the email containing the link to the document. You need to sign the CLA only once to cover submission to any Microsoft OSS project.

## Contact
If you have further questions or are running into trouble that cannot be resolved by any of the steps here, feel free to open a Github issue here or contact us at mobilecentersdk@microsoft.com.
