// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Contoso.Forms.Puppet.Views
{
    [Android.Runtime.Preserve(AllMembers = true)]
    public partial class TextAttachmentView : ContentPage
    {
        private readonly TaskCompletionSource<string> _taskCompletionSource = new TaskCompletionSource<string>();
        private INavigation _navigation;

        public TextAttachmentView()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            Text.Focus();
        }

        public static Task<string> Show(INavigation navegation)
        {
            var view = new TextAttachmentView();
            view._navigation = navegation;
            navegation.PushModalAsync(view);
            return view._taskCompletionSource.Task;
        }

        async void OK(object sender, EventArgs args)
        {
            var result = Text.Text;
            await _navigation.PopModalAsync();
            _taskCompletionSource.SetResult(result);
        }

        async void Cancel(object sender, EventArgs args)
        {
            await _navigation.PopModalAsync();
            _taskCompletionSource.SetResult(null);
        }
    }
}
