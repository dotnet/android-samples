using System;
using Android.App;
using Android.OS;
using Android.Widget;

namespace Notifications
{
    [Activity(Label = "Second Activity")]
    public class SecondActivity : Activity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Get the count value passed to us from MainActivity:
            int count = Intent.Extras.GetInt("count", -1);

            // No count was passed? Then just return.
            if (count <= 0)
            {
                return;
            }

            // Display the count sent from the first activity:
            SetContentView(Resource.Layout.Second);
            TextView txtView = FindViewById<TextView>(Resource.Id.textView1);
            txtView.Text = String.Format("You clicked the button {0} times in the previous activity.", count);
        }
    }
}
