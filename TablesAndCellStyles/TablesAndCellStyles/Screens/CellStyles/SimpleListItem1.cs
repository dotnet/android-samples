using System;
using Android.App;
using Android.OS;
using Android.Widget;

namespace TablesAndCellStyles {
    
    //Since: API Level 1 
    [Activity(Label = "SimpleListItem1")]
    public class SimpleListItem1 : ListActivity {

        string[] items;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            items = new string []{"Row 1", "Row 2", "Row 3"};

            this.ListAdapter = new ArrayAdapter<String> (this, Android.Resource.Layout.SimpleListItem1, items);
        }
        protected override void OnListItemClick(Android.Widget.ListView l, Android.Views.View v, int position, long id)
        {
            var t = items[position];
            Android.Widget.Toast.MakeText(this, t, Android.Widget.ToastLength.Short).Show();
            Console.WriteLine("Clicked on " + t);
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

