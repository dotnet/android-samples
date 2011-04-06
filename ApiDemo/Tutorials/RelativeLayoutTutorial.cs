using System;
using Android.App;
using Android.Content;
using Android.OS;

namespace MonoDroid.ApiDemo
{
	[Activity (Label = "Tutorials/Relative Layout")]
	[IntentFilter (new[] { Intent.ActionMain }, Categories = new string[] { Intent.CategorySampleCode })]
	public class RelativeLayoutTutorial : Activity
	{
		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);
			SetContentView (Resource.Layout.RelativeLayoutTutorial);
		}
	}
}