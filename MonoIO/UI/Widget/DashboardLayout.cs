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
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace MonoIO.Widget
{
	// Custom layout that arranges children in a grid-like manner, 
	// optimizing for even horizontal and vertical whitespace.
	public class DashboardLayout : ViewGroup
	{
		private const int UNEVEN_GRID_PENALTY_MULTIPLIER = 10;

		private int mMaxChildWidth = 0;
		private int mMaxChildHeight = 0;

		public DashboardLayout (Context context, IAttributeSet attrs) :
			base (context, attrs)
		{
			Initialize ();
		}

		public DashboardLayout (Context context, IAttributeSet attrs, int defStyle) :
			base (context, attrs, defStyle)
		{
			Initialize ();
		}

		private void Initialize ()
		{
		}

		protected override void OnMeasure (int widthMeasureSpec, int heightMeasureSpec)
		{
			mMaxChildWidth = 0;
			mMaxChildHeight = 0;

			// Measure once to find the maximum child size.
			int childWidthMeasureSpec = MeasureSpec.MakeMeasureSpec (
				MeasureSpec.GetSize (widthMeasureSpec), MeasureSpecMode.AtMost);
			int childHeightMeasureSpec = MeasureSpec.MakeMeasureSpec (
				MeasureSpec.GetSize (widthMeasureSpec), MeasureSpecMode.AtMost);

			int count = ChildCount;

			for (int i = 0; i < count; i++) {
				var child = GetChildAt (i);

				if (child.Visibility == ViewStates.Gone)
					continue;

				child.Measure (childWidthMeasureSpec, childHeightMeasureSpec);

				mMaxChildWidth = Math.Max (mMaxChildWidth, child.MeasuredWidth);
				mMaxChildHeight = Math.Max (mMaxChildHeight, child.MeasuredHeight);
			}

			// Measure again for each child to be exactly the same size.
			childWidthMeasureSpec = MeasureSpec.MakeMeasureSpec (
				mMaxChildWidth, MeasureSpecMode.Exactly);
			childHeightMeasureSpec = MeasureSpec.MakeMeasureSpec (
				mMaxChildHeight, MeasureSpecMode.Exactly);

			for (int i = 0; i < count; i++) {
				var child = GetChildAt (i);

				if (child.Visibility == ViewStates.Gone)
					continue;

				child.Measure (childWidthMeasureSpec, childHeightMeasureSpec);
			}

			SetMeasuredDimension (
				ResolveSize (mMaxChildWidth, widthMeasureSpec),
				ResolveSize (mMaxChildHeight, heightMeasureSpec));
		}

		protected override void OnLayout (bool changed, int l, int t, int r, int b)
		{
			var width = r - l;
			var height = b - t;

			var visible_count = 0;

			for (int i = 0; i < ChildCount; i++)
				if (GetChildAt (i).Visibility != ViewStates.Gone)
					visible_count++;

			if (visible_count == 0)
				return;

			// Calculate what number of rows and columns will optimize for even horizontal and
			// vertical whitespace between items. Start with a 1 x N grid, then try 2 x N, and so on.
			int best_space_difference = int.MaxValue;
			int space_difference;

			// Horizontal and vertical space between items
			int hspace = 0;
			int vspace = 0;

			int cols = 1;
			int rows;

			while (true) {
				rows = (visible_count - 1) / cols + 1;

				hspace = ((width - mMaxChildWidth * cols) / (cols + 1));
				vspace = ((height - mMaxChildHeight * rows) / (rows + 1));

				space_difference = Math.Abs (vspace - hspace);

				if (rows * cols != visible_count)
					space_difference *= UNEVEN_GRID_PENALTY_MULTIPLIER;

				if (space_difference < best_space_difference) {
					// Found a better whitespace squareness/ratio
					best_space_difference = space_difference;

					// If we found a better whitespace squareness and there's only 1 row, this is
					// the best we can do.
					if (rows == 1)
						break;
				} else {
					// This is a worse whitespace ratio, use the previous value of cols and exit.
					--cols;
					rows = (visible_count - 1) / cols + 1;
					hspace = ((width - mMaxChildWidth * cols) / (cols + 1));
					vspace = ((height - mMaxChildHeight * rows) / (rows + 1));
					break;
				}

				++cols;
			}

			// Lay out children based on calculated best-fit number of rows and cols.

			// If we chose a layout that has negative horizontal or vertical space, force it to zero.
			hspace = Math.Max (0, hspace);
			vspace = Math.Max (0, vspace);

			// Re-use width/height variables to be child width/height.
			width = (width - hspace * (cols + 1)) / cols;
			height = (height - vspace * (rows + 1)) / rows;

			int left, top;
			int col, row;
			int visibleIndex = 0;

			for (int i = 0; i < ChildCount; i++) {
				var child = GetChildAt (i);

				if (child.Visibility == ViewStates.Gone)
					continue;

				row = visibleIndex / cols;
				col = visibleIndex % cols;

				left = hspace * (col + 1) + width * col;
				top = vspace * (row + 1) + height * row;

				child.Layout (left, top,
					(hspace == 0 && col == cols - 1) ? r : (left + width),
					(vspace == 0 && row == rows - 1) ? b : (top + height));

				++visibleIndex;
			}
		}
	}
}