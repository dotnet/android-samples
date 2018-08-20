using System;

using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Util;

using com.xamarin.sample.fragments;

namespace com.xamarin.sample.fragments
{
    [Activity(Label = "@string/app_name", MainLauncher = true, Icon = "@drawable/launcher")]
    public class MainActivity : AppCompatActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            try
            {
                SetContentView(Resource.Layout.activity_main);
            }
            catch (Exception ex)
            {
                Log.Debug("MainActivity", ex.Message);
            }
        }
    }
}
