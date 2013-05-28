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
            
            // Populate the TextView with the data that was added to the intent in FirstActivity 
            label.Text = Intent.GetStringExtra("FirstData") ?? "Data not available";
        }
    }
}