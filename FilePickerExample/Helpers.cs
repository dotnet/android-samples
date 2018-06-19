using System.IO;
using System.Linq;

using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V4.Content;
using Android.Views;

namespace com.xamarin.recipes.filepicker
{
    public static class Helpers
    {

        public static readonly int REQUEST_STORAGE =1;

        /// <summary>
        /// Displays a simple text message in a Snackbar on the root view.
        /// </summary>
        /// <param name="activity"></param>
        /// <param name="message"></param>
        public static void ShowSimpleSnackbar(this Activity activity, string message)
        {
            var rootView = activity.FindViewById<View>(Android.Resource.Id.Content);
            Snackbar.Make(rootView, message, Snackbar.LengthShort).Show();
        }
        public static bool PermissionsHaveBeenGranted(this Activity activity)
        {
            return ContextCompat.CheckSelfPermission(activity, Manifest.Permission.ReadExternalStorage ) == Permission.Granted;
        }

        /// <summary>
        ///     Will obtain an instance of a LayoutInflater for the specified Context.
        /// </summary>
        /// <param name="context"> </param>
        /// <returns> </returns>
        public static LayoutInflater GetLayoutInflater(this Context context)
        {
            return context.GetSystemService(Context.LayoutInflaterService).JavaCast<LayoutInflater>();
        }

        /// <summary>
        ///     This method will tell us if the given FileSystemInfo instance is a directory.
        /// </summary>
        /// <param name="fsi"> </param>
        /// <returns> </returns>
        public static bool IsDirectory(this FileSystemInfo fsi)
        {
            if (fsi == null || !fsi.Exists)
            {
                return false;
            }

            return (fsi.Attributes & FileAttributes.Directory) == FileAttributes.Directory;
        }

        /// <summary>
        ///     This method will tell us if the the given FileSystemInfo instance is a file.
        /// </summary>
        /// <param name="fsi"> </param>
        /// <returns> </returns>
        public static bool IsFile(this FileSystemInfo fsi)
        {
            if (fsi == null || !fsi.Exists)
            {
                return false;
            }

            return !IsDirectory(fsi);
        }

        public static bool IsVisible(this FileSystemInfo fsi)
        {
            if (fsi == null || !fsi.Exists)
            {
                return false;
            }

            var isHidden = (fsi.Attributes & FileAttributes.Hidden) == FileAttributes.Hidden;
            return !isHidden;
        }

        /**
		* Check that all given permissions have been granted by verifying that each entry in the
		* given array is of the value Permission.Granted.
		*
		* See Activity#onRequestPermissionsResult (int, String[], int[])
		*/
        public static bool VerifyPermissions(this Permission[] grantResults)
        {
            // At least one result must be checked.
            if (grantResults.Length < 1)
            {
                return false;
            }

            // Verify that each required permission has been granted, otherwise return false.
            return grantResults.All(result => result == Permission.Granted);
        }
    }
}
