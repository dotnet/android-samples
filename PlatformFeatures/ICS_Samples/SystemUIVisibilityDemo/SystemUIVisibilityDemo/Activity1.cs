using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

namespace SystemUIVisibilityDemo
{
    [Activity (Label = "SystemUIVisibilityDemo", MainLauncher = true)]
    public class Activity1 : Activity
    {

        protected override void OnCreate (Bundle bundle)
        {
            base.OnCreate (bundle);

            // Set our view from the "main" layout resource
            SetContentView (Resource.Layout.Main);
            
            var tv = FindViewById<TextView> (Resource.Id.systemUiFlagTextView);
            var lowProfileButton = FindViewById<Button> (Resource.Id.lowProfileButton);
            var hideNavButton = FindViewById<Button> (Resource.Id.hideNavigation);
            var visibleButton = FindViewById<Button> (Resource.Id.visibleButton);
            
            lowProfileButton.Click += delegate { 
                tv.SystemUiVisibility = (StatusBarVisibility)View.SystemUiFlagLowProfile;
            };
            
            hideNavButton.Click += delegate {
                tv.SystemUiVisibility = (StatusBarVisibility)View.SystemUiFlagHideNavigation;         
            };
            
            visibleButton.Click += delegate {
                tv.SystemUiVisibility = (StatusBarVisibility)View.SystemUiFlagVisible;
            };
            
            tv.SystemUiVisibilityChange += delegate(object sender, View.SystemUiVisibilityChangeEventArgs e) {
                
                tv.Text = String.Format ("Visibility = {0}", e.Visibility);
            };
          
        }
    }
}


