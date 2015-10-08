using System;
using Android.App;
using Android.Gms.Common.Apis;
using Android.Gms.Wearable;
using Android.Support.Wearable.Activity;
using Java.Interop;
using Android.Util;
using Java.Util.Concurrent;
using Android.Content;

namespace Wearable
{
	[Service()]
	public class CheckInAndDeleteDataItemsService : IntentService, GoogleApiClient.IConnectionCallbacks, GoogleApiClient.IOnConnectionFailedListener
	{
		private GoogleApiClient mGoogleApiClient;
		public CheckInAndDeleteDataItemsService ()
			:base(typeof(CheckInAndDeleteDataItemsService).Name)
		{

		}

		public override void OnCreate ()
		{
			base.OnCreate ();
			mGoogleApiClient = new GoogleApiClient.Builder (this)
				.AddApi (WearableClass.API)
				.AddConnectionCallbacks (this)
				.AddOnConnectionFailedListener (this)
				.Build ();
		}

		protected override void OnHandleIntent (Android.Content.Intent intent)
		{
			if (Constants.ACTION_CHECK_IN.Equals (intent.Action)) {
				// In a real app, code for checking in would go here. For this sample, we will simply disaply a success animation
				StartConfirmationActivity (ConfirmationActivity.SuccessAnimation, GetString (Resource.String.check_in_success));
				// Dismiss the check-in notification
				GetSystemService (NotificationService).JavaCast<NotificationManager> ().Cancel (Constants.NOTIFICATION_ID);
			} else if (!Constants.ACTION_DELETE_DATA_ITEM.Equals (intent.Action)) {
				// The only possible actions should be checking in or dismissing the notification
				// (which causes an intent with ACTION_DELETE_DATA_ITEM)
				Log.Error (Constants.TAG, "Unrecognized Action: " + intent.Action);
				return;
			}

			// Regardless of the action, delete the DataItem (we are only handling intents if the notification is dismissed or if the user
			// has chosen to check in, either of which would be completed at this point
			mGoogleApiClient.BlockingConnect (Constants.CONNECTION_TIME_OUT_MS, TimeUnit.Milliseconds);
			Android.Net.Uri dataItemUri = intent.Data;
			if (mGoogleApiClient.IsConnected) {
				var result = WearableClass.DataApi.DeleteDataItems (mGoogleApiClient, dataItemUri).Await ().JavaCast<IDataApiDeleteDataItemsResult> ();
				if (!result.Status.IsSuccess) {
					Log.Error (Constants.TAG, "CheckInAndDeleteDataItemsService.OnHandleIntent: " +
					"Failed to delete dataItem: " + dataItemUri);
				} else if (Log.IsLoggable (Constants.TAG, LogPriority.Debug)) {
					Log.Debug (Constants.TAG, "Successfully deleted data item: " + dataItemUri);
				}
			} else {
				Log.Error (Constants.TAG, "Failed to delete data item: " + dataItemUri
				+ " - Client disconnected from Google Play Services");
			}
			mGoogleApiClient.Disconnect ();
		}

		private void StartConfirmationActivity(int animationType, String message) {
			Intent confirmationActivity = new Intent (this, typeof(ConfirmationActivity))
				.SetFlags (ActivityFlags.NewTask | ActivityFlags.NoAnimation)
				.PutExtra (ConfirmationActivity.ExtraAnimationType, animationType)
				.PutExtra (ConfirmationActivity.ExtraMessage, message);
			StartActivity (confirmationActivity);
		}

		public void OnConnected (Android.OS.Bundle p0)
		{

		}

		public void OnConnectionSuspended (int p0)
		{

		}

		public void OnConnectionFailed (Android.Gms.Common.ConnectionResult p0)
		{

		}
	}
}

