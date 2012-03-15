using System;
using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;

namespace StandardControls {

    [Activity(Label = "OptionsMenu")]
    public class OptionsMenuScreen : Activity {
        
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.OptionsMenu);
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            menu.Add("Item 1");
            menu.Add("Item 2");
            menu.Add("Item 3");
            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.TitleFormatted.ToString()) { 
            case "Item 1":
                MenuItemClicked(item.TitleFormatted.ToString()); break;
            case "Item 2":
                MenuItemClicked(item.TitleFormatted.ToString()); break;
            case "Item 3":
                MenuItemClicked(item.TitleFormatted.ToString()); break;
            }
            return base.OnOptionsItemSelected(item);
        }

        void MenuItemClicked(string item)
        {
            Console.WriteLine(item + " option menuitem clicked");
            var t = Toast.MakeText(this, "Options Menu '"+item+"' clicked", ToastLength.Short);
            t.SetGravity(GravityFlags.Center, 0, 0);
            t.Show();
        }
    }
}