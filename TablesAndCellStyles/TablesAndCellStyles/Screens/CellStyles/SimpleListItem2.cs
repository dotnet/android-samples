using System;
using System.Collections.Generic;
using Android.App;
using Android.OS;

namespace TablesAndCellStyles {
    //Since: API Level 1 
    [Activity(Label = "SimpleListItem2")]
    public class SimpleListItem2 : ListActivity {

        List<Tuple<string, string>> items;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            items = new List<Tuple<string, string>>();
            items.Add(new Tuple<string,string>("Fiji", "A nice beach"));
            items.Add(new Tuple<string,string>("Beijing", "AKA Shanghai"));
            items.Add(new Tuple<string,string>("Seedlings", "Tiny plants"));
            items.Add(new Tuple<string,string>("Plants", "Green plants"));

            ListAdapter = new SimpleListItem2_Adapter(this, items);
        }
        protected override void OnListItemClick(Android.Widget.ListView l, Android.Views.View v, int position, long id)
        {
            var t = items[position];
            Android.Widget.Toast.MakeText(this, t.Item1, Android.Widget.ToastLength.Short).Show();
            Console.WriteLine("Clicked on " + t.Item1);
        }
    }
    /*
      <TwoLineListItem xmlns:android="http://schemas.android.com/apk/res/android" 
        android:paddingTop="2dip" 
        android:paddingBottom="2dip" 
        android:layout_width="fill_parent" 
        android:layout_height="wrap_content" 
        android:minHeight="?android:attr/listPreferredItemHeight" 
        android:mode="twoLine">
      <TextView android:id="@android:id/text1" 
        android:layout_width="fill_parent" 
        android:layout_height="wrap_content" 
        android:layout_marginLeft="6dip" 
        android:layout_marginTop="6dip" 
        android:textAppearance="?android:attr/textAppearanceLarge" /> 
      <TextView android:id="@android:id/text2" 
        android:layout_width="fill_parent" 
        android:layout_height="wrap_content" 
        android:layout_below="@android:id/text1" 
        android:layout_alignLeft="@android:id/text1" 
        android:textAppearance="?android:attr/textAppearanceSmall" /> 
      </TwoLineListItem>
     */
}

