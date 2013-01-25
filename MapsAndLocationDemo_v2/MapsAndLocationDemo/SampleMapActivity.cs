namespace MapsAndLocationDemo
{
    using Android.App;
    using Android.Content.PM;
    using Android.Gms.Maps;
    using Android.Gms.Maps.Model;
    using Android.OS;
    using Android.Support.V4.App;
    using Android.Widget;

    using Debug = System.Diagnostics.Debug;

    [Activity(Label = "@string/activity_label_samplemap", ConfigurationChanges=ConfigChanges.Orientation)]
    public class SampleMapActivity : FragmentActivity
    {
        private static readonly LatLng Passchendaele = new LatLng(50.897778, 3.013333);
        private static readonly LatLng VimyRidge = new LatLng(50.379444, 2.773611);
        private GoogleMap _map;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.MapLayout);

            InitMap();
            SetupAnimateToButton();
            SetupZoomInButton();
            SetupZoomOutButton();
        }

        protected override void OnResume()
        {
            base.OnResume();
            var mapFragment =  (SupportMapFragment) SupportFragmentManager.FindFragmentByTag("map");

            // The value of mapFragment.Map may be null if the mapFragment isn't completely initialize yet.
            // This will cause problems with other things too, like the CameraUpdateFactory.
            // By initializing our GoogleMap here in OnResume, the MapFragment should be
            // properly instantiated and ready for use.
            _map = mapFragment.Map;
            Debug.Assert(_map != null, "The _map cannot be null!");

            // We create an instance of CameraUpdate, and move the map to it.
            var cameraUpdate = CameraUpdateFactory.NewLatLngZoom(VimyRidge, 15);
            _map.MoveCamera(cameraUpdate);
        }

        /// <summary>
        ///   All we do here is add a SupportMapFragment
        /// </summary>
        private void InitMap()
        {
            var mapOptions = new GoogleMapOptions()
                .InvokeMapType(GoogleMap.MapTypeSatellite)
                .InvokeZoomControlsEnabled(false)
                .InvokeCompassEnabled(true);

            var fragTx = SupportFragmentManager.BeginTransaction();
            var mapFragment = SupportMapFragment.NewInstance(mapOptions);
            fragTx.Add(Resource.Id.map, mapFragment, "map");
            fragTx.Commit();
        }

        private void SetupAnimateToButton()
        {
            var animateButton = FindViewById<Button>(Resource.Id.animateButton);
            animateButton.Click += (sender, e) =>
                                       {
                                           // Move the camera to the Passchendaele Memorial in Belgium.
                                           var builder = CameraPosition.InvokeBuilder();
                                           builder.Target(Passchendaele);
                                           builder.Zoom(18);
                                           builder.Bearing(155);
                                           builder.Tilt(25);
                                           var cameraPosition = builder.Build();

                                           // AnimateCamera provides a smooth, animation effect while moving
                                           // the camera to the the position.
                                           _map.AnimateCamera(CameraUpdateFactory.NewCameraPosition(cameraPosition));
                                       };
        }

        private void SetupZoomInButton()
        {
            var zoomInButton = FindViewById<Button>(Resource.Id.zoomInButton);
            zoomInButton.Click += (sender, e) => { _map.AnimateCamera(CameraUpdateFactory.ZoomIn()); };
        }

        private void SetupZoomOutButton()
        {
            var zoomOutButton = FindViewById<Button>(Resource.Id.zoomOutButton);
            zoomOutButton.Click += (sender, e) => { _map.AnimateCamera(CameraUpdateFactory.ZoomOut()); };
        }
    }
}
