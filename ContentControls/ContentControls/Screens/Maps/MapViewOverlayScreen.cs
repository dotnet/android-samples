using Android.App;
using Android.OS;
using Android.Views;
using Android.Webkit;
using Android.GoogleMaps;
using Android.Graphics; // Added this reference to project manually - Maps isn't "built in"

namespace ContentControls {

    [Activity(Label = "MapViewOverlay")]
    public class MapViewOverlayScreen : MapActivity {  // NOTE: subclasses MapActivity (and implement IsRouteDisplayed)
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
            map.Controller.SetZoom(17);
            map.Controller.SetCenter(new GeoPoint((int)(29.97611 * 1e6), (int)(31.132778 * 1e6))); // Pyramids of Giza

            AddPinOverlay(map);
        }

        // REQUIRED for overlay
        void AddPinOverlay(MapView map)
        {
            var pinOverlay = new CustomMapOverlay();
            map.Overlays.Add(pinOverlay);
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

        class CustomMapOverlay : Overlay {
            public override void Draw(Android.Graphics.Canvas canvas,
                MapView mapView, bool shadow)
            {
                base.Draw(canvas, mapView, shadow);

                var paint = new Paint();
                paint.AntiAlias = true;
                paint.Color = Color.Purple;
                paint.Alpha = 127;

                var gp = new GeoPoint((int)(29.97611 * 1e6), (int)(31.132778 * 1e6));
                var pt = mapView.Projection.ToPixels(gp, null);
                float distance = mapView.Projection.MetersToEquatorPixels(200);

                canvas.DrawCircle(pt.X, pt.Y, distance/2, paint);

            }
        }
    }
}