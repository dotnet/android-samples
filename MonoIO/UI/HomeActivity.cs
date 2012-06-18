/*
 * Copyright 2011 Google Inc.
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
using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using MonoIO.Utilities;
using MonoIO.UI;
using Fragment = Android.Support.V4.App.Fragment;
using Android.Support.V4.App;
using Android.Graphics;

namespace MonoIO
{
	[Activity (Label = "@string/app_name", MainLauncher = true, Theme = "@style/Theme.IOSched.Home")]
	public class HomeActivity : BaseActivity
	{
		private static String TAG = "HomeActivity";
		TagStreamFragment tagStream;
		SyncStatusUpdaterFragment syncStatusUpdaterFragment;
		
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			
			if (!EulaHelper.HasAcceptedEula (this)) {
				EulaHelper.ShowEula (false, this);
			}

			//AnalyticsUtils.getInstance(this).trackPageView("/Home");
			
			SetContentView (Resource.Layout.ActivityHome);
			
			ActivityHelper.SetupActionBar (null, new Color (0));
			
			var fm = SupportFragmentManager;
			
			tagStream = fm.FindFragmentById (Resource.Id.fragment_tag_stream) as TagStreamFragment;
			
			syncStatusUpdaterFragment = fm.FindFragmentByTag (SyncStatusUpdaterFragment.TAG) as SyncStatusUpdaterFragment;
			if (syncStatusUpdaterFragment == null) {
				syncStatusUpdaterFragment = new SyncStatusUpdaterFragment ();
				fm.BeginTransaction ().Add (syncStatusUpdaterFragment, SyncStatusUpdaterFragment.TAG).Commit ();
	
				TriggerRefresh ();
			}
			
		}
		
		protected override void OnPostCreate (Bundle savedInstanceState)
		{
			base.OnPostCreate (savedInstanceState);
			ActivityHelper.SetupHomeActivity ();
		}
		
		public override bool OnCreateOptionsMenu (IMenu menu)
		{
			MenuInflater.Inflate (Resource.Menu.refresh_menu_items, menu);
			base.OnCreateOptionsMenu (menu);
			return true;
		}
		
		public override bool OnOptionsItemSelected (IMenuItem item)
		{
			if (item.ItemId == Resource.Id.menu_refresh) {
				TriggerRefresh ();
				return true;
			}
			return base.OnOptionsItemSelected (item);
		}
		
		void TriggerRefresh ()
		{
			Console.WriteLine ("Trigger refreshed...");
			Intent intent = new Intent (Intent.ActionSync, null, this, typeof(SyncService));
			intent.PutExtra (SyncService.EXTRA_STATUS_RECEIVER, syncStatusUpdaterFragment.mReceiver);
			StartService (intent);
			
			if (tagStream != null)	
				tagStream.Refresh ();
		}
		
		private void UpdateRefreshStatus (bool refreshing)
		{
			ActivityHelper.SetRefreshActionButtonCompatState (refreshing);
		}
		
		/**
		* A non-UI fragment, retained across configuration changes, that updates its activity's UI
		* when sync status changes.
		*/
		public class SyncStatusUpdaterFragment : Fragment, DetachableResultReceiver.Receiver
		{
			public static string TAG = Java.Lang.Class.FromType (typeof(SyncStatusUpdaterFragment)).Name;
			bool syncing = false;
			public DetachableResultReceiver mReceiver;
	
			public override void OnCreate (Bundle savedInstanceState)
			{
				base.OnCreate (savedInstanceState);
				RetainInstance = true;
				mReceiver = new DetachableResultReceiver (new Handler ());
				mReceiver.SetReceiver (this);
			}
			
			public void OnReceiveResult (int resultCode, Bundle resultData)
			{
				HomeActivity activity = (HomeActivity)Activity;
				if (activity == null) {
					return;
				}
	
				switch (resultCode) {
				case (int) SyncService.StatusRunning: {
						syncing = true;
						break;
					}
				case (int) SyncService.StatusFinished: {
						syncing = false;
						break;
					}
				case (int) SyncService.StatusError: {
						// Error happened down in SyncService, show as toast.
						syncing = false;
						String errorText = GetString (Resource.String.toast_sync_error, resultData.GetString (Intent.ExtraText));
						Toast.MakeText (activity, errorText, ToastLength.Long).Show ();
						break;
					}
				}
	
				activity.UpdateRefreshStatus (syncing);
			}
			
			public override void OnActivityCreated (Bundle savedInstanceState)
			{
				base.OnActivityCreated (savedInstanceState);
				((HomeActivity)Activity).UpdateRefreshStatus (syncing);
			}
		}
	}
}

