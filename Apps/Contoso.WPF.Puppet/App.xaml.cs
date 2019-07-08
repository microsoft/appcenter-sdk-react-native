// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Globalization;
using System.Windows;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;

namespace Contoso.WPF.Puppet
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            AppCenter.LogLevel = LogLevel.Verbose;
            AppCenter.SetLogUrl("https://in-integration.dev.avalanch.es");


            // User callbacks.
            Crashes.ShouldAwaitUserConfirmation = ConfirmationHandler;
            Crashes.ShouldProcessErrorReport = (report) =>
            {
                Log($"Determining whether to process error report with an ID: {report.Id}");
                return true;
            };

            // Event handlers.
            Crashes.SendingErrorReport += (_, args) => Log($"Sending error report for an error ID: {args.Report.Id}");
            Crashes.SentErrorReport += (_, args) => Log($"Sent error report for an error ID: {args.Report.Id}");
            Crashes.FailedToSendErrorReport += (_, args) => Log($"Failed to send error report for an error ID: {args.Report.Id}");

            // Start AppCenter.
            AppCenter.Start("42f4a839-c54c-44da-8072-a2f2a61751b2", typeof(Analytics), typeof(Crashes));
            Crashes.HasCrashedInLastSessionAsync().ContinueWith(hasCrashed =>
            {
                Log("Crashes.HasCrashedInLastSession=" + hasCrashed.Result);
            });
            Crashes.GetLastSessionCrashReportAsync().ContinueWith(task =>
            {
                Log("Crashes.LastSessionCrashReport.Exception=" + task.Result?.Exception);
            });
        }

        private static bool ConfirmationHandler()
        {
            Current.Dispatcher.Invoke(() =>
            {
                var dialog = new UserConfirmationDialog();
                if (dialog.ShowDialog() ?? false)
                {
                    Crashes.NotifyUserConfirmation(dialog.ClickResult);
                }
            });
            return true;
        }

        private static void Log(string message)
        {
            var timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture);
            System.Diagnostics.Debug.WriteLine($"{timestamp} [AppCenterPuppet] Info: {message}");
        }
    }
}
