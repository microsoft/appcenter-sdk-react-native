[![Build Status](https://www.bitrise.io/app/2f5448791ead7158.svg?token=OXmRpllvCk374SWQCVevkA&branch=develop)](https://www.bitrise.io/app/2f5448791ead7158)

# Mobile Center SDK for Xamarin

## Introduction

The Mobile Center Xamarin SDK lets you add our services to your iOS and Android applications.

The SDK supports the following services:

1. **Analytics**: Mobile Center Analytics helps you understand user behavior and customer engagement to improve your app. The SDK automatically captures session count and device properties like model, OS Version etc. You can define your own custom events to measure things that matter to your business. All the information captured is available in the Mobile Center portal for you to analyze the data.

2. **Crashes**: The Mobile Center SDK will automatically generate a crash log every time your app crashes. The log is first written to the device's storage and when the user starts the app again, the crash report will be forwarded to Mobile Center. Collecting crashes works for both beta and live apps, i.e. those submitted to Google Play or other app stores. Crash logs contain viable information for you to help resolve the issue. The SDK gives you a lot of flexibility how to handle a crash log. As a developer you can collect and add additional information to the report if you like.

This document contains the following sections:

1. [Prerequisites](#1-prerequisites)
2. [Supported Platforms](#2-supported-platforms)
3. [Setup](#3-setup)
4. [Start the SDK](#3-start-the-sdk)
5. [Analytics APIs](#4-analytics-apis)
6. [Crashes APIs](#5-crashes-apis)
7. [Advanced APIs](#6-advanced-apis)

Let's get started with setting up Mobile Center Xamarin SDK in your app to use these services:

## 1. Prerequisites

Before you begin, please make sure that the following prerequisites are met:

* A project setup in Xamarin Studio or Xamarin for Visual Studio.
* You are not using other crash services on the same mobile app for iOS platform.

## 2. Supported Platforms 

We support the following platforms:

* Xamarin.iOS
* Xamarin.Android
* Xamarin.Forms (iOS and Android)

## 3. Setup

Mobile Center SDK is designed with a modular approach – a developer only needs to integrate the modules of the services that they're interested in. If you'd like to get started with just Analytics or Crashes, include their packages in your app. For each iOS, Android and Forms project, add the 'Microsoft Mobile Center Analytics' and 'Microsoft Mobile Center Crashes' packages.

## For Xamarin Studio ##

**For Xamarin.iOS and Xamarin.Android:**  
* Navigate to the Project -> 'Add NuGet Packages...'
* Search and select "Mobile Center Analytics" and "Mobile Center Crashes". Then Click 'Add Packages'  

**For Xamarin.Forms**  
Multiplatform Xamarin.Forms app has three projects in your solution - portal or shared class library, project.Droid, project.iOS . You need to add NuGet packages to each of these projects.

* Navigate to the Project -> 'Add NuGet Packages...'
* Search and select "Mobile Center Analytics" and "Mobile Center Crashes". Then Click 'Add Packages'

## For Xamarin for Visual Studio ##

* Navigate Project -> Manage NuGet Packages...
* Search and select "Mobile Center Analytics" and "Mobile Center Crashes". Then Click 'Add Packages'

Now that you've integrated the SDK in your application, it's time to start the SDK and make use of Mobile Center services.

## 4. Start the SDK

To start the SDK in your app, follow these steps:

1. **Add using statements:**  Add the appropriate namespaces befor eyou get started with using our APIs.

    **Xamarin.iOS** -  Open AppDelegate.cs file and add the lines below the existing using statements

    **Xamarin.Android** - Open MainActivity.cs file and add the lines below the existing using statements

    **Xamarin.Forms** - Open App.xaml.cs file in your shared project and add these using statements 

    ```Xamarin
    using Microsoft.Azure.Mobile;
    using Microsoft.Azure.Mobile.Analytics;
    using Microsoft.Azure.Mobile.Crashes;
    ```

2. **Start the SDK:**  Mobile Center provides developers with two modules to get started – Analytics and Crashes. In order to use these modules, you need to opt in for the module(s) that you'd like, meaning by default no module is started and you will have to explicitly call each of them when starting the SDK.   

    **Xamarin.iOS**

    Open AppDelegate.cs file and add the `Start` API in FinishedLaunching() method

    ```csharp
    MobileCenter.Start("{Your App Secret}", typeof(Analytics), typeof(Crashes));
    ```

    **Xamarin.Android**
    
    Open MainActivity.cs file and add the `Start` API in OnCreate() method

    ```csharp
    MobileCenter.Start("{Your App Secret}", typeof(Analytics), typeof(Crashes));
    ```

    **Xamarin.Forms**
    
    Start SDK call is split into two methods for Xamarin.Forms. That's because you need two different AppSecrets - one for iOS and other for your Android app. Open App.xaml.cs file in your shared project and add the API below in the `App()` constructor.

    ```csharp
    MobileCenter.Start(typeof(Analytics), typeof(Crashes));
    ```
     
    In the iOS project of the Forms app, open AppDelegate.cs and add the API in `FinishedLaunching()` method  
    ```csharp
    MobileCenter.Initialize("{Your iOS App Secret}");
    ```

    In the Droid project of the Forms app, open MainActivity.cs and add the API in `OnCreate()` method  
    ```csharp
    MobileCenter.Initialize("{Your Android App Secret}");
    ```

    You can also copy paste the code from the Overview page on Mobile Center portal once your app is selected. It already includes the App Secret so that all the data collected by the SDK corresponds to your application. Make sure to replace {Your App Secret} text with the actual value for your application.
    
    The example above shows how to use the `Start()` method and include both the Analytics and Crashes module. If you wish not to use Analytics, remove the parameter from the method call above. Note that, unless you explicitly specify each module as parameters in the start method, you can't use that Mobile Center service. Also, the `Start()` API can be used only once in the lifecycle of your app – all other calls will log a warning to the console and only the modules included in the first call will be available.

## 5. Analytics APIs

* **Track Session, Device Properties:**  Once the Analytics module is included in your app and the SDK is started, it will automatically track sessions, device properties like OS Version, model, manufacturer etc. and you don’t need to add any additional code.
    Look at the section above on how to [Start the SDK](#4-start-the-sdk) if you haven't started it yet.

* **Custom Events:** You can track your own custom events with specific properties to know what's happening in your app, understand user actions, and see the aggregates in the Mobile Center portal. Once you have started the SDK, use the `TrackEvent()` method to track your events with properties.

    ```csharp
    Analytics.TrackEvent("Video clicked", new Dictionary<string, string> { { "Category", "Music" }, { "FileName", "favorite.avi"}});
    ```

* **Enable or disable Analytics:**  You can change the enabled state of the Analytics module at runtime by calling the `Analytics.Enabled` property. If you disable it, the SDK will not collect any more analytics information for the app. To re-enable it, set property value as `true`.

    ```csharp
    Analytics.Enabled = true;
    ```
    You can also check if the module is enabled or not using:

    ```csharp
    bool isEnabled = Analytics.Enabled;
    ```

## 6. Crashes APIs

Once you set up and start the Mobile Center SDK to use the Crashes module in your application, the SDK will automatically start logging any crashes in the device's local storage. When the user opens the application again, all pending crash logs will automatically be forwarded to Mobile Center and you can analyze the crash along with the stack trace on the Mobile Center portal. Refer to the section to [Start the SDK](#4-start-the-sdk) if you haven't done so already.

* **Generate a test crash:** The SDK provides you with a static API to generate a test crash for easy testing of the SDK:

    ```csharp
    Crashes.GenerateTestCrash();
    ```

    Note that this API checks for debug vs release configurations. So you can only use it when debuging as it won't work for release apps.

* **Did the app crash in last session:** At any time after starting the SDK, you can check if the app crashed in the previous session:

    ```csharp
    bool didAppCrash = Crashes.HasCrashedInLastSession;
    ```

* **Enable or disable the Crashes module:**  You can disable and opt out of using the Crashes module by setting the `Enabled` property to `false` and the SDK will collect no crashes for your app. Use the same API to re-enable it by setting property as `true`.

    ```csharp
    Crashes.Enabled = true;
    ```

    You can also check whether the module is enabled or not using:

    ```csharp
    bool isEnabled = Crashes.Enabled;
    ```

## 7. Advanced APIs

* **Debugging**: You can control the amount of log messages that show up from the SDK. Use the API below to enable additional logging while debugging. By default, it is set it to `ASSERT` for non-debuggable applications and `WARN` for debuggable applications.

    ```csharp
        MobileCenter.LogLevel = LogLevel.Verbose;
    ```

* **Get Install Identifier**: The Mobile Center SDK creates a UUID for each device once the app is installed. This identifier remains the same for a device when the app is updated and a new one is generated only when the app is re-installed. The following API is useful for debugging purposes.

    ```csharp
        System.Guid installId = MobileCenter.InstallId;
    ```

* **Enable/Disable Mobile Center SDK:** If you want the Mobile Center SDK to be disabled completely, use the `Enabled` property. When disabled, the SDK will not forward any information to MobileCenter.

    ```csharp
        MobileCenter.Enabled = false;
    ```
