// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;

namespace Contoso.Android.Puppet
{
    [Activity(Label = "PropertiesActivity")]
	public class PropertiesActivity : ListActivity
	{
		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
            var properties = Intent.GetStringArrayExtra("properties");
            ListAdapter = new ArrayAdapter<string>(this, global::Android.Resource.Layout.SimpleListItem1, properties);
		}
	}
}
