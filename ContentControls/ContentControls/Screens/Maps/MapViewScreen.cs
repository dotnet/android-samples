using Android.App;
using Android.OS;
using Android.Views;
using Android.Webkit;
using Android.GoogleMaps; // NOTE: added this reference to project manually - Maps isn't "built in"

namespace ContentControls {

    [Activity(Label = "MapView")]
    public class MapViewScreen : MapActivity {  // NOTE: subclasses MapActivity (and implement IsRouteDisplayed)
        MapView map;

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

