using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.GoogleMaps;

namespace MapsAndLocationDemo
{
    [Activity (Label = "SampleMapActivity")]         
    public class SampleMapActivity : MapActivity
    {
        protected override void OnCreate (Bundle bundle)
        {
            base.OnCreate (bundle);

            SetContentView (Resource.Layout.MapLayout);
         
            var map = FindViewById<MapView> (Resource.Id.map);
         
            map.Clickable = true;
            map.Traffic = false;
            map.Satellite = true;
         
            map.SetBuiltInZoomControls (true);   
            map.Controller.SetZoom (10);
            map.Controller.SetCenter (new GeoPoint ((int)42.374260E6, (int)-71.120824E6));
         
            var zoomInButton = FindViewById<Button> (Resource.Id.zoomInButton);
            var zoomOutButton = FindViewById<Button> (Resource.Id.zoomOutButton);
            var animateButton = FindViewById<Button> (Resource.Id.animateButton);
         
            zoomInButton.Click += (sender, e) => {
                map.Controller.ZoomIn ();
                //map.Controller.ZoomInFixing (200, 200);
            };
         
            zoomOutButton.Click += (sender, e) => {
                map.Controller.ZoomOut ();
                //map.Controller.ZoomOutFixing (200, 200);
            };
         
            animateButton.Click += (sender, e) => {
                //map.Controller.AnimateTo(new GeoPoint ((int)40.741773E6, (int)-74.004986E6));
             
                map.Controller.AnimateTo (
                 new GeoPoint ((int)40.741773E6, (int)-74.004986E6), () => {
                    var toast = Toast.MakeText (this, "Welcome to NY", ToastLength.Short);
                    toast.Show ();
                });
                                      
            };
        }

        protected override bool IsRouteDisplayed {
            get {
                return false;
            }
        }
    }
}

