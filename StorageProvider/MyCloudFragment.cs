/*
* Copyright 2013 The Android Open Source Project
*
* Licensed under the Apache License, Version 2.0 (the "License");
* you may not use this file except in compliance with the License.
* You may obtain a copy of the License at
*
*     http://www.apache.org/licenses/LICENSE-2.0
*
* Unless required by applicable law or agreed to in writing, software
* distributed under the License is distributed on an "AS IS" BASIS,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the License for the specific language governing permissions and
* limitations under the License.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using CommonSampleLibrary;
using Android.Provider;

namespace StorageProvider
{
	/**
 	* Toggles the user's login status via a login menu option, and enables/disables the cloud storage
 	* content provider.
 	*/
	public class MyCloudFragment : Android.Support.V4.App.Fragment
	{
		static readonly string TAG = "MyCloudFragment";
		static readonly string AUTHORITY = "storageprovider.documents";
		bool mLoggedIn = false;

		public override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);
			mLoggedIn = ReadLoginValue ();

			SetHasOptionsMenu (true);
		}

		public override void OnPrepareOptionsMenu (IMenu menu)
		{
			base.OnPrepareOptionsMenu (menu);
			IMenuItem item = menu.FindItem (Resource.Id.sample_action);
			item.SetTitle (mLoggedIn ? Resource.String.log_out : Resource.String.log_in);
		}

		public override bool OnOptionsItemSelected (IMenuItem item)
		{
			if (item.ItemId == Resource.Id.sample_action) {
				ToggleLogin ();
				item.SetTitle (mLoggedIn ? Resource.String.log_out : Resource.String.log_in);

				// Notify the system that the status of our roots has changed.  This will trigger
				// a call to MyCloudProvider.queryRoots() and force a refresh of the system
				// picker UI.  It's important to call this or stale results may persist.
				Activity.ContentResolver.NotifyChange (DocumentsContract.BuildRootsUri (AUTHORITY), null, false);
			}
			return true;
		}

		/**
     	* Dummy function to change the user's authorization status.
     	*/
		void ToggleLogin ()
		{
			// Replace this with your standard method of authentication to determine if your app
			// should make the user's documents available.
			mLoggedIn = !mLoggedIn;
			WriteLoginValue (mLoggedIn);
			Log.Info (TAG, GetString (mLoggedIn ? Resource.String.logged_in_info : Resource.String.logged_out_info));
		}

		/**
     	* Dummy function to save whether the user is logged in.
     	*/
		void WriteLoginValue (bool loggedIn)
		{
			ISharedPreferences sharedPreferences = Activity.GetSharedPreferences (
				GetString (Resource.String.app_name), FileCreationMode.Private);
			sharedPreferences.Edit().PutBoolean (GetString (Resource.String.key_logged_in), loggedIn).Commit ();
		}

		/**
     	* Dummy function to determine whether the user is logged in.
     	*/
		bool ReadLoginValue ()
		{
			ISharedPreferences sharedPreferences = Activity.GetSharedPreferences (
				GetString (Resource.String.app_name), FileCreationMode.Private);
			return sharedPreferences.GetBoolean (GetString (Resource.String.key_logged_in), false);
		}

	}
}

