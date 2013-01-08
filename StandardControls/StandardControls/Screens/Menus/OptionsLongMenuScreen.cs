using System;
using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;

namespace StandardControls {

    [Activity(Label = "OptionsLongMenu")]
    public class OptionsLongMenuScreen : Activity { //, IMenuItemOnMenuItemClickListener {
        
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.OptionsMenu);
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            // GroupId, ItemId, OrderId
            menu.Add(0, 0, 0, "Item 1").SetIcon(Android.Resource.Drawable.IcDialogEmail);
            menu.Add(0, 1, 1, "Item 2").SetIcon(Resource.Drawable.icon);
            menu.Add(0, 2, 2, "Item 3").SetIcon(Android.Resource.Drawable.IcDialogMap);
            menu.Add(0, 3, 3, "Item 4").SetIcon(Android.Resource.Drawable.IcDialogInfo);
            menu.Add(0, 4, 4, "Item 5").SetIcon(Android.Resource.Drawable.IcMenuCamera);
            menu.Add(0, 5, 5, "Item 6").SetIcon(Android.Resource.Drawable.IcInputAdd);
            menu.Add(0, 6, 6, "Item 7").SetIcon(Android.Resource.Drawable.IcMenuAdd);
            menu.Add(0, 7, 7, "Item 8").SetIcon(Resource.Drawable.Beach); ;
            return true;
        }

        //public bool OnMenuItemClick(IMenuItem item)
        //{
        //    var id = item.ItemId;
        //    Console.WriteLine(id + " option menuitem clicked");
        //    var t = Toast.MakeText(this, "Options Menu '"+id+"' clicked", ToastLength.Short);
        //    t.SetGravity(GravityFlags.Top, 0, 0);
        //    t.Show();
        //    return false; // do not execute any other callbacks
        //}

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
           // Console.WriteLine("Shouldn't get called, unless OnMenuItemClick returns 'true'");
            
            var id = item.ItemId + 1; // (Id is zero-based :)
            Console.WriteLine(id + " option menuitem clicked");
            var t = Toast.MakeText(this, "Options Menu '" + id + "' clicked", ToastLength.Short);
            t.SetGravity(GravityFlags.Center, 0, 0);
            t.Show();

            return true;
        }
    }
}