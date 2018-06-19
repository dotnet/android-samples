using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Support.V7.App;

namespace com.xamarin.recipes.filepicker
{
    [Activity(Label = "@string/app_name", MainLauncher = true, Icon = "@drawable/ic_launcher", Theme="@style/AppTheme")]
    public class FilePickerActivity : AppCompatActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.main);
        }


        public override void OnBackPressed()
        {
//            base.OnBackPressed();

            var fileListFrag = (FileListFragment) SupportFragmentManager.FindFragmentById(Resource.Id.file_list_fragment);
            fileListFrag.NavigateUpOneDirectory();

        }
    }
}
