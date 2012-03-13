using System;
using Android.App;
using Android.OS;
using Android.Widget;
using System.Collections.Generic;

namespace TablesAndCellStyles {
    
    [Activity(Label = "DateList")]
    public class DateList : ListActivity {

        List<Tuple<string, DateTime>> items;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            items = new List<Tuple<string, DateTime>>();
            items.Add(new Tuple<string, DateTime>("New features of MonoTouch 5.2", new DateTime(2012, 03, 12)));
            items.Add(new Tuple<string, DateTime>("MonoTouch and iOS 5.1", new DateTime(2012, 03, 11)));
            items.Add(new Tuple<string, DateTime>("Introducing the Xamarin Samples Gallery", new DateTime(2012, 03, 05)));
            items.Add(new Tuple<string, DateTime>("Release Candidates and Preview Updates", new DateTime(2012, 03, 01)));
            items.Add(new Tuple<string, DateTime>("Xamarin Adding Support for MIPS Architecture to Mono for Android", new DateTime(2012, 02, 29)));
            

            this.ListAdapter = new DateList_Adapter(this, items);
        }
        protected override void OnListItemClick(Android.Widget.ListView l, Android.Views.View v, int position, long id)
        {
            var t = items[position];
            Android.Widget.Toast.MakeText(this, t.Item1, Android.Widget.ToastLength.Short).Show();
            Console.WriteLine("Clicked on " + t.Item1);
        }
    }
}
/*
   <TextView xmlns:android="http://schemas.android.com/apk/res/android" 
      android:id="@android:id/text1" 
      android:layout_width="fill_parent" 
      android:layout_height="wrap_content" 
      android:textAppearance="?android:attr/textAppearanceLarge" 
      android:gravity="center_vertical" 
      android:paddingLeft="6dip" 
      android:minHeight="?android:attr/listPreferredItemHeight" /> 
 */

