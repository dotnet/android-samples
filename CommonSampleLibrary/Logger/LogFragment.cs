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

using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using Android.Graphics;

namespace CommonSampleLibrary
{
	/**
	* Simple fraggment which contains a LogView and uses is to output log data it receives
	* through the LogNode interface.
	*/
	public class LogFragment : Fragment
	{
		LogView mLogView;
		ScrollView mScrollView;

		public LogFragment ()
		{
		}

		public View InflateViews ()
		{
			mScrollView = new ScrollView (Activity);
			ViewGroup.LayoutParams scrollParams = new ViewGroup.LayoutParams (
				ViewGroup.LayoutParams.MatchParent,
				ViewGroup.LayoutParams.MatchParent);
			mScrollView.LayoutParameters = scrollParams;

			mLogView = new LogView (Activity);
			ViewGroup.LayoutParams logParams = new ViewGroup.LayoutParams (scrollParams);
			logParams.Height = ViewGroup.LayoutParams.WrapContent;
			mLogView.LayoutParameters = logParams;
			mLogView.Clickable = true;
			mLogView.Focusable = true;
			mLogView.Typeface = Typeface.Monospace;

			// Want to set padding as 16 dips, setPadding takes pixels.  Hooray math!
			int paddingDips = 16;
			float scale = Resources.DisplayMetrics.Density;
			int paddingPixels = (int) ((paddingDips * (scale)) + .5);
			mLogView.SetPadding (paddingPixels, paddingPixels, paddingPixels, paddingPixels);
			mLogView.CompoundDrawablePadding = paddingPixels;

			mLogView.Gravity = GravityFlags.Bottom;
			mLogView.SetTextAppearance (Activity, Android.Resource.Style.TextAppearanceMedium);

			mScrollView.AddView (mLogView);
			return mScrollView;
		}

		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			View result = InflateViews ();
			mLogView.AfterTextChanged += (sender, e) => mScrollView.FullScroll (FocusSearchDirection.Down); 

			return result;
		}

		public LogView LogView {
			get { return mLogView; }
		}
	}
}

