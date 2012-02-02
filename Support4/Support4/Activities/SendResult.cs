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

namespace Support4
{
	[Activity (Label = "SendResult", Theme = "@style/ThemeDialogWhenLarge")]
	public class SendResult : Activity
	{
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			// Create your application here
			
			// See assets/res/any/layout/hello_world.xml for this
	        // view layout definition, which is being set here as
	        // the content of our screen.
	        SetContentView(Resource.Layout.send_result);
	
	        // Watch for button clicks.
	        Button button = FindViewById<Button>(Resource.Id.corky);
	        button.Click += (sender, e) => {
				SetResult(Result.Ok, new Intent().SetAction("Corky!"));
            	Finish();
			};
	        button = FindViewById<Button>(Resource.Id.violet);
	        button.Click += (sender, e) => {
				SetResult(Result.Ok, new Intent().SetAction("Violet!"));
            	Finish();
			};
		}
	}
}

