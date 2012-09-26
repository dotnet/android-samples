namespace MapviewBalloons.Example
{
    using System.Diagnostics.CodeAnalysis;

    using Android.App;
    using Android.GoogleMaps;
    using Android.Graphics.Drawables;
    using Android.OS;

    [Activity(Label = "Simple Mapview Balloons Example", MainLauncher = true, Icon = "@drawable/icon")]
    public class Activity1 : MapActivity
    {
        private Drawable _drawable1;
        private Drawable _drawable2;

        private SimpleItemizedOverlay _itemizedOverlay;
        private SimpleItemizedOverlay _itemizedOverlay2;
        private MapView _mapView;

        [SuppressMessage("StyleCop.CSharp.LayoutRules", "SA1502:ElementMustNotBeOnSingleLine", Justification = "Reviewed. Suppression is OK here.")]
        protected override bool IsRouteDisplayed { get { return false; } }

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.main);
            _mapView = FindViewById<MapView>(Resource.Id.mapview);
            _mapView.SetBuiltInZoomControls(true);

            // First overlay
            _drawable1 = Resources.GetDrawable(Resource.Drawable.marker);
            _itemizedOverlay = new SimpleItemizedOverlay(_drawable1, _mapView);
            var point = new GeoPoint((int)(51.5174723 * 1E6), (int)(-0.0899537 * 1E6));
            var overlayItem = new OverlayItem(point, "Tomorrow Never Dies (1997)", "(M gives Bond his mission in Daimler car)");
            _itemizedOverlay.AddOverlay(overlayItem);

            var point2 = new GeoPoint((int)(51.515259 * 1E6), (int)(-0.086623 * 1E6));
            var overlayItem2 = new OverlayItem(point2, "GoldenEye (1995)", "(Interiors Russian defence ministry council chambers in St Petersburg)");
            _itemizedOverlay.AddOverlay(overlayItem2);

            _mapView.Overlays.Add(_itemizedOverlay);

            // second overlay
            _drawable2 = Resources.GetDrawable(Resource.Drawable.marker2);
            _itemizedOverlay2 = new SimpleItemizedOverlay(_drawable2, _mapView);

            var point3 = new GeoPoint((int)(51.513329 * 1E6), (int)(-0.08896 * 1E6));
            var overlayItem3 = new OverlayItem(point3, "Sliding Doors (1998)", null);
            _itemizedOverlay2.AddOverlay(overlayItem3);

            var point4 = new GeoPoint((int)(51.51738 * 1E6), (int)(-0.08186 * 1E6));
            var overlayItem4 = new OverlayItem(point4, "Mission: Impossible (1996)", "(Ethan & Jim cafe meeting)");
            _itemizedOverlay2.AddOverlay(overlayItem4);

            _mapView.Overlays.Add(_itemizedOverlay2);

            if (bundle == null)
            {
                var mc = _mapView.Controller;
                mc.SetZoom(16);
                mc.AnimateTo(point2);
            }
        }
    }
}
