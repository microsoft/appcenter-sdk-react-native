using System;
using Microsoft.ReactNative;
using Microsoft.ReactNative.Managed;

using Microsoft.AppCenter;
using Microsoft.AppCenter.Crashes;
using Microsoft.AppCenter.ReactNative.Shared;
using Windows.Media.Ocr;
using System.Linq;
using Windows.Devices.PointOfService;
using System.Collections.Generic;
using Windows.Data.Json;
using System.Runtime.CompilerServices;


namespace Microsoft.AppCenter.Crashes.ReactNative
{
	sealed public class RedBoxHandler : IRedBoxHandler
	{
		private IRedBoxHandler defaultHandler;
		public RedBoxHandler(ReactNativeHost host) {
			defaultHandler = RedBoxHelper.CreateDefaultHandler(host);
		}

		// Does not work in release mode
		public void ShowNewError(IRedBoxErrorInfo info, RedBoxErrorType type) {
			int index = 0;

			var callstack = new JsonArray();
			foreach (var a in info.Callstack) {
				var toAdd = new JsonObject();
				toAdd.SetNamedValue("index", JsonValue.CreateNumberValue(index++));
				toAdd.SetNamedValue("method", JsonValue.CreateStringValue(a.Method));
				toAdd.SetNamedValue("file", JsonValue.CreateStringValue(a.File));
				toAdd.SetNamedValue("line", JsonValue.CreateNumberValue(a.Line));
				toAdd.SetNamedValue("column", JsonValue.CreateNumberValue(a.Column));
				callstack.Add(toAdd);
			}

			var attachments = new ErrorAttachmentLog[]
			{
				ErrorAttachmentLog.AttachmentWithText(callstack.ToString(), "callstack.json")
			};

			switch (type) {
				case RedBoxErrorType.JavaScriptFatal:
				Crashes.TrackError(new Exception(info.Message), attachments: attachments);
				break;
				case RedBoxErrorType.JavaScriptSoft:
				Crashes.TrackError(new Exception(info.Message), attachments: attachments);
				break;
				case RedBoxErrorType.Native:
				Crashes.TrackError(new Exception(info.Message), attachments: attachments);
				break;
			}

			defaultHandler.ShowNewError(info, type);
		}

		public void UpdateError(IRedBoxErrorInfo info) {
			defaultHandler.UpdateError(info);
		}

		public void DismissRedBox() {
			defaultHandler.DismissRedBox();
		}

		public bool IsDevSupportEnabled => defaultHandler.IsDevSupportEnabled;
	}

	[ReactModule]
	class AppCenterReactNativeCrashes
	{

		public AppCenterReactNativeCrashes() {
			StartCrashes();
		}

		private async void StartCrashes()
        {
			await await AppCenterReactNativeShared.ConfigureAppCenter();
			AppCenter.Start(typeof(Crashes));

			Crashes.SendingErrorReport += Crashes_SendingErrorReport;
			Crashes.SentErrorReport += Crashes_SentErrorReport;
			Crashes.FailedToSendErrorReport += Crashes_FailedToSendErrorReport;
		}

		[ReactMethod("setEnabled")]
		public async void SetEnabled(bool enabled, ReactPromise<JSValue> promise) {
			await Crashes.SetEnabledAsync(enabled);
			promise.Resolve(JSValue.Null);
		}

		[ReactMethod("isEnabled")]
		public async void IsEnabled(ReactPromise<bool> promise) {
			promise.Resolve(await Crashes.IsEnabledAsync());
		}

		[ReactMethod("hasCrashedInLastSession")]
		public async void HasCrashedInLastSession(ReactPromise<bool> promise) {
			promise.Resolve(await Crashes.HasCrashedInLastSessionAsync());
		}

		[ReactMethod("hasReceivedMemoryWarningInLastSession")]
		public void HasReceivedMemoryWarningInLastSession(ReactPromise<bool> promise) {
			promise.Resolve(Crashes.HasReceivedMemoryWarningInLastSessionAsync().Result);
		}

		[ReactMethod("generateTestCrash")]
		public void GenerateTestCrash(ReactPromise<JSValue> promise) {
			Crashes.GenerateTestCrash();
			promise.Resolve(JSValue.Null);
		}

		[ReactMethod("notifyWithUserConfirmation")]
		public void NotifyWithUserConfirmation(UserConfirmation userConfirmation) {
			Crashes.NotifyUserConfirmation(userConfirmation);
		}

		[ReactMethod("lastSessionCrashReport")]
		public async void LastSessionCrashReport(ReactPromise<ErrorReport> promise) {
			promise.Resolve(await Crashes.GetLastSessionCrashReportAsync());
		}

		// ***********************************************************************************************************************
		// Required for Listener, but do rely on WrapperSdkExceptionManager (Android) or MSWrapperCrashesHelper (iOS).
		// Cannot find the implementation of how the do it, and the UWP implementation is not concerned with them for some reason.
		[ReactMethod("getUnprocessedCrashReports")]
		public async void GetUnprocessedCrashReports(ReactPromise<ErrorReport> promise) {
			promise.Resolve(await Crashes.GetLastSessionCrashReportAsync());
		}

		[ReactMethod("sendCrashReportsOrAwaitUserConfirmationForFilteredIds")]
		public void SendCrashReportsOrAwaitUserConfirmationForFilteredIds(JSValueArray filteredIDs, ReactPromise<bool> promise) {
			promise.Resolve(true);
		}
		// **********************************************************************************************************************

		bool ShouldProcess(ErrorReport report) {
			AppCenterLog.Info("words", "Determining whether to process error report");
			return true;
		}
		private void Crashes_SendingErrorReport(object sender, SendingErrorReportEventArgs e) {
			onBeforeSending?.Invoke(e.Report);
		}

		private void Crashes_SentErrorReport(object sender, SentErrorReportEventArgs e) {
			onSendingSucceeded?.Invoke(e.Report);
		}

		private void Crashes_FailedToSendErrorReport(object sender, FailedToSendErrorReportEventArgs e) {
			onSendingFailed?.Invoke(e.Report);
		}

		[ReactEvent]
		public Action<ErrorReport> onBeforeSending { get; set; }

		[ReactEvent]
		public Action<ErrorReport> onSendingSucceeded { get; set; }

		[ReactEvent]
		public Action<ErrorReport> onSendingFailed { get; set; }

		[ReactEvent]
		public Action<ErrorReport> getErrorAttachments { get; set; }

		[ReactEvent]
		public Action<ErrorReport> shouldAwaitUserConfirmation { get; set; }

	}
	
	static class ErrorReportWriter
	{
		public static void WriteValue(this IJSValueWriter writer, ErrorReport errorReport) {
			if (errorReport != null) {
				writer.WriteObjectBegin();
				writer.WriteObjectProperty("id", errorReport.Id);
				//writer.WriteObjectProperty("threadName", errorReport.ThreadName); Should list thread, but it is not a property of ErrorReport. A UWP issue
				writer.WriteObjectProperty("appErrorTime", errorReport.AppErrorTime.ToUnixTimeMilliseconds());

				writer.WriteObjectProperty("appStartTime", errorReport.AppStartTime.ToUnixTimeMilliseconds());

				var stackTrace = errorReport.StackTrace;
				if (stackTrace != null) {
					writer.WriteObjectProperty("exception", stackTrace);
				}

				/* Convert device info. */
				var deviceInfo = errorReport.Device;
				if (deviceInfo != null) {
					writer.WriteObjectProperty("device", deviceInfo);
				}
				writer.WriteObjectEnd();
			}
			else {
				writer.WriteNull();
			}
		}

		public static void WriteValue(this IJSValueWriter writer, Microsoft.AppCenter.Device deviceInfo) {
			writer.WriteObjectBegin();
			writer.WriteObjectProperty("appBuild", deviceInfo.AppBuild);
			writer.WriteObjectProperty("appNamespace", deviceInfo.AppNamespace);
			writer.WriteObjectProperty("appVersion", deviceInfo.AppVersion);
			writer.WriteObjectProperty("carrierCountry", deviceInfo.CarrierCountry ?? "");
			writer.WriteObjectProperty("carrierName", deviceInfo.CarrierName ?? "");
			writer.WriteObjectProperty("locale", deviceInfo.Locale ?? "");
			writer.WriteObjectProperty("model", deviceInfo.Model ?? "");
			writer.WriteObjectProperty("oemName", deviceInfo.OemName ?? "");
			writer.WriteObjectProperty("osAPILevel", deviceInfo.OsApiLevel);
			writer.WriteObjectProperty("osBuild", deviceInfo.OsBuild ?? "");
			writer.WriteObjectProperty("osName", deviceInfo.OsName ?? "");
			writer.WriteObjectProperty("osVersion", deviceInfo.OsVersion ?? "");
			writer.WriteObjectProperty("screenSize", deviceInfo.ScreenSize ?? "");
			writer.WriteObjectProperty("sdkName", deviceInfo.SdkName ?? "");
			writer.WriteObjectProperty("sdkVersion", deviceInfo.SdkVersion ?? "");
			writer.WriteObjectProperty("timeZoneOffset", deviceInfo.TimeZoneOffset);
			writer.WriteObjectEnd();
		}
	}
}
