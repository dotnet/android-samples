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

    [Activity(Label = "@string/app_name", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : AppCompatActivity
    {
        public static readonly int InstallGooglePlayServicesId = 1000;
        public static readonly string Tag = "XamarinMapDemo";

        static readonly List<SampleMetaData> SampleMetaDataList = new List<SampleMetaData>
                                                                  {
                                                                      new SampleMetaData(Resource.String.mapsAppText,
                                                                                         Resource.String.mapsAppTextDescription, null),
                                                                      new SampleMetaData(Resource.String.activity_label_axml,
                                                                                         Resource.String.activity_description_axml,
                                                                                         typeof(BasicDemoActivity)),
                                                                      new SampleMetaData(Resource.String.activity_label_mapwithmarkers,
                                                                                         Resource.String.activity_description_mapwithmarkers,
                                                                                         typeof(MapWithMarkersActivity)),
                                                                      new SampleMetaData(Resource.String.activity_label_mapwithoverlays,
                                                                                         Resource.String.activity_description_mapwithoverlays,
                                                                                         typeof(MapWithOverlaysActivity))
                                                                  };

        bool isGooglePlayServicesInstalled;
        SamplesListAdapter listAdapter;
        ListView listView;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.MainActivity);
            isGooglePlayServicesInstalled = TestIfGooglePlayServicesIsInstalled();

            InitializeListView();
        }


        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            switch (resultCode)
            {
                case Result.Ok:
                    // Try again.
                    isGooglePlayServicesInstalled = true;
                    break;

                default:
                    Log.Debug("MainActivity", "Unknown resultCode {0} for request {1}", resultCode, requestCode);
                    break;
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
                Log.Error("MainActivity", "Google Play Services is not installed");
                listAdapter = new SamplesListAdapter(this, null);
            }

            listView.Adapter = listAdapter;
        }

        bool TestIfGooglePlayServicesIsInstalled()
        {
            var queryResult = GoogleApiAvailability.Instance.IsGooglePlayServicesAvailable(this);
            if (queryResult == ConnectionResult.Success)
            {
                Log.Info(Tag, "Google Play Services is installed on this device.");
                return true;
            }

            if (GoogleApiAvailability.Instance.IsUserResolvableError(queryResult))
            {
                var errorString = GoogleApiAvailability.Instance.GetErrorString(queryResult);
                Log.Error(Tag, "There is a problem with Google Play Services on this device: {0} - {1}", queryResult, errorString);
                var errorDialog = GoogleApiAvailability.Instance.GetErrorDialog(this, queryResult, InstallGooglePlayServicesId);
                var dialogFrag = new ErrorDialogFragment(errorDialog);

                dialogFrag.Show(FragmentManager, "GooglePlayServicesDialog");
            }

            return false;
        }
    }
}
