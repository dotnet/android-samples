using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;

namespace StandardControls {
    [Activity(Label = "StandardControls", MainLauncher = true, Icon = "@drawable/icon")]
    public class Home : ListActivity {
        
        Home_Adapter adapter;
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            adapter = new Home_Adapter(this);
            ListAdapter = adapter;
        }
        protected override void OnListItemClick(ListView l, View v, int position, long id)
        {
            var s = adapter[position];
            var sample = new Intent(this, s.Screen);
            this.StartActivity(sample);
        }
    }
}

