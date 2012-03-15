using System;
using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;

namespace StandardControls {

    [Activity(Label = "ContextMenu")]
    public class ContextMenuScreen : Activity {

        TextView contextMenuText;
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.ContextMenu);

            contextMenuText = FindViewById<TextView>(Resource.Id.ContextMenuText);
            RegisterForContextMenu(contextMenuText);
        }

        public override void OnCreateContextMenu(IContextMenu menu, View v, IContextMenuContextMenuInfo info)
        {
            menu.SetHeaderTitle("Context Menu");
            menu.Add(0, 0, 0, "Text version 1");
            menu.Add(0, 1, 0, "Text version 2");
        }

        public override bool OnContextItemSelected(IMenuItem item)
        {
            switch (item.ItemId) { 
            case 0:
                contextMenuText.Text = "Context menu changed text 1";
                ContextItemClicked(item.TitleFormatted.ToString()); break;
            case 1:
                contextMenuText.Text = "Context menu changed text 2";
                ContextItemClicked(item.TitleFormatted.ToString()); break;
            }
            return base.OnOptionsItemSelected(item);
        }

        void ContextItemClicked(string item)
        {
            Console.WriteLine(item + " option menuitem clicked");
            var t = Toast.MakeText(this, "Options Menu '"+item+"' clicked", ToastLength.Short);
            t.Show();
        }
    }
}