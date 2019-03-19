// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.ComponentModel;
using System.Windows.Input;
using Microsoft.AppCenter;
using Xamarin.Forms;

namespace Contoso.Forms.Demo
{
    [Android.Runtime.Preserve(AllMembers = true)]
    public partial class AppCenterContentPage : ContentPage
    {
        public AppCenterContentPage()
        {
            InitializeComponent();
            if (Xamarin.Forms.Device.RuntimePlatform == Xamarin.Forms.Device.iOS)
            {
                Icon = "bolt.png";
            }
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            AppCenterEnabledSwitchCell.On = await AppCenter.IsEnabledAsync();
        }

        async void UpdateEnabled(object sender, ToggledEventArgs e)
        {
            await AppCenter.SetEnabledAsync(e.Value);
        }
    }

    [Android.Runtime.Preserve(AllMembers = true)]
    public class EntryCellTextChanged : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private string _userId;

        public string UserId
        {
            get { return _userId; }

            set
            {
                _userId = value;
                OnTextChanged(_userId);
            }
        }

        public ICommand TextChanged;

        protected virtual void OnTextChanged(string inputText)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(inputText));
                var text = string.IsNullOrEmpty(inputText) ? null : inputText;
                AppCenter.SetUserId(text);
            }
        }
    }
}
