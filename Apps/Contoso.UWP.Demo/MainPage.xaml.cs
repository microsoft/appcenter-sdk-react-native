// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using System;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Contoso.UWP.Demo
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
        }

        private void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            e.Handled = HandleExceptions.IsOn;
        }

        private void TrackEvent(object sender, RoutedEventArgs e)
        {
            Analytics.TrackEvent("Test");
        }

        private async void ThrowException(object sender, RoutedEventArgs e)
        {
            // Contoso.Forms.Puppet.UWP has more crash types and UI features to test properties.
            // This app is just for smoke testing.
            // Also this app uses min SDK version to 10240, which changes the .NET native generated code to have missing symbols for handled errors.
            // Handled errors in the forms app never hit that case because we need to use v16299 there.
            await GenerateComplexException(2);
        }

        private async Task GenerateComplexException(int loop)
        {
            if (loop == 0)
            {
                try
                {
                    try
                    {
                        throw new ArgumentException("Hello, I'm an inner exception!");
                    }
                    catch (Exception ex)
                    {
                        throw new InvalidOperationException("Hola! I'm an outer exception!", ex);
                    }
                }
                catch (Exception ex)
                {
                    if (HandleExceptions.IsOn)
                    {
                        Crashes.TrackError(ex);
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            else
            {
                await Task.Run(() => { });
                await GenerateComplexException(loop - 1);
            }
        }
    }
}
