using Android.App;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.Graphics;
using Android.OS;
using Android.Support.V7.App;
using Android.Widget;

namespace SimpleMapDemo
{
    using System;

    [Activity(Label = "@string/activity_label_mapwithoverlays")]
    public class MapWithOverlaysActivity : AppCompatActivity, IOnMapReadyCallback
    {
        static readonly LatLng InMaui = new LatLng(20.72110, -156.44776);
        static readonly LatLng LeaveFromHereToMaui = new LatLng(58.768410, -94.164963);

        static readonly LatLng[] LocationForCustomIconMarkers =
        {
            new LatLng(40.741773, -74.004986),
            new LatLng(41.051696, -73.545667),
            new LatLng(41.311197, -72.902646)
        };

        GoogleMap googleMap;
        SupportMapFragment mapFragment;
        string gotMauiMarkerId;
        Marker polarBearMarker;
        GroundOverlay polarBearOverlay;
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.MapWithOverlayLayout);

            mapFragment = (SupportMapFragment)SupportFragmentManager.FindFragmentById(Resource.Id.map);
            mapFragment.GetMapAsync(this);

        }

        public void OnMapReady(GoogleMap map)
        {
            googleMap = map;

            var mapOptions = new GoogleMapOptions()
                             .InvokeMapType(GoogleMap.MapTypeNormal)
                             .InvokeZoomControlsEnabled(false)
                             .InvokeCompassEnabled(true);



            AddMonkeyMarkersToMap();
            AddInitialPolarBarToMap();

            googleMap.MyLocationEnabled = true;
            
            // Animate the move on the map so that it is showing the markers we added above.
            googleMap.AnimateCamera(CameraUpdateFactory.NewLatLngZoom(LocationForCustomIconMarkers[1], 2));

            // Setup a handler for when the user clicks on a marker.
            googleMap.MarkerClick += MapOnMarkerClick;
        }


        protected override void OnPause()
        {
            // Pause the GPS - we won't have to worry about showing the 
            // location.
            googleMap.MyLocationEnabled = false;

            googleMap.MarkerClick -= MapOnMarkerClick;

            googleMap.InfoWindowClick += HandleInfoWindowClick;

            base.OnPause();

        }


        void AddInitialPolarBarToMap()
        {
            var markerOptions = new MarkerOptions()
                                .SetSnippet("Click me to go on vacation.")
                                .SetPosition(LeaveFromHereToMaui)
                                .SetTitle("Goto Maui");
            polarBearMarker = googleMap.AddMarker(markerOptions);
            polarBearMarker.ShowInfoWindow();

            gotMauiMarkerId = polarBearMarker.Id;

            PositionPolarBearGroundOverlay(LeaveFromHereToMaui);
        }

        /// <summary>
        ///     Add three markers to the map.
        /// </summary>
        void AddMonkeyMarkersToMap()
        {
            for (var i = 0; i < LocationForCustomIconMarkers.Length; i++)
            {
                var icon = BitmapDescriptorFactory.FromResource(Resource.Drawable.monkey);
                var markerOptions = new MarkerOptions()
                                    .SetPosition(LocationForCustomIconMarkers[i])
                                    .SetIcon(icon)
                                    .SetSnippet($"This is marker #{i}.")
                                    .SetTitle($"Marker {i}");
                googleMap.AddMarker(markerOptions);
            }
        }

        void HandleInfoWindowClick(object sender, GoogleMap.InfoWindowClickEventArgs e)
        {
            var circleOptions = new CircleOptions();
            circleOptions.InvokeCenter(InMaui);
            circleOptions.InvokeRadius(100.0);
        }


        void MapOnMarkerClick(object sender, GoogleMap.MarkerClickEventArgs markerClickEventArgs)
        {
            markerClickEventArgs.Handled = true;

            var marker = markerClickEventArgs.Marker;
            if (marker.Id.Equals(gotMauiMarkerId))
            {
                PositionPolarBearGroundOverlay(InMaui);
                googleMap.AnimateCamera(CameraUpdateFactory.NewLatLngZoom(InMaui, 13));
                gotMauiMarkerId = null;
                polarBearMarker.Remove();
                polarBearMarker = null;
            }
            else
            {
                Toast.MakeText(this, $"You clicked on Marker ID {marker.Id}", ToastLength.Short).Show();
            }
        }

        void PositionPolarBearGroundOverlay(LatLng position)
        {
            if (polarBearOverlay == null)
            {
               
                var polarBear = BitmapDescriptorFactory.FromResource(Resource.Drawable.polarbear);
                var groundOverlayOptions = new GroundOverlayOptions()
                                           .InvokeImage(polarBear)
                                           .Anchor(0, 1)
                                           .Position(position, 150, 200);
                polarBearOverlay = googleMap.AddGroundOverlay(groundOverlayOptions);
            }
            else
            {
                polarBearOverlay.Position = InMaui;
            }
        }
    }
}
