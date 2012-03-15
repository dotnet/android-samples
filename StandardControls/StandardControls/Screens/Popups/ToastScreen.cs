using System;
using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;

namespace StandardControls {

    [Activity(Label = "Toast")]
    public class ToastScreen : Activity {
        
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.Toast);

            var shortButton = FindViewById<Button>(Resource.Id.ShortToastButton);
            shortButton.Click += (sender, args) => {
                Toast.MakeText(this, "Short toast message", ToastLength.Short).Show();
            };

            var longButton = FindViewById<Button>(Resource.Id.LongToastButton);
            longButton.Click += (sender, args) => {
                Toast.MakeText(this, "Longer toast duration because you need extra time if there's more to read", ToastLength.Long).Show();
            };

            var topButton = FindViewById<Button>(Resource.Id.TopToastButton);
            topButton.Click += (sender, args) => {
                var t = Toast.MakeText(this, "Wow this toast appears near the top", ToastLength.Short);
                t.SetGravity(GravityFlags.Top, 0, 200);
                t.Show();
            };

            var customButton = FindViewById<Button>(Resource.Id.CustomToastButton);
            customButton.Click += (sender, args) => {
                var toastView = LayoutInflater.Inflate(Resource.Layout.ToastCustom, FindViewById<ViewGroup>(Resource.Id.ToastLayout));
                toastView.FindViewById<TextView>(Resource.Id.ToastText).Text = "REALLY OBVIOUS TOAST @ " + DateTime.Now.ToShortTimeString();
                var t = new Toast(this);
                t.SetGravity(GravityFlags.Center, 0, 0);
                t.View = toastView;
                t.Show();
            };
        }
    }
}

