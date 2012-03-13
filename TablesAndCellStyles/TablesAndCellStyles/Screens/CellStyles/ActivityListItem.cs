using System;
using System.Collections.Generic;
using Android.App;
using Android.OS;

namespace TablesAndCellStyles {
    [Activity(Label = "ActivityListItem")]
    public class ActivityListItem : ListActivity {
        
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            var items = new List<Tuple<string, int>>();
            items.Add(new Tuple<string, int>("Fiji", Resource.Drawable.Beach));
            items.Add(new Tuple<string, int>("Beijing", Resource.Drawable.Shanghai));
            items.Add(new Tuple<string, int>("Seedlings", Resource.Drawable.Seeds));
            items.Add(new Tuple<string, int>("Plants", Resource.Drawable.Plants));

            this.ListAdapter = new ActivityListItem_Adapter(this, items);
        }
        
    }
    /*
      <LinearLayout xmlns:android="http://schemas.android.com/apk/res/android" 
          android:layout_width="fill_parent" 
          android:layout_height="wrap_content" 
          android:paddingTop="1dip" 
          android:paddingBottom="1dip" 
          android:paddingLeft="6dip" 
          android:paddingRight="6dip">
        <ImageView android:id="@+id/icon" 
            android:layout_width="24dip" 
            android:layout_height="24dip" /> 
        <TextView android:id="@android:id/text1" 
            android:layout_width="wrap_content" 
            android:layout_height="wrap_content" 
            android:layout_gravity="center_horizontal" 
            android:paddingLeft="6dip" /> 
       </LinearLayout>
     */
}

