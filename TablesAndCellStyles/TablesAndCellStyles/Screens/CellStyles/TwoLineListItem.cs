using System;
using System.Collections.Generic;
using Android.App;
using Android.OS;

namespace TablesAndCellStyles {
    //Since: API Level 1 
    [Activity(Label = "TwoLineListItem")]
    public class TwoLineListItem : ListActivity {
        
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            var items = new List<Tuple<string, string>>();
            items.Add(new Tuple<string,string>("Fiji", "A nice beach"));
            items.Add(new Tuple<string,string>("Beijing", "AKA Shanghai"));
            items.Add(new Tuple<string,string>("Seedlings", "Tiny plants"));
            items.Add(new Tuple<string,string>("Plants", "Green plants"));

            ListAdapter = new TwoLineListItem_Adapter(this, items);
        }
        
    }
    /*
      <LinearLayout xmlns:android="http://schemas.android.com/apk/res/android" android:layout_width="fill_parent" android:layout_height="wrap_content" android:orientation="vertical">
      <TextView android:id="@android:id/text1" android:textSize="16sp" android:textStyle="bold" android:layout_width="fill_parent" android:layout_height="wrap_content" /> 
      <TextView android:id="@android:id/text2" android:textSize="16sp" android:layout_width="fill_parent" android:layout_height="wrap_content" /> 
      </LinearLayout>
     */
}

