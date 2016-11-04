/*
 * Copyright (C) 2014 Google Inc. All Rights Reserved.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Gms.Common;
using Android.Gms.Common.Apis;
using Android.Gms.Location;
using Android.Gms.Wearable;
using Android.Locations;
using Android.OS;
using Android.Preferences;
using Android.Support.V4.App;
using Android.Support.Wearable.Activity;
using Android.Util;
using Android.Views;
using Android.Widget;
using Java.Lang;
using Java.Util;
using Java.Util.Concurrent;
using Shared;
using ILocationListener = Android.Gms.Location.ILocationListener;

namespace Wearable
{
	/**
	 * The main activity for the wearable app. User can pick a speed limit, and after this activity
	 * obtains a fix on the GPS, it starts reporting the speed. In addition to showing the current
	 * speed, if user's speed gets close to the selected speed limit, the color of speed turns yellow
	 * and if the user exceeds the speed limit, it will turn red. In order to show the user that GPS
	 * location data is coming in, a small green dot keeps on blinking while GPS data is available.
	 */
	public class WearableMainActivity : WearableActivity, GoogleApiClient.IConnectionCallbacks, 
		GoogleApiClient.IOnConnectionFailedListener, ActivityCompat.IOnRequestPermissionsResultCallback,
		ILocationListener
	{
		private static readonly string TAG = "WearableActivity";

		private static readonly long UpdateIntervalMs = TimeUnit.Seconds.ToMillis(5);
		private static readonly long FastestIntervalMs = TimeUnit.Seconds.ToMillis(5);

		private static readonly float MphInMetersPerSecond = 2.23694f;

		private static readonly int SPEED_LIMIT_DEFAULT_MPH = 45;

		private static readonly long INDICATOR_DOT_FADE_AWAY_MS = 500L;

		// Request codes for changing speed limit and location permissions.
		private static readonly int RequestPickSpeedLimit = 0;

		// Id to identify Location permission request.
		private static readonly int RequestGpsPermission = 1;

		// Shared Preferences for saving speed limit and location permission between app launches.
		private static readonly string PrefsSpeedLimitKey = "SpeedLimit";

		private Calendar mCalendar;

		private TextView mSpeedLimitTextView;
		private TextView mSpeedTextView;
		private ImageView mGpsPermissionImageView;
		private TextView mCurrentSpeedMphTextView;
		private TextView mGpsIssueTextView;
		private View mBlinkingGpsStatusDotView;

		private string mGpsPermissionNeededMessage;
		private string mAcquiringGpsMessage;

		private int mSpeedLimit;
		private float mSpeed;

		private bool mGpsPermissionApproved;

		private bool mWaitingForGpsSignal;

		private GoogleApiClient mGoogleApiClient;

		private Handler mHandler = new Handler();

		private enum SpeedState
		{
			Below = Resource.Color.speed_below,
			Close = Resource.Color.speed_close,
			Above = Resource.Color.speed_above
		}

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			Log.Debug(TAG, "onCreate()");

			SetContentView(Resource.Layout.ActivityMain);

			/*
			 * Enables Always-on, so our app doesn't shut down when the watch goes into ambient mode.
			 * Best practice is to override onEnterAmbient(), onUpdateAmbient(), and onExitAmbient() to
			 * optimize the display for ambient mode. However, for brevity, we aren't doing that here
			 * to focus on learning location and permissions. For more information on best practices
			 * in ambient mode, check this page:
			 * https://developer.android.com/training/wearables/apps/always-on.html
			 */
			SetAmbientEnabled();

			mCalendar = Calendar.Instance;

			// Enables app to handle 23+ (M+) style permissions.
			mGpsPermissionApproved = ActivityCompat.CheckSelfPermission(this, 
				Manifest.Permission.AccessFineLocation) == Permission.Granted;

			mGpsPermissionNeededMessage = GetString(Resource.String.permission_rationale);
			mAcquiringGpsMessage = GetString(Resource.String.acquiring_gps);

			var sharedPreferences = PreferenceManager.GetDefaultSharedPreferences(this);
			mSpeedLimit = sharedPreferences.GetInt(PrefsSpeedLimitKey, SPEED_LIMIT_DEFAULT_MPH);

			mSpeed = 0;

			mWaitingForGpsSignal = true;

			/*
			 * If this hardware doesn't support GPS, we warn the user. Note that when such device is
			 * connected to a phone with GPS capabilities, the framework automatically routes the
			 * location requests from the phone. However, if the phone becomes disconnected and the
			 * wearable doesn't support GPS, no location is recorded until the phone is reconnected.
			 */
			if (!HasGps())
			{
				Log.Warn(TAG, "This hardware doesn't have GPS, so we warn user.");
				new AlertDialog.Builder(this)
					.SetMessage(GetString(Resource.String.gps_not_available))
					.SetPositiveButton(GetString(Resource.String.ok), new OnPositiveGpsAlertListener())
					.SetOnDismissListener(new OnDismisGpsAlertListener())
					.SetCancelable(false)
					.Create()
					.Show();
			}

			SetupViews();

			mGoogleApiClient = new GoogleApiClient.Builder(this)
				.AddApi(LocationServices.API)
				.AddApi(WearableClass.API)
				.AddConnectionCallbacks(this)
				.AddOnConnectionFailedListener(this)
				.Build();
		}

		protected override void OnPause()
		{
			base.OnPause();
			if ((mGoogleApiClient == null) || (!mGoogleApiClient.IsConnected) || (!mGoogleApiClient.IsConnecting)) return;
			LocationServices.FusedLocationApi.RemoveLocationUpdates(mGoogleApiClient, this);
			mGoogleApiClient.Disconnect();
		}

		protected override void OnResume()
		{
			base.OnResume();
			if (mGoogleApiClient != null)
			{
				mGoogleApiClient.Connect();
			}
		}

		private void SetupViews()
		{
			mSpeedLimitTextView = (TextView)FindViewById(Resource.Id.max_speed_text);
			mSpeedTextView = (TextView)FindViewById(Resource.Id.current_speed_text);
			mCurrentSpeedMphTextView = (TextView)FindViewById(Resource.Id.current_speed_mph);

			mGpsPermissionImageView = (ImageView)FindViewById(Resource.Id.gps_permission);
			mGpsIssueTextView = (TextView)FindViewById(Resource.Id.gps_issue_text);
			mBlinkingGpsStatusDotView = FindViewById(Resource.Id.dot);

			UpdateActivityViewsBasedOnLocationPermissions();
		}

		public void OnSpeedLimitClick(View view)
		{
			var speedIntent = new Intent(this, typeof(SpeedPickerActivity));
			StartActivityForResult(speedIntent, RequestPickSpeedLimit);
		}

		public void OnGpsPermissionClick(View view)
		{
			if (mGpsPermissionApproved) return;
			Log.Info(TAG, "Location permission has NOT been granted. Requesting permission.");

			// On 23+ (M+) devices, GPS permission not granted. Request permission.
			ActivityCompat.RequestPermissions(
				this,
				new string[] { Manifest.Permission.AccessFineLocation },
				RequestGpsPermission);
		}

		/**
		 * Adjusts the visibility of views based on location permissions.
		 */
		private void UpdateActivityViewsBasedOnLocationPermissions()
		{
			/*
			 * If the user has approved location but we don't have a signal yet, we let the user know
			 * we are waiting on the GPS signal (this sometimes takes a little while). Otherwise, the
			 * user might think something is wrong.
			 */
			if (mGpsPermissionApproved && mWaitingForGpsSignal)
			{
				// We are getting a GPS signal w/ user permission.
				mGpsIssueTextView.Text = mAcquiringGpsMessage;
				mGpsIssueTextView.Visibility = ViewStates.Visible;
				mGpsPermissionImageView.SetImageResource(Resource.Drawable.ic_gps_saving_grey600_96dp);

				mSpeedTextView.Visibility = ViewStates.Gone;
				mSpeedLimitTextView.Visibility = ViewStates.Gone;
				mCurrentSpeedMphTextView.Visibility = ViewStates.Gone;
			} 
			else if (mGpsPermissionApproved)
			{
				mGpsIssueTextView.Visibility = ViewStates.Gone;

				mSpeedTextView.Visibility = ViewStates.Visible;
				mSpeedLimitTextView.Visibility = ViewStates.Visible;
				mCurrentSpeedMphTextView.Visibility = ViewStates.Visible;
				mGpsPermissionImageView.SetImageResource(Resource.Drawable.ic_gps_saving_grey600_96dp);
			}
			else
			{
				// User needs to enable location for the app to work.
				mGpsIssueTextView.Visibility = ViewStates.Visible;
				mGpsIssueTextView.Text = mGpsPermissionNeededMessage;
				mGpsPermissionImageView.SetImageResource(Resource.Drawable.ic_gps_not_saving_grey600_96dp);

				mSpeedTextView.Visibility = ViewStates.Gone;
				mSpeedLimitTextView.Visibility = ViewStates.Gone;
				mCurrentSpeedMphTextView.Visibility = ViewStates.Gone;
			}
		}

		private void UpdateSpeedInViews()
		{
			if (mGpsPermissionApproved)
			{
				mSpeedLimitTextView.Text = GetString(Resource.String.speed_limit);
				mSpeedTextView.Text = string.Format(GetString(Resource.String.speed_format), mSpeed);

				// Adjusts the color of the speed based on its value relative to the speed limit.
				var state = (int) SpeedState.Above;
				if (mSpeed <= mSpeedLimit - 5)
				{
					state = (int) SpeedState.Below;
				}
				else if (mSpeed <= mSpeedLimit)
				{
					state = (int) SpeedState.Close;
				}
				mSpeedTextView.SetTextColor(Resources.GetColor(state));

				// Causes the (green) dot blinks when new GPS location data is acquired.
				mHandler.Post(new BlinkingGpsStatusDowViewVisibility(true, mBlinkingGpsStatusDotView));
				mBlinkingGpsStatusDotView.Visibility = ViewStates.Visible;
				mHandler.PostDelayed(new BlinkingGpsStatusDowViewVisibility(false, mBlinkingGpsStatusDotView),
					INDICATOR_DOT_FADE_AWAY_MS);
			}
		}

		private class BlinkingGpsStatusDowViewVisibility : Java.Lang.Object, IRunnable
		{
			bool Visible { get; set; }
			View BlinkingGpsStatusDotView { get; set; }

			public BlinkingGpsStatusDowViewVisibility(bool visible, View mBlinkingGpsStatusDotView)
			{
				Visible = visible;
				BlinkingGpsStatusDotView = mBlinkingGpsStatusDotView;
			}

			public void Run()
			{
				BlinkingGpsStatusDotView.Visibility = Visible ? ViewStates.Visible : ViewStates.Invisible;
			}
		}

		public void OnConnected(Bundle bundle)
		{
			Log.Debug(TAG, "onConnected()");
			RequestLocation();
		}

		private void RequestLocation()
		{
			Log.Debug(TAG, "requestLocation()");

			/*
			 * mGpsPermissionApproved covers 23+ (M+) style permissions. If that is already approved or
			 * the device is pre-23, the app uses mSaveGpsLocation to save the user's location
			 * preference.
			 */
			if (mGpsPermissionApproved)
			{
				var locationRequest = LocationRequest.Create()
					.SetPriority(LocationRequest.PriorityHighAccuracy)
					.SetInterval(UpdateIntervalMs)
					.SetFastestInterval(FastestIntervalMs);

				LocationServices.FusedLocationApi
					.RequestLocationUpdates(mGoogleApiClient, locationRequest, this)
					.SetResultCallback(new RequestLocationResultCallback());
			}
		}

		private class RequestLocationResultCallback : Java.Lang.Object, IResultCallback
		{
			public void OnResult(Object result)
			{
				var status = result as LocationSettingsResult;
				if (status == null) return;
				if (status.Status.IsSuccess)
				{
					if (Log.IsLoggable(TAG, LogPriority.Debug))
					{
						Log.Debug(TAG, "Successfully requested location updates");
					}
				}
				else
				{
					Log.Error(TAG,"Failed in requesting location updates, " + "status code: "
						+ status.Status.StatusCode + ", message: " + status.Status.StatusMessage);
				}
			}
		}

		public void OnConnectionSuspended(int cause)
		{
			Log.Debug(TAG, "onConnectionSuspended(): connection to location client suspended");
			LocationServices.FusedLocationApi.RemoveLocationUpdates(mGoogleApiClient, this);
		}

		public void OnConnectionFailed(ConnectionResult result)
		{
			Log.Error(TAG, "onConnectionFailed(): " + result.ErrorMessage);
		}

		public void OnLocationChanged(Location location)
		{
			Log.Debug(TAG, "onLocationChanged() : " + location);

			if (mWaitingForGpsSignal)
			{
				mWaitingForGpsSignal = false;
				UpdateActivityViewsBasedOnLocationPermissions();
			}

			mSpeed = location.Speed * MphInMetersPerSecond;
			UpdateSpeedInViews();
			AddLocationEntry(location.Latitude, location.Longitude);
		}

		/*
		 * Adds a data item to the data Layer storage.
		 */
		private void AddLocationEntry(double latitude, double longitude)
		{
			if (!mGpsPermissionApproved || !mGoogleApiClient.IsConnected)
			{
				return;
			}
			mCalendar.TimeInMillis = JavaSystem.CurrentTimeMillis();
			var entry = new LocationEntry(mCalendar, latitude, longitude);
			var path = Constants.Path + "/" + mCalendar.TimeInMillis;
			var putDataMapRequest = PutDataMapRequest.Create(path);

			putDataMapRequest.DataMap.PutDouble(Constants.KeyLatitude, entry.latitude);
			putDataMapRequest.DataMap.PutDouble(Constants.KeyLongitude, entry.longitude);
			putDataMapRequest.DataMap.PutLong(Constants.KeyTime, entry.calendar.TimeInMillis);

			var request = putDataMapRequest.AsPutDataRequest();
			request.SetUrgent();

			WearableClass.DataApi.PutDataItem(mGoogleApiClient, request).SetResultCallback(new DataApiResultCallback());
		}

		private class DataApiResultCallback : Java.Lang.Object, IResultCallback
		{
			public void OnResult(Object result)
			{
				var dataItemResult = result as IDataApiDataItemResult;
				if (!dataItemResult.Status.IsSuccess)
				{
					Log.Error(TAG, "AddPoint:onClick(): Failed to set the data, " + "status: " + dataItemResult.Status.StatusCode);
				}
			}
		}

		/**
		 * Handles user choices for both speed limit and location permissions (GPS tracking).
		 */
		protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
		{
			if (requestCode != RequestPickSpeedLimit) return;
			if (resultCode != Result.Ok) return;
			// The user updated the speed limit.
			var newSpeedLimit = data.GetIntExtra(SpeedPickerActivity.ExtraNewSpeedLimit, mSpeedLimit);

			var sharedPreferences = PreferenceManager.GetDefaultSharedPreferences(this);
			var editor = sharedPreferences.Edit();
			editor.PutInt(WearableMainActivity.PrefsSpeedLimitKey, newSpeedLimit);
			editor.Apply();

			mSpeedLimit = newSpeedLimit;

			UpdateSpeedInViews();
		}

		/**
		 * Callback received when a permissions request has been completed.
		 */
		public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
		{
			Log.Debug(TAG, "onRequestPermissionsResult(): " + permissions);

			if (requestCode != RequestGpsPermission) return;
			Log.Info(TAG, "Received response for GPS permission request.");

			if ((grantResults.Length == 1) && (grantResults[0] == Permission.Granted))
			{
				Log.Info(TAG, "GPS permission granted.");
				mGpsPermissionApproved = true;

				if (mGoogleApiClient != null && mGoogleApiClient.IsConnected)
				{
					RequestLocation();
				}
			}
			else
			{
				Log.Info(TAG, "GPS permission NOT granted.");
				mGpsPermissionApproved = false;
			}

			UpdateActivityViewsBasedOnLocationPermissions();
		}

		/**
		 * Returns {@code true} if this device has the GPS capabilities.
		 */
		private bool HasGps()
		{
			return PackageManager.HasSystemFeature(PackageManager.FeatureLocationGps);
		}

		private class OnPositiveGpsAlertListener : Java.Lang.Object, IDialogInterfaceOnClickListener
		{
			public void OnClick(IDialogInterface dialog, int which)
			{
				dialog.Cancel();
			}
		}

		private class OnDismisGpsAlertListener : Java.Lang.Object, IDialogInterfaceOnDismissListener
		{
			public void OnDismiss(IDialogInterface dialog)
			{
				throw new System.NotImplementedException();
			}
		}
	}
}