using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.Support.V4.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.GoogleMaps;

namespace MapsAndLocationDemo
{
    [Activity (Label = "SampleMapActivity")]         
    public class SampleMapActivity : FragmentActivity
    {
        private static readonly LatLng VimyRidge = new LatLng(50.379444, 2.773611);
        private static readonly LatLng Passchendaele = new LatLng(50.897778, 3.013333);
        private GoogleMap _map;
        protected override void OnResume()
        {
            base.OnResume();
            var mapFragment = SupportFragmentManager.FindFragmentByTag("map") as SupportMapFragment;
            _map = mapFragment.Map;
            System.Diagnostics.Debug.Assert(_map != null, "The _map cannot be null!");

            System.Diagnostics.Debug.Assert(VimyRidge != null, "The LatLng is null for some weird reason.");
            var cameraUpdate = CameraUpdateFactory.NewLatLngZoom(VimyRidge, 15);
            _map.MoveCamera(cameraUpdate);

        }
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.MapLayout);

            var mapOptions = new GoogleMapOptions()
                .InvokeMapType(GoogleMap.MapTypeSatellite)
                .InvokeZoomControlsEnabled(false)
                .InvokeCompassEnabled(true);

            var fragTx = SupportFragmentManager.BeginTransaction();
            var mapFragment = SupportMapFragment.NewInstance(mapOptions);
            fragTx.Add(Resource.Id.map, mapFragment, "map");
            fragTx.Commit();

            var animateButton = FindViewById<Button> (Resource.Id.animateButton);
            animateButton.Click += (sender, e) => {
                // Move the camera to the Passchendaele Memorial in Belgium.
                var builder = CameraPosition.InvokeBuilder();
                builder.Target(Passchendaele);
                builder.Zoom(18);  // Zoom level of 18
                builder.Bearing(155);
                builder.Tilt(25);
                var cameraPosition = builder.Build();

                _map.AnimateCamera(CameraUpdateFactory.NewCameraPosition(cameraPosition));
            };

            var zoomInButton = FindViewById<Button> (Resource.Id.zoomInButton);
         
            zoomInButton.Click += (sender, e) => {
                _map.AnimateCamera(CameraUpdateFactory.ZoomIn());
            };
         
            var zoomOutButton = FindViewById<Button> (Resource.Id.zoomOutButton);
            zoomOutButton.Click += (sender, e) => {
                _map.AnimateCamera(CameraUpdateFactory.ZoomOut());
            };
         
        }
    }
}

