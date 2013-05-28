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

namespace HelloMultiScreen
{
    [Activity (Label = "SecondActivity")]            
    public class SecondActivity : Activity
    {
        protected override void OnCreate (Bundle bundle)
        {
            base.OnCreate (bundle);
   
            // Load the UI defined in Second.axml
            SetContentView (Resource.Layout.Second);
            
            // Get a reference to the TextView
            var label = FindViewById<TextView> (Resource.Id.screen2Label);
			var bButton = FindViewById<Button> (Resource.Id.bButton);
            
            // Populate the TextView with the data that was added to the intent in FirstActivity 
            label.Text = Intent.GetStringExtra("FirstData") ?? "Data not available";

			// Send data (a greeting string) back to the first Activity
			bButton.Click += delegate { 
				Intent myIntent = new Intent (this, typeof(FirstActivity));
				myIntent.PutExtra ("greeting", "Hello from the Second Activity!");
				SetResult (Result.Ok, myIntent);
				Finish();
			};
        }
    }
}