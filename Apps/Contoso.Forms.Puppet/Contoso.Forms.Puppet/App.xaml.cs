// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Auth;
using Microsoft.AppCenter.Crashes;
using Microsoft.AppCenter.Data;
using Microsoft.AppCenter.Distribute;
using Microsoft.AppCenter.Push;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using XamarinDevice = Xamarin.Forms.Device;

namespace Contoso.Forms.Puppet
{
    public interface IClearCrashClick
    {
        void ClearCrashButton();
    }

    public partial class App
    {
        public const string LogTag = "AppCenterXamarinPuppet";

        // App Center B2C secrets
        static readonly IReadOnlyDictionary<string, string> B2CAuthAppSecrets = new Dictionary<string, string>
        {
            { XamarinDevice.UWP, "a678b499-1912-4a94-9d97-25b569284d3a" }, // same for all devices
            { XamarinDevice.Android, "bff0949b-7970-439d-9745-92cdc59b10fe" },
            { XamarinDevice.iOS, "b889c4f2-9ac2-4e2e-ae16-dae54f2c5899" }
        };

        // App Center AAD secrets
        static readonly IReadOnlyDictionary<string, string> AADAuthAppSecrets = new Dictionary<string, string>
        {
            { XamarinDevice.UWP, "a678b499-1912-4a94-9d97-25b569284d3a" }, // same for all devices
            { XamarinDevice.Android, "9c77fb6e-7fff-4ae9-ac18-46c0041a6355" },
            { XamarinDevice.iOS, "4ca276ee-9a50-4ad6-9746-50c420f9df88" }
        };
        
        // OneCollector secrets
        static readonly IReadOnlyDictionary<string, string> OneCollectorTokens = new Dictionary<string, string>
        {
            { XamarinDevice.Android, "7be01f52a17a455ca07566a4e978d961-de99cbfd-41a4-463a-9c23-92cabd834b0d-6966" },
            { XamarinDevice.iOS, "1ad92bec07bb4cbf8d98f37c345f1982-c261ed35-7a36-429f-9f5f-6162117fbb72-7166" }
        };

        public App()
        {
            InitializeComponent();
            MainPage = new NavigationPage(new MainPuppetPage());
        }

        protected override void OnStart()
        {
            if (!AppCenter.Configured)
            {
                AppCenterLog.Assert(LogTag, "AppCenter.LogLevel=" + AppCenter.LogLevel);
                AppCenter.LogLevel = LogLevel.Verbose;
                AppCenterLog.Info(LogTag, "AppCenter.LogLevel=" + AppCenter.LogLevel);
                AppCenterLog.Info(LogTag, "AppCenter.Configured=" + AppCenter.Configured);

                // Set callbacks
                Crashes.ShouldProcessErrorReport = ShouldProcess;
                Crashes.ShouldAwaitUserConfirmation = ConfirmationHandler;
                Crashes.GetErrorAttachments = GetErrorAttachmentsCallback;
                Distribute.ReleaseAvailable = OnReleaseAvailable;

                // Event handlers
                Crashes.SendingErrorReport += SendingErrorReportHandler;
                Crashes.SentErrorReport += SentErrorReportHandler;
                Crashes.FailedToSendErrorReport += FailedToSendErrorReportHandler;
                Push.PushNotificationReceived += PrintNotification;

                AppCenterLog.Assert(LogTag, "AppCenter.Configured=" + AppCenter.Configured);

                if (!StartType.OneCollector.Equals(StartTypeUtils.GetPersistedStartType()))
                {
                    AppCenter.SetLogUrl("https://in-integration.dev.avalanch.es");
                }               

                Distribute.SetInstallUrl("https://install.portal-server-core-integration.dev.avalanch.es");
                Distribute.SetApiUrl("https://api-gateway-core-integration.dev.avalanch.es/v0.1");
                Auth.SetConfigUrl("https://config-integration.dev.avalanch.es");
                Data.SetTokenExchangeUrl("https://token-exchange-mbaas-integration.dev.avalanch.es/v0.1");

                AppCenter.Start(GetTokensString(), typeof(Analytics), typeof(Crashes), typeof(Distribute), typeof(Auth), typeof(Data));
                if (Current.Properties.ContainsKey(Constants.UserId) && Current.Properties[Constants.UserId] is string id)
                {
                    AppCenter.SetUserId(id);
                }

                // Work around for SetUserId race condition.
                AppCenter.Start(typeof(Push));
                AppCenter.IsEnabledAsync().ContinueWith(enabled =>
                {
                    AppCenterLog.Info(LogTag, "AppCenter.Enabled=" + enabled.Result);
                });
                AppCenter.GetInstallIdAsync().ContinueWith(installId =>
                {
                    AppCenterLog.Info(LogTag, "AppCenter.InstallId=" + installId.Result);
                });
                AppCenterLog.Info(LogTag, "AppCenter.SdkVersion=" + AppCenter.SdkVersion);
                Crashes.HasCrashedInLastSessionAsync().ContinueWith(hasCrashed =>
                {
                    AppCenterLog.Info(LogTag, "Crashes.HasCrashedInLastSession=" + hasCrashed.Result);
                });
                Crashes.GetLastSessionCrashReportAsync().ContinueWith(task =>
                {
                    AppCenterLog.Info(LogTag, "Crashes.LastSessionCrashReport.StackTrace=" + task.Result?.StackTrace);
                });
            }
        }

        private string GetOneCollectorTokenString()
        {
            return $"androidTarget={OneCollectorTokens[XamarinDevice.Android]};iosTarget={OneCollectorTokens[XamarinDevice.iOS]}";
        }

        private string GetAppCenterTokenString()
        {
            var appSecrets = GetAppSecretDictionary();
            return $"uwp={appSecrets[XamarinDevice.UWP]};android={appSecrets[XamarinDevice.Android]};ios={appSecrets[XamarinDevice.iOS]}";
        }

        private string GetTokensString()
        {
            var persistedStartType = StartTypeUtils.GetPersistedStartType();
            switch (persistedStartType)
            {
                case StartType.OneCollector:
                    return GetOneCollectorTokenString();
                case StartType.Both:
                    return $"{GetAppCenterTokenString()};{GetOneCollectorTokenString()}";
                default:
                    return GetAppCenterTokenString();
            }
        }

        static IReadOnlyDictionary<string, string> GetAppSecretDictionary()
        {
            // If user has selected another Auth Type, override the secret dictionary accordingly.
            var persistedAuthType = AuthTypeUtils.GetPersistedAuthType();
            switch (persistedAuthType)
            {
                case AuthType.AAD:
                    return AADAuthAppSecrets;
                case AuthType.B2C:
                default:
                    return B2CAuthAppSecrets;
            }
        }

        static void PrintNotification(object sender, PushNotificationReceivedEventArgs e)
        {
            Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
            {
                var message = e.Message;
                if (e.CustomData != null)
                {
                    message += "\nCustom data = {" + string.Join(",", e.CustomData.Select(kv => kv.Key + "=" + kv.Value)) + "}";
                }
                Current.MainPage.DisplayAlert(e.Title, message, "OK");
            });
        }

        static void SendingErrorReportHandler(object sender, SendingErrorReportEventArgs e)
        {
            AppCenterLog.Info(LogTag, "Sending error report");
        }

        static void SentErrorReportHandler(object sender, SentErrorReportEventArgs e)
        {
            AppCenterLog.Info(LogTag, "Sent error report");
        }

        static void FailedToSendErrorReportHandler(object sender, FailedToSendErrorReportEventArgs e)
        {
            AppCenterLog.Info(LogTag, "Failed to send error report");
        }

        bool ShouldProcess(ErrorReport report)
        {
            AppCenterLog.Info(LogTag, "Determining whether to process error report");
            return true;
        }

        bool ConfirmationHandler()
        {
            Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
            {
                Current.MainPage.DisplayActionSheet("Crash detected. Send anonymous crash report?", null, null, "Send", "Always Send", "Don't Send").ContinueWith((arg) =>
                {
                    var answer = arg.Result;
                    UserConfirmation userConfirmationSelection;
                    if (answer == "Send")
                    {
                        userConfirmationSelection = UserConfirmation.Send;
                    }
                    else if (answer == "Always Send")
                    {
                        userConfirmationSelection = UserConfirmation.AlwaysSend;
                    }
                    else
                    {
                        userConfirmationSelection = UserConfirmation.DontSend;
                    }
                    AppCenterLog.Debug(LogTag, "User selected confirmation option: \"" + answer + "\"");
                    Crashes.NotifyUserConfirmation(userConfirmationSelection);
                });
            });

            return true;
        }

        static IEnumerable<ErrorAttachmentLog> GetErrorAttachmentsCallback(ErrorReport report)
        {
            return GetErrorAttachments();
        }

        public static IEnumerable<ErrorAttachmentLog> GetErrorAttachments()
        {
            var attachments = new List<ErrorAttachmentLog>();
            if (Current.Properties.TryGetValue(CrashesContentPage.TextAttachmentKey, out var textAttachment) &&
                textAttachment is string text)
            {
                var attachment = ErrorAttachmentLog.AttachmentWithText(text, "hello.txt");
                attachments.Add(attachment);
            }
            if (Current.Properties.TryGetValue(CrashesContentPage.FileAttachmentKey, out var fileAttachment) &&
                fileAttachment is string file)
            {
                var filePicker = DependencyService.Get<IFilePicker>();
                if (filePicker != null)
                {
                    try
                    {
                        var result = filePicker.ReadFile(file);
                        if (result != null)
                        {
                            var attachment = ErrorAttachmentLog.AttachmentWithBinary(result.Item1, result.Item2, result.Item3);
                            attachments.Add(attachment);
                        }
                    }
                    catch (Exception e)
                    {
                        AppCenterLog.Warn(LogTag, "Couldn't read file attachment", e);
                        Current.Properties.Remove(CrashesContentPage.FileAttachmentKey);
                    }
                }
            }
            return attachments;
        }

        bool OnReleaseAvailable(ReleaseDetails releaseDetails)
        {
            AppCenterLog.Info(LogTag, "OnReleaseAvailable id=" + releaseDetails.Id
                                            + " version=" + releaseDetails.Version
                                            + " releaseNotesUrl=" + releaseDetails.ReleaseNotesUrl);
            var custom = releaseDetails.ReleaseNotes?.ToLowerInvariant().Contains("custom") ?? false;
            if (custom)
            {
                var title = "Version " + releaseDetails.ShortVersion + " available!";
                Task answer;
                if (releaseDetails.MandatoryUpdate)
                {
                    answer = Current.MainPage.DisplayAlert(title, releaseDetails.ReleaseNotes, "Update now!");
                }
                else
                {
                    answer = Current.MainPage.DisplayAlert(title, releaseDetails.ReleaseNotes, "Update now!", "Maybe tomorrow...");
                }
                answer.ContinueWith((task) =>
                {
                    if (releaseDetails.MandatoryUpdate || ((Task<bool>)task).Result)
                    {
                        Distribute.NotifyUpdateAction(UpdateAction.Update);
                    }
                    else
                    {
                        Distribute.NotifyUpdateAction(UpdateAction.Postpone);
                    }
                });
            }
            return custom;
        }
    }
}
