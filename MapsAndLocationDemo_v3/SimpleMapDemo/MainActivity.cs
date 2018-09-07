using System;
using System.Collections.Generic;

using Android.App;
using Android.Content;
using Android.Gms.Common;
using Android.OS;
using Android.Support.V7.App;
using Android.Util;
using Android.Widget;

using Uri = Android.Net.Uri;

namespace SimpleMapDemo
{
    using AndroidUri = Uri;

    [Activity(Label = "@string/app_name", MainLauncher = true, Icon = "@drawable/icon", Theme = "@style/AppTheme")]
    public class MainActivity : AppCompatActivity
    {
        public static readonly int RC_INSTALL_GOOGLE_PLAY_SERVICES = 1000;
        public static readonly string TAG = "XamarinMapDemo";

        // This is a list of the examples that will be display in the Main Activity.
        static readonly List<SampleActivityMetaData> SampleMetaDataList = new List<SampleActivityMetaData>
                                                                          {
                                                                              new SampleActivityMetaData(Resource.String.mapsAppText,
                                                                                                         Resource.String.mapsAppTextDescription,
                                                                                                         null),
                                                                              new SampleActivityMetaData(Resource.String.activity_label_axml,
                                                                                                         Resource.String.activity_description_axml,
                                                                                                         typeof(BasicDemoActivity)),
                                                                              new
                                                                                  SampleActivityMetaData(Resource.String.activity_label_mapwithmarkers,
                                                                                                         Resource
                                                                                                             .String
                                                                                                             .activity_description_mapwithmarkers,
                                                                                                         typeof(MapWithMarkersActivity)),
                                                                              new
                                                                                  SampleActivityMetaData(Resource.String.activity_label_mapwithoverlays,
                                                                                                         Resource
                                                                                                             .String
                                                                                                             .activity_description_mapwithoverlays,
                                                                                                         typeof(MapWithOverlaysActivity)),
                                                                              new SampleActivityMetaData(Resource.String.activity_label_mylocation,
                                                                                                         Resource
                                                                                                             .String.activity_description_mylocation,
                                                                                                         typeof(MyLocationActivity))
                                                                          };

        bool isGooglePlayServicesInstalled;
        SamplesListAdapter listAdapter;
        ListView listView;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.MainActivity);
            isGooglePlayServicesInstalled = TestIfGooglePlayServicesIsInstalled();

            InitializeListView();
        }


        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            if (RC_INSTALL_GOOGLE_PLAY_SERVICES == requestCode && resultCode == Result.Ok)
            {
                isGooglePlayServicesInstalled = true;
            }
            else
            {
                Log.Warn(TAG, $"Don't know how to handle resultCode {resultCode} for request {requestCode}.");
            }
        }


        protected override void OnResume()
        {
            base.OnResume();
            listView.ItemClick += SampleSelected;
        }

        void SampleSelected(object sender, AdapterView.ItemClickEventArgs e)
        {
            var position = e.Position;
            if (position == 0)
            {
                var geoUri = AndroidUri.Parse("geo:42.374260,-71.120824");
                var mapIntent = new Intent(Intent.ActionView, geoUri);
                StartActivity(mapIntent);
                return;
            }


            var sampleToStart = SampleMetaDataList[position];
            sampleToStart.Start(this);
        }

        protected override void OnPause()
        {
            listView.ItemClick -= SampleSelected;
            base.OnPause();
        }


        void InitializeListView()
        {
            listView = FindViewById<ListView>(Resource.Id.listView);
            if (isGooglePlayServicesInstalled)
            {
                listAdapter = new SamplesListAdapter(this, SampleMetaDataList);
            }
            else
            {
                Log.Error(TAG, "Google Play Services is not installed");
                listAdapter = new SamplesListAdapter(this, null);
            }

            listView.Adapter = listAdapter;
        }

        bool TestIfGooglePlayServicesIsInstalled()
        {
            var queryResult = GoogleApiAvailability.Instance.IsGooglePlayServicesAvailable(this);
            if (queryResult == ConnectionResult.Success)
            {
                Log.Info(TAG, "Google Play Services is installed on this device.");
                return true;
            }

            if (GoogleApiAvailability.Instance.IsUserResolvableError(queryResult))
            {
                var errorString = GoogleApiAvailability.Instance.GetErrorString(queryResult);
                Log.Error(TAG, "There is a problem with Google Play Services on this device: {0} - {1}", queryResult, errorString);
                var errorDialog = GoogleApiAvailability.Instance.GetErrorDialog(this, queryResult, RC_INSTALL_GOOGLE_PLAY_SERVICES);
                var dialogFrag = new ErrorDialogFragment(errorDialog);

                dialogFrag.Show(FragmentManager, "GooglePlayServicesDialog");
            }

            return false;
        }
    }
}
