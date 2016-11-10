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

using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.Wearable.Views;
using Android.Widget;
using Wearable.Ui;

namespace Wearable
{
	/**
	 * An activity that presents a list of speeds to user and allows user to pick one, to be used as
	 * the current speed limit.
	 */
	[Activity(Label = "Wearable", MainLauncher = true, Icon = "@drawable/ic_launcher")]
	public class SpeedPickerActivity : Activity, WearableListView.IClickListener
	{
		public static readonly string ExtraNewSpeedLimit = "Wearable.Wearable.extra.NEW_SPEED_LIMIT";

		/* Speeds, in mph, that will be shown on the list */
		private int[] speeds = { 25, 30, 35, 40, 45, 50, 55, 60, 65, 70, 75 };

		TextView header;

		protected override void OnCreate(Bundle bundle)
		{
			base.OnCreate(bundle);
			SetContentView(Resource.Layout.ActivitySpeedPicker);

			header = (TextView)FindViewById(Resource.Id.header);

			// Get the list component from the layout of the activity
			var listView = (WearableListView)FindViewById(Resource.Id.wearable_list);

			// Assign an adapter to the list
			listView.SetAdapter(new SpeedPickerListAdapter(this, speeds));

			// Set a click listener
			listView.SetClickListener(this);

			listView.AddOnScrollListener(new OnScrollListener(this));
		}

		private class OnScrollListener : Java.Lang.Object, WearableListView.IOnScrollListener
		{
			private SpeedPickerActivity Owner { get; set; }

			public OnScrollListener(SpeedPickerActivity owner)
			{
				Owner = owner;
			}

			public void OnScroll(int i)
			{
			}

			public void OnAbsoluteScrollChange(int i)
			{
				// only scroll the header up from the base position, not down...
				if (i > 0)
				{
					Owner.header.SetY(-i);
				}
			}

			public void OnCentralPositionChanged(int i)
			{
			}

			public void OnScrollStateChanged(int i)
			{
			}
		}

		public void OnClick(WearableListView.ViewHolder viewHolder)
		{
			var newSpeedLimit = speeds[viewHolder.AdapterPosition];

			var resultIntent = new Intent(Intent.ActionPick);
			resultIntent.PutExtra(ExtraNewSpeedLimit, newSpeedLimit);
			SetResult(Result.Ok, resultIntent);

			Finish();
		}

		public void OnTopEmptyRegionClick()
		{
		}
	}
}


