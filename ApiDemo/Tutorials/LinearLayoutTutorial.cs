using System;
using Android.App;
using Android.Content;
using Android.OS;

namespace MonoDroid.ApiDemo
{
	[Activity (Label = "Tutorials/Linear Layout")]
	[IntentFilter (new[] { Intent.ActionMain }, Categories = new string[] { ApiDemo.SAMPLE_CATEGORY })]
	public class LinearLayoutTutorial : Activity
	{
		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);
			SetContentView (Resource.Layout.LinearLayoutTutorial);
		}
	}
}