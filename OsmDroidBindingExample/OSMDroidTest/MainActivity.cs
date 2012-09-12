namespace OSMDroidTest
{
    using Android.App;
    using Android.OS;


    using OsmDroid.Api;
    using OsmDroid.TileProvider.TileSource;
    using OsmDroid.Util;
    using OsmDroid.Views;

    [Activity(Label = "OSMDroidTest", MainLauncher = true)]
    public class MainActivity : Activity
    {
        private IMapController _mapController;
        private MapView _mapView;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            _mapView = FindViewById<MapView>(Resource.Id.mapview);
            _mapView.SetTileSource(TileSourceFactory.DefaultTileSource);
            _mapView.SetBuiltInZoomControls(true);

            _mapController = _mapView.Controller;
            _mapController.SetZoom(25);

            var centreOfMap = new GeoPoint(51496994, -134733);
            _mapController.SetCenter(centreOfMap);
        }
    }
}
