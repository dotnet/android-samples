using System;
using Android.App;
using Android.Gms.Common.Apis;
using Java.Util.Concurrent;
using Android.Util;
using Android.Content;
using Android.Gms.Wearable;
using Android.Support.Wearable.Activity;
using Android.Runtime;

namespace Wearable
{
	[Service ()]
	public class DeleteService : IntentService, GoogleApiClient.IConnectionCallbacks, GoogleApiClient.IOnConnectionFailedListener
	{
		// Timeout for making a connection to GoogleApiClient (in milliseconds)
		private const long TIME_OUT = 100;

		private GoogleApiClient mGoogleApiClient;


		public DeleteService ()
			:base(typeof(DeleteService).Name)
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
			mGoogleApiClient.BlockingConnect (TIME_OUT, TimeUnit.Milliseconds);
			Android.Net.Uri dataItemUri = intent.Data;
			if (Log.IsLoggable (Constants.TAG, LogPriority.Verbose)) {
				Log.Verbose (Constants.TAG, "DeleteService.OnHandleIntent = " + dataItemUri);
			}
			if (mGoogleApiClient.IsConnected) {
				IDataApiDeleteDataItemsResult result = WearableClass.DataApi
					.DeleteDataItems (mGoogleApiClient, dataItemUri).Await().JavaCast<IDataApiDeleteDataItemsResult>();
				if (result.Status.IsSuccess && !intent.GetBooleanExtra (Constants.EXTRA_SILENT, false)) {
					// Show the success animaton on the watch unless Silent extra is true.
					StartConfirmationActivity (ConfirmationActivity.SuccessAnimation, GetString (Resource.String.delete_successful));
				} else {
					if (Log.IsLoggable (Constants.TAG, LogPriority.Verbose)) {
						Log.Verbose (Constants.TAG, "DeleteService.OnHandleIntent: Failed to delete dataITem:"
						+ dataItemUri);
					}

					// Show the failure animation on the watch unless Silent extra is true.
					if (!intent.GetBooleanExtra (Constants.EXTRA_SILENT, false)) {
						StartConfirmationActivity (ConfirmationActivity.FailureAnimation, GetString (Resource.String.delete_unsuccessful));
					}
				}
			}
			else { 
				Log.Error (Constants.TAG, "Failed to delete data item: " + dataItemUri +
				" - Client disconnected from Google Play Services");
				// Show the failure animation on the watch unless Silent extra is true.
				if (!intent.GetBooleanExtra (Constants.EXTRA_SILENT, false)) {
					StartConfirmationActivity (ConfirmationActivity.FailureAnimation, GetString (Resource.String.delete_unsuccessful));
				}
			}
			mGoogleApiClient.Disconnect ();
		}

		private void StartConfirmationActivity(int animationType, string Message)
		{
			Intent confirmationActivity = new Intent (this, typeof(ConfirmationActivity))
				.SetFlags (ActivityFlags.NewTask | ActivityFlags.NoAnimation)
				.PutExtra (ConfirmationActivity.ExtraAnimationType, animationType)
				.PutExtra (ConfirmationActivity.ExtraMessage, Message);
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

