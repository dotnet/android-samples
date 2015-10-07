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
	[Activity (WindowSoftInputMode = SoftInput.AdjustPan)]
	[IntentFilter (new []{ Intent.ActionMain }, Categories = new[] {
		Intent.CategoryLauncher,
		Intent.CategoryLeanbackLauncher
	})]
	public class MainActivity : Activity, 
		GoogleApiClient.IConnectionCallbacks, 
		GoogleApiClient.IOnConnectionFailedListener, 
		View.IOnClickListener, 
		IConnectionsConnectionRequestListener, 
		IConnectionsMessageListener, 
		IConnectionsEndpointDiscoveryListener
	{
		static readonly string TAG = typeof(MainActivity).Name;

		const long TIMEOUT_ADVERTISE = 1000L * 30L;
		const long TIMEOUT_DISCOVER = 1000L * 30L;

		public enum NearbyConnectionState
		{
			Idle = 1023,
			Ready = 1024,
			Advertising = 1025,
			Discovering = 1026,
			Connected = 1027
		}

		GoogleApiClient mGoogleApiClient;

		TextView mDebugInfo;
		EditText mMessageText;
		AlertDialog mConnectionRequestDialog;
		MyListDialog mMyListDialog;

		NearbyConnectionState mState = NearbyConnectionState.Idle;

		string mOtherEndpointId;

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
            var result = await NearbyClass.Connections.StartAdvertisingAsync (mGoogleApiClient, name, appMetadata, TIMEOUT_ADVERTISE, this);
			
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
                serviceId, TIMEOUT_DISCOVER, this);

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

        async Task ConnectTo (string endpointId, string endpointName)
		{
			DebugLog ("connectTo:" + endpointId + ":" + endpointName);

			string myName = null;
			byte[] myPayload = null;
			var connectionResponseCallback = new ConnectionResponseCallback ();

			connectionResponseCallback.OnConnectionResponseImpl = (remoteEndpointId, status, payload) => {
				Log.Debug (TAG, "onConnectionResponse:" + remoteEndpointId + ":" + status);
				if (status.IsSuccess) {
					DebugLog ("onConnectionResponse: " + endpointName + " SUCCESS");
					Toast.MakeText (this, "Connected to " + endpointName,
						ToastLength.Short).Show ();

					mOtherEndpointId = remoteEndpointId;
					UpdateViewVisibility (NearbyConnectionState.Connected);
				} else {
					DebugLog ("onConnectionResponse: " + endpointName + " FAILURE");
				}
			};

			await NearbyClass.Connections.SendConnectionRequestAsync (mGoogleApiClient, myName, endpointId, 
                myPayload, connectionResponseCallback, this);
		}

		class ConnectionResponseCallback : Java.Lang.Object, IConnectionsConnectionResponseCallback
		{
			public Action<string, Statuses, byte[]> OnConnectionResponseImpl { get; set; }

			public void OnConnectionResponse (string remoteEndpointId, Statuses status, byte[] payload)
			{
				OnConnectionResponseImpl (remoteEndpointId, status, payload);
			}
			
		}

		public void OnConnectionRequest (string remoteEndpointId, string remoteDeviceId, string remoteEndpointName, byte[] payload)
		{
			DebugLog ("onConnectionRequest:" + remoteEndpointId + ":" + remoteEndpointName);

			// This device is advertising and has received a connection request. Show a dialog asking
			// the user if they would like to connect and accept or reject the request accordingly.
			mConnectionRequestDialog = new AlertDialog.Builder (this)
				.SetTitle ("Connection Request")
				.SetMessage ("Do you want to connect to " + remoteEndpointName + "?")
				.SetCancelable (false)
				.SetPositiveButton ("Connect", (sender, e) => {
					byte[] pLoad = null;
					NearbyClass.Connections.AcceptConnectionRequest (mGoogleApiClient, 
						remoteEndpointId, pLoad, this).SetResultCallback ((Statuses status) => {
							if (status.IsSuccess) {
								DebugLog ("acceptConnectionRequest: SUCCESS");
								mOtherEndpointId = remoteEndpointId;
								UpdateViewVisibility (NearbyConnectionState.Connected);
							} else {
								DebugLog ("acceptConnectionRequest: FAILURE");
							}
						});
				})
				.SetNegativeButton ("No", (sender, e) => NearbyClass.Connections.RejectConnectionRequest (mGoogleApiClient, remoteEndpointId))
				.Create ();

			mConnectionRequestDialog.Show ();
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

		public void OnEndpointFound (string endpointId, string deviceId, string serviceId, string name)
		{
			Log.Debug (TAG, "onEndpointFound:" + endpointId + ":" + name);

			if (mMyListDialog == null) {
				var builder = new AlertDialog.Builder (this)
					.SetTitle ("Endpoint(s) Found")
					.SetCancelable (true)
					.SetNegativeButton ("Cancel", (sender, e) => mMyListDialog.Dismiss ());

				// Create the MyListDialog with a listener
				mMyListDialog = new MyListDialog (this, builder, async (sender, e) => { 
					var selectedEndpointName = mMyListDialog.GetItemKey (e.Which);
					var selectedEndpointId = mMyListDialog.GetItemValue (e.Which);

					await ConnectTo (selectedEndpointId, selectedEndpointName);
					mMyListDialog.Dismiss ();
				});
			}

			mMyListDialog.AddItem (name, endpointId);
			mMyListDialog.Show ();
		}

		public void OnEndpointLost (string endpointId)
		{
			DebugLog ("onEndpointLost:" + endpointId);

			// An endpoint that was previously available for connection is no longer. It may have
			// stopped advertising, gone out of range, or lost connectivity. Dismiss any dialog that
			// was offering a connection.
			if (mMyListDialog != null) {
				mMyListDialog.RemoveItemByValue (endpointId);
			}
		}

		void UpdateViewVisibility (NearbyConnectionState newState)
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

		void DebugLog (string msg)
		{
			Log.Debug (TAG, msg);
			mDebugInfo.Append ("\n" + msg);
		}
	}
}


