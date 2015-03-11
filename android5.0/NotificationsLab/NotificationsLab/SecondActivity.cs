using System;
using Android.App;
using Android.Widget;
using Android.OS;

namespace NotificationsLab
{
    [Activity(Label = "Second Activity", Theme = "@android:style/Theme.Material")]
    public class SecondActivity : Activity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Get the notification message from the intent:
            string message = Intent.Extras.GetString("message", "");

            // Set the view from the "Second" layout resource:
            SetContentView(Resource.Layout.Second);

            // Update the text box with the text the user types into the last activity:
            TextView textView = FindViewById<TextView>(Resource.Id.textView1);
            textView.Text = String.Format("Passed to SecondActivity:\n {0}", message);
        }
    }
}

