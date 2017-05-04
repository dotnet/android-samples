using Android.App;
using Android.Gms.Common.Apis;
using Android.Gms.Nearby;
using Android.Gms.Nearby.Connection;

namespace ConnectionsQuickstart
{
	public class MyConnectionsConnectionRequestListener : ConnectionsConnectionRequestListener
	{
		MainActivity Self;

		public MyConnectionsConnectionRequestListener (MainActivity self)
		{
			Self = self;
		}

		public override void OnConnectionRequest(string remoteEndpointId, string remoteEndpointName, byte[] handshakeData)
		{
			Self.DebugLog("onConnectionRequest:" + remoteEndpointId + ":" + remoteEndpointName);

			// This device is advertising and has received a connection request. Show a dialog asking
			// the user if they would like to connect and accept or reject the request accordingly.
			Self.mConnectionRequestDialog = new AlertDialog.Builder (Self)
				.SetTitle("Connection Request")
				.SetMessage("Do you want to connect to " + remoteEndpointName + "?")
				.SetCancelable(false)
				.SetPositiveButton("Connect", (sender, e) =>
				{
					byte[] pLoad = null;
					NearbyClass.Connections.AcceptConnectionRequest(Self.mGoogleApiClient,
						remoteEndpointId, pLoad, Self).SetResultCallback((Statuses status) =>
						{
							if (status.IsSuccess)
							{
								Self.DebugLog("acceptConnectionRequest: SUCCESS");
								Self.mOtherEndpointId = remoteEndpointId;
								Self.UpdateViewVisibility(NearbyConnectionState.Connected);
							}
							else
							{
								Self.DebugLog("acceptConnectionRequest: FAILURE");
							}
						});
				})
				.SetNegativeButton("No", (sender, e) => NearbyClass.Connections.RejectConnectionRequest(Self.mGoogleApiClient, remoteEndpointId))
				.Create();

			Self.mConnectionRequestDialog.Show();
		}
	}
}
