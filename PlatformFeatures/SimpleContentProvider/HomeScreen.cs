using Android.App;
using Android.Database;
using Android.OS;
using Android.Widget;
using Android.Content;
using Android.Net;

namespace CursorTableAdapter {
    [Activity(Label = "SimpleContentProvider", MainLauncher = true, Icon = "@drawable/icon")]
    public class HomeScreen : Activity { //, LoaderManager.ILoaderCallbacks {

        ListView listView;
        ICursor cursor;
        SimpleCursorAdapter adapter;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.HomeScreen);
            listView = FindViewById<ListView>(Resource.Id.List);

            string[] projection = new string[] { VegetableProvider.InterfaceConsts.Id, VegetableProvider.InterfaceConsts.Name };
            string[] fromColumns = new string[] { VegetableProvider.InterfaceConsts.Name };
            int[] toControlIds = new int[] { Android.Resource.Id.Text1 };

            // ManagedQuery is deprecated in Honeycomb (3.0, API11)
            cursor = ManagedQuery(VegetableProvider.CONTENT_URI, projection, null, null, null);
            
            // ContentResolver requires you to close the query yourself
            //cursor = ContentResolver.Query(VegetableProvider.CONTENT_URI, projection, null, null, null);
            //StartManagingCursor(cursor);

            // CursorLoader introduced in Honeycomb (3.0, API11)
            var loader = new CursorLoader(this,
                VegetableProvider.CONTENT_URI, projection, null, null, null);
            cursor = (ICursor)loader.LoadInBackground();

            // create a SimpleCursorAdapter
            adapter = new SimpleCursorAdapter(this, Android.Resource.Layout.SimpleListItem1, cursor, 
                fromColumns, toControlIds);

            listView.Adapter = adapter;
            
            listView.ItemClick += OnListItemClick;
        }

        protected void OnListItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            var id = e.Id;
            string[] projection = new string[] { "name" };
            var uri = Uri.WithAppendedPath(VegetableProvider.CONTENT_URI, id.ToString());

            ICursor vegeCursor = ContentResolver.Query(uri, projection, null, new string[] { id.ToString() }, null);

            string text = "";
            if (vegeCursor.MoveToFirst()) {
                text = vegeCursor.GetInt(0) + " " + vegeCursor.GetString(1);
                Android.Widget.Toast.MakeText(this, text, Android.Widget.ToastLength.Short).Show();
            }

            vegeCursor.Close();

            System.Console.WriteLine("ClickUri" + uri.ToString());
            System.Console.WriteLine("Clicked on " + text);
        }

        protected override void OnDestroy()
        {
            // ONLY if cursor was created with ContentResolver.Query
            //StopManagingCursor(cursor);
            //cursor.Close();

            base.OnDestroy();
        }
        ///// <summary>
        ///// This is called whena new Loader needs to be created. This
        ///// sample only has one loader, so we don't care about the ID.
        ///// </summary>
        //public Android.Content.Loader OnCreateLoader(int id, Bundle args)
        //{
        //    return new CursorLoader(this);
        //}

        //public void OnLoadFinished(Android.Content.Loader loader, Java.Lang.Object data)
        //{
        //    adapter.SwapCursor((ICursor)data);
        //}
        ///// <summary>
        ///// This is called when the last Cursor produced to OnLoadFinished()
        ///// above is about to be closed. We need to make sure we are no longer using it.
        ///// </summary>
        //public void OnLoaderReset(Android.Content.Loader loader)
        //{
        //    adapter.SwapCursor(null);
        //}
    }
}

