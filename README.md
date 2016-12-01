# Mobile Center React-Native

Development repository for the Mobile Center SDK for React Native.

## Getting Started (iOS)

1. Clone the repository
2. Download the latest Mobile Center frameworks from https://github.com/microsoft/mobile-center-sdk-ios
3. open up `RNMobileCenter/ios/RNMobileCenter.xcodeproj`, drag in `MobileCenter.framework` and build the `Fat Framework` target
4. Build Demo app
  - `cd` into MobileCenterDemoApp folder and `npm install`
  - run `react-native link` to hook up the native sources and specify app secret 
5. Run Demo app
  - `cd` into MobileCenterDemoApp folder and `react-native run-ios`

## Getting Started (Android)

1. Clone the repository
2. Build Demo app
  - `cd` into MobileCenterDemoApp folder and `npm install`
  - run `react-native link` to hook up native sources and specify app secret
4. Run Demo app
  - Boot up Android emulator
  - `cd` into MobileCenterDemoApp folder and `react-native run-android`

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
