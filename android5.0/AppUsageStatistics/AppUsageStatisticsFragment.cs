/*
* Copyright 2014 The Android Open Source Project
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
using Android.App;
using Android.App.Usage;
using Android.Content;
using Android.Content.PM;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Provider;
using Android.Support.V7.Widget;
using Android.Util;
using Android.Views;
using Android.Widget;
using Java.Util;

namespace AppUsageStatistics
{
	/// <summary>
	/// Fragment that demonstrates how to use App Usage Statistics API.
	/// </summary>
	public class AppUsageStatisticsFragment : Fragment
	{
		static readonly string TAG = typeof(AppUsageStatisticsFragment).Name;

		UsageStatsManager mUsageStatsManager;
		UsageListAdapter mUsageListAdapter;
		RecyclerView mRecyclerView;
		RecyclerView.LayoutManager mLayoutManager;
		Button mOpenUsageSettingButton;
		Spinner mSpinner;

		AppUsageStatisticsFragment ()
		{

		}

		/// <summary>
		/// Use this factory method to create a new instance of this fragment using the provided parameters.
		/// </summary>
		/// <returns>A new instance of fragment <see cref="AppUsageStatistics.AppUsageStatisticsFragment"/>.</returns>
		public static AppUsageStatisticsFragment NewInstance ()
		{
			var fragment = new AppUsageStatisticsFragment ();
			return fragment;
		}

		public override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);

			mUsageStatsManager = (UsageStatsManager)Activity
				.GetSystemService ("usagestats"); //Context.USAGE_STATS_SERVICE
		}

		public override View OnCreateView (LayoutInflater inflater, ViewGroup container,
		                                   Bundle savedInstanceState)
		{
			return inflater.Inflate (Resource.Layout.fragment_app_usage_statistics, container, false);
		}

		public override void OnViewCreated (View rootView, Bundle savedInstanceState)
		{
			base.OnViewCreated (rootView, savedInstanceState);
			mLayoutManager = new LinearLayoutManager (Activity);
			mUsageListAdapter = new UsageListAdapter ();
			mRecyclerView = rootView.FindViewById<RecyclerView> (Resource.Id.recyclerview_app_usage);
			mRecyclerView.SetLayoutManager (mLayoutManager);
			mRecyclerView.ScrollToPosition (0);
			mRecyclerView.SetAdapter (mUsageListAdapter);
			mOpenUsageSettingButton = rootView.FindViewById<Button> (Resource.Id.button_open_usage_setting);
			mSpinner = rootView.FindViewById<Spinner> (Resource.Id.spinner_time_span);
			var spinnerAdapter = ArrayAdapter.CreateFromResource (Activity,
				                     Resource.Array.action_list, Android.Resource.Layout.SimpleSpinnerDropDownItem);
			mSpinner.Adapter = spinnerAdapter;
			mSpinner.OnItemSelectedListener = new MyOnItemSelectedListener (this);
		}

		class MyOnItemSelectedListener : Java.Lang.Object, AdapterView.IOnItemSelectedListener
		{
			String[] strings;
			AppUsageStatisticsFragment myFragment;

			public MyOnItemSelectedListener (AppUsageStatisticsFragment myFragment)
			{
				this.myFragment = myFragment;
				strings = myFragment.Resources.GetStringArray (Resource.Array.action_list);
			}

			public void OnItemSelected (AdapterView parent, View view, int position, long id)
			{
				UsageStatsInterval usageStatsInterval;
				if (Enum.TryParse (strings [position], true, out usageStatsInterval)
				    && Enum.IsDefined (typeof(UsageStatsInterval), usageStatsInterval)) {
					var usageStatsList = new List<UsageStats> (
						                     myFragment.GetUsageStatistics (usageStatsInterval));
					usageStatsList = usageStatsList.OrderBy (x => -(x.LastTimeUsed)).ToList ();
					myFragment.UpdateAppsList (usageStatsList);
				}
			}

			public void OnNothingSelected (AdapterView parent)
			{
			}
		}

		/// <summary>
		/// Returns the <see cref="Android.Support.V7.Widget.RecyclerView"/> including the time span 
		/// specified by the intervalType argument.
		/// </summary>
		/// <returns>A list of <see cref="Android.App.Usage.UsageStats"/>.</returns>
		/// <param name="intervalType">The time interval by which the stats are aggregated.</param>
		public IList<UsageStats> GetUsageStatistics (UsageStatsInterval intervalType)
		{
			var cal = Calendar.GetInstance (Java.Util.Locale.Default);
			cal.Add (CalendarField.Year, -1);	

			// Query stats beginning one year ago to the current date.
			var queryUsageStats = mUsageStatsManager
				.QueryUsageStats (intervalType, cal.TimeInMillis,
				                      Java.Lang.JavaSystem.CurrentTimeMillis ());

			if (queryUsageStats.Count == 0) {
				Log.Info (TAG, "The user may not allow the access to apps usage. ");
				Toast.MakeText (Activity,
					GetString (Resource.String.explanation_access_to_appusage_is_not_enabled),
					ToastLength.Long).Show ();
				mOpenUsageSettingButton.Visibility = ViewStates.Visible;
				mOpenUsageSettingButton.Click += ( sender, e) => 
					StartActivity (new Intent (Settings.ActionUsageAccessSettings));
			}
			return queryUsageStats;
		}

		/// <summary>
		/// Updates the <see cref="Android.Support.V7.Widget.RecyclerView"/> with the list of 
		/// <see cref="Android.App.Usage.UsageStats"/> passed as an argument.
		/// </summary>
		/// <param name="usageStatsList">
		/// A list of <see cref="Android.App.Usage.UsageStats"/> from which 
		/// upadte the <see cref="Android.Support.V7.Widget.RecyclerView"/>.
		/// </param>
		protected void UpdateAppsList (List<UsageStats> usageStatsList)
		{
			var customUsageStatsList = new List<CustomUsageStats> ();
			for (int i = 0; i < usageStatsList.Count; i++) {
				var customUsageStats = new CustomUsageStats ();
				customUsageStats.UsageStats = usageStatsList [i];
				try {
					var appIcon = Activity.PackageManager
						.GetApplicationIcon (customUsageStats.UsageStats.PackageName);
					customUsageStats.AppIcon = appIcon;
				} catch (PackageManager.NameNotFoundException) {
					Log.Warn (TAG, string.Format ("App Icon is not found for {0}",
						customUsageStats.UsageStats.PackageName));
					customUsageStats.AppIcon = Activity
						.GetDrawable (Resource.Drawable.ic_default_app_launcher);
				}
				customUsageStatsList.Add (customUsageStats);
			}
			mUsageListAdapter.SetCustomUsageStatsList (customUsageStatsList);
			mUsageListAdapter.NotifyDataSetChanged ();
			mRecyclerView.ScrollToPosition (0);
		}
	}
}

