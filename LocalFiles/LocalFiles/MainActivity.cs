using System.Threading.Tasks;

using Android;
using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Util;
using Android.Widget;

namespace LocalFiles
{
    [Activity(Label = "@string/app_name", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        const int RC_WRITE_EXTERNAL_STORAGE_PERMISSION = 1000;
        const int RC_READ_EXTERNAL_STORAGE_PERMISSION = 1100;
        const int RC_DELETE_STORAGE_FILE = 1200;

        static readonly string TAG = "LF:MainActivity";
        static readonly string[] PERMISSIONS_TO_REQUEST = {Manifest.Permission.WriteExternalStorage};

        IGenerateNameOfFile filenameGenerator;
        CountOfClicksFileStorage fileStorage;
        Button saveCountButton;
        Button incrementCountButton;
        TextView storedValueTextView;

        protected override async void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.Main);
            InitializeDependencies();

            storedValueTextView = FindViewById<TextView>(Resource.Id.stored);

            FindViewById<TextView>(Resource.Id.filename_generator).Text = filenameGenerator.GetType().Name;

            var pathToFileTextView = FindViewById<TextView>(Resource.Id.path);
            fileStorage.DisplayPathIn(filenameGenerator, pathToFileTextView);

            incrementCountButton = FindViewById<Button>(Resource.Id.myButton);
            incrementCountButton.Click += delegate
                                          {
                                              incrementCountButton.Text = string.Format(GetString(Resource.String.click_count), ++fileStorage.Count);
                                          };

            saveCountButton = FindViewById<Button>(Resource.Id.btnSave);
            saveCountButton.Click += async (sender, args) =>
                                     {
                                         if (RequestExternalStoragePermissionIfNecessary(RC_WRITE_EXTERNAL_STORAGE_PERMISSION))
                                         {
                                             await WriteCountToFileAsync();
                                         }
                                     };

            var resetCountButton = FindViewById<Button>(Resource.Id.btnReset);
            resetCountButton.Click += async (sender, args) =>
                                      {
                                          if (RequestExternalStoragePermissionIfNecessary(RC_DELETE_STORAGE_FILE))
                                          {
                                              await DeleteCountFileAsync();
                                          }
                                      };

            await LoadCountFromFileAsync();
        }

        public override async void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            switch (requestCode)
            {
                case RC_READ_EXTERNAL_STORAGE_PERMISSION:
                    if (grantResults.Length == 1 && grantResults[0] == Permission.Granted)
                    {
                        await WriteCountToFileAsync();
                    }

                    break;
                case RC_WRITE_EXTERNAL_STORAGE_PERMISSION:
                    if (grantResults.Length == 1 && grantResults[0] == Permission.Granted)
                    {
                        await LoadCountFromFileAsync();
                    }

                    break;
                case RC_DELETE_STORAGE_FILE:
                    await DeleteCountFileAsync();
                    break;
                default:
                    base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
                    break;
            }
        }

        void InitializeDependencies()
        {
            fileStorage = new CountOfClicksFileStorage();

            if (Environment.MediaMounted.Equals(Environment.ExternalStorageState))
            {
                filenameGenerator = new ExternalStorageFilenameGenerator(this);
            }
            else
            {
                filenameGenerator = new InternalCacheFilenameGenerator(this);
            }

            Log.Info(TAG, "Using the " + filenameGenerator.GetType().Name + " for the internal storage.");
        }

        bool RequestExternalStoragePermissionIfNecessary(int requestCode)
        {
            if (Environment.MediaMounted.Equals(Environment.ExternalStorageState))
            {
                if (CheckSelfPermission(Manifest.Permission.WriteExternalStorage) == Permission.Granted)
                {
                    return true;
                }

                if (ShouldShowRequestPermissionRationale(Manifest.Permission.WriteExternalStorage))
                {
                    Snackbar.Make(FindViewById(Android.Resource.Id.Content),
                                  Resource.String.write_external_permissions_rationale,
                                  Snackbar.LengthIndefinite)
                            .SetAction(Resource.String.ok, delegate { RequestPermissions(PERMISSIONS_TO_REQUEST, requestCode); });
                }
                else
                {
                    RequestPermissions(PERMISSIONS_TO_REQUEST, requestCode);
                }

                return false;
            }

            Log.Warn(TAG, "External storage is not mounted; cannot request permission");
            return false;
        }

        async Task WriteCountToFileAsync()
        {
            await fileStorage.WriteFileAsync(filenameGenerator, fileStorage.Count);
            storedValueTextView.Text = string.Format(GetString(Resource.String.stored), fileStorage.Count);

        }

        async Task LoadCountFromFileAsync()
        {
            if (RequestExternalStoragePermissionIfNecessary(RC_READ_EXTERNAL_STORAGE_PERMISSION))
            {
                var count = await fileStorage.ReadFileAsync(filenameGenerator);
                fileStorage.Count = count;
                storedValueTextView.Text = string.Format(GetString(Resource.String.stored), count);
            }
            else
            {
                storedValueTextView.SetText(Resource.String.cannot_load_count);
            }
        }

        async Task DeleteCountFileAsync()
        {
            fileStorage.Count = 0;

            await fileStorage.DeleteFileAsync(filenameGenerator);

            storedValueTextView.Text = string.Format(GetString(Resource.String.stored), 0);
            incrementCountButton.SetText(Resource.String.click_to_increment);
        }
    }
}
