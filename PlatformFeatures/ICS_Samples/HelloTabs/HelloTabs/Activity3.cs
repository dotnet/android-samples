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

namespace HelloTabs
{
    [Activity (Label = "Activity3")]            
    public class Activity3 : Activity
    {
        protected override void OnCreate (Bundle bundle)
        {
            base.OnCreate (bundle);   

            TextView textview = new TextView (this);
            textview.Text = "This is the activity under tab 2";
            SetContentView (textview);
        }    
    }
}

