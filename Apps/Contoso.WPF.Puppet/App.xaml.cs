// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Contoso.WPF.Puppet.Properties;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using System.IO;
using System.Net.Mime;
using System.Net.Mail;
using System.Windows;

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
                string fileName = new FileInfo(Settings.Default.FileErrorAttachments).Name;
                string mimeType = "";
                if (File.Exists(Settings.Default.FileErrorAttachments))
                {
                    mimeType = new Attachment(fileName, MediaTypeNames.Application.Octet).ContentType.MediaType;
                    fileContent = File.ReadAllBytes(Settings.Default.FileErrorAttachments);
                }
                return new ErrorAttachmentLog[]
                {
                    ErrorAttachmentLog.AttachmentWithText(Settings.Default.TextErrorAttachments, "text.txt"),
                    ErrorAttachmentLog.AttachmentWithBinary(fileContent, fileName, mimeType)
                };
            };
            AppCenter.Start("42f4a839-c54c-44da-8072-a2f2a61751b2", typeof(Analytics), typeof(Crashes));
        }
    }
}
