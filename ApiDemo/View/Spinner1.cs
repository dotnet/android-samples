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
	[Activity (Label = "Views/Spinner1")]			
	[IntentFilter (new[] { Intent.ActionMain }, Categories = new string[] { ApiDemo.SAMPLE_CATEGORY })]
	public class Spinner1 : Activity
	{
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			SetContentView (Resource.Layout.spinner_1);

			Spinner s1 = FindViewById <Spinner> (Resource.Id.spinner1);
			var adapter = ArrayAdapter.CreateFromResource (
				this, Resource.Array.colors, Android.Resource.Layout.SimpleSpinnerItem);
			adapter.SetDropDownViewResource (Android.Resource.Layout.SimpleSpinnerDropDownItem);
			s1.Adapter = adapter;

			s1.ItemSelected += delegate (object sender, AdapterView.ItemSelectedEventArgs e) {
				Toast.MakeText (this, "Spinner1: position=" + e.Position + " id=" + e.Id, 
				                ToastLength.Short).Show ();
			};

			s1.NothingSelected += delegate (object sender, AdapterView.NothingSelectedEventArgs e) {
				Toast.MakeText (this, "Spinner1: unselected", ToastLength.Short).Show ();
			};


			Spinner s2 = FindViewById <Spinner> (Resource.Id.spinner2);
			adapter = ArrayAdapter.CreateFromResource (this, Resource.Array.planets,
			                                           Android.Resource.Layout.SimpleSpinnerItem);
			adapter.SetDropDownViewResource (Android.Resource.Layout.SimpleSpinnerDropDownItem);
			s2.Adapter = adapter;

			s2.ItemSelected += delegate (object sender, AdapterView.ItemSelectedEventArgs e) {
				Toast.MakeText (this, "Spinner2: position=" + e.Position + " id=" + e.Id, 
				                ToastLength.Short).Show ();
			};

			s2.NothingSelected += delegate (object sender, AdapterView.NothingSelectedEventArgs e) {
				Toast.MakeText (this, "Spinner1: unselected", ToastLength.Short).Show ();
			};
		}
	}
}

