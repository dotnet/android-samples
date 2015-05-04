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
using Android.Support.V4.App;

using CommonSampleLibrary;
using Fragment = Android.Support.V4.App.Fragment;

namespace ElevationBasic
{
	public class ElevationBasicFragment : Fragment
	{
		private string Tag = "ElevationBasicFragment";

		public override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			HasOptionsMenu = true;
		}

		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			/**
	         * Inflates an XML containing two shapes: the first has a fixed elevation
	         * and the second ones raises when tapped.
	         */
			var rootView = inflater.Inflate (Resource.Layout.elevation_basic, container, false);
			var shape2 = rootView.FindViewById (Resource.Id.floating_shape_2);

			/**
	         * Sets a {@Link View.OnTouchListener} that responds to a touch event on shape2.
	         */
			shape2.Touch += (object sender, View.TouchEventArgs e) => {
				var view = (View)sender;
				switch (e.Event.Action) {
				case MotionEventActions.Down:
					Log.Debug (Tag, "ACTION_DOWN on view");
					view.TranslationZ = 120;
					break;
				case MotionEventActions.Up:
					Log.Debug (Tag, "ACTION_UP on view");
					view.TranslationZ = 0;
					break;
				default:
					e.Handled = false;
					return;
				}
				e.Handled = true;
			};
			return rootView;
		}

	}
}

