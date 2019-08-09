// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using System;
using System.Collections.Generic;
using System.Threading;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Contoso.UWP.Puppet
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        List<byte[]> buffer = new List<byte[]>();

        const int BlockSize = 128 * 1024 * 1024;

        public MainPage()
        {
            this.InitializeComponent();
            Application.Current.UnhandledException += OnUnhandledException;
            var memoryWarning = Crashes.HasReceivedMemoryWarningInLastSessionAsync().Result;
            MemoryWarningTextBlock.Text = memoryWarning ? "Yes" : "No";
        }

        private void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            e.Handled = HandleExceptions.IsOn;
        }

        private void TrackEvent(object sender, RoutedEventArgs e)
        {
            Analytics.TrackEvent("Test");
        }

        private void ThrowException(object sender, RoutedEventArgs e)
        {
            throw new Exception();
        }

        private void HandleMemoryWarning(object sender, RoutedEventArgs e)
        {
            DispatcherTimer dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += dispatcherTimer_Tick;
            dispatcherTimer.Interval = TimeSpan.FromMilliseconds(100);
            dispatcherTimer.Start();
        }

        void dispatcherTimer_Tick(object sender, object e)
        {
            buffer.Add(new byte[BlockSize]);
            System.Diagnostics.Debug.WriteLine("Memory allocated: {0} MB", buffer.Count * 128);
        }
    }
}
