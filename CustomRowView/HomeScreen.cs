using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using System.Collections.Generic;

namespace CustomRowView {
    [Activity(Label = "CustomRowView", MainLauncher = true, Icon = "@drawable/icon")]
    public class HomeScreen : Activity{//, View.IOnClickListener {

        List<TableItem> tableItems = new List<TableItem>();
        ListView listView;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.HomeScreen);
            listView = FindViewById<ListView>(Resource.Id.List);

            tableItems.Add(new TableItem() { Heading = "Vegetables", SubHeading = "65 items", ImageResourceId = Resource.Drawable.Vegetables });
            tableItems.Add(new TableItem() { Heading = "Fruits", SubHeading = "17 items", ImageResourceId = Resource.Drawable.Fruits });
            tableItems.Add(new TableItem() { Heading = "Flower Buds", SubHeading = "5 items", ImageResourceId = Resource.Drawable.FlowerBuds });
            tableItems.Add(new TableItem() { Heading = "Legumes", SubHeading = "33 items", ImageResourceId = Resource.Drawable.Legumes });
            tableItems.Add(new TableItem() { Heading = "Bulbs", SubHeading = "18 items", ImageResourceId = Resource.Drawable.Bulbs });
            tableItems.Add(new TableItem() { Heading = "Tubers", SubHeading = "43 items", ImageResourceId = Resource.Drawable.Tubers });

            listView.Adapter = new HomeScreenAdapter(this, tableItems);

            listView.ItemClick += OnListItemClick;
        }

        protected void OnListItemClick(object sender, Android.Widget.AdapterView.ItemClickEventArgs e)
        {
            var listView = sender as ListView;
            var t = tableItems[e.Position];
            Android.Widget.Toast.MakeText(this, t.Heading, Android.Widget.ToastLength.Short).Show();
            Console.WriteLine("Clicked on " + t.Heading);
        }
    }
}