// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Contoso.WPF.Puppet.Properties;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using System;
using System.IO;
using System.Windows;
using System.Web;

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
            Crashes.GetErrorAttachments = (ErrorReport report) =>
            {
                byte[] fileContent = null;
                var fileName = new FileInfo(Settings.Default.FileErrorAttachments).Name;
                var mimeType = "";
                if (File.Exists(Settings.Default.FileErrorAttachments))
                {
                    mimeType = MimeMapping.GetMimeMapping(Settings.Default.FileErrorAttachments);
                    fileContent = File.ReadAllBytes(Settings.Default.FileErrorAttachments);
                }
                return new ErrorAttachmentLog[]
                {
                    ErrorAttachmentLog.AttachmentWithText(Settings.Default.TextErrorAttachments, "text.txt"),
                    ErrorAttachmentLog.AttachmentWithBinary(fileContent, fileName, mimeType)
                };
            };
            Crashes.SendingErrorReport += (sender, args) => Console.WriteLine($@"[App] Sending report Id={args.Report.Id} Exception={args.Report.Exception}");
            Crashes.SentErrorReport += (sender, args) => Console.WriteLine($@"[App] Sent report Id={args.Report.Id}");
            Crashes.FailedToSendErrorReport += (sender, args) => Console.WriteLine($@"[App] FailedToSend report Id={args.Report.Id} Error={args.Exception}");
            AppCenter.Start("42f4a839-c54c-44da-8072-a2f2a61751b2", typeof(Analytics), typeof(Crashes));
        }
    }
}
