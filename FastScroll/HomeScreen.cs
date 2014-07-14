using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using System.IO;
using System.Collections.Generic;

namespace FastScroll {
    [Activity(Label = "FastScroll", MainLauncher = true, Icon = "@drawable/icon")]
    public class HomeScreen : ListActivity {
        string[] items;
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            List<string> veges = new List<string>();
            Stream seedDataStream = Assets.Open(@"VegeData.txt");
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            using (StreamReader reader = new StreamReader(seedDataStream)) {
                while (!reader.EndOfStream) {
                    veges.Add(reader.ReadLine());
                }
            }
            veges.Sort((x, y) => { return x.CompareTo(y); });
            items = veges.ToArray();

            ListAdapter = new ArrayAdapter<String>(this, Android.Resource.Layout.SimpleListItem1, items);

            ListView.FastScrollEnabled = true;
        }
        protected override void OnListItemClick(ListView l, View v, int position, long id)
        {
            var t = items[position];
            Android.Widget.Toast.MakeText(this, t, Android.Widget.ToastLength.Short).Show();
            Console.WriteLine("Clicked on " + t);
        }
    }
}

