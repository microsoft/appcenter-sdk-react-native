# App Center SDK for .NET Change Log

## Version 1.12.0 

### AppCenter
#### Android
* **[Fix]** Do not delete old logs when trying to add a log larger than the maximum storage capacity.
* **[Fix]** Fix minimum storage size verification to match minimum possible value.
* **[Fix]** Fix disabling logging of network state changes according to AppCenter.getLogLevel.
* **[Fix]** Fix logs duplication on unstable network.
#### iOS
* **[Fix]** Do not delete old logs when trying to add a log larger than the maximum storage capacity.
* **[Fix]** Fix minimum storage size verification to match minimum possible value.
* **[Fix]** Fix reporting carrier information using new iOS 12 APIs when running on iOS 12+.
* **[Fix]** Fix a memory leak issue during executing SQL queries.
* **[Feature]** Add preview support for arm64e CPU architecture.

### AppCenterCrashes
#### UWP
* **[Fix]** Downgrade the platforms verification error to a warning.
* **[Fix]** Update vulnerable `Microsoft.NETCore.UniversalWindowsPlatform` dependency from version `5.2.2` to `5.2.6`.
#### Android
* **[Fix]** Fix a bug where crash data file could leak when the database is full.

### AppCenterPush
#### Android
* **[Fix]** Fix PushNotificationReceived event for pushes received in foreground after re-enabling the push service.
___

## Version 1.11.0

### AppCenter

#### iOS

* **[Fix]** Fix an issue where concurrent modification of custom properties was not thread safe.
* **[Fix]** Fix validating and discarding Not a Number (NaN) and infinite double values for custom properties.
* **[Fix]** Use standard SQL syntax to avoid affecting users with custom SQLite libraries.
* **[Fix]** Get database page size dynamically to support custom values.

### AppCenterCrashes

#### Android

* **[Fix]** Preventing stack overflow crash while reading a huge throwable file.

### AppCenterPush

#### iOS

* **[Fix]** Fix `PushNotificationReceived` callback not triggered on notification tapped or received in foreground when a `UNUserNotificationCenterDelegate` is set. If you have implemented this delegate please remove any call to the `Push.DidReceiveRemoteNotification` method as it's now handled by the new [User Notification Center Delegate Forwarder](https://docs.microsoft.com/en-us/appcenter/sdk/push/xamarin-ios).

___

## Version 1.10.0

### AppCenter

#### Android

* **[Security]** To enforce TLS 1.2 on all HTTPS connections the SDK makes, we are dropping support for API level 15 (which supports only TLS 1.0), the minimum SDK version thus becomes 16. Previous versions of the SDK were already using TLS 1.2 on API level 16+.
* **[Bug fix]** Fix validating and discarding `NaN` and infinite double values when calling `setCustomProperties`.

#### iOS

* **[Fix]** Add missing network request error logging.

#### UWP

* **[Fix]** Fix a crash when system or user locale cannot be read by falling back to using app locale.
* **[Fix]** Fix compatibility with SQLite 1.5.231.

### App Center Crashes

#### iOS

* **[Fix]** Fix the list of binary images in crash reports for arm64e-based devices.

### App Center Distribute

#### iOS

* **[Fix]** Fix translation of closing a dialog in Portuguese.

___

## Version 1.9.0

### App Center

#### iOS

* **[Fix]** Fix a potential deadlock that can freeze the application launch causing the iOS watchdog to kill the application.

### App Center Crashes

#### Android

* **[Fix]** Fix a bug where some initialize operations were executed twice.
* **[Fix]** Fix a bug where device information could be null when reading the error report client side.

#### iOS

* **[Fix]** The above deadlock was mostly impacting the Crashes module.

### App Center Distribute

#### Android

* **[Fix]** Fix a crash that could happen when starting the application.

### App Center Push

* **[Fix]** Fix a crash that could happen when using Push on devices behind a firewall or unstable network.

___

## Version 1.8.0

### App Center

#### UWP

* **[Fix]** Fix occasional crash when checking network connectivity.

### App Center Distribute

#### iOS

* **[Fix]** Fix a bug where browser would open randomly when in-app updates were already enabled.

### App Center Push

#### Android

The Firebase messaging SDK is now a dependency of the App Center Push SDK to be able to support Android P and also prevent features to break after April 2019 based on [this announcement](https://firebase.googleblog.com/2018/04/time-to-upgrade-from-gcm-to-fcm.html).

You need to follow [some migration steps](https://docs.microsoft.com/en-us/appcenter/sdk/push/migration/xamarin-android) after updating the SDK to actually use Firebase instead of the manual registration mechanism that we are providing. The non Firebase mechanism still works after updating the SDK but you will see a deprecation message, but this will not work on Android P devices until you migrate.

After updating the app to use Firebase, you will also no longer see duplicate notifications when uninstalling and reinstalling the app on the same device and user.
___

## Version 1.7.0

### AppCenter

#### Android

* **[Improvement]** Enable TLS 1.2 on API levels where it's supported but not enabled by default (API level 16-19, this became a default starting API level 20). Please note we still support Android API level 15 and it uses TLS 1.0.
* **[Improvement]** Gzip is used over HTTPS when request size is larger than 1.4KB.
* **[Fix]** Fix a crash when disabling a module at the same time logs are sent.
* **[Fix]** Fix pretty print JSON in Android P when verbose logging is enabled.

### AppCenterCrashes

#### UWP

* **[Fix #543]** Fix warning "The referenced component 'Microsoft.AppCenter.Crashes.dll' could not be found". You might need to clean solution and package cache after updating nuget to resolve the warning for good.

### AppCenterPush

#### UWP

* **[Fix]** Prevent application from crashing when push setup fails due to unexpected errors.

___

## Version 1.6.1

This version contains bug fixes.

### AppCenter

#### Android

* **[Fix]** Fix a crash when network state changes at same time as SDK initializing.
* **[Fix #664]** Fix crashes when trying to detect we run on instrumented test environment.
* **[Fix #649]** Fix a deadlock when setting wrapper SDK information or setting log url while other channel operations performed such as when Crashes is starting.

#### iOS

* **[Fix #670]** [Previous release](https://github.com/Microsoft/AppCenter-SDK-DotNet/releases/tag/1.6.0) was unintentionally requiring Mono 5.8 on iOS applications. This build issue is now fixed.

#### UWP

* **[Fix #613]** When marking an unhandled exception as handled, the SDK was shutting down and no further log could be sent. This is now fixed.

### AppCenterCrashes

#### Xamarin

* **[Fix]** Fix a crash when calling `Crashes.TrackError` exception parameter with null, now prints an error instead.
* **[Fix]** Fix a SDK crash while saving original crash if SDK cannot serialize the exception for client side inspection (with an exception that is not SerializableException).
* **[Fix]** When looking at `(await Crashes.GetLastSessionCrashReportAsync()).Exception` or when using the sending callbacks, the Exception object is now null instead of containing a serialization exception (on failure to get original exception object after restart). This is only for client side inspection, the crash stack trace is correctly reported to the server in all scenarios.

#### Android

* **[Fix]** Fix reporting crash when process name cannot be determined.

#### iOS

* **[Fix]** Fix an issue in breadcrumbs feature when events are being tracked on the main thread just before a crash.
* **[Fix]** Fix an issue with cached logs for breadcrumbs feature which are sometimes not sent during app start.

### AppCenterPush

#### Android

* **[Fix]** Fix notification text being truncated when large and now supports multi-line.

___

## Version 1.6.0

This version contains improvements and bug fixes.

### Android, iOS and UWP

#### Analytics

* **[Improvement]** Analytics now allows a maximum of 20 properties by event, each property key and value length can be up to 125 characters long.

### Android

#### Crashes

* **[Fix]** Fix capturing exceptions thrown directly from a .NET Thread.

#### Push

* **[Feature]** Configure default notification icon and color using meta-data.
* **[Fix]** Fixes the google.ttl field being considered custom data.
* **[Fix]** Fixes push notification not displayed if Google Play Services too old on the device (#630).
* **[Fix]** Don't crash the application when invalid notification color is pushed.

### iOS

#### Push

* **[Fix]** Fix parsing custom data for push to avoid crash in the application.

### UWP

#### Analytics

* **[Fix]** Fix session identifier when renewing session after background or after re-enabling SDK.
* **[Fix]** Avoid using tasks in background that can hang, use timers instead. Related to #517.
* **[Fix]** Fix storage exception catching so that they don't crash the app.
* **[Fix]** Fix some stability issues in the https sending logic.

___

## Version 1.5.0

This version contains new features.

### Android and iOS

#### AppCenterCrashes

* **[Feature #140]** Add support for handled exceptions with `Crashes.TrackError` API.

#### AppCenterDistribute

* **[Feature]** Add Session statistics for distribution group.
___

## Version 1.4.0

This version contains a new feature.

### Android and iOS

#### AppCenterDistribute

* **[Feature]** Use tester app to enable in-app updates if it's installed.
* **[Feature]** Add reporting of downloads for in-app update.
* **[Improvement]** Add distribution group to all logs that are sent.
___

## Version 1.3.0

This version has a **breaking change** as the SDK now requires iOS 9 or later. It also contains bug fixes and improvements.

### iOS

#### AppCenter

* **[Improvement]** Successful configuration of the SDK creates a success message in the console with log level INFO instead of ASSERT. Errors during configuration will still show up in the console with the log level ASSERT.

#### AppCenterCrashes

* **[Fix]** Fix an issue where crashes were not reported reliably in some cases when used in Xamarin apps or when apps would take a long time to launch.

### Android

#### AppCenter

* **[Fix]** Fix events association with crashes.
* **[Fix]** Fix network state detection.
* **[Fix]** Don't retry sending logs on HTTP error 429.
* **[Fix]** Some logs were not sent or discarded correctly on AppCenter enabled/disabled state changes.

#### AppCenterCrashes

* **[Improvement]** Increase attachment file size limit from 1.5MB to 7MB.

#### AppCenterPush

* **[Fix]** Fix a crash on Android 8.0 (exact version, this does not happen in 8.1) where having an adaptive icon (like launcher icon) would cause crash of the entire system U.I. on push. On Android 8.0 we replace the adaptive icon by a placeholder icon (1 black pixel) to avoid the crash, starting Android 8.1 the adaptive icon is displayed without fallback.
___

## Version 1.2.0

This version has a potentially **breaking change** (in push behavior) with bug fixes and improvements.

### UWP

#### AppCenter

* **[Fix]** The network loss detection code was missing some scenarios.
* **[Fix]** Disabling the SDK could send extra logs and could also discard logs supposed to be sent when re-enabled.

### iOS

#### AppCenter

* **[Fix]** Fix an issue that enables internal services even if App Center was disabled in previous sessions.
* **[Fix]** Fix an issue not to delete pending logs after maximum retries.

#### AppCenterCrashes

* **[Improvement]** Improve session tracking to get appropriate session information for crashes if an application also uses Analytics.

#### AppCenterPush

* **[Fix]** Fix "Missing Push Notification Entitlement" warning message after uploading an application to TestFlight and publishing to App Store.
* **[Improvement] (Breaking Change)** In previous versions, it was required to add code to `DidReceiveRemoteNotification` callback in your application delegate if you or 3rd party libraries already implemented this callback. This is no longer necessary.
  This is a **breaking change** for some use cases because it required modifications in your code. Not changing your implementation might cause push notifications to be received twice.
  * If you don't see any implementation of `DidReceiveRemoteNotification` callback in your application delegate, you don't need to do anything, there is no breaking change for you.
  * If you want to keep automatic forwarding disabled, you also don't need to do anything.
  * If your application delegate contains implementation of `DidReceiveRemoteNotification`, you need to remove the following code from your implementation of the callback. This is typically the case when you or your 3rd party libraries implement the callback.

    ```csharp
    public override void DidReceiveRemoteNotification(UIApplication application, NSDictionary userInfo, System.Action<UIBackgroundFetchResult> completionHandler)
    {
        var result = Push.DidReceiveRemoteNotification(userInfo);
        if (result)
        {
            completionHandler?.Invoke(UIBackgroundFetchResult.NewData);
        }
        else
        {
            completionHandler?.Invoke(UIBackgroundFetchResult.NoData);
        }
    }
    ```

___

## Version 1.1.0

### AppCenter

#### iOS

* **[Fix]** Fix a locale issue that doesn't properly report system locale if an application doesn't support current language.
* **[Improvement]** Change log level to make HTTP failures more visible, and add more logs.

#### UWP

* **[Fix #390]** The SDK is now compatible with sqlite-net-pcl 1.5.\* (currently in beta). It is still compatible with 1.3.\*. Note that it is still not compatible with versions in the 1.4.\* range due to a known issue.
* **[Fix]** Fix a locale issue that doesn't properly report system locale if an application doesn't support current language.
* **[Improvement]** HTTP warning and error logs now include more details: status code and full response body are now printed.

### AppCenterCrashes

#### Android

* **[Fix]** Fix a crash when sending an attachment larger than 1.4MB. The SDK is still unable to send large attachments (for Android) in this release but now it does not crash anymore. An error log is printed instead.
* **[Improvement]** Java exceptions caught from .NET runtime are now reported with .NET stack trace frames on the server. However the client side exception object returned by the SDK APIs will still be null and you have to look at `AndroidDetails` property to get the Java Throwable that caused this type of crash. (Java.Lang.Throwable is **not serializable** to a System.Exception object).

#### UWP

* **[Fix #544]** Remove message that crashes are not supported on UWP. Please note that none of the client side APIs besides `AppCenter.Start` are supported in UWP as crash reporting is provided by a system level component in Windows. Errors when registering the system level crash reporting are now also logged.

### AppCenterDistribute

#### Android and iOS

* **[Improvement]** Updated translations.
* **[Improvement]** Users with app versions that still use Mobile Center can directly upgrade to versions that use this version of App Center, without the need to reinstall.

### AppCenterPush

#### Android

* **[Improvement]** The Firebase SDK dependency is now optional. If Firebase SDK is not available at runtime, the push registers and generate notifications using only App Center SDK. The Firebase application and servers are still used whether the Firebase SDK is installed into the application or not.
  * The SDK is still compatible with Firebase packages. But if you don't use Firebase besides App Center, you can now remove these packages and refer to the [updated getting started instructions](https://docs.microsoft.com/en-us/appcenter/sdk/push/xamarin-android) to migrate the set up. Migrating the push set up in the application remains optional.

#### UWP

* **[Fix #518]** Fix a crash that could occur when starting Push.
___

## Version 1.0.1

This release fixes the NuGet packages that were released for [1.0.0](https://github.com/Microsoft/AppCenter-SDK-DotNet/releases/tag/1.0.0).

Please read the [1.0.0 release notes](https://github.com/Microsoft/AppCenter-SDK-DotNet/releases/tag/1.0.0) if you are not yet aware of this version.

Fixes from 1.0.1:

* Fix support for PCL that was broken in 1.0.0.
* Fix nuget licence acceptance.

___

## Version 1.0.0

### General Availability (GA) Announcement.

This version contains **breaking changes** due to the renaming from Mobile Center to App Center. In the unlikely event there was data on the device not sent prior to the update, that data will be discarded.

Please follow [the migration guide](https://docs.microsoft.com/en-us/appcenter/sdk/sdk-migration/xamarin) to update from an earlier version of Mobile Center SDK.

### AppCenter

#### iOS

* **[Fix]** Don't send startService log while SDK is disabled.

### AppCenterDistribute

#### iOS

* **[Fix]** Fix a bug where unrecoverable HTTP error wouldn't popup the reinstall app dialog after an app restart.
* **[Improvement]** Adding missing translations.
* **[Known bug]** Checking for last updates will fail if the app was updating from a Mobile Center app. A pop up will show next time the app is restarted to ask for reinstallation.

#### Android

* **[Fix]** The view release notes button was not correctly hidden when no release notes were available.
* **[Fix]** Added missing translations for failed to enable in-app update dialog title and buttons. The main message, however, is not localized yet as it's extracted from a REST API text response.
* **[Known issue]** When updating an application that uses Mobile Center SDK using the in-app update dialog to an application version that uses AppCenter SDK version, the browser will be re-opened and will fail. User will need to re-install the application from the App Center portal.

___

## Version 1.17.1

This release include 2 bug fixes.

### MobileCenterAnalytics

* **[Fix]** Prevents a possible crash on UWP startup #484

### MobileCenterPush

* **[Improvement]** Updates dependencies on Xamarin Firebase Push to avoid most package conflicts when updating Mobile Center Push on Android.
* **[Fix]** Fixes a crash when receiving a silent push in background with no message on iOS.

### Native SDK Updates

* Fixes from [iOS SDK version 0.14.1](https://github.com/Microsoft/mobile-center-sdk-ios/releases/tag/0.14.1).

___

## Version 1.17.0

This version contains bug fixes, a new API, and improvements.

### MobileCenter

* **[Improvement]** The NuGet packages now support .NET standard without adding extra project configuration, and the nugets remain compatible with PCL projects.
* **[Feature]** Added a `MobileCenter.SdkVersion` property to get the current version of Mobile Center SDK programmatically.

### MobileCenterPush

* **[Breaking change]** To support Android 8.0 applications that use `targetSdkVersion="26"` we updated the Firebase SDK dependency which forces applications to use Android 8.0 Mono frameworks.
    * You need to have Xamarin tools up to date, select Android 8.0 as a target framework and then make sure that in `package.config` the attribute for every package looks like this: `targetFramework="monoandroid80"` and update as necessary. It's highly recommended to close Studio and clean every obj/bin folder manually in case of any build error after the update.
    * If you get a conflict with `Xamarin.GooglePlayServices.Basement` package, you need to update this one to **prerelease** version `60.1142.0-beta1` (ore more recent) and retry updating the Mobile Center Push package.
* **[Fix]** Fixed: prior to this release, calling `Push.SetEnabledAsync(true)` during startup could prevent UWP push registration. Please note that any call to `SetEnabledAsync(true)` for any module is useless if you have not called it with `false` before, modules are enabled by default once included in the `Start` call.

### Native SDK updates

Embed changes from the underlying native SDK releases:

* https://github.com/Microsoft/mobile-center-sdk-android/releases/tag/0.13.0
* https://github.com/Microsoft/mobile-center-sdk-ios/releases/tag/0.14.0

___

## Version 0.16.0

This release includes bug fixes for iOS from the native SDK: https://github.com/Microsoft/mobile-center-sdk-ios/releases/tag/0.13.0.

Please note that this release does not yet provide the .NET equivalent of the sdkVersion method introduced by the iOS change.

___

## Version 0.15.2

This release includes bug fixes for iOS from the native SDK: https://github.com/Microsoft/mobile-center-sdk-ios/releases/tag/0.12.3.

This release thus fixes the Mobile Center build incompatibility introduced in version [0.15.1](https://github.com/Microsoft/mobile-center-sdk-dotnet/releases/tag/0.15.1).

___

## Version 0.15.1

This release includes bug fixes for iOS from the native SDK: https://github.com/Microsoft/mobile-center-sdk-ios/releases/tag/0.12.2

**Known issue** This release causes build error in Mobile Center or in environments where XCode version is lower than 9. As a consequence, this release was unlisted from nuget.org.

___

## Version 0.15.0

When you update to this release, there will be potential data loss if an application installed with previous versions of MobileCenter SDK on devices that has pending logs which are not sent to server yet at the time of the application is being updated.

* New feature: in app updates can now be received from public distribution groups.
* Fix a bug in UWP SDK where an `InvalidOperationException` could sometimes be thrown at launch time.
* This release also includes changes from underlying Android and iOS SDKs of the following versions:
  * Android
    * https://github.com/Microsoft/mobile-center-sdk-android/releases/tag/0.12.0
  * iOS
    * https://github.com/Microsoft/mobile-center-sdk-ios/releases/tag/0.12.1
    * https://github.com/Microsoft/mobile-center-sdk-ios/releases/tag/0.12.0

___

## Version 0.14.2

* Fix a bug in UWP where some crashes, especially those caused by `AggregateException`, would not be sent properly.
* Fix a bug in UWP where Analytics would always emit a warning on startup.
* Reword some logs that used to look like errors when starting Mobile Center in multiple places to appear less harmful. (Issue #396).
* Improve the reliability of device screen size retrieval in UWP (used in device properties sent with logs).
* Fix a bug where getting the screen size in UWP could, in some cases, cause a crash. (Issue #398).
* Change the behavior of Analytics events so that now, when event fields exceed their capacity, they are truncated instead of dropped altogether. Limits:
  * Event name maximum length: 256 characters.
  * Maximum number of event properties: 5.
  * Event property key maximum length: 64 characters.
  * Event property value maximum length: 64 characters.
* This release also includes changes from underlying Android and iOS SDKs of the following versions:
  * Android
    * https://github.com/Microsoft/mobile-center-sdk-android/releases/tag/0.11.2
  * iOS
    * https://github.com/Microsoft/mobile-center-sdk-ios/releases/tag/0.11.2
    * https://github.com/Microsoft/mobile-center-sdk-ios/releases/tag/0.11.1

___

## Version 0.14.0

### Async APIs and Android strict mode

This release focuses on fixing Android strict mode issues (including Android 8 ones).

Since strict mode checks if you spend time reading storage on U.I. thread we had to make the following APIs asynchronous and is thus a breaking change on PCL and all targets:

* `{AnyClass}.Enabled` is now split into `GetEnabledAsync()` and `SetEnabledAsync(bool)`
* `MobileCenter.InstallId` is now `MobileCenter.GetInstallIdAsync()`
* `Crashes.HasCrashedInLastSession` is now `Crashes.HasCrashedInLastSessionAsync()`

### Other changes

#### Android

* The Android crash reporting is now compatible with debugger attached (you need to continue execution after exception), Hockey App for Xamarin (only if initialized like getting started suggests) and `XA_BROKEN_EXCEPTION_TRANSITIONS=true`. This change is for **Android only**.
* This release also includes changes from underlying Android SDK of the following versions:
  * https://github.com/Microsoft/mobile-center-sdk-android/releases/tag/0.11.0
  * https://github.com/Microsoft/mobile-center-sdk-android/releases/tag/0.11.1

#### iOS

* Fix null argument exceptions when passing null arguments to most APIs, an error is logged instead now.
* This release also includes changes from underlying iOS SDK:
  * https://github.com/Microsoft/mobile-center-sdk-ios/releases/tag/0.11.0
* Xamarin SDK calls `MSCrashes.disableMachExceptionHandler()` automatically as it's not suitable in Xamarin environment.

#### UWP

* Fix various race conditions.
* Fix screen size reporting.
* Fix a bug where U.I. could be blocked for a long time when initializing push.
* Fix null reference exception when calling `SetCustomProperties` before `Start`, now logs an error.

___

## Version 0.13.1

Roll back version [0.13.0](https://github.com/Microsoft/mobile-center-sdk-dotnet/releases/tag/0.13.0) .NET standard change for portable libraries due to reported issues with it.

Also, version 0.13.0 was adding unnecessary packages to Android and iOS projects, and they can now be safely uninstalled. You will find a list of these unnecessary packages in the [packages.config](https://github.com/Microsoft/mobile-center-sdk-dotnet/releases/download/0.13.1/packages.config) file attached to this release.

If your portable project is using .NET standard, you can still use our nugets by adding this to your **project.json** file:

```
  "frameworks": {
    "netstandard1.{DotNETStandardVersion}": {
        "imports": "portable-net45+win8+wpa81"
    }
}
```

After the dependencies section.

Example:

```
{
  "dependencies": {
    "Microsoft.Azure.Mobile.Analytics": "0.13.1",
    "Microsoft.Azure.Mobile.Crashes": "0.13.1",
    "Microsoft.Azure.Mobile.Distribute": "0.13.1",
    "Microsoft.Azure.Mobile.Push": "0.13.1",
    "NETStandard.Library": "1.6.1"
  },
  "frameworks": {
    "netstandard1.0": {
        "imports": "portable-net45+win8+wpa81"
    }
  }
}
```

___

## Version 0.13.0

* Add `MobileCenter.SetCustomProperties` method to enable audience segmentation with push notifications.
* Fix a bug in UWP where logs would be mishandled on a network outage.
* Move initialization of `HttpClient` class to a background thread in UWP.
* Fix bug in UWP SDK where starting `MobileCenter` from another place than `OnLaunched` sometimes crashed. Also removed the error log, now you can init the SDK in constructor too if you prefer.
* Fix various race conditions in UWP.
* Fix a bug where 2 sessions could be reported at once when resuming from background (UWP and Android).
* Add a new Android specific method `Push.CheckLaunchedFromNotification` to use in `OnNewIntent` if `launchMode` of the activity is not standard to fix push listener when clicking on background push and recycling the activity.
* Switch from PCL to .NET Standard for the portable projects.
* This release also includes changes from underlying Android and iOS SDKs of the following versions:
  * Android
    * https://github.com/Microsoft/mobile-center-sdk-android/releases/tag/0.10.0
  * iOS
    * https://github.com/Microsoft/mobile-center-sdk-ios/releases/tag/0.10.1
    * https://github.com/Microsoft/mobile-center-sdk-ios/releases/tag/0.10.0

___

## Version 0.12.0

* Add capability to attach data to crash reports (via the `Crashes.GetErrorAttachments` callback) on Xamarin.
* Fix a crash on old Xamarin.Android versions where mono version could not be read (`System.IndexOutOfRangeException` when calling `Start`).
* Fix push event on UWP.
* Fix `Push.Enabled` property behavior on UWP.
* This release also includes changes from underlying Android and iOS SDKs of the following versions:
  * Android
    * https://github.com/Microsoft/mobile-center-sdk-android/releases/tag/0.9.0
  * iOS
    * https://github.com/Microsoft/mobile-center-sdk-ios/releases/tag/0.9.0

___

## Version 0.11.1

* Add Push service to Xamarin.Android and Xamarin.iOS to enable sending push notifications to your apps from the Mobile Center portal.
* This release also includes changes from underlying Android and iOS SDKs of the following versions:
  * Android
    * https://github.com/Microsoft/mobile-center-sdk-android/releases/tag/0.8.0
    * https://github.com/Microsoft/mobile-center-sdk-android/releases/tag/0.8.1
  * iOS
    * https://github.com/Microsoft/mobile-center-sdk-ios/releases/tag/0.8.0
    * https://github.com/Microsoft/mobile-center-sdk-ios/releases/tag/0.8.1

___

## Version 0.10.0

* Add Push service to UWP to enable sending push notifications to your apps from the Mobile Center portal.

___

## Version 0.9.2

* Fix `ReleaseAvailable` callback in iOS to be able to customize the update dialog on iOS too.
* Fix showing default update dialog after resetting `ReleaseAvailable` callback to `null`.

___

## Version 0.9.1

This version contains bug fixes, improvements and new features.

### MobileCenter

* Add method to manually set the country code in UWP apps: `MobileCenter.SetCountryCode(string)`.
* Fix a bug where installing the Mobile Center NuGet packages can cause Mobile Center types not to appear in Intellisense.
* Use another sqlite package dependency in UWP.
* Add xamarin runtime version in logs sent to backend.

### Analytics

Events have some validation and you will see the following in logs:

* An error if the event name is null, empty or longer than 256 characters (event is not sent in that case).
* A warning for invalid event properties (the event is sent but without invalid properties):
  * More than 5 properties per event (in that case we send only 5 of them and log warnings).
  * Property key null, empty or longer than 64 characters.
  * Property value null or longer than 64 characters.

### Crashes

* Improve reporting and rendering of some Java `ClassNotFoundException` caught from .NET side.

### Distribute

* New distribute callback to provide an ability of in-app update customization (on Android only for now).
* New default update dialog with release notes view.

### Android

Include changes from https://github.com/Microsoft/mobile-center-sdk-android/releases/0.7.0

### iOS

Include changes from https://github.com/Microsoft/mobile-center-sdk-ios/releases/0.7.0

___

## Version 0.8.1

This release introduces the following features:

* Distribute module for Xamarin.iOS and Xamarin.Android.
* Analytics module support for UWP platform.

Those 2 features works also when using PCL and Xamarin Forms.

This release also includes changes from underlying Android and iOS SDKs of the following versions:

* Android
    * https://github.com/Microsoft/mobile-center-sdk-android/releases/tag/0.6.0
    * https://github.com/Microsoft/mobile-center-sdk-android/releases/tag/0.6.1
* iOS
    * https://github.com/Microsoft/mobile-center-sdk-ios/releases/tag/0.6.0
    * https://github.com/Microsoft/mobile-center-sdk-ios/releases/tag/0.6.1

___

## Version 0.7.0

- **[Feature]** Can now initialize the SDK with multiple app secrets for different platforms (Android vs iOS) in the same `Start` call. No need to split the initialization between `Start` and `Configure` anymore. Just use `MobileCenter.Start("android={androidAppSecret};ios={iosAppSecret}", typeof(Analytics), typeof(Crashes));` for example.
- **[Bug fix]** Fix a crash happening on iOS devices before iOS 10 when reading `MobileCenter.InstallId` property.
- **[Misc]** Renamed `SetServerUrl` to `SetLogUrl`.
- **[iOS]** Include underlying iOS SDK changes from the following releases:
  * https://github.com/Microsoft/mobile-center-sdk-ios/releases/tag/0.5.1
  * https://github.com/Microsoft/mobile-center-sdk-ios/releases/tag/0.5.0
  * https://github.com/Microsoft/mobile-center-sdk-ios/releases/tag/0.4.2
  * https://github.com/Microsoft/mobile-center-sdk-ios/releases/tag/0.4.1
  * https://github.com/Microsoft/mobile-center-sdk-ios/releases/tag/0.4.0

___

## Version 0.6.0

**[Breaking changes]**
- Remove Crashes APIs related to attachments as it's not supported by backend yet.
- `Crashes.LastSessionCrashReport` property becomes `Crashes.GetLastSessionCrashReportAsync()` async method.

[Bug fixes]
- Include underlying iOS SDK changes from the following releases:
  - https://github.com/Microsoft/mobile-center-sdk-ios/releases/tag/0.3.7
  - https://github.com/Microsoft/mobile-center-sdk-ios/releases/tag/0.3.6
  - https://github.com/Microsoft/mobile-center-sdk-ios/releases/tag/0.3.5
- Include underlying Android SDK changes from the following releases:
  - https://github.com/Microsoft/mobile-center-sdk-android/releases/tag/0.5.0
  - https://github.com/Microsoft/mobile-center-sdk-android/releases/tag/0.4.0

___

## Version 0.5.0

- Add `MobileCenter.IsConfigured` boolean property to check for SDK already configured.
- Fix issue where sdk could cause ios apps to crash while trying to handle SIGFPE signal.
- Fix a crash when starting analytics when using "Link All" in iOS.
- Fix a crash when calling `MobileCenter.InstallId` before `MobileCenter.Configure` in Android, now returns `null` like in the underlying SDK in that case.
- Include underlying [iOS SDK v0.3.4](https://github.com/Microsoft/mobile-center-sdk-ios/releases/tag/0.3.4).
- Include underlying [Android SDK v0.3.3](https://github.com/Microsoft/mobile-center-sdk-android/releases/tag/0.3.3).

___

## Version 0.4.1

- Fix crash that would occur when disabling Crashes service in iOS.
- Fix crash when trying to include a null ErrorAttachment in an ErrorReport.

___

## Version 0.4.0

- Log a debug message instead of crashing in UWP projects.
- Add raw stack trace information for backend processing.
- Include underlying [iOS SDK v0.3.3](https://github.com/Microsoft/mobile-center-sdk-ios/releases/tag/0.3.3).
- Include underlying [Android SDK v0.3.2](https://github.com/Microsoft/mobile-center-sdk-android/releases/tag/0.3.2).

___

## Version 0.3.0

- Fix linker issues in iOS, you can also remove safely the `MobileCenterFrameworks` folder from iOS projects.
- Fix issue where sdk could cause ios apps to crash while trying to handle signals.
- Fix a crash when starting MobileCenter on Android when "link all assemblies" is used.
- Add advanced crash features that were already available since version 0.2.0 of Android and iOS SDKs:
  - Last session crash report.
  - Crash callbacks.

___

## Version 0.2.1

Rename Android bindings so that they don't get used by mistake instead of the PCL.

___

## Version 0.2.0

- Rename `Initialize` to `Configure`.
- Embed Android and iOS SDKs version 0.3.0.

___

## Version 0.1.0

- First release of Xamarin SDK.
- Using version 0.2.0 of underlying Android and iOS SDKs.
- Last session error report details and crash callbacks are not yet available.
