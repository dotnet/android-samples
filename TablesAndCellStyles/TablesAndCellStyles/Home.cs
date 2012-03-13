using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

namespace TablesAndCellStyles {
    [Activity(Label = "TablesAndCellStyles", MainLauncher = true, Icon = "@drawable/icon")]
    public class Home : ListActivity {
        
        Home_Adapter adapter;
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            adapter = new Home_Adapter(this);
            ListAdapter = adapter;

            //http://www.netmite.com/android/mydroid/frameworks/base/core/res/res/layout/
            //http://developer.android.com/reference/android/R.layout.html#simple_list_item_1
        }
        protected override void OnListItemClick(ListView l, View v, int position, long id)
        {
            var s = adapter[position];
            var sample = new Intent(this, s.Screen);
            this.StartActivity(sample);
        }
    }
}

