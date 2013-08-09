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

/**
 * This is a simple wrapper for {@link LinearLayout} that implements the {@link Checkable}
 * interface by keeping an internal 'checked' state flag.
 * <p>
 * This can be used as the root view for a custom list item layout for
 * {@link android.widget.AbsListView} elements with a
 * {@link android.widget.AbsListView#setChoiceMode(int) choiceMode} set.
 */
using Android.Util;

namespace CustomChoiceList
{
	public class CheckableLinearLayout : LinearLayout, ICheckable
	{
		static readonly int[] CHECKED_STATE_SET = {Android.Resource.Attribute.StateChecked};

		bool mChecked = false;

		public CheckableLinearLayout (Context context, IAttributeSet attrs) : base (context, attrs)
		{
		}

		public bool Checked {
			get {
				return mChecked;
			} set {
				if (value != mChecked) {
					mChecked = value;
					RefreshDrawableState ();
				}
			}
		}

		public void Toggle ()
		{
			Checked = !mChecked;
		}

		protected override int[] OnCreateDrawableState (int extraSpace)
		{
			int[] drawableState = base.OnCreateDrawableState (extraSpace + 1);

			if (Checked)
				MergeDrawableStates (drawableState, CHECKED_STATE_SET);

			return drawableState;
		}
	}
}

