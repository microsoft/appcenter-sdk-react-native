// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Linq;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Auth;
using Microsoft.AppCenter.Crashes;
using Microsoft.AppCenter.Data;
using Microsoft.AppCenter.Distribute;
using Microsoft.AppCenter.Push;
using Xamarin.Forms;

namespace Contoso.Forms.Demo
{
    using XamarinDevice = Xamarin.Forms.Device;

    [Android.Runtime.Preserve(AllMembers = true)]
    public partial class OthersContentPage
    { 
        static OthersContentPage()
        {
            Data.RemoteOperationCompleted += (sender, eventArgs) =>
            {
                AppCenterLog.Info(App.LogTag, "Remote operation completed event=" + eventArgs.Operation + " " + eventArgs.DocumentMetadata.ETag, eventArgs.Error);
            };
        }

        public OthersContentPage()
        {
            InitializeComponent();
            if (XamarinDevice.RuntimePlatform == XamarinDevice.iOS)
            {
                Icon = "handbag.png";
            }
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            var acEnabled = await AppCenter.IsEnabledAsync();
            DistributeEnabledSwitchCell.On = await Distribute.IsEnabledAsync();
            DistributeEnabledSwitchCell.IsEnabled = acEnabled;
            PushEnabledSwitchCell.On = await Push.IsEnabledAsync();
            PushEnabledSwitchCell.IsEnabled = acEnabled;
        }

        async void UpdateDistributeEnabled(object sender, ToggledEventArgs e)
        {
            await Distribute.SetEnabledAsync(e.Value);
        }

        async void UpdatePushEnabled(object sender, ToggledEventArgs e)
        {
            await Push.SetEnabledAsync(e.Value);
        }

        async void RunMBaaSAsync(object sender, EventArgs e)
        {
            try
            {
                var userInfo = await Auth.SignInAsync();
                AppCenterLog.Info(App.LogTag, "Auth.SignInAsync succeeded accountId=" + userInfo.AccountId);
            }
            catch (Exception ex)
            {
                AppCenterLog.Error(App.LogTag, "Auth scenario failed", ex);
                Crashes.TrackError(ex);
            }
            try
            {
                var list = await Data.ListAsync<CustomDocument>(DefaultPartitions.UserDocuments);
                foreach (var doc in list)
                {
                    AppCenterLog.Info(App.LogTag, "List result=" + doc.DeserializedValue.id);
                }
                var document = list.CurrentPage.Items.First();
                AppCenterLog.Info(App.LogTag, "List first result=" + document.DeserializedValue.id);
                document = await Data.DeleteAsync<CustomDocument>(document.Id, DefaultPartitions.UserDocuments);
                AppCenterLog.Info(App.LogTag, "Delete result=" + document.DeserializedValue);
            }
            catch (Exception ex)
            {
                AppCenterLog.Error(App.LogTag, "Data list/delete first scenario failed", ex);
                Crashes.TrackError(ex);
            }
            try
            {
                var customDoc = new CustomDocument
                {
                    id = Guid.NewGuid(),
                    timestamp = DateTime.UtcNow
                };
                var id = customDoc.id.ToString();
                var document = await Data.ReplaceAsync(id, customDoc, DefaultPartitions.UserDocuments);
                AppCenterLog.Info(App.LogTag, "Replace result=" + document.DeserializedValue.id);
                document = await Data.ReadAsync<CustomDocument>(id, DefaultPartitions.UserDocuments);
                AppCenterLog.Info(App.LogTag, "Read result=" + document.DeserializedValue.id);
            }
            catch (Exception ex)
            {
                AppCenterLog.Error(App.LogTag, "Data person scenario failed", ex);
                Crashes.TrackError(ex);
            }
        }

        void SignOut(object sender, EventArgs e)
        {
            Auth.SignOut();
        }

        public class CustomDocument
        {
            public Guid? id { get; set; }

            public DateTime timestamp { get; set; }
        }
    }
}
