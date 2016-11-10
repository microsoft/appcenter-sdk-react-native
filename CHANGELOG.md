# MobileCenter-SDK-React-Native Change Log

___

## Version 0.1.0

Initial release of Mobile Center for React Native.

### RNMobileCenter

* **[Feature]** Initial release!
* **[Feature]** Conditional initialization of Mobile Center
* **[Feature]** Can specify App Secret either via configuration file (`MobileCenter-Config.plist` in iOS or `mobilecenter-config.json` in Android) or in code

### RNAnalytics

* **[Feature]** Initial release!
* **[Feature]** Can be configured to start a user session immediately, or to wait until javascript calls `Analytics.setEnabled(true);`
* **[Feature]** Provides access to `Analytics.trackEvent("eventName", {event: "properties", which: "must be strings"});` in javascript
* **[Misc]** Once the underlying SDK support is in, minor changes will allow access to `Analytics.trackPage("pageName", {event: "properties"});` as well.

### RNCrashes

* **[Feature]** Initial release!
* **[Feature]** Captures application crashes and submits them to Mobile Center
* **[Feature]** Supports inspecting crashes in JS before submitting them, and allows text data to be attached to crashes
