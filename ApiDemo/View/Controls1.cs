using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace MonoDroid.ApiDemo
{
	[Activity (Label = "Views/Controls/1. Light Theme", Theme = "@android:style/Theme.Light")]
	[IntentFilter (new[] { Intent.ActionMain }, Categories = new string[] { ApiDemo.SAMPLE_CATEGORY })]
	public class Controls1 : Activity
	{
		string[] planets = {
			"Mercury", "Venus", "Earth", "Mars", "Jupiter", "Saturn", "Uranus", "Neptune"
		};

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			SetContentView (Resource.Layout.controls_1);
			var s1 = FindViewById <Spinner> (Resource.Id.spinner1);
			var adapter = new ArrayAdapter <string> (this, Android.Resource.Layout.SimpleSpinnerItem, planets);
			adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
			s1.Adapter = adapter;
		}
	}
}

