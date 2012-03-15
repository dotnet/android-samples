using System;
using Android.App;
using Android.OS;
using Android.Widget;

namespace StandardControls {

    [Activity(Label = "Spinner")]
    public class SpinnerScreen : Activity {

        String[] items = new String[] { "Cupcake", "Donut", "Eclair", "Froyo", "Gingerbread", "Ice Cream Sandwich", "Jellybean", "Key Lime Pie"};

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.Spinner);

            ArrayAdapter ad = new ArrayAdapter(this, Android.Resource.Layout.SimpleSpinnerItem, items);
            ad.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            Spinner spinner = FindViewById<Spinner>(Resource.Id.Spinner);
            spinner.Adapter = ad;
            spinner.ItemSelected += (sender, e) => {
                var s = sender as Spinner;
                Toast.MakeText(this, "My favorite is " + s.GetItemAtPosition(e.Position), ToastLength.Short).Show();
            };

        }
    }
}

