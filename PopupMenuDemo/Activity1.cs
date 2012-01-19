using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

namespace PopupMenuDemo
{
    [Activity (Label = "PopupMenuDemo", MainLauncher = true)]
    public class Activity1 : Activity
    {
        protected override void OnCreate (Bundle bundle)
        {
            base.OnCreate (bundle);

            SetContentView (Resource.Layout.Main);

            Button showPopupMenu = FindViewById<Button> (Resource.Id.popupButton);
            
            showPopupMenu.Click += (s, arg) => {
                
                PopupMenu menu = new PopupMenu (this, showPopupMenu);
                
                // with Android 3 need to use MenuInfater to inflate the menu
                //menu.MenuInflater.Inflate (Resource.Menu.popup_menu, menu.Menu);
                
                // with Android 4 Inflate can be called directly on the menu
                menu.Inflate (Resource.Menu.popup_menu);
                
                menu.MenuItemClick += (s1, arg1) => {
                    Console.WriteLine ("{0} selected", arg1.Item.TitleFormatted);
                };
                
                // Android 4 now has the DismissEvent
                menu.DismissEvent += (s2, arg2) => {
                    Console.WriteLine ("menu dismissed"); 
                };
                
                menu.Show ();
            };
        }
    }
}