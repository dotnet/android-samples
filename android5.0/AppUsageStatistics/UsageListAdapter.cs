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

using System.Collections.Generic;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Java.Text;
using Java.Util;

namespace AppUsageStatistics
{
	/// <summary>
	/// Provide views to RecyclerView with the directory entries.
	/// </summary>
	public class UsageListAdapter : RecyclerView.Adapter
	{
		List<CustomUsageStats> mCustomUsageStatsList = new List<CustomUsageStats> ();
		DateFormat mDateFormat = new SimpleDateFormat ();

		public class ViewHolder : RecyclerView.ViewHolder
		{
			readonly TextView mPackageName;
			readonly TextView mLastTimeUsed;
			readonly ImageView mAppIcon;

			public ViewHolder (View v) : base (v)
			{
				mPackageName = v.FindViewById<TextView> (Resource.Id.textview_package_name);
				mLastTimeUsed = v.FindViewById<TextView> (Resource.Id.textview_last_time_used);
				mAppIcon = v.FindViewById<ImageView> (Resource.Id.app_icon);
			}

			public TextView PackageName { get { return mPackageName; } }

			public TextView LastTimeUsed { get { return mLastTimeUsed; } }

			public ImageView AppIcon { get { return mAppIcon; } }
		}

		public override RecyclerView.ViewHolder OnCreateViewHolder (ViewGroup parent, int viewType)
		{
			View v = LayoutInflater.From (parent.Context)
				.Inflate (Resource.Layout.usage_row, parent, false);
			return new ViewHolder (v);
		}

		public override void OnBindViewHolder (RecyclerView.ViewHolder holder, int position)
		{
			var myHolder = (ViewHolder)holder;
			myHolder.PackageName.Text =
				mCustomUsageStatsList [position].UsageStats.PackageName;
			long lastTimeUsed = mCustomUsageStatsList [position].UsageStats.LastTimeUsed;
			myHolder.LastTimeUsed.Text = mDateFormat.Format (new Date (lastTimeUsed));
			myHolder.AppIcon.SetImageDrawable (mCustomUsageStatsList [position].AppIcon);
		}

		public override int ItemCount {
			get {
				return mCustomUsageStatsList.Count;
			}
		}

		public void SetCustomUsageStatsList (List<CustomUsageStats> customUsageStats)
		{
			mCustomUsageStatsList = customUsageStats;
		}
	}
}

