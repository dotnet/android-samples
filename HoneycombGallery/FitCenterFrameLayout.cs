/*
 * Copyright (C) 2011 The Android Open Source Project
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

using Android.Content;
using Android.Util;
using Android.Views;

namespace com.example.monodroid.hcgallery
{
	/**
	* A simple layout that fits and centers each child view, maintaining aspect ratio.
	*/
	public class FitCenterFrameLayout : ViewGroup 
	{
		public FitCenterFrameLayout (Context context) 
			: base (context)
		{
		}

		public FitCenterFrameLayout (Context context, IAttributeSet attrs) 
			: base (context, attrs)
		{
		}

		protected override void OnMeasure (int widthMeasureSpec, int heightMeasureSpec) 
		{
			// We purposely disregard child measurements.
			int width = ResolveSize (SuggestedMinimumWidth, widthMeasureSpec);
			int height = ResolveSize (SuggestedMinimumHeight, heightMeasureSpec);
			SetMeasuredDimension (width, height);

			int childWidthSpec = MeasureSpec.MakeMeasureSpec (width, MeasureSpecMode.Unspecified);
			int childHeightSpec = MeasureSpec.MakeMeasureSpec (height, MeasureSpecMode.Unspecified);

        
			int childCount = ChildCount;
			for (int i = 0; i < childCount; i++) {
				GetChildAt (i).Measure (childWidthSpec, childHeightSpec);
			}
		}

		protected override void OnLayout (bool changed, int l, int t, int r, int b) {
			int childCount = ChildCount;
			
			int parentLeft = PaddingLeft;
			int parentTop = PaddingTop;
			int parentRight = r - l - PaddingRight;
			int parentBottom = b - t - PaddingBottom;
			
			int parentWidth = parentRight - parentLeft;
			int parentHeight = parentBottom - parentTop;
			
			int unpaddedWidth, unpaddedHeight, parentUnpaddedWidth, parentUnpaddedHeight;
			int childPaddingLeft, childPaddingTop, childPaddingRight, childPaddingBottom;
			
			for (int i = 0; i < childCount; i++) {
				View child = GetChildAt (i);
				if (child.Visibility == ViewStates.Gone) {
					continue;
				}
			
				// Fit and center the child within the parent. Make sure not to consider padding
				// as part of the child's aspect ratio.
				
				childPaddingLeft = child.PaddingLeft;
				childPaddingTop = child.PaddingTop;
				childPaddingRight = child.PaddingRight;
				childPaddingBottom = child.PaddingBottom;
				
				unpaddedWidth = child.MeasuredWidth - childPaddingLeft - childPaddingRight;
				unpaddedHeight = child.MeasuredHeight - childPaddingTop - childPaddingBottom;
				
				parentUnpaddedWidth = parentWidth - childPaddingLeft - childPaddingRight;
				parentUnpaddedHeight = parentHeight - childPaddingTop - childPaddingBottom;
				
				if (parentUnpaddedWidth * unpaddedHeight > parentUnpaddedHeight * unpaddedWidth) {
					// The child view should be left/right letterboxed.
					int scaledChildWidth = unpaddedWidth * parentUnpaddedHeight
					/ unpaddedHeight + childPaddingLeft + childPaddingRight;
					child.Layout (
						parentLeft + (parentWidth - scaledChildWidth) / 2,
						parentTop,
						parentRight - (parentWidth - scaledChildWidth) / 2,
						parentBottom);
				} else {
					// The child view should be top/bottom letterboxed.
					int scaledChildHeight = unpaddedHeight * parentUnpaddedWidth
					/ unpaddedWidth + childPaddingTop + childPaddingBottom;
					child.Layout(
						parentLeft,
						parentTop + (parentHeight - scaledChildHeight) / 2,
						parentRight,
						parentTop + (parentHeight + scaledChildHeight) / 2);
				}
			}
		}
	}
}