// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;

namespace Contoso.Android.Puppet
{
    [Activity(Label = "LogLevelActivity")]
    public class LogLevelActivity : ListActivity
    {
        private static readonly string[] Levels = {
            Constants.Verbose,
            Constants.Debug,
            Constants.Info,
            Constants.Warning,
            Constants.Error
        };

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            ListAdapter = new ArrayAdapter<string>(this, global::Android.Resource.Layout.SimpleListItem1, Levels);
        }

        protected override void OnListItemClick(ListView l, View v, int position, long id)
        {
            var intent = new Intent();
            intent.PutExtra("log_level", position);
            SetResult(Result.Ok, intent);
            Finish();
        }
    }
}
