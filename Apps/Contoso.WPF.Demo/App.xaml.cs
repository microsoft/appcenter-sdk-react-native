// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Windows;
using Contoso.WPF.Demo.Properties;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;

namespace Contoso.WPF.Demo
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            AppCenter.LogLevel = LogLevel.Verbose;
            Crashes.GetErrorAttachments = report =>
            {
                var attachments = new List<ErrorAttachmentLog>();

                // Text attachment
                if (!string.IsNullOrEmpty(Settings.Default.TextErrorAttachments))
                {
                    attachments.Add(
                        ErrorAttachmentLog.AttachmentWithText(Settings.Default.TextErrorAttachments, "text.txt"));
                }

                // Binary attachment
                if (!string.IsNullOrEmpty(Settings.Default.FileErrorAttachments))
                {
                    if (File.Exists(Settings.Default.FileErrorAttachments))
                    {
                        var fileName = new FileInfo(Settings.Default.FileErrorAttachments).Name;
                        var mimeType = MimeMapping.GetMimeMapping(Settings.Default.FileErrorAttachments);
                        var fileContent = File.ReadAllBytes(Settings.Default.FileErrorAttachments);
                        attachments.Add(ErrorAttachmentLog.AttachmentWithBinary(fileContent, fileName, mimeType));
                    }
                    else
                    {
                        Settings.Default.FileErrorAttachments = null;
                    }
                }

                return attachments;
            };
            AppCenter.Start("f4e2a83d-3052-4884-8176-8b2c50277d16", typeof(Analytics), typeof(Crashes));
        }
    }
}
