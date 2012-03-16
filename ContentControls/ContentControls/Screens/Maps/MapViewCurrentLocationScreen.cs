using Android.App;
using Android.OS;
using Android.Views;
using Android.Webkit;
using Android.GoogleMaps;
using Android.Widget; // Added this reference to project manually - Maps isn't "built in"

namespace ContentControls {

    [Activity(Label = "MapViewCurrentLocation")]
    public class MapViewCurrentLocationScreen : MapActivity {  // NOTE: subclasses MapActivity (and implement IsRouteDisplayed)
        
        MapView map;
        MyLocationOverlay myLocationOverlay;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.MapView);

            map = FindViewById<MapView>(Resource.Id.Map);

            map.Clickable = true;
            map.Traffic = false;
            map.Satellite = true;

            map.SetBuiltInZoomControls(true);
            map.Controller.SetZoom(15);
            map.Controller.SetCenter(new GeoPoint((int)(48.857 * 1e6), (int)(2.351 * 1e6))); // Paris

            AddMyLocationOverlay(map);
        }
        
        // REQUIRED for 'my location'
        void AddMyLocationOverlay(MapView map)
        {
            myLocationOverlay = new MyLocationOverlay(this, map);
            map.Overlays.Add(myLocationOverlay);

            myLocationOverlay.RunOnFirstFix(() => {
                map.Controller.AnimateTo(myLocationOverlay.MyLocation);

                RunOnUiThread(() => {
                    var toast = Toast.MakeText(this, "Located", ToastLength.Short);
                    toast.Show();
                });
            });
        }
        protected override void OnResume()
        {
            base.OnResume();
            myLocationOverlay.EnableMyLocation();
        }
        protected override void OnPause()
        {
            base.OnPause();
            myLocationOverlay.DisableMyLocation();
        }

        // REQUIRED by MapActivity
        protected override bool IsRouteDisplayed
        {
            get { return false; }
        }


        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            menu.Add("Satellite");
            menu.Add("Road");
            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.TitleFormatted.ToString()) {
            case "Road":
            map.Satellite = false; break;
            default:
            map.Satellite = true; break;
            }
            return base.OnOptionsItemSelected(item);
        }
    }
}