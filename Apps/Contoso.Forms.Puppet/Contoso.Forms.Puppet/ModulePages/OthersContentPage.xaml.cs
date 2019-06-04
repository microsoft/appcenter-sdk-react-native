// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Auth;
using Microsoft.AppCenter.Crashes;
using Microsoft.AppCenter.Data;
using Microsoft.AppCenter.Distribute;
using Microsoft.AppCenter.Push;
using Microsoft.AppCenter.Rum;
using Newtonsoft.Json;
using Xamarin.Forms;
using System.Text;
using Newtonsoft.Json.Linq;

namespace Contoso.Forms.Puppet
{
    using XamarinDevice = Xamarin.Forms.Device;

    [Android.Runtime.Preserve(AllMembers = true)]
    public partial class OthersContentPage
    {
        static bool _rumStarted;

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
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            var acEnabled = await AppCenter.IsEnabledAsync();
            DistributeEnabledSwitchCell.On = await Distribute.IsEnabledAsync();
            DistributeEnabledSwitchCell.IsEnabled = acEnabled;
            PushEnabledSwitchCell.On = await Push.IsEnabledAsync();
            PushEnabledSwitchCell.IsEnabled = acEnabled;
            RumEnabledSwitchCell.On = _rumStarted && await RealUserMeasurements.IsEnabledAsync();
            RumEnabledSwitchCell.IsEnabled = acEnabled;
            EventFilterEnabledSwitchCell.On = _eventFilterStarted && await EventFilterHolder.Implementation?.IsEnabledAsync();
            EventFilterEnabledSwitchCell.IsEnabled = acEnabled && EventFilterHolder.Implementation != null;
        }

        async void UpdateDistributeEnabled(object sender, ToggledEventArgs e)
        {
            await Distribute.SetEnabledAsync(e.Value);
        }

        async void UpdatePushEnabled(object sender, ToggledEventArgs e)
        {
            await Push.SetEnabledAsync(e.Value);
        }

        async void UpdateRumEnabled(object sender, ToggledEventArgs e)
        {
            if (!_rumStarted)
            {
                RealUserMeasurements.SetRumKey("b1919553367d44d8b0ae72594c74e0ff");
                AppCenter.Start(typeof(RealUserMeasurements));
                _rumStarted = true;
            }
            await RealUserMeasurements.SetEnabledAsync(e.Value);
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
                await Auth.SignInAsync().ContinueWith(task => {
                    userInfo = task.Result;
                    SignInInformationButton.Text = "User authenticated " +userInfo.AccountId;
                });
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
            SignInInformationButton.Text = "User Not Authenticated";
        }

        private static byte[] Base64UrlDecode(string input)
        {
            var output = input;
            output = output.Replace('-', '+'); // 62nd char of encoding
            output = output.Replace('_', '/'); // 63rd char of encoding
            switch (output.Length % 4) // Pad with trailing '='s
            {
                case 0: break; // No pad chars in this case
                case 2: output += "=="; break; // Two pad chars
                case 3: output += "="; break; // One pad char
                default: throw new System.Exception("Illegal base64url string!");
            }
            var converted = Convert.FromBase64String(output); // Standard base64 decoder
            return converted;
        }

        public static string Decode(string token, string key)
        {
            var parts = token.Split('.');
            var header = parts[0];
            var payload = parts[1];
            byte[] crypto = Base64UrlDecode(parts[2]);
            byte[] headerData = Base64UrlDecode(header);
            var headerJson = Encoding.UTF8.GetString(headerData,0, headerData.Length);
            byte[] payloadData = Base64UrlDecode(payload);
            var payloadJson = Encoding.UTF8.GetString(payloadData, 0, payloadData.Length);
            return payloadJson;
        }

        async void SignInInformation(object sender, EventArgs e)
        {
            if (userInfo!=null)
            {
                string accessToken = userInfo.AccessToken;
                var token = Decode(accessToken, "");
               // if (accessToken?.Length > 10) accessToken = "<...>" + accessToken?.Substring(accessToken.Length - 10);
                string idToken = userInfo.IdToken;
                if (idToken?.Length > 10) idToken = "<...>" + idToken?.Substring(idToken.Length - 10);
                await Navigation.PushModalAsync(new SignInInformationContentPage(userInfo.AccountId, token, idToken));
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
