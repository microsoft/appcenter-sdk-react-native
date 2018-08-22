___

## Version 1.7.1

### Bugfixes
- Fix AppCenterReactNativeShared pod version issue for appcenter-crashes module. https://github.com/Microsoft/AppCenter-SDK-React-Native/pull/361

___

## Version 1.7.0

### Features

[App Center Push Android]
The Firebase messaging SDK is now a dependency of the App Center Push SDK to be able to support Android P and also prevent features to break after April 2019 based on this announcement.
You need to follow some migration steps after updating the SDK to actually use Firebase instead of the manual registration mechanism that we are providing. The non Firebase mechanism still works after updating the SDK but you will see a deprecation message, but this will not work on Android P devices until you migrate.
After updating the app to use Firebase, you will also no longer see duplicate notifications when uninstalling and reinstalling the app on the same device and user.

### Bugfixes
- Fix jest tests mock path issue. https://github.com/Microsoft/AppCenter-SDK-React-Native/pull/340

### Misc
- SDK supports react-native v0.56.0.
- This release includes the changes from the underlying AppCenter Apple SDK in version 1.8.0. Please check out its changelog.
- This release includes the changes from the underlying AppCenter Android SDK in version 1.7.0. Please check out its changelog.

___

## Version 1.6.0

### Bugfixes
- Fix jest tests failure when importing App Center packages. https://github.com/Microsoft/AppCenter-SDK-React-Native/pull/303
- Fix framework imports that break CocoaPods install. https://github.com/Microsoft/AppCenter-SDK-React-Native/pull/319
- Update Android build tools to the latest version 27.0.3. https://github.com/Microsoft/AppCenter-SDK-React-Native/pull/320
- Remove Android support library that isn't used anywhere. https://github.com/Microsoft/AppCenter-SDK-React-Native/pull/328

### Misc
- This release includes the changes from the underlying AppCenter Apple SDK in version 1.7.1. Please check out its changelog.
- This release includes the changes from the underlying AppCenter Android SDK in version 1.6.1. Please check out its changelog.

___

## Version 1.5.1

### Bugfixes
- Fix recursive header expansion and argument list too long issue. https://github.com/Microsoft/AppCenter-SDK-React-Native/pull/290
- Shorten log tags for Android bridge code to address tag too long issue on Android API <= 23. https://github.com/Microsoft/AppCenter-SDK-React-Native/pull/300
- Fix App Center log not working issue. https://github.com/Microsoft/AppCenter-SDK-React-Native/pull/301

### Misc
- This release includes the changes from the underlying AppCenter Apple SDK in version 1.6.1. Please check out its changelog.
- This release includes the changes from the underlying AppCenter Android SDK in version 1.5.1. Please check out its changelog.

___

## Version 1.5.0

### Bugfixes

- Make "Vendor" folder part of the framework search path. https://github.com/Microsoft/AppCenter-SDK-React-Native/pull/275
- Make push notfication title and message null but never undefined. https://github.com/Microsoft/AppCenter-SDK-React-Native/pull/277

### Misc
- Refined on-boarding prompt message of react-native link. https://github.com/Microsoft/AppCenter-SDK-React-Native/pull/271.
- Allow using cocoapods without react native link  https://github.com/Microsoft/AppCenter-SDK-React-Native/pull/244
- This release includes the changes from the underlying AppCenter Apple SDK in version 1.6.0. Please check out its changelog.
- This release includes the changes from the underlying AppCenter Android SDK in version 1.5.0. Please check out its changelog.

___

## Version 1.4.0

### Bugfixes
- Returns NO from requiresMainQueueSetup() in iOS native modules to avoid react native warnings. https://github.com/Microsoft/AppCenter-SDK-React-Native/pull/238
- Adds LICENSE.md to Products folder to make cocoapod integration easier during SDK release process. https://github.com/Microsoft/AppCenter-SDK-React-Native/pull/250
- Fixes react-native 0.53 prompt glitches when linking. https://github.com/Microsoft/AppCenter-SDK-React-Native/pull/252
- Avoid android link duplicates for appcenter in react-native 0.53. https://github.com/Microsoft/AppCenter-SDK-React-Native/pull/254
- Improves error handling in react-native android linker script. https://github.com/Microsoft/AppCenter-SDK-React-Native/pull/256
- Adds GitHub Issue Template. https://github.com/Microsoft/AppCenter-SDK-React-Native/pull/258

### Misc
- This release includes the changes from the underlying AppCenter Apple SDK in version 1.5.0. Please check out its changelog.
- This release includes the changes from the underlying AppCenter Android SDK in version 1.4.0. Please check out its changelog.

___

## Version 1.3.0

This release contains a bug fix.

### Bugfixes

- Support CocoaPods 1.4 https://github.com/Microsoft/AppCenter-SDK-React-Native/pull/237

### Known Issue
react-native link for this and earlier releases using React Native versions above 0.52.2 does not work properly with our SDK. We are currently investigating this, but in the meantime, please use version 0.52.2.

___

## Version 1.2.0

This release contains bugfixes and a breaking change. The SDK now requires iOS 9 or later.

### Bugfixes
- Fix warnings when sending crashes without a listener. https://github.com/Microsoft/AppCenter-SDK-React-Native/pull/192
- The test app doesn't show an error when a binary attachment is not present. https://github.com/Microsoft/AppCenter-SDK-React-Native/pull/193
- Fix a bug where Analytics.trackEvent doesn't allow message with no properties. https://github.com/Microsoft/AppCenter-SDK-React-Native/pull/202
- Perform a case-insensitive search for AppDelegate based on package.json. https://github.com/Microsoft/AppCenter-SDK-React-Native/pull/213
- Don't check for the platform when checking for the pod command. https://github.com/Microsoft/AppCenter-SDK-React-Native/pull/217

### Misc
- This release includes the changes from the underlying AppCenter Apple SDK in version 1.3.0. Please check out its changelog.
- This release includes the changes from the underlying AppCenter Android SDK in version 1.2.0. Please check out its changelog.

___

## Version 1.1.0

### Features
- [Android] The Firebase SDK dependency is now optional. If Firebase SDK is not available at runtime, the push registers and generate notifications using only App Center SDK. The Firebase application and servers are still used whether the Firebase SDK is installed into the application or not. The SDK is still compatible with Firebase packages. But if you don't use Firebase besides App Center, you can now remove these packages and refer to the updated getting started instructions to migrate the set up. Migrating the push set up in the application remains optional.
- [iOS] Support brownfield iOS app with Podfile to install using react-native link (with react-native v0.50 or later) https://github.com/Microsoft/AppCenter-SDK-React-Native/pull/177. See example Podfile here.
- appcenter-crashes getErrorAttachments callback now works with ES2107 async/await functions https://github.com/Microsoft/AppCenter-SDK-React-Native/pull/179. Please see Add attachments to a crash report of App Center documentation for more information.

### Bugfixes
- Fix a bug where the license is not included in AppCenterReactNativeShared CocoaPod https://github.com/Microsoft/AppCenter-SDK-React-Native/pull/174
- Fix a bug in appcenter-crashes module to make sure crashes won't get processed when crashes is disabled https://github.com/Microsoft/AppCenter-SDK-React-Native/pull/181

### Misc
- This release also includes changes from underlying AppCenter Apple SDK:
https://github.com/Microsoft/AppCenter-SDK-Apple/releases/tag/1.1.0
- This release also includes changes from underlying AppCenter Android SDK:
https://github.com/Microsoft/AppCenter-SDK-Android/releases/tag/1.1.0

___

## Version 1.0.1

### Bugfixes
- Fixes an issue in React Native iOS where Crashes.setListener() sometimes doesn't get the callback when iOS application launches too quickly.

### Misc
- This release also includes changes from underlying AppCenter Apple SDK:
https://github.com/Microsoft/AppCenter-SDK-Apple/releases/tag/1.0.1

___

## Version 1.0.0

### Visual Studio App Center General Availability (GA).

#### Breaking Change
This version contains breaking changes due to the renaming from Mobile Center to App Center. If you have existing apps using Mobile Center SDK, please follow the react-native sdk migration guide to upgrade to App Center SDK.

#### Misc
- This release also includes changes from underlying AppCenter Apple SDK:
https://github.com/Microsoft/AppCenter-SDK-Apple/releases/tag/1.0.0
- This release also includes changes from underlying AppCenter Android SDK:
https://github.com/Microsoft/AppCenter-SDK-Android/releases/tag/1.0.0

___

## Version 0.11.2

### Bugfixes
- Fix a compiler warning in RNCrashes native module https://github.com/Microsoft/AppCenter-SDK-React-Native/pull/153

### Misc
- This release also includes changes from underlying iOS SDK:
https://github.com/Microsoft/mobile-center-sdk-ios/releases/tag/0.14.1

___

## Version 0.11.1

### Breaking Change

- Crashes.process() API that process crashes in JavaScript is deprecated and removed from this version.
- Crashes.setListener() API is introduced to provide more functionality and flexibility of processing crashes in JavaScript. For more information, please see Customize your usage of Mobile Center Crashes.
- Crashes.setEventListener() API that get sending status for a crash log is deprecated and removed from this version. Use Crashes.setListener() API instead. The three callbacks in Crashes.setEventListener() are renamed in Crashes.setListener() as follow:
  - willSendCrash() -> onBeforeSending()
  - didSendCrash() -> onSendingSucceeded()
  - failedSendingCrash() -> onSendingFailed()
- Push.setEventListener() API is renamed as Push.setListener(), and the callback is renamed as follow:
  - pushNotificationReceived() -> onPushNotificationReceived()

### New Feature

- Crashes.setListener() provides the following callbacks: 
  - shouldProcess() callback allows you decide if a particular crash needs to be processed or not.
  - shouldAwaitUserConfirmation() and Crashes.notifyUserConfirmation() callback allows you wait for user confirmation before sending crashes.
  - getErrorAttachments() callback allows you to add attachments to a crashe report

For more information, please see Customize your usage of Mobile Center Crashes.

### Bugfixes

- Fix a bug in Android that JS send events before native Java loads would crash application https://github.com/Microsoft/AppCenter-SDK-React-Native/pull/135

### Misc

- This release also includes changes from underlying iOS SDK:
https://github.com/Microsoft/mobile-center-sdk-ios/releases/tag/0.14.0
- This release also includes changes from underlying Android SDK:
https://github.com/Microsoft/mobile-center-sdk-android/releases/tag/0.13.0

___

## Version 0.10.0

- New Feature - getSdkVersion() API for getting mobile center react native SDK version at runtime https://github.com/Microsoft/AppCenter-SDK-React-Native/pull/126
- Support Android only project in react-native link https://github.com/Microsoft/AppCenter-SDK-React-Native/pull/107
- Support iOS only project in react-native link https://github.com/Microsoft/AppCenter-SDK-React-Native/pull/107
- Fix a bug in setCustomProperties() API where boolean property is always set to true https://github.com/Microsoft/AppCenter-SDK-React-Native/pull/114
- Fix a bug where mobile center analytics is always enabled even analytics is supposed to "Enable in JavaScript" https://github.com/Microsoft/AppCenter-SDK-React-Native/pull/117
- Fix a typo in crashes logging in onSendingFailed https://github.com/Microsoft/AppCenter-SDK-React-Native/pull/120
- Fix a bug in iOS react-native link script issue https://github.com/Microsoft/AppCenter-SDK-React-Native/pull/123
- This release also includes changes from underlying iOS SDK:
https://github.com/Microsoft/mobile-center-sdk-ios/releases/tag/0.12.2
https://github.com/Microsoft/mobile-center-sdk-ios/releases/tag/0.12.3
https://github.com/Microsoft/mobile-center-sdk-ios/releases/tag/0.13.0

___

## Version 0.9.0

- Breaking change Updated CustomProperties API parameters to align with other Mobile Center SDKs. Please refer to the updated documentation.
- Fix updating code when changing iOS prompt answers when running react-native link a second time.
- This release also includes changes from underlying Android SDK of the following versions:
https://github.com/Microsoft/mobile-center-sdk-android/releases/tag/0.12.0
- This release also includes changes from underlying iOS SDK:
https://github.com/Microsoft/mobile-center-sdk-ios/releases/tag/0.12.0
https://github.com/Microsoft/mobile-center-sdk-ios/releases/tag/0.12.1

___

## Version 0.8.1

- Fix hasCrashedInLastSession in iOS to return a boolean value instead of an integer (was 0 or 1).
- Fix getLastSessionCrashReport on iOS to return null instead of an empty report object when app did not crash in last session.
- Fix importing Javascript modules into Jest tests where Babel is typically disabled.
- This release also includes changes from underlying Android SDK of the following versions:
https://github.com/Microsoft/mobile-center-sdk-android/releases/tag/0.11.2
- This release also includes changes from underlying iOS SDK:
https://github.com/Microsoft/mobile-center-sdk-ios/releases/tag/0.11.1
https://github.com/Microsoft/mobile-center-sdk-ios/releases/tag/0.11.2

___

## Version 0.8.0

- Implement error attachments for crash reports.
- Fix updating cocoapods dependencies on Mobile Center for iOS when running react-native link after updating the npm packages.
- Fix react-native link on ios projects that were renamed after initial creation such as when using create-react-native-app and then ejecting to a different name.
- Fix Android bridge on applications that depend on React Native Android bridge version 0.47.0 or later.
- Fix a bug where analytics and crash dependencies for iOS were always installed even if you don't use both modules.
- This release also includes changes from underlying Android SDK of the following versions:
https://github.com/Microsoft/mobile-center-sdk-android/releases/tag/0.11.0
https://github.com/Microsoft/mobile-center-sdk-android/releases/tag/0.11.1
- This release also includes changes from underlying iOS SDK:
https://github.com/Microsoft/mobile-center-sdk-ios/releases/tag/0.11.0

___

## Version 0.7.0

This update contains fixes some issues in our new push & custom properties functionality:
- When the user taps on a push notification message to launch the app, the notification info is now properly passed along to the push callback
- Custom properties of type 'Date' are now handled properly, with the right data type
- The MobileCenter class now has a getInstallId method, returning the Mobile Center device unique identifier. It can be useful in debugging.

If you are not using Cocoapods, use frameworks from the version 0.10.1 of the Mobile Center SDK for iOS.

___

## Version 0.6.1

- Fixed naming issue with RNMobileCenterShared modulemap. This problem caused RNMobileCenterShared import not found errors in 0.6.0, but those should disappear with this fix.

___

## Version 0.6.0

- Updated to use 0.10.x versions of the iOS and Android native Mobile Center SDKs.
- Added new mobile-center package to handle common Mobile Center functionality, including support for custom properties and setting the logging level.
- Added callback support for Push.
- Added the ability to set custom properties. Custom properties are name/value pairs you set on the client which are then sent to Mobile Center. They can be used in the Mobile Center web portal to create targeted audiences for push notifications, based on the value of those properties. And in the future you'll be able to use them for other things in Mobile Center too.
- Improved error messages for the react-native link step. Also updated to no longer prompt for app secret during react native link if the app secret is already set; instead we just output the current value of the app secret along with the path where it's stored if the dev wants to change it manually. That all makes for a more streamlined getting started experience.

___

## Version 0.5.0

- Updated to use new 0.9.x versions of the iOS and Android native Mobile Center SDKs.
- Added support for Mobile Center Push. That's in the new mobile-center-push package.
- Improved error handling in the react-native link scripts.
- Renamed Analytics.getEnabled to Analytics.isEnabled, to be consistent with the other RN modules and native SDKs, which all use isEnabled. This also fixes a bug on Android where that method didn't exist.

___

## Version 0.4.0

- Updated to use new 0.6.x version of the iOS and Android native Mobile Center SDK.
- Updated RN mobile-center-analytics/mobile-center-crashes dependencies accordingly

___

## Version 0.3.0

- Updated to use new 0.5.x version of the iOS native Mobile Center SDK.
- That SDK update had one breaking API change:
[Misc] setServerUrl method has renamed to setLogUrl.

___

## Version 0.2.1

Changed iOS build flags:

- Added bitcode support for release builds. Building your app with bitcode isn't encouraged, as it complicates uploading symbols for Mobile Center crash stack traces, but now it's possible if desired.
- Added some security related compiler flags, making consistent with the Mobile Center native iOS SDK
- Build for release

___

## Version 0.2.0

This version has breaking changes.

With this update, the React Native Mobile Center SDK now wraps the latest native Mobile Center SDKs: iOS SDK 0.4.x and Android SDK 0.5.x.

API changes
Crashes.hasCrashedInLastSession() and Crashes.lastSessionCrashReport() and now methods; they were formerly constants. They are also async methods. This change was needed since those two APIs are now async on Android, in order to support strict mode. On iOS, the wrapped native SDK methods are synchronous and the JavaScript versions just post a response right away.

APIs related to crash attachments have been removed, as that feature isn't yet available. Previously the APIs could have been called but didn't do anything, which was confusing. We'll add the APIs back when the feature ships.

___

## Version 0.1.0

Initial release.