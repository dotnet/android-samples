namespace SimpleMapDemo
{
    using Android.App;
    using Android.Gms.Maps;
    using Android.Gms.Maps.Model;
    using Android.OS;
    using Android.Widget;

    [Activity(Label = "@string/activity_label_mapwithmarkers")]
    public class MapWithMarkersActivity : Activity
    {
        private static readonly LatLng Passchendaele = new LatLng(50.897778, 3.013333);
        private static readonly LatLng VimyRidge = new LatLng(50.379444, 2.773611);
        private GoogleMap _map;
        private MapFragment _mapFragment;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.MapLayout);

            InitMapFragment();

            SetupAnimateToButton();
            SetupZoomInButton();
            SetupZoomOutButton();
        }

        protected override void OnResume()
        {
            base.OnResume();
            SetupMapIfNeeded();
        }

        private void InitMapFragment()
        {
            _mapFragment = FragmentManager.FindFragmentByTag("map") as MapFragment;
            if (_mapFragment == null)
            {
                GoogleMapOptions mapOptions = new GoogleMapOptions()
                    .InvokeMapType(GoogleMap.MapTypeSatellite)
                    .InvokeZoomControlsEnabled(false)
                    .InvokeCompassEnabled(true);

                FragmentTransaction fragTx = FragmentManager.BeginTransaction();
                _mapFragment = MapFragment.NewInstance(mapOptions);
                fragTx.Add(Resource.Id.map, _mapFragment, "map");
                fragTx.Commit();
            }
        }

        private void SetupAnimateToButton()
        {
            Button animateButton = FindViewById<Button>(Resource.Id.animateButton);
            animateButton.Click += (sender, e) =>{
                // Move the camera to the Passchendaele Memorial in Belgium.
                CameraPosition.Builder builder = CameraPosition.InvokeBuilder();
                builder.Target(Passchendaele);
                builder.Zoom(18);
                builder.Bearing(155);
                builder.Tilt(65);
                CameraPosition cameraPosition = builder.Build();

                // AnimateCamera provides a smooth, animation effect while moving
                // the camera to the the position.

                _map.AnimateCamera(CameraUpdateFactory.NewCameraPosition(cameraPosition));
            };
        }

        private void SetupMapIfNeeded()
        {
            if (_map == null)
            {
                _map = _mapFragment.Map;
                if (_map != null)
                {
                    MarkerOptions markerOpt1 = new MarkerOptions();
                    markerOpt1.SetPosition(VimyRidge);
                    markerOpt1.SetTitle("Vimy Ridge");
                    markerOpt1.InvokeIcon(BitmapDescriptorFactory.DefaultMarker(BitmapDescriptorFactory.HueCyan));
                    _map.AddMarker(markerOpt1);

                    MarkerOptions markerOpt2 = new MarkerOptions();
                    markerOpt2.SetPosition(Passchendaele);
                    markerOpt2.SetTitle("Passchendaele");
                    _map.AddMarker(markerOpt2);

                    // We create an instance of CameraUpdate, and move the map to it.
                    CameraUpdate cameraUpdate = CameraUpdateFactory.NewLatLngZoom(VimyRidge, 15);
                    _map.MoveCamera(cameraUpdate);
                }
            }
        }

        private void SetupZoomInButton()
        {
            Button zoomInButton = FindViewById<Button>(Resource.Id.zoomInButton);
            zoomInButton.Click += (sender, e) => { _map.AnimateCamera(CameraUpdateFactory.ZoomIn()); };
        }

        private void SetupZoomOutButton()
        {
            Button zoomOutButton = FindViewById<Button>(Resource.Id.zoomOutButton);
            zoomOutButton.Click += (sender, e) => { _map.AnimateCamera(CameraUpdateFactory.ZoomOut()); };
        }
    }
}
