using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Gms.Common.Apis;
using Android.Gms.Nearby.Connection;
using Android.Net;
using Android.Text.Method;
using Android.Gms.Nearby;
using Android.Util;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConnectionsQuickstart
{
	public enum NearbyConnectionState
	{
		Idle = 1023,
		Ready = 1024,
		Advertising = 1025,
		Discovering = 1026,
		Connected = 1027
	}

	[Activity (WindowSoftInputMode = SoftInput.AdjustPan)]
	[IntentFilter (new []{ Intent.ActionMain }, Categories = new[] {
		Intent.CategoryLauncher,
		Intent.CategoryLeanbackLauncher
	})]
	public class MainActivity : Activity, 
		GoogleApiClient.IConnectionCallbacks, 
		GoogleApiClient.IOnConnectionFailedListener, 
		View.IOnClickListener, 
		IConnectionsMessageListener
	{
		public static readonly string TAG = typeof(MainActivity).Name;

		const long TIMEOUT_ADVERTISE = 1000L * 30L;
		const long TIMEOUT_DISCOVER = 1000L * 30L;

        public GoogleApiClient mGoogleApiClient { get; private set; }
        public AlertDialog mConnectionRequestDialog { get; set; }
        public string mOtherEndpointId { get; set; }
		TextView mDebugInfo;
		EditText mMessageText;
        public MyListDialog mMyListDialog { get; set; }
		NearbyConnectionState mState = NearbyConnectionState.Idle;

		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);
			SetContentView (Resource.Layout.activity_main);

			FindViewById (Resource.Id.button_advertise).SetOnClickListener (this);
			FindViewById (Resource.Id.button_discover).SetOnClickListener (this);
			FindViewById (Resource.Id.button_send).SetOnClickListener (this);

			mMessageText = FindViewById<EditText> (Resource.Id.edittext_message);

			mDebugInfo = FindViewById<TextView> (Resource.Id.debug_text);
			mDebugInfo.MovementMethod = new ScrollingMovementMethod ();

			mGoogleApiClient = new GoogleApiClient.Builder (this)
				.AddConnectionCallbacks (this)
				.AddOnConnectionFailedListener (this)
				.AddApi (NearbyClass.CONNECTIONS_API)
				.Build ();
		}

		protected override void OnStart ()
		{
			base.OnStart ();
			Log.Debug (TAG, "onStart");
			mGoogleApiClient.Connect ();
		}

		protected override void OnStop ()
		{
			base.OnStop ();
			Log.Debug (TAG, "onStop");

			if (mGoogleApiClient != null) {
				mGoogleApiClient.Disconnect ();
			}
		}

		bool IsConnectedToNetwork {
			get {
				var connManager = (ConnectivityManager)GetSystemService (ConnectivityService);
				NetworkInfo info = connManager.GetNetworkInfo (ConnectivityType.Wifi);

				return (info != null && info.IsConnectedOrConnecting);
			}
		}

        async Task StartAdvertising ()
		{
			DebugLog ("startAdvertising");
			if (!IsConnectedToNetwork) {
				DebugLog ("startAdvertising: not connected to WiFi network.");
				return;
			}

            var appIdentifierList = new List<AppIdentifier> ();
			appIdentifierList.Add (new AppIdentifier (PackageName));
			var appMetadata = new AppMetadata (appIdentifierList);

			var name = string.Empty;
            var result = await NearbyClass.Connections.StartAdvertisingAsync (mGoogleApiClient, name, appMetadata, TIMEOUT_ADVERTISE, new MyConnectionsConnectionRequestListener(this));
			
            Log.Debug (TAG, "startAdvertising:onResult:" + result);
			if (result.Status.IsSuccess) {
				DebugLog ("startAdvertising:onResult: SUCCESS");

				UpdateViewVisibility (NearbyConnectionState.Advertising);
			} else {
				DebugLog ("startAdvertising:onResult: FAILURE ");

				int statusCode = result.Status.StatusCode;
				if (statusCode == ConnectionsStatusCodes.StatusAlreadyAdvertising) {
					DebugLog ("STATUS_ALREADY_ADVERTISING");
				} else {
					UpdateViewVisibility (NearbyConnectionState.Ready);
				}
			}
		}

        async Task StartDiscovery ()
		{
			DebugLog ("startDiscovery");
			if (!IsConnectedToNetwork) {
				DebugLog ("startDiscovery: not connected to WiFi network.");
				return;
			}

			string serviceId = GetString (Resource.String.service_id);
			var status = await NearbyClass.Connections.StartDiscoveryAsync (mGoogleApiClient, 
                serviceId, TIMEOUT_DISCOVER, new MyConnectionsEndpointDiscoveryListener (this));

        	if (status.IsSuccess) {
				DebugLog ("startDiscovery:onResult: SUCCESS");

				UpdateViewVisibility (NearbyConnectionState.Discovering);
			} else {
				DebugLog ("startDiscovery:onResult: FAILURE");

				int statusCode = status.StatusCode;
				if (statusCode == ConnectionsStatusCodes.StatusAlreadyDiscovering) {
					DebugLog ("STATUS_ALREADY_DISCOVERING");
				} else {
					UpdateViewVisibility (NearbyConnectionState.Ready);
				}
			}
		}

		void SendMessage ()
		{
			string msg = mMessageText.Text;
			NearbyClass.Connections.SendReliableMessage (mGoogleApiClient, mOtherEndpointId, System.Text.Encoding.Default.GetBytes (msg));

			mMessageText.Text = null;
		}

		public void OnConnected (Bundle bundle)
		{
			DebugLog ("onConnected");
			UpdateViewVisibility (NearbyConnectionState.Ready);
		}

		public void OnConnectionSuspended (int i)
		{
			DebugLog ("onConnectionSuspended: " + i);
			UpdateViewVisibility (NearbyConnectionState.Idle);

			// Try to re-connect
			mGoogleApiClient.Reconnect ();
		}

		public void OnConnectionFailed (Android.Gms.Common.ConnectionResult result)
		{
			DebugLog ("onConnectionFailed: " + result);
			UpdateViewVisibility (NearbyConnectionState.Idle);
		}

		public async void OnClick (View v)
		{
			switch (v.Id) {
			case Resource.Id.button_advertise:
				await StartAdvertising ();
				break;
			case Resource.Id.button_discover:
				await StartDiscovery ();
				break;
			case Resource.Id.button_send:
				SendMessage ();
				break;
			}
		}

		public void OnDisconnected (string remoteEndpointId)
		{
			DebugLog ("onDisconnected:" + remoteEndpointId);

			UpdateViewVisibility (NearbyConnectionState.Ready);
		}

		public void OnMessageReceived (string remoteEndpointId, byte[] payload, bool isReliable)
		{
			DebugLog ("onMessageReceived:" + remoteEndpointId + ":" + System.Text.Encoding.Default.GetString (payload));
		}

		public void UpdateViewVisibility (NearbyConnectionState newState)
		{
			mState = newState;
			switch (mState) {
			case NearbyConnectionState.Idle:
				FindViewById (Resource.Id.layout_nearby_buttons).Visibility = ViewStates.Gone;
				FindViewById (Resource.Id.layout_message).Visibility = ViewStates.Gone;
				break;
			case NearbyConnectionState.Ready:
				FindViewById (Resource.Id.layout_nearby_buttons).Visibility = ViewStates.Visible;
				FindViewById (Resource.Id.layout_message).Visibility = ViewStates.Gone;
				break;
			case NearbyConnectionState.Advertising:
				break;
			case NearbyConnectionState.Discovering:
				break;
			case NearbyConnectionState.Connected:
				FindViewById (Resource.Id.layout_nearby_buttons).Visibility = ViewStates.Visible;
				FindViewById (Resource.Id.layout_message).Visibility = ViewStates.Visible;
				break;
			}
		}

		public void DebugLog (string msg)
		{
			Log.Debug (TAG, msg);
			mDebugInfo.Append ("\n" + msg);
		}
	}
}


