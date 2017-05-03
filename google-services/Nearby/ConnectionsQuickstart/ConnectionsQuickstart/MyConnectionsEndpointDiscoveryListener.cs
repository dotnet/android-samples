using System;
using System.Threading.Tasks;

using Android.Util;
using Android.Gms.Nearby.Connection;
using Android.App;
using Android.Gms.Common.Apis;
using Android.Gms.Nearby;
using Android.Widget;

namespace ConnectionsQuickstart
{
    public class MyConnectionsEndpointDiscoveryListener : ConnectionsEndpointDiscoveryListener
    {
        MainActivity Self;

        public MyConnectionsEndpointDiscoveryListener (MainActivity self)
        {
            Self = self;
        }

		async Task ConnectTo(string endpointId, string endpointName)
		{
            Self.DebugLog("connectTo:" + endpointId + ":" + endpointName);

			string myName = null;
			byte[] myPayload = null;
			var connectionResponseCallback = new ConnectionResponseCallback();

			connectionResponseCallback.OnConnectionResponseImpl = (remoteEndpointId, status, payload) =>
			{
				Log.Debug(MainActivity.TAG, "onConnectionResponse:" + remoteEndpointId + ":" + status);
				if (status.IsSuccess)
				{
                    Self.DebugLog("onConnectionResponse: " + endpointName + " SUCCESS");
					Toast.MakeText(Self, "Connected to " + endpointName,
                        ToastLength.Short).Show();

                    Self.mOtherEndpointId = remoteEndpointId;
                    Self.UpdateViewVisibility(NearbyConnectionState.Connected);
				}
				else
				{
                    Self.DebugLog("onConnectionResponse: " + endpointName + " FAILURE");
				}
			};

			await NearbyClass.Connections.SendConnectionRequestAsync(Self.mGoogleApiClient, myName, endpointId,
                myPayload, connectionResponseCallback, Self);
		}

        public override void OnEndpointFound(string endpointId, string serviceId, string name)
        {
            Log.Debug(MainActivity.TAG, "onEndpointFound:" + endpointId + ":" + name);

			if (Self.mMyListDialog == null)
			{
				var builder = new AlertDialog.Builder(Self)
					.SetTitle("Endpoint(s) Found")
					.SetCancelable(true)
					.SetNegativeButton("Cancel", (sender, e) => Self.mMyListDialog.Dismiss());

                // Create the MyListDialog with a listener
                Self.mMyListDialog = new MyListDialog(Self, builder, async (sender, e) =>
                {
                    var selectedEndpointName = Self.mMyListDialog.GetItemKey(e.Which);
                    var selectedEndpointId = Self.mMyListDialog.GetItemValue(e.Which);

                    await ConnectTo(selectedEndpointId, selectedEndpointName);
                    Self.mMyListDialog.Dismiss();
                });
			}

            Self.mMyListDialog.AddItem(name, endpointId);
            Self.mMyListDialog.Show();
        }

        public override void OnEndpointLost(string endpointId)
        {
            Self.DebugLog("onEndpointLost:" + endpointId);

			// An endpoint that was previously available for connection is no longer. It may have
			// stopped advertising, gone out of range, or lost connectivity. Dismiss any dialog that
			// was offering a connection.
			if (Self.mMyListDialog != null)
			{
                Self.mMyListDialog.RemoveItemByValue(endpointId);
			}
        }
    }

	class ConnectionResponseCallback : Java.Lang.Object, IConnectionsConnectionResponseCallback
	{
		public Action<string, Statuses, byte[]> OnConnectionResponseImpl { get; set; }

		public void OnConnectionResponse(string remoteEndpointId, Statuses status, byte[] payload)
		{
			OnConnectionResponseImpl(remoteEndpointId, status, payload);
		}
	}
}
