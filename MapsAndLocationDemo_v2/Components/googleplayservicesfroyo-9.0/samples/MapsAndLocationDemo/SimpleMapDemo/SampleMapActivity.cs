namespace SimpleMapDemo
{
	using Android.App;
	using Android.Gms.Maps;
	using Android.Gms.Maps.Model;
	using Android.OS;
	using Android.Support.V4.App;
	using Android.Widget;

	using FragmentTransaction = Android.Support.V4.App.FragmentTransaction;

	[Activity(Label = "@string/activity_label_samplemap")]
	public class SampleMapActivity : FragmentActivity
	{
		private static readonly LatLng Passchendaele = new LatLng(50.897778, 3.013333);
		private static readonly LatLng VimyRidge = new LatLng(50.379444, 2.773611);
		private GoogleMap _map;
		private SupportMapFragment _mapFragment;

		protected override void OnCreate(Bundle bundle)
		{
			base.OnCreate(bundle);
			SetContentView(Resource.Layout.MapLayout);

			InitMapFragment();

			SetupMapIfNeeded(); // It's not gauranteed that the map will be available at this point.

			SetupAnimateToButton();
			SetupZoomInButton();
			SetupZoomOutButton();
		}

		protected override void OnResume()
		{
			base.OnResume();
			SetupMapIfNeeded();
		}

		private void InitMapFragment()
		{
			_mapFragment = SupportFragmentManager.FindFragmentByTag("map") as SupportMapFragment;
			if (_mapFragment == null)
			{
				GoogleMapOptions mapOptions = new GoogleMapOptions()
					.InvokeMapType(GoogleMap.MapTypeSatellite)
						.InvokeZoomControlsEnabled(false)
						.InvokeCompassEnabled(true);

				FragmentTransaction fragTx = SupportFragmentManager.BeginTransaction();
				_mapFragment = SupportMapFragment.NewInstance(mapOptions);
				fragTx.Add(Resource.Id.map, _mapFragment, "map");
				fragTx.Commit();
			}
		}

		private void SetupAnimateToButton()
		{
			Button animateButton = FindViewById<Button>(Resource.Id.animateButton);
			animateButton.Click += (sender, e) =>{
				// Move the camera to the Passchendaele Memorial in Belgium.
				CameraPosition.Builder builder = CameraPosition.InvokeBuilder();
				builder.Target(Passchendaele);
				builder.Zoom(18);
				builder.Bearing(155);
				builder.Tilt(25);
				CameraPosition cameraPosition = builder.Build();

				// AnimateCamera provides a smooth, animation effect while moving
				// the camera to the the position.
				_map.AnimateCamera(CameraUpdateFactory.NewCameraPosition(cameraPosition));
			};
		}

		private void SetupMapIfNeeded()
		{
			if (_map == null)
			{
				_map = _mapFragment.Map;
				if (_map != null)
				{
					MarkerOptions marker1 = new MarkerOptions();
					marker1.SetPosition(VimyRidge);
					marker1.SetTitle("Vimy Ridge");
					_map.AddMarker(marker1);

					MarkerOptions marker2 = new MarkerOptions();
					marker2.SetPosition(Passchendaele);
					marker2.SetTitle("Passchendaele");
					_map.AddMarker(marker2);

					// We create an instance of CameraUpdate, and move the map to it.
					CameraUpdate cameraUpdate = CameraUpdateFactory.NewLatLngZoom(VimyRidge, 15);
					_map.MoveCamera(cameraUpdate);
				}
			}
		}

		private void SetupZoomInButton()
		{
			Button zoomInButton = FindViewById<Button>(Resource.Id.zoomInButton);
			zoomInButton.Click += (sender, e) => { _map.AnimateCamera(CameraUpdateFactory.ZoomIn()); };
		}

		private void SetupZoomOutButton()
		{
			Button zoomOutButton = FindViewById<Button>(Resource.Id.zoomOutButton);
			zoomOutButton.Click += (sender, e) => { _map.AnimateCamera(CameraUpdateFactory.ZoomOut()); };
		}
	}
}
