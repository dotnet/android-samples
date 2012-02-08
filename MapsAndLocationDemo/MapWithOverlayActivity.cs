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
using Android.Graphics.Drawables;
using Android.Graphics;

namespace MapsAndLocationDemo
{
    [Activity (Label = "MapWithOverlayActivity")]
    public class MapWithOverlayActivity : MapActivity
    {
        MyLocationOverlay _myLocationOverlay;
     
        protected override void OnCreate (Bundle bundle)
        {
            base.OnCreate (bundle);
         
            SetContentView (Resource.Layout.MapWithOverlayLayout);
         
            var map = InitMap (); 
         
            AddMyLocationOverlay (map);
         
            AddItemizedOverlays (map);
         
            AddCustomOverlay (map);
         
            AddButtonToMap (map);
        }

        MapView InitMap ()
        {
            var map = FindViewById<MapView> (Resource.Id.mapWithOverlay);
         
            map.Clickable = true;
            map.Traffic = false;
            map.Satellite = false;
            map.Controller.SetZoom (10);
         
            return map;
        }

        void AddMyLocationOverlay (MapView map)
        {
            _myLocationOverlay = new MyLocationOverlay (this, map);
            map.Overlays.Add (_myLocationOverlay);
         
            _myLocationOverlay.RunOnFirstFix (() => {
                map.Controller.AnimateTo (_myLocationOverlay.MyLocation);
             
                RunOnUiThread (() => {
                    var toast = Toast.MakeText (this, "Located", ToastLength.Short);
                    toast.Show ();
                });
            });
        }
     
        void AddItemizedOverlays (MapView map)
        {
            var monkey = Resources.GetDrawable (Resource.Drawable.monkey);       
            var monkeyOverlay = new MonkeyItemizedOverlay (monkey);
            map.Overlays.Add (monkeyOverlay);
        }
     
        void AddCustomOverlay (MapView map)
        {    
            var customOverlay = new CustomMapOverlay ();
            map.Overlays.Add (customOverlay);
        }
     
        void AddButtonToMap (MapView map)
        {
            var mapButton = new Button (this){Text = "Go to Maui"};
         
            var layoutParams = new MapView.LayoutParams (100, 50, 
             new GeoPoint ((int)42.374260E6, (int)-71.120824E6),
             MapView.LayoutParams.TopLeft);
         
            mapButton.Click += (sender, e) => {
                map.Controller.SetCenter (new GeoPoint ((int)20.866667E6, (int)-156.500556E6));
            };
         
            map.AddView (mapButton, layoutParams);
        }
     
        protected override void OnResume ()
        {
            base.OnResume ();
         
            _myLocationOverlay.EnableMyLocation ();
        }
     
        protected override void OnPause ()
        {
            base.OnPause ();
            
            _myLocationOverlay.DisableMyLocation ();
        }

        protected override bool IsRouteDisplayed {
            get {
                return false;
            }
        }
     
        class MonkeyItemizedOverlay: ItemizedOverlay
        {
            List<OverlayItem> _items;
         
            public MonkeyItemizedOverlay (Drawable monkey) : base(monkey)
            {    
                // populate some sample location data for the overlay items
                _items = new List<OverlayItem>{
                 new OverlayItem (new GeoPoint ((int)40.741773E6, (int)-74.004986E6), null, null),
                 new OverlayItem (new GeoPoint ((int)41.051696E6, (int)-73.545667E6), null, null),
                 new OverlayItem (new GeoPoint ((int)41.311197E6, (int)-72.902646E6), null, null)
             };
             
                BoundCenterBottom (monkey);
                Populate ();
            }
         
            protected override Java.Lang.Object CreateItem (int i)
            {
                var item = _items [i];
                return item;
            }
         
            public override int Size ()
            {
                return _items.Count ();
            }
        }
 
        class CustomMapOverlay : Overlay
        {
     
            public override void Draw (Android.Graphics.Canvas canvas, MapView mapView, bool shadow)
            {
                base.Draw (canvas, mapView, shadow);
         
                var paint = new Paint ();
                paint.AntiAlias = true;
                paint.Color = Color.Purple;
         
                // to draw fixed graphics that will not move or scale with the map
                //canvas.DrawRect (0, 0, 100, 100, paint);
         
                // to draw graphics at a geocoded location that move and scale with the map
                var gp = new GeoPoint ((int)41.940542E6, (int)-73.363447E6);     
                var pt = mapView.Projection.ToPixels (gp, null);     
                float distance = mapView.Projection.MetersToEquatorPixels (20000);
         
                canvas.DrawRect (pt.X, pt.Y, pt.X + distance, pt.Y + distance, paint);       
            }
        }
    }
}

