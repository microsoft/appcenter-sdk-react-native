# Sonoma SDK for Xamarin

Work in progress, only Android and Portable projects right now.
Still lacking build scripts and CI etc...
The AAR files for Android are ignored from GIT, the idea is to script the download of them and avoid committing any binary.

You can download the Android AAR files at https://github.com/Microsoft/Sonoma-SDK-Android/releases/tag/0.1.1

* Copy `core-release.aar` to `Microsoft.Sonoma.Xamarin.Core.Android.Bindings\Jars`
* Copy `analytics-release.aar` to `Microsoft.Sonoma.Xamarin.Analytics.Android.Bindings\Jars`
* Copy `crashes-release.aar` to `Microsoft.Sonoma.Xamarin.Crashes.Android.Bindings\Jars`

For the documentation generator project, please install the `SandCastle` tool https://github.com/EWSoftware/SHFB/releases/tag/v2016.9.17.0.
