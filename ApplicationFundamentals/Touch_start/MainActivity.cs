namespace TouchWalkthrough
{
    using Android.App;
    using Android.Content;
    using Android.OS;
    using Android.Views;
    using Android.Widget;

    /// <summary>
    ///   The main activity is implemented as a ListActivity. When the user
    ///   clicks on a item in the ListView, we will display the appropriate activity.
    /// </summary>
    [Activity(Label = "@string/app_name", MainLauncher = true, Icon = "@drawable/ic_launcher")]
    public class MainActivity : ListActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            ListAdapter = new MainMenuAdapter(this);
        }

        protected override void OnListItemClick(ListView l, View v, int position, long id)
        {
            Intent startActivity = (Intent)ListAdapter.GetItem(position);
            StartActivity(startActivity);
        }
    }
}
