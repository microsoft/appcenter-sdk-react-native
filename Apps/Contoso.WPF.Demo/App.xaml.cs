// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Contoso.WPF.Demo.Properties;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using System.IO;
using System.Windows;

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
            Crashes.GetErrorAttachments = (ErrorReport report) =>
            {
                byte[] fileContent = null;
                string fileName = new FileInfo(Settings.Default.FileErrorAttachments).Name;
                if (File.Exists(Settings.Default.FileErrorAttachments))
                {
                    fileContent = File.ReadAllBytes(Settings.Default.FileErrorAttachments);
                }
                return new ErrorAttachmentLog[]
                {
                    ErrorAttachmentLog.AttachmentWithText(Settings.Default.TextErrorAttachments, fileName),
                    ErrorAttachmentLog.AttachmentWithBinary(fileContent, fileName, "text/plain")
                };
            };
            AppCenter.Start("f4e2a83d-3052-4884-8176-8b2c50277d16", typeof(Analytics), typeof(Crashes));
        }
    }
}
