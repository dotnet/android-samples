using System;
using Android.App;
using Android.OS;
using Android.Widget;
using System.Collections.Generic;

namespace TablesAndCellStyles {
    
    [Activity(Label = "ImageAndSubtitle")]
    public class ImageAndSubtitle : ListActivity {

        List<Tuple<string, string, int>> items;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            items = new List<Tuple<string, string, int>>();
            items.Add(new Tuple<string, string, int>("Fiji", "A nice beach", Resource.Drawable.Beach));
            items.Add(new Tuple<string, string, int>("Beijing", "AKA Shanghai", Resource.Drawable.Shanghai));
            items.Add(new Tuple<string, string, int>("Seedlings", "Tiny plants", Resource.Drawable.Seeds));
            items.Add(new Tuple<string, string, int>("Plants", "Green plants", Resource.Drawable.Plants));

            this.ListAdapter = new ImageAndSubtitle_Adapter(this, items);
        }
        protected override void OnListItemClick(Android.Widget.ListView l, Android.Views.View v, int position, long id)
        {
            var t = items[position];
            Android.Widget.Toast.MakeText(this, t.Item1, Android.Widget.ToastLength.Short).Show();
            Console.WriteLine("Clicked on " + t);
        }
    }
}
