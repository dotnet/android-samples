using Android.App;
using Android.Database;
using Android.OS;
using Android.Widget;

namespace CursorTableAdapter {
    [Activity(Label = "SimpleCursorAdapter", MainLauncher = true, Icon = "@drawable/icon")]
    public class HomeScreen : Activity {

        ListView listView;
        VegetableDatabase vdb;
        ICursor cursor;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.HomeScreen);
            listView = FindViewById<ListView>(Resource.Id.List);

            // create the cursor
            vdb = new VegetableDatabase(this);
            cursor = vdb.ReadableDatabase.RawQuery("SELECT * FROM vegetables", null);
            StartManagingCursor(cursor);

            // which columns map to which layout controls
            string[] fromColumns = new string[] { "name" };
            int[] toControlIds = new int [] {Android.Resource.Id.Text1};

            // use a SimpleCursorAdapter
            listView.Adapter = new SimpleCursorAdapter(this, Android.Resource.Layout.SimpleListItem1, cursor, fromColumns, toControlIds);

            listView.ItemClick += OnListItemClick;
        }

        protected void OnListItemClick(object sender, Android.Widget.AdapterView.ItemClickEventArgs e)
        {
            var obj = listView.Adapter.GetItem(e.Position);
            var curs = (ICursor)obj;
            var text = curs.GetString(1); // 'name' is column 1
            Android.Widget.Toast.MakeText(this, text, Android.Widget.ToastLength.Short).Show();
            System.Console.WriteLine("Clicked on " + text);
        }

        protected override void OnDestroy()
        {
            StopManagingCursor(cursor);
            cursor.Close();

            base.OnDestroy();
        }
    }
}

