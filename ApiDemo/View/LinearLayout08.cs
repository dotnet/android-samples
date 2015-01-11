/*
 * Copyright (C) 2007 The Android Open Source Project
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
using Android.OS;
using Android.Widget;
using Android.Views;

namespace MonoDroid.ApiDemo
{
	/**
 	* Demonstrates horizontal and vertical gravity
 	*/
	[Activity (Label = "Views/Layouts/LinearLayout/08. Gravity")]
	[IntentFilter (new[] { Intent.ActionMain }, Categories = new string[] { ApiDemo.SAMPLE_CATEGORY })]
	public class LinearLayout8 : Activity
	{
		LinearLayout mLinearLayout;

		// Menu item Ids
		public const int VERTICAL_ID = Menu.First;
		public const int HORIZONTAL_ID = Menu.First + 1;

		public const int TOP_ID = Menu.First + 2;
		public const int MIDDLE_ID = Menu.First + 3;
		public const int BOTTOM_ID = Menu.First + 4;

		public const int LEFT_ID = Menu.First + 5;
		public const int CENTER_ID = Menu.First + 6;
		public const int RIGHT_ID = Menu.First + 7;

		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);
			SetContentView (Resource.Layout.linear_layout_8);
			mLinearLayout = FindViewById <LinearLayout> (Resource.Id.layout);
		}

		public override bool OnCreateOptionsMenu (IMenu menu)
		{
			base.OnCreateOptionsMenu (menu);

			menu.Add (0, VERTICAL_ID, 0, Resource.String.linear_layout_8_vertical);
			menu.Add (0, HORIZONTAL_ID, 0, Resource.String.linear_layout_8_horizontal);
			menu.Add (0, TOP_ID, 0, Resource.String.linear_layout_8_top);
			menu.Add (0, MIDDLE_ID, 0, Resource.String.linear_layout_8_middle);
			menu.Add (0, BOTTOM_ID, 0, Resource.String.linear_layout_8_bottom);
			menu.Add (0, LEFT_ID, 0, Resource.String.linear_layout_8_left);
			menu.Add (0, CENTER_ID, 0, Resource.String.linear_layout_8_center);
			menu.Add (0, RIGHT_ID, 0, Resource.String.linear_layout_8_right);

			return true;
		}

		public override bool OnOptionsItemSelected (IMenuItem item)
		{
			switch (item.ItemId) {
			case VERTICAL_ID:
				mLinearLayout.Orientation = Orientation.Vertical;
				return true;

			case HORIZONTAL_ID:
				mLinearLayout.Orientation = Orientation.Horizontal;
				return true;

			case TOP_ID:
				mLinearLayout.SetVerticalGravity (GravityFlags.Top);
				return true;

			case MIDDLE_ID:
				mLinearLayout.SetVerticalGravity (GravityFlags.CenterVertical);
				return true;

			case BOTTOM_ID:
				mLinearLayout.SetVerticalGravity (GravityFlags.Bottom);
				return true;

			case LEFT_ID:
				mLinearLayout.SetHorizontalGravity (GravityFlags.Left);
				return true;

			case CENTER_ID:
				mLinearLayout.SetHorizontalGravity (GravityFlags.CenterHorizontal);
				return true;

			case RIGHT_ID:
				mLinearLayout.SetHorizontalGravity (GravityFlags.Right);
				return true;

			}

			return base.OnOptionsItemSelected (item);
		}
	}
}