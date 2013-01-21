using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

using Com.Slidingmenu.Lib.App;

namespace SlidingMenuExample
{
	[Activity (Label = "SlidingMenuExample", MainLauncher = true)]
	public class Activity1 : SlidingActivity
	{
		int count = 1;

		public override void OnCreate(Bundle bundle)
		{
			base.OnCreate(bundle);

			// Set our view from the "main" layout resource
			SetContentView(Resource.Layout.Main);
			SetBehindContentView(Resource.Layout.menu);


			// Get our button from the layout resource,
			// and attach an event to it
			Button button = FindViewById<Button>(Resource.Id.myButton);
			
			button.Click += (object sender, EventArgs e) => 
			{
				button.Text = string.Format("{0} clicks!", count++);
			};
		}
	}
}


