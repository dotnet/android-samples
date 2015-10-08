
using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Gms.Common.Apis;
using Android.Gms.Wearable;
using Android.Support.V4.App;
using Common;
using Android.Util;
using Android.Text.Format;
using Java.Interop;

namespace SynchronizedNotifications
{
	[Activity (Label = "@string/app_name", MainLauncher = true)]
	public class PhoneActivity : Activity, IResultCallback, GoogleApiClient.IConnectionCallbacks, GoogleApiClient.IOnConnectionFailedListener
	{
		const string Tag = "PhoneActivity";
		GoogleApiClient googleApiClient;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			SetContentView (Resource.Layout.activity_phone);
			googleApiClient = new GoogleApiClient.Builder (this)
				.AddApi (WearableClass.API)
				.AddConnectionCallbacks (this)
				.AddOnConnectionFailedListener (this)
				.Build ();
		}

		void BuildLocalOnlyNotification(string title, string content, int notificationId, bool withDismissal) {
			NotificationCompat.Builder builder = new NotificationCompat.Builder(this);
			builder.SetContentTitle (title)
				.SetContentText (content)
				.SetLocalOnly (true)
				.SetSmallIcon (Resource.Drawable.ic_launcher);

			if (withDismissal) {
				Intent dismissalIntent = new Intent (Constants.ActionDismiss);
				dismissalIntent.PutExtra (Constants.KeyNotificationId, Constants.BothId);
				PendingIntent pendingIndent = PendingIntent.GetService (this, 0, dismissalIntent, PendingIntentFlags.UpdateCurrent);
				builder.SetDeleteIntent(pendingIndent);
			}
			NotificationManagerCompat.From (this).Notify (notificationId, builder.Build ());
		}

		void BuildWearableOnlyNotification(string title, string content, string path) {
			if (googleApiClient.IsConnected) {
				var putDataMapRequest = PutDataMapRequest.Create (path);
				putDataMapRequest.DataMap.PutString (Constants.KeyContent, content);
				putDataMapRequest.DataMap.PutString (Constants.KeyTitle, title);
				var request = putDataMapRequest.AsPutDataRequest ();
				WearableClass.DataApi.PutDataItem (googleApiClient, request).SetResultCallback (this);
			} else {
				Log.Error (Tag, "BuildWearableOnlyNotification(): No Google API Client connection");
			}
		}

		public void OnResult (Java.Lang.Object result)
		{
			IDataApiDataItemResult dataItemResult;
			try 
			{
				dataItemResult = Java.Interop.JavaObjectExtensions.JavaCast<IDataApiDataItemResult>(result);
			}
			catch {
				return;
			}
			if (!dataItemResult.Status.IsSuccess) {
				Log.Error (Tag, "BuildWatchOnlyNotification(): Failed to set the data, status: " + dataItemResult.Status.StatusCode);
			}
		}

		private void BuildMirroredNotifications(string phoneTitle, string watchTitle, string content) {
			if (googleApiClient.IsConnected) {
				// Wearable notification
				BuildWearableOnlyNotification (watchTitle, content, Constants.BothPath);

				// Local notification, with a pending intent for dismissal
				BuildLocalOnlyNotification (phoneTitle, content, Constants.BothId, true);
			}
		}

		protected override void OnStart ()
		{
			base.OnStart ();
			googleApiClient.Connect ();
		}

		protected override void OnStop ()
		{
			googleApiClient.Disconnect ();
			base.OnStop ();
		}

		public void OnConnected (Bundle connectionHint)
		{

		}

		public void OnConnectionSuspended (int cause)
		{

		}

		public void OnConnectionFailed (Android.Gms.Common.ConnectionResult result)
		{
			Log.Error (Tag, "Failed to connect to Google API Client");
		}

		String Now {
			get {
				return DateTime.Now.ToString ();
			}
		}

		[Export("onClick")]
		public void OnClick(View view)
		{
			int id = view.Id;
			switch (id) {
			case Resource.Id.phone_only:
				BuildLocalOnlyNotification (GetString (Resource.String.phone_only), Now, Constants.PhoneOnlyId, false);
				break;
			case Resource.Id.wear_only:
				BuildWearableOnlyNotification (GetString (Resource.String.wear_only), Now, Constants.WatchOnlyPath);
				break;
			case Resource.Id.different_notifications:
				BuildMirroredNotifications (GetString (Resource.String.phone_both), GetString (Resource.String.watch_both), Now);
				break;
			}
		}
	}
}


