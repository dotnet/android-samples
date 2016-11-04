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

using System;
using Android.Content;
using Android.Graphics.Drawables;
using Android.Support.Wearable.Views;
using Android.Util;
using Android.Widget;

namespace Wearable.Ui
{
	/**
	 * A simple extension of the {@link android.widget.LinearLayout} to represent a single item in a
	 * {@link android.support.wearable.view.WearableListView}.
	 */
	public class SpeedPickerLayout : LinearLayout, WearableListView.IOnCenterProximityListener
	{
		private float mFadedTextAlpha;
		private int mFadedCircleColor;
		private int mChosenCircleColor;
		private ImageView mCircle;
		private TextView mName;

		public SpeedPickerLayout(Context context) : this(context, null)
		{
		}

		public SpeedPickerLayout(Context context, IAttributeSet attrs) : this(context, null, 0)
		{
		}

		public SpeedPickerLayout(Context context, IAttributeSet attrs, int defStyle) : base(context, attrs, defStyle)
		{
			mFadedTextAlpha = Resources.GetInteger(Resource.Integer.action_text_faded_alpha)/100f;
			mFadedCircleColor = Resources.GetColor(Resource.Color.grey);
			mChosenCircleColor = Resources.GetColor(Resource.Color.blue);
		}

		protected override void OnFinishInflate()
		{
			base.OnFinishInflate();
			mCircle = (ImageView)FindViewById(Resource.Id.circle);
			mName = (TextView)FindViewById(Resource.Id.name);
		}

		public void OnCenterPosition(bool animate)
		{
			mName.Alpha = 1f;
			var gradientDrawable = (GradientDrawable)mCircle.Drawable;
			gradientDrawable.SetColor(mChosenCircleColor);
		}

		public void OnNonCenterPosition(bool animate)
		{
			var gradientDrawable = (GradientDrawable)mCircle.Drawable;
			gradientDrawable.SetColor(mFadedCircleColor);
			mName.Alpha = mFadedTextAlpha;
		}
	}
}