using System;
using System.Collections.Generic;
using Android.App;
using Android.OS;
using Android.Widget;

namespace TablesAndCellStyles {
    [Activity(Label = "GradientBackground")]
    public class GradientBackground : Activity {

        List<Tuple<string, string, int>> items;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.GradientBackground);

            items = new List<Tuple<string, string, int>>();
            items.Add(new Tuple<string, string, int>("Fiji", "A nice beach", Resource.Drawable.Beach));
            items.Add(new Tuple<string, string, int>("Beijing", "AKA Shanghai", Resource.Drawable.Shanghai));
            items.Add(new Tuple<string, string, int>("Seedlings", "Tiny plants", Resource.Drawable.Seeds));
            items.Add(new Tuple<string, string, int>("Plants", "Green plants", Resource.Drawable.Plants));


            var listView = FindViewById<ListView>(Resource.Id.List);
            listView.Adapter = new GradientBackground_Adapter(this, items);
            listView.ItemClick += (object sender, ItemEventArgs e) => {
                var t = items[e.Position];
                Android.Widget.Toast.MakeText(this, t.Item1, Android.Widget.ToastLength.Short).Show();
                Console.WriteLine("Clicked on " + t.Item1);
            };
        }
    }
}