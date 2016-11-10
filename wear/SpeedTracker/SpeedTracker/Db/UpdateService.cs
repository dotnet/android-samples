using System;
using System.Collections.Generic;
using Android.App;
using Android.Gms.Common;
using Android.Gms.Common.Apis;
using Android.Gms.Wearable;
using Android.Net;
using Android.OS;
using Android.Util;
using Java.Util;
using Shared;
using Object = Java.Lang.Object;
using Uri = Android.Net.Uri;

namespace SpeedTracker.Db
{
	/**
	 * A {@link com.google.android.gms.wearable.WearableListenerService} that is responsible for
	 * reading location data that gets added to the Data Layer storage.
	 */
	 [Service]
	 [IntentFilter(new []{ "com.google.android.gms.wearable.DATA_CHANGED" }, 
		DataHost = "*", 
		DataScheme = "wear", 
		DataPathPrefix = "/location")]
	public class UpdateService : WearableListenerService, GoogleApiClient.IConnectionCallbacks, 
		GoogleApiClient.IOnConnectionFailedListener
	{
		private static readonly string TAG = "UpdateService";
		private LocationDataManager mDataManager;
		private GoogleApiClient mGoogleApiClient;
		private HashSet<Uri> mToBeDeletedUris = new HashSet<Uri>();
		public static readonly string ACTION_NOTIFY = "SpeedTracker.wearable.speedtracker.Message";
		public static readonly string EXTRA_ENTRY = "entry";
		public static readonly string EXTRA_LOG = "log";

		public override void OnCreate()
		{
			base.OnCreate();
			mGoogleApiClient = new GoogleApiClient.Builder(this)
				.AddApi(WearableClass.API)
				.AddConnectionCallbacks(this)
				.AddOnConnectionFailedListener(this)
				.Build();
			mGoogleApiClient.Connect();
			mDataManager = ((PhoneApplication)ApplicationContext).GetDataManager();
		}

		public override void OnDataChanged(DataEventBuffer dataEvents)
		{
			foreach (var dataEvent in dataEvents)
			{
				if (!dataEvent.GetType().IsEquivalentTo(DataEvent.TypeChanged.GetType())) continue;
				var dataItemUri = dataEvent.DataItem.Uri;
				if (Log.IsLoggable(TAG, LogPriority.Debug))
				{
					Log.Debug(TAG, "Received a data item with uri: " + dataItemUri.Path);
				}
				if (!dataItemUri.Path.StartsWith(Constants.Path)) continue;
				var dataMap = DataMapItem.FromDataItem(dataEvent.DataItem).DataMap;
				var longitude = dataMap.GetDouble(Constants.KeyLongitude);
				var latitude = dataMap.GetDouble(Constants.KeyLatitude);
				var time = dataMap.GetLong(Constants.KeyTime);
				var calendar = Calendar.Instance;
				calendar.TimeInMillis = time;
				mDataManager.AddPoint(new LocationEntry(calendar, latitude, longitude));
				if (mGoogleApiClient.IsConnected)
				{
					WearableClass.DataApi.DeleteDataItems(
						mGoogleApiClient, dataItemUri).SetResultCallback(new ResultCallbackImplemented());
				}
				else
				{
					lock(mToBeDeletedUris) {
						mToBeDeletedUris.Add(dataItemUri);
					}
				}
			}
		}

		public void OnConnected(Bundle bundle)
		{
			if (Log.IsLoggable(TAG, LogPriority.Debug))
			{
				Log.Debug(TAG, "onConnected(): api client is connected now");
			}
			lock (mToBeDeletedUris)
			{
				if (mToBeDeletedUris.Count == 0) return;
				foreach (var dataItemUri in mToBeDeletedUris)
				{
					WearableClass.DataApi.DeleteDataItems(
						mGoogleApiClient, dataItemUri).SetResultCallback(new ResultCallbackImplemented());
				}
			}
		}

		public void OnConnectionSuspended(int cause)
		{
		}

		public void OnConnectionFailed(ConnectionResult result)
		{
			Log.Error(TAG, "Failed to connect to the Google API client");
		}

		private class ResultCallbackImplemented : Java.Lang.Object, IResultCallback
		{
			public void OnResult(Object result)
			{
				var deleteDataItemsResult = result as IDataApiDeleteDataItemsResult;
				if (deleteDataItemsResult != null && !deleteDataItemsResult.Status.IsSuccess)
				{
					Log.Error(TAG,
							"Failed to delete a dataItem, status code: " + deleteDataItemsResult.Status.StatusCode + 
							deleteDataItemsResult.Status.StatusMessage);
				}
			}
		}
	}

}