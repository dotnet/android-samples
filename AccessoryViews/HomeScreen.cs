using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using System.Collections.Generic;

namespace AccessoryViews {
    [Activity(Label = "AccessoryViews", MainLauncher = true, Icon = "@drawable/icon")]
    public class HomeScreen : ListActivity {

        string[] items;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            items = new string[] { "Vegetables", "Fruits", "Flower Buds", "Legumes", "Bulbs", "Tubers" };
            ListAdapter = new ArrayAdapter<String>(this, Android.Resource.Layout.SimpleListItemChecked, items);
            //ListAdapter = new ArrayAdapter<String>(this, Android.Resource.Layout.SimpleListItemSingleChoice, items);
            //ListAdapter = new ArrayAdapter<String>(this, Android.Resource.Layout.SimpleListItemMultipleChoice, items);

            ListView lv = FindViewById<ListView>(Android.Resource.Id.List);
#if __ANDROID_11__
            lv.ChoiceMode = Android.Widget.ChoiceMode.Single; // 1
            //lv.ChoiceMode = Android.Widget.ChoiceMode.Multiple; // 2
            //lv.ChoiceMode = Android.Widget.ChoiceMode.None; // 0
#else
            lv.ChoiceMode = 1; // Single
            //lv.ChoiceMode = 0; // none
            //lv.ChoiceMode = 2; // Multiple
            //lv.ChoiceMode = 3; // MultipleModal
#endif
            // Set the initially checked row ("Fruits")
            lv.SetItemChecked(1, true);

            // Set another initially checked row ("Bulbs") IF multiple selection allowed
            lv.SetItemChecked(4, true);
        }
        protected override void OnListItemClick(ListView l, View v, int position, long id)
        {
            var t = items[position];
            Android.Widget.Toast.MakeText(this, t, Android.Widget.ToastLength.Short).Show();
            Console.WriteLine("Clicked on: " + t);

            // For Single Choice mode
            Console.WriteLine("Now checked: " + FindViewById<ListView>(Android.Resource.Id.List).CheckedItemPosition);
            
            // For Multiple Choice mode
            Console.Write("Now checked: ");
            var sparseArray = FindViewById<ListView>(Android.Resource.Id.List).CheckedItemPositions;
            for (var i = 0; i < sparseArray.Size(); i++ )
                Console.Write(sparseArray.KeyAt(i) + "=" + sparseArray.ValueAt(i) + ",");
            Console.WriteLine();
        }
    }
}

