namespace SimpleMapDemo
{
	using System;

	using Android.App;
	using Android.Gms.Maps;
	using Android.Gms.Maps.Model;
	using Android.OS;
	using Android.Support.V4.App;
	using Android.Widget;

	using FragmentTransaction = Android.Support.V4.App.FragmentTransaction;

	[Activity(Label = "@string/activity_label_mapwithmarkers")]
	public class MapWithMarkersActivity : FragmentActivity
	{
		private static readonly LatLng InMaui = new LatLng(20.72110, -156.44776);
		private static readonly LatLng LeaveFromHereToMaui = new LatLng(82.4986, -62.348);
		private static readonly LatLng[] LocationForCustomIconMarkers = new[]
		{
			new LatLng(40.741773, -74.004986),
			new LatLng(41.051696, -73.545667),
			new LatLng(41.311197, -72.902646)
		};
		private string _gotoMauiMarkerId;
		private GoogleMap _map;
		private SupportMapFragment _mapFragment;
		private Marker _polarBearMarker;
		private GroundOverlay _polarBearOverlay;

		protected override void OnCreate(Bundle bundle)
		{
			base.OnCreate(bundle);

			SetContentView(Resource.Layout.MapWithOverlayLayout);
			InitMapFragment();
			SetupMapIfNeeded();
		}

		protected override void OnPause()
		{
			base.OnPause();

			// Pause the GPS - we won't have to worry about showing the 
			// location.
			_map.MyLocationEnabled = false;

			_map.MarkerClick -= MapOnMarkerClick;
		}

		protected override void OnResume()
		{
			base.OnResume();
			SetupMapIfNeeded();

			_map.MyLocationEnabled = true;

			// Setup a handler for when the user clicks on a marker.
			_map.MarkerClick += MapOnMarkerClick;
		}

		private void AddInitialPolarBarToMap()
		{
			MarkerOptions markerOptions = new MarkerOptions()
				.SetSnippet("Click me to go on vacation.")
					.SetPosition(LeaveFromHereToMaui)
					.SetTitle("Goto Maui");
			_polarBearMarker = _map.AddMarker(markerOptions);
			_polarBearMarker.ShowInfoWindow();

			_gotoMauiMarkerId = _polarBearMarker.Id;

			PositionPolarBearGroundOverlay(LeaveFromHereToMaui);
		}

		/// <summary>
		///   Add three markers to the map.
		/// </summary>
		private void AddMonkeyMarkersToMap()
		{
			for (int i = 0; i < LocationForCustomIconMarkers.Length; i++)
			{
				BitmapDescriptor icon = BitmapDescriptorFactory.FromResource(Resource.Drawable.monkey);
				MarkerOptions mapOption = new MarkerOptions()
					.SetPosition(LocationForCustomIconMarkers[i])
						.InvokeIcon(icon)
						.SetSnippet(String.Format("This is marker #{0}.", i))
						.SetTitle(String.Format("Marker {0}", i));
				_map.AddMarker(mapOption);
			}
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
				fragTx.Add(Resource.Id.mapWithOverlay, _mapFragment, "map");
				fragTx.Commit();
			}
		}

		private void MapOnMarkerClick(object sender, GoogleMap.MarkerClickEventArgs markerClickEventArgs)
		{
			Marker marker = markerClickEventArgs.P0; // TODO [TO201212142221] Need to fix the name of this with MetaData.xml
			if (marker.Id.Equals(_gotoMauiMarkerId))
			{
				PositionPolarBearGroundOverlay(InMaui);
				_map.AnimateCamera(CameraUpdateFactory.NewLatLngZoom(InMaui, 13));
				_gotoMauiMarkerId = null;
				_polarBearMarker.Remove();
				_polarBearMarker = null;
			}
			else
			{
				Toast.MakeText(this, String.Format("You clicked on Marker ID {0}", marker.Id), ToastLength.Short).Show();
			}
		}

		private void PositionPolarBearGroundOverlay(LatLng position)
		{
			if (_polarBearOverlay == null)
			{
				BitmapDescriptor image = BitmapDescriptorFactory.FromResource(Resource.Drawable.polarbear);
				GroundOverlayOptions groundOverlayOptions = new GroundOverlayOptions()
					.Position(position, 150, 200)
						.InvokeImage(image);
				_polarBearOverlay = _map.AddGroundOverlay(groundOverlayOptions);
			}
			else
			{
				_polarBearOverlay.Position = InMaui;
			}
		}

		private void SetupMapIfNeeded()
		{
			if (_map == null)
			{
				_map = _mapFragment.Map;
				if (_map != null)
				{
					AddMonkeyMarkersToMap();
					AddInitialPolarBarToMap();

					// Move the map so that it is showing the markers we added above.
					_map.AnimateCamera(CameraUpdateFactory.NewLatLngZoom(LocationForCustomIconMarkers[1], 2));
				}
			}
		}
	}
}
