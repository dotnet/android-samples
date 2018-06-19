using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Android;
using Android.Content.PM;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V4.App;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace com.xamarin.recipes.filepicker
{
    /// <summary>
    ///     A ListFragment that will show the files and subdirectories of a given directory.
    /// </summary>
    /// <remarks>
    ///     <para> This was placed into a ListFragment to make this easier to share this functionality with with tablets. </para>
    ///     <para>
    ///         Note that this is a incomplete example. It lacks things such as the ability to go back up the directory
    ///         tree, or any special handling of a file when it is selected.
    ///     </para>
    /// </remarks>
    public class FileListFragment : ListFragment
    {
        public static readonly string DefaultInitialDirectory = "/sdcard";
        FileListAdapter adapter;
        DirectoryInfo directoryInfo;
        Action<View> requestPermissionHandler;
        static string[] REQUIRED_PERMISSIONS = new[] {Manifest.Permission.ReadExternalStorage};

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            adapter = new FileListAdapter(Activity, new FileSystemInfo[0]);
            ListAdapter = adapter;

        }

        public override void OnListItemClick(ListView l, View v, int position, long id)
        {
            var fileSystemInfo = adapter.GetItem(position);

            if (fileSystemInfo.IsFile())
            {
                // Do something with the file.  In this case we just pop some toast.
                Log.Verbose("FileListFragment", "The file {0} was clicked.", fileSystemInfo.FullName);
                Toast.MakeText(Activity, "You selected file " + fileSystemInfo.FullName, ToastLength.Short).Show();
            }
            else
            {
                // Dig into this directory, and display it's contents
                RefreshFilesList(fileSystemInfo.FullName);
            }

            base.OnListItemClick(l, v, position, id);
        }

        public override void OnResume()
        {
            base.OnResume();
            if (Activity.PermissionsHaveBeenGranted())
            {
                RefreshFilesList(DefaultInitialDirectory);
            }
            else
            {
                RequestExternalStoragePermissions();
            }
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            if (requestCode == Helpers.REQUEST_STORAGE)
            {
                if (grantResults.VerifyPermissions())
                {
                    RefreshFilesList(DefaultInitialDirectory);
                }
            }
            else
            {
                base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            }
        }

        void RequestExternalStoragePermissions()
        {
            if (ActivityCompat.ShouldShowRequestPermissionRationale(Activity, Manifest.Permission.ReadExternalStorage))
            {
                var rootView = Activity.FindViewById<View>(Android.Resource.Id.Content);
                requestPermissionHandler = delegate
                                           {
                                               ActivityCompat.RequestPermissions(Activity, REQUIRED_PERMISSIONS, Helpers.REQUEST_STORAGE);
                                           };
                Snackbar.Make(rootView, Resource.String.permission_externalstorage_rationale, Snackbar.LengthIndefinite)
                        .SetAction(Resource.String.ok, requestPermissionHandler)
                        .Show();
            }
            else
            {
                ActivityCompat.RequestPermissions(Activity, REQUIRED_PERMISSIONS, Helpers.REQUEST_STORAGE);
            }
        }
        public void RefreshFilesList(string directory)
        {

            if (!Activity.PermissionsHaveBeenGranted())
            {
                Activity.ShowSimpleSnackbar("Don't have permissions to read external storage.");
                return;
            }

            IList<FileSystemInfo> visibleThings = new List<FileSystemInfo>();
            var dir = new DirectoryInfo(directory);

            try
            {
                foreach (var item in dir.GetFileSystemInfos().Where(item => item.IsVisible()))
                {
                    visibleThings.Add(item);
                }
            }
            catch (Exception ex)
            {
                Log.Error("FileListFragment", "Couldn't access the directory " + dir.FullName + "; " + ex);
                Toast.MakeText(Activity, "Problem retrieving contents of " + directory, ToastLength.Long).Show();
                return;
            }

            directoryInfo = dir;
            adapter.AddDirectoryContents(visibleThings);

            // If we don't do this, then the ListView will not update itself when then data set 
            // in the adapter changes. It will appear to the user that nothing has happened.
            ListView.RefreshDrawableState();

            Log.Verbose("FileListFragment", "Displaying the contents of directory {0}.", directory);
        }

        public void NavigateUpOneDirectory()
        {
            var parentDir = directoryInfo.Parent;
            var nextDir = directoryInfo.Parent.FullName;

            if ("/".Equals(nextDir))
            {
                RefreshFilesList(DefaultInitialDirectory);
            }
            else
            {
                RefreshFilesList(parentDir.FullName);
            }
        }
    }
}
