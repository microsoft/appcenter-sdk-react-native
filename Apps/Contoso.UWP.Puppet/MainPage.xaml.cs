// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
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
        public MainPage()
        {
            this.InitializeComponent();
            Application.Current.UnhandledException += OnUnhandledException;
            SetMemoryWarning();
        }

        private async void SetMemoryWarning()
        {
            var memoryWarning = await Crashes.HasReceivedMemoryWarningInLastSessionAsync();
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
            var blockSize = 256 * 1024 * 1024;
            byte[] a = Enumerable.Repeat((byte)blockSize, int.MaxValue).ToArray();
        }
    }
}
