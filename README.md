# react-native-sonoma-private

Development repository for the Sonoma SDK for React Native.

## Getting Started (iOS)

1. Clone the repository

2. Build native SDK dependencies
  - Open up `AvalancheSDK.xcworkspace` from Sonoma-SDK-iOS submodule directory
  - Select "AllDoc + Frameworks" target and build

3. Build Demo app
  - `cd` into SonomaDemoApp folder and `npm install`
  - `cd` into ios folder and `pod install`. If you have not installed CocoaPods, `sudo gem install cocoapods`.
  - If you make changes in any of the individual SDKs (eg. RNSonomaHub, RNSonomaErrorReporting), you will need to run the above two steps again to test your changes using the sample app.

4. Run Demo app
  - `cd` into SonomaDemoApp folder and `npm start`
  - Open up `SonomaDemoApp/ios/SonomaDemoApp.xcworkspace`, select desired simulator and click the "Run" button

## Getting Started (Android)

TODO

## To-do items:

### iOS

1. Error Reporting
  - Send special JS error report if exception is from JS
  - Get last session crash details from JS
  - Set custom user confirmation handler from JS

### Android

TODO

### JS

1. Error Reporting
  - Add global exception handler to capture JS unhandled exceptions send to server

2. Analytics
  - Figure out how auto page tracking should work for React Native

2. Type definitions

3. Write tests
