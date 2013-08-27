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
	[Activity (Label = "Views/Auto Complete/6. Multiple Items")]
	[IntentFilter (new[] { Intent.ActionMain }, Categories = new string[] { ApiDemo.SAMPLE_CATEGORY })]			
	public class AutoComplete6 : Activity
	{
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			SetContentView (Resource.Layout.autocomplete_6);

			var adapter = new ArrayAdapter<String>(this, Android.Resource.Layout.SimpleDropDownItem1Line, AutoComplete1.COUNTRIES);
			var textView = FindViewById <MultiAutoCompleteTextView> (Resource.Id.edit);
			textView.Adapter = adapter;
			textView.SetTokenizer (new MultiAutoCompleteTextView.CommaTokenizer ());
		}
	}
}

