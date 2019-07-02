// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using System;
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
            Crashes.SendingErrorReport += (sender, args) => Console.WriteLine($@"[App] Sending report Id={args.Report.Id} Exception={args.Report.Exception}");
            Crashes.SentErrorReport += (sender, args) => Console.WriteLine($@"[App] Sent report Id={args.Report.Id}");
            Crashes.FailedToSendErrorReport += (sender, args) => Console.WriteLine($@"[App] FailedToSend report Id={args.Report.Id} Error={args.Exception}");
            AppCenter.Start("42f4a839-c54c-44da-8072-a2f2a61751b2", typeof(Analytics), typeof(Crashes));
        }
    }
}
