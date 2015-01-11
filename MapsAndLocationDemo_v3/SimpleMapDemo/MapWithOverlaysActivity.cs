namespace SimpleMapDemo
{
    using System;

    using Android.App;
    using Android.Gms.Maps;
    using Android.Gms.Maps.Model;
    using Android.OS;
    using Android.Widget;

    [Activity(Label = "@string/activity_label_mapwithoverlays")]
    public class MapWithOverlaysActivity : Activity
    {
        private static readonly LatLng InMaui = new LatLng(20.72110, -156.44776);
        private static readonly LatLng LeaveFromHereToMaui = new LatLng(82.4986, -62.348);
        private static readonly LatLng[] LocationForCustomIconMarkers = new[]
                                                                            {
                                                                                new LatLng(40.741773, -74.004986),
                                                                                new LatLng(41.051696, -73.545667),
                                                                                new LatLng(41.311197, -72.902646)
                                                                            };
        private string _gotoMauiMarkerId;
        private GoogleMap _map;
        private MapFragment _mapFragment;
        private Marker _polarBearMarker;
        private GroundOverlay _polarBearOverlay;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.MapWithOverlayLayout);
            InitMapFragment();
            SetupMapIfNeeded();
        }

        protected override void OnPause()
        {
            base.OnPause();

            // Pause the GPS - we won't have to worry about showing the 
            // location.
            _map.MyLocationEnabled = false;

            _map.MarkerClick -= MapOnMarkerClick;

            _map.InfoWindowClick += HandleInfoWindowClick;
        }

        protected override void OnResume()
        {
            base.OnResume();
            if (SetupMapIfNeeded())
            {
                _map.MyLocationEnabled = true;

                // Setup a handler for when the user clicks on a marker.
                _map.MarkerClick += MapOnMarkerClick;
            }
        }

        private void AddInitialPolarBarToMap()
        {
            MarkerOptions markerOptions = new MarkerOptions()
                .SetSnippet("Click me to go on vacation.")
                .SetPosition(LeaveFromHereToMaui)
                .SetTitle("Goto Maui");
            _polarBearMarker = _map.AddMarker(markerOptions);
            _polarBearMarker.ShowInfoWindow();

            _gotoMauiMarkerId = _polarBearMarker.Id;

            PositionPolarBearGroundOverlay(LeaveFromHereToMaui);
        }

        /// <summary>
        ///   Add three markers to the map.
        /// </summary>
        private void AddMonkeyMarkersToMap()
        {
            for (int i = 0; i < LocationForCustomIconMarkers.Length; i++)
            {
                BitmapDescriptor icon = BitmapDescriptorFactory.FromResource(Resource.Drawable.monkey);
                MarkerOptions markerOptions = new MarkerOptions()
                    .SetPosition(LocationForCustomIconMarkers[i])
                    .InvokeIcon(icon)
                    .SetSnippet(String.Format("This is marker #{0}.", i))
                    .SetTitle(String.Format("Marker {0}", i));
                _map.AddMarker(markerOptions);
            }
        }

        private void HandleInfoWindowClick(object sender, GoogleMap.InfoWindowClickEventArgs e)
        {
            CircleOptions circleOptions = new CircleOptions();
            circleOptions.InvokeCenter(InMaui);
            circleOptions.InvokeRadius(100.0);
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
                fragTx.Add(Resource.Id.mapWithOverlay, _mapFragment, "map");
                fragTx.Commit();
            }
        }

        private void MapOnMarkerClick(object sender, GoogleMap.MarkerClickEventArgs markerClickEventArgs)
        {
            markerClickEventArgs.Handled = true;

            Marker marker = markerClickEventArgs.P0;
            if (marker.Id.Equals(_gotoMauiMarkerId))
            {
                PositionPolarBearGroundOverlay(InMaui);
                _map.AnimateCamera(CameraUpdateFactory.NewLatLngZoom(InMaui, 13));
                _gotoMauiMarkerId = null;
                _polarBearMarker.Remove();
                _polarBearMarker = null;
            }
            else
            {
                Toast.MakeText(this, String.Format("You clicked on Marker ID {0}", marker.Id), ToastLength.Short).Show();
            }
        }

        private void PositionPolarBearGroundOverlay(LatLng position)
        {
            if (_polarBearOverlay == null)
            {
                BitmapDescriptor image = BitmapDescriptorFactory.FromResource(Resource.Drawable.polarbear);
                GroundOverlayOptions groundOverlayOptions = new GroundOverlayOptions()
                    .Position(position, 150, 200)
                    .InvokeImage(image);
                _polarBearOverlay = _map.AddGroundOverlay(groundOverlayOptions);
            }
            else
            {
                _polarBearOverlay.Position = InMaui;
            }
        }

        private bool SetupMapIfNeeded()
        {
            if (_map == null)
            {
                _map = _mapFragment.Map;
                if (_map != null)
                {
                    AddMonkeyMarkersToMap();
                    AddInitialPolarBarToMap();

                    // Animate the move on the map so that it is showing the markers we added above.
                    _map.AnimateCamera(CameraUpdateFactory.NewLatLngZoom(LocationForCustomIconMarkers[1], 2));
                    return true;
                }
                return false;
            }
            return true;
        }
    }
}
