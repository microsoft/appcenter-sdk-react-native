// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using Xamarin.Forms;

namespace Contoso.Forms.Demo
{
    [Android.Runtime.Preserve(AllMembers = true)]
    public partial class SignInInformationContentPage : ContentPage
    {
        public SignInInformationContentPage(string accountIdText, string accessTokenText, string idTokenText)
        {
            InitializeComponent();
            AccountIdEntry.Text = accountIdText;
            AccessTokenEntry.Text = accessTokenText;
            IdTokenEntry.Text = idTokenText;
        }

        async void OkOnClick(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync();
        }
    }
}