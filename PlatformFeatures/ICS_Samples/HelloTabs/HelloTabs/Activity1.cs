using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

namespace HelloTabs
{
    [Activity (MainLauncher=true, Label="HelloTabs")]
    public class Activity1 : TabActivity
    {
        protected override void OnCreate (Bundle bundle)
        {
            base.OnCreate (bundle);
            SetContentView (Resource.Layout.Main);

            TabHost.TabSpec spec;
            Intent intent;

            intent = new Intent (this, typeof(Activity1));
            intent.AddFlags (ActivityFlags.NewTask);

            intent = new Intent (this, typeof(Activity2));
            intent.AddFlags (ActivityFlags.NewTask);

            spec = TabHost.NewTabSpec ("tab1");
            spec.SetIndicator ("Tab 1", Resources.GetDrawable (Resource.Drawable.ic_tab));
            spec.SetContent (intent);
            TabHost.AddTab (spec);

            intent = new Intent (this, typeof(Activity3));
            intent.AddFlags (ActivityFlags.NewTask);

            spec = TabHost.NewTabSpec ("tab2");
            spec.SetIndicator ("Tab 2", Resources.GetDrawable (Resource.Drawable.ic_tab));
            spec.SetContent (intent);
            TabHost.AddTab (spec);

            TabHost.CurrentTab = 0;
        }
    }
}


