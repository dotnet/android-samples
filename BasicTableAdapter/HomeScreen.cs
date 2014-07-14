using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

namespace BasicTableAdapter {
    [Activity(Label = "BasicTable", MainLauncher = true, Icon = "@drawable/icon")]
    public class HomeScreen : ListActivity {
        string[] items;
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            items = new string[] { "Vegetables","Fruits","Flower Buds","Legumes","Bulbs","Tubers" };
            ListAdapter = new HomeScreenAdapter(this, items);
        }
        protected override void OnListItemClick(ListView l, View v, int position, long id)
        {
            var t = items[position];
            Android.Widget.Toast.MakeText(this, t, Android.Widget.ToastLength.Short).Show();
            Console.WriteLine("Clicked on " + t);
        }
    }
}

