// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Auth;
using Microsoft.AppCenter.Crashes;
using Microsoft.AppCenter.Data;
using Microsoft.AppCenter.Distribute;
using Microsoft.AppCenter.Push;
using Newtonsoft.Json;
using Xamarin.Forms;

namespace Contoso.Forms.Demo
{
    using XamarinDevice = Xamarin.Forms.Device;

    [Android.Runtime.Preserve(AllMembers = true)]
    public partial class OthersContentPage
    {
        private const string AccountId = "accountId";

        static bool _eventFilterStarted;

        private UserInformation userInfo = null;

        static OthersContentPage()
        {
            Data.RemoteOperationCompleted += (sender, eventArgs) =>
            {
                AppCenterLog.Info(App.LogTag, "Remote operation completed event=" + JsonConvert.SerializeObject(eventArgs) + " sender=" + sender);
            };
        }

        public OthersContentPage()
        {
            InitializeComponent();
            if (XamarinDevice.RuntimePlatform == XamarinDevice.iOS)
            {
                Icon = "handbag.png";
            }

            // Setup auth type dropdown choices
            foreach (var authType in AuthTypeUtils.GetAuthTypeChoiceStrings())
            {
                this.AuthTypePicker.Items.Add(authType);
            }
            this.AuthTypePicker.SelectedIndex = (int)(AuthTypeUtils.GetPersistedAuthType());
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            var acEnabled = await AppCenter.IsEnabledAsync();
            RefreshDistributeEnabled(acEnabled);
            RefreshPushEnabled(acEnabled);
            RefreshAuthEnabled(acEnabled);
            EventFilterEnabledSwitchCell.On = _eventFilterStarted && await EventFilterHolder.Implementation?.IsEnabledAsync();
            EventFilterEnabledSwitchCell.IsEnabled = acEnabled && EventFilterHolder.Implementation != null;
            if (!Application.Current.Properties.ContainsKey(AccountId))
            {
                SignInInformationButton.Text = "Authentication status unknown";
            }
            else if (Application.Current.Properties[AccountId] is string)
            {
                SignInInformationButton.Text = "User is authenticated";
            }
            else
            {
                SignInInformationButton.Text = "User is not authenticated";
            }
        }

        async void UpdateDistributeEnabled(object sender, ToggledEventArgs e)
        {
            await Distribute.SetEnabledAsync(e.Value);
            var acEnabled = await AppCenter.IsEnabledAsync();
            RefreshDistributeEnabled(acEnabled);
        }

        async void UpdatePushEnabled(object sender, ToggledEventArgs e)
        {
            await Push.SetEnabledAsync(e.Value);
            var acEnabled = await AppCenter.IsEnabledAsync();
            RefreshPushEnabled(acEnabled);
        }

        async void UpdateAuthEnabled(object sender, ToggledEventArgs e)
        {
            await Auth.SetEnabledAsync(e.Value);
            var acEnabled = await AppCenter.IsEnabledAsync();
            RefreshAuthEnabled(acEnabled);
        }

        async void RefreshDistributeEnabled(bool _appCenterEnabled)
        {
            DistributeEnabledSwitchCell.On = await Distribute.IsEnabledAsync();
            DistributeEnabledSwitchCell.IsEnabled = _appCenterEnabled;
        }

        async void RefreshPushEnabled(bool _appCenterEnabled)
        {
            PushEnabledSwitchCell.On = await Push.IsEnabledAsync();
            PushEnabledSwitchCell.IsEnabled = _appCenterEnabled;
        }

        async void RefreshAuthEnabled(bool _appCenterEnabled)
        {
            AuthEnabledSwitchCell.On = await Auth.IsEnabledAsync();
            AuthEnabledSwitchCell.IsEnabled = _appCenterEnabled;
        }

        async void ChangeAuthType(object sender, PropertyChangedEventArgs e)
        {
            // IOS sends an event every time user rests their selection on an item without hitting "done", and the only event they send when hitting "done" is that the control is no longer focused.
            // So we'll process the change at that time. This works for android as well.
            if (e.PropertyName == "IsFocused" && !this.AuthTypePicker.IsFocused)
            {
                var newSelectionCandidate = this.AuthTypePicker.SelectedIndex;
                var persistedAuthType = AuthTypeUtils.GetPersistedAuthType();
                if (newSelectionCandidate != (int)persistedAuthType)
                {
                    AuthTypeUtils.SetPersistedAuthType((AuthType)newSelectionCandidate);
                    await Application.Current.SavePropertiesAsync();
                    await DisplayAlert("Authorization Type Changed", "Authorization type has changed, which alters the app secret. Please close and re-run the app for the new app secret to take effect.", "OK");
                }
            }
        }

        async void UpdateEventFilterEnabled(object sender, ToggledEventArgs e)
        {
            if (EventFilterHolder.Implementation != null)
            {
                if (!_eventFilterStarted)
                {
                    AppCenter.Start(EventFilterHolder.Implementation.BindingType);
                    _eventFilterStarted = true;
                }
                await EventFilterHolder.Implementation.SetEnabledAsync(e.Value);
            }
        }

        async void RunMBaaSAsync(object sender, EventArgs e)
        {
            try
            {
                userInfo = await Auth.SignInAsync();
                if (userInfo.AccountId != null)
                {
                    Application.Current.Properties[AccountId] = userInfo.AccountId;
                    SignInInformationButton.Text = "User authenticated";
                }
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
                    AppCenterLog.Info(App.LogTag, "List result=" + JsonConvert.SerializeObject(doc));
                }
                var document = list.CurrentPage.Items.First();
                AppCenterLog.Info(App.LogTag, "List first result=" + JsonConvert.SerializeObject(document));
                document = await Data.DeleteAsync<CustomDocument>(document.Id, DefaultPartitions.UserDocuments);
                AppCenterLog.Info(App.LogTag, "Delete result=" + JsonConvert.SerializeObject(document));
            }
            catch (Exception ex)
            {
                AppCenterLog.Error(App.LogTag, "Data list/delete first scenario failed", ex);
                Crashes.TrackError(ex);
            }
            try
            {
                var objectCollection = new List<Uri>();
                objectCollection.Add(new Uri("http://google.com/"));
                objectCollection.Add(new Uri("http://microsoft.com/"));
                objectCollection.Add(new Uri("http://facebook.com/"));
                var primitiveCollection = new List<int>();
                primitiveCollection.Add(1);
                primitiveCollection.Add(2);
                primitiveCollection.Add(3);
                var dict = new Dictionary<string, Uri>();
                dict.Add("key1", new Uri("http://google.com/"));
                dict.Add("key2", new Uri("http://microsoft.com/"));
                dict.Add("key3", new Uri("http://facebook.com/"));
                var customDoc = new CustomDocument
                {
                    Id = Guid.NewGuid(),
                    TimeStamp = DateTime.UtcNow,
                    SomeNumber = 123,
                    SomeObject = dict,
                    SomePrimitiveArray = new int[] { 1, 2, 3 },
                    SomeObjectArray = new CustomDocument[] {
                        new CustomDocument {
                            Id = Guid.NewGuid(),
                            TimeStamp = DateTime.UtcNow,
                            SomeNumber = 123,
                            SomeObject = dict,
                            SomePrimitiveArray = new int[] { 1, 2, 3 },
                            SomeObjectCollection = objectCollection,
                            SomePrimitiveCollection = primitiveCollection
                        }
                    },
                    SomeObjectCollection = objectCollection,
                    SomePrimitiveCollection = primitiveCollection,
                    Custom = new CustomDocument
                    {
                        Id = Guid.NewGuid(),
                        TimeStamp = DateTime.UtcNow,
                        SomeNumber = 123,
                        SomeObject = dict,
                        SomePrimitiveArray = new int[] { 1, 2, 3 },
                        SomeObjectCollection = objectCollection,
                        SomePrimitiveCollection = primitiveCollection
                    }
                };
                var id = customDoc.Id.ToString();
                var document = await Data.ReplaceAsync(id, customDoc, DefaultPartitions.UserDocuments);
                AppCenterLog.Info(App.LogTag, "Replace result=" + JsonConvert.SerializeObject(document));
                document = await Data.ReadAsync<CustomDocument>(id, DefaultPartitions.UserDocuments);
                AppCenterLog.Info(App.LogTag, "Read result=" + JsonConvert.SerializeObject(document));
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
            userInfo = null;
            SignInInformationButton.Text = "User not authenticated";
            Application.Current.Properties[AccountId] = null;
        }

        async void SignInInformation(object sender, EventArgs e)
        {
            if (userInfo != null)
            {
                string accessToken = userInfo.AccessToken?.Length > 0 ? "Set" : "Unset";
                string idToken = userInfo.IdToken?.Length > 0 ? "Set" : "Unset";
                await Navigation.PushModalAsync(new SignInInformationContentPage(userInfo.AccountId, accessToken, idToken));
            }
        }

        public class CustomDocument
        {
            [JsonProperty("id")]
            public Guid? Id { get; set; }

            [JsonProperty("timestamp")]
            public DateTime TimeStamp { get; set; }

            [JsonProperty("somenumber")]
            public int SomeNumber { get; set; }

            [JsonProperty("someprimitivearray")]
            public int[] SomePrimitiveArray { get; set; }

            [JsonProperty("someobjectarray")]
            public CustomDocument[] SomeObjectArray { get; set; }

            [JsonProperty("someprimitivecollection")]
            public IList SomePrimitiveCollection { get; set; }

            [JsonProperty("someobjectcollection")]
            public IList SomeObjectCollection { get; set; }

            [JsonProperty("someobject")]
            public Dictionary<string, Uri> SomeObject { get; set; }

            [JsonProperty("customdocument")]
            public CustomDocument Custom { get; set; }
        }
    }
}
