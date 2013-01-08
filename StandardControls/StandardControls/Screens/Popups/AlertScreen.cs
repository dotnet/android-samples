using System;
using Android.App;
using Android.OS;
using Android.Widget;

namespace StandardControls {

    [Activity(Label = "Alert")]
    public class AlertScreen : Activity {
        
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.Alert);

            var simpleButton = FindViewById<Button>(Resource.Id.SimpleAlertButton);
            simpleButton.Click += (sender, args) => {
                Android.App.AlertDialog.Builder builder = new AlertDialog.Builder(this);
                AlertDialog ad = builder.Create();
                ad.SetTitle("An Alert");
                ad.SetIcon(Android.Resource.Drawable.IcDialogAlert);
                ad.SetMessage("Alert message...");
                ad.SetButton("OK", (s,e) => { Console.WriteLine("OK Button clicked, alert dismissed"); });
                ad.Show();
            };

            var complexButton = FindViewById<Button>(Resource.Id.ComplexAlertButton);
            complexButton.Click += (sender, args) => {
                Android.App.AlertDialog.Builder builder = new AlertDialog.Builder(this);
                AlertDialog ad = builder.Create();
                ad.SetTitle("An Alert");
                ad.SetIcon(Resource.Drawable.icon);
                ad.SetMessage("Alert message...");
                // Positive
                ad.SetButton("OK", (s, e) => { Console.WriteLine("OK button clicked, alert dismissed"); });
                // Negative
                ad.SetButton2("Cancel", (s, e) => { Console.WriteLine("Cancel button clicked, alert dismissed"); });
                // Neutral
                ad.SetButton3("Middle ground", (s, e) => { Console.WriteLine("Middle button clicked, alert dismissed"); });
                ad.Show();
            };
        }
    }
}

