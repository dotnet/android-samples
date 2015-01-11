﻿using System;
using System.Linq;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Gms.Wearable;
using Android.Gms.Common;
using Android.Gms.Common.Apis;
using Android.Util;
using System.Collections.Generic;
using Android.Gms.Common.Data;
using Java.Interop;

namespace AgendaData
{
	[Activity (Label = "AgendaData", MainLauncher = true, Icon = "@drawable/icon")]
	public class MainActivity : Activity, INodeApiNodeListener, IGoogleApiClientOnConnectionFailedListener, IGoogleApiClientConnectionCallbacks
	{

		// Request code for launching the Intent to resolve Google Play services errors
		private const int REQUEST_RESOLVE_ERROR = 1000;

		private IGoogleApiClient mGoogleApiClient;
		private bool mResolvingError = false;

		private TextView mLogTextView;
		ScrollView mSCroller;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			// Set our view from the "main" layout resource
			SetContentView (Resource.Layout.main);
			mLogTextView = (TextView)FindViewById (Resource.Id.log);
			mSCroller = (ScrollView)FindViewById (Resource.Id.scroller);
			mGoogleApiClient = new GoogleApiClientBuilder (this).AddApi (WearableClass.Api)
				.AddConnectionCallbacks (this)
				.AddOnConnectionFailedListener (this)
				.Build ();
		}

		protected override void OnStart ()
		{
			base.OnStart ();
			if (!mResolvingError)
				mGoogleApiClient.Connect ();
		}
		protected override void OnStop ()
		{
			if (mGoogleApiClient.IsConnected) {
				WearableClass.NodeApi.RemoveListener (mGoogleApiClient, this);
			}
			mGoogleApiClient.Disconnect ();
			base.OnStop ();
		}
		[Export("onGetEventsClicked")]
		public void OnGetEventsClicked(View v)
		{
				StartService(new Intent(this, typeof(CalendarQueryService)));
		}

		[Export("onDeleteEventsClicked")]
		public void OnDeleteEventsClicked(View v)
		{
			if (mGoogleApiClient.IsConnected) {
				WearableClass.DataApi.GetDataItems (mGoogleApiClient)
						.SetResultCallback (new ResultCallback () {
						OnResultFunc = (Java.Lang.Object result) =>
							{
							var deleteResult = Android.Runtime.Extensions.JavaCast<DataItemBuffer>(result);
							if (deleteResult.Status.IsSuccess) {
								DeleteDataItems(deleteResult);
								}
								else {
									if (Log.IsLoggable(Constants.TAG, LogPriority.Debug)) {
										Log.Debug(Constants.TAG, "OnDeleteEventsClicked(): failed to get Data Items");
									}
								}
							}
				});
			}
		}
		private void DeleteDataItems(DataItemBuffer dataItems)
		{
			if (mGoogleApiClient.IsConnected) {
				// Store the DataItem URIs in a List and close the bugger. Then we use these URIs to delete the DataItems.
				var freezableItems = FreezableUtils.FreezeIterable (dataItems);
				var dataItemList = new List<IDataItem> ();
				for (int i = 0; i < freezableItems.Count; i++) {
					Log.Verbose (Constants.TAG, "Loop " + i);

					dataItemList.Add (Android.Runtime.Extensions.JavaCast<IDataItem> ((IJavaObject)freezableItems [i]));
				}
				dataItems.Close ();
				if (dataItemList.Count > 0) {
					foreach (IDataItem dataItem in dataItemList) {
						Android.Net.Uri dataItemUri = dataItem.Uri;
						// In a real calendar application, this might delete the corresponding calendar event from the calendar
						// data provider. In this sample, we simply delete the
						// DataItem, but leave the phone's calendar data intact
						WearableClass.DataApi.DeleteDataItems (mGoogleApiClient, dataItemUri)
						.SetResultCallback (new ResultCallback () {
								OnResultFunc = (Java.Lang.Object deleteResult) => {
									if (Android.Runtime.Extensions.JavaCast<IDataApiDeleteDataItemsResult>(deleteResult).Status.IsSuccess) {
									AppendLog ("Successfully deleted data item: " + dataItemUri);
								} else {
									AppendLog ("Failed to delete data item: " + dataItemUri);
								}
							}
						});
					}
				} else {
					AppendLog ("There are no data items");
				}
			} else {
				Log.Error (Constants.TAG, "Failed to delete data items" + " - Client disconnected from Google Play Services");
			}
		}
		private class ResultCallback : Java.Lang.Object, IResultCallback
		{
			public Action<Java.Lang.Object> OnResultFunc;
			public void OnResult (Java.Lang.Object res)
			{
				if (OnResultFunc != null) {
					OnResultFunc (res);
				}
			}
		}
		private void AppendLog(string s)
		{
			mLogTextView.Post (() => {
				mLogTextView.Append(s);
				mLogTextView.Append(System.Environment.NewLine);
				mSCroller.FullScroll(FocusSearchDirection.Down);
			});
		}

		public void OnPeerConnected (INode peer)
		{
			AppendLog ("Device connected");
		}

		public void OnPeerDisconnected (INode peer)
		{
			AppendLog ("Device disconnected");
		}

		public void OnConnected (Bundle connectionHint)
		{
			if (Log.IsLoggable (Constants.TAG, LogPriority.Debug)) {
				Log.Debug (Constants.TAG, "Connected to Google Api Service");
			}
		}

		public void OnConnectionSuspended (int cause)
		{
			// Ignore
		}

		public void OnConnectionFailed (ConnectionResult result)
		{
			if (Log.IsLoggable(Constants.TAG, LogPriority.Debug)) {
				Log.Debug (Constants.TAG, "Disconnected from Google Api Service");
			}
			if (WearableClass.NodeApi != null) {
				WearableClass.NodeApi.RemoveListener (mGoogleApiClient, this);
			}
			if (mResolvingError) {
				// Already attempting to resolve the error
				return;
			} else if (result.HasResolution) {
				try {
					mResolvingError = true;
					result.StartResolutionForResult (this, REQUEST_RESOLVE_ERROR);
				} catch {
					// There was an error with the resolution intent. Try again
					mGoogleApiClient.Connect ();
				}
			} else {
				mResolvingError = false;
			}
		}
	}
}


