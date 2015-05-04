/*
 * Copyright (C) 2014 The Android Open Source Project
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
using Android.Util;
using Android.Widget;
using Android.Views;

namespace ActivitySceneTransitionBasic
{
	/**
 	* FrameLayout which forces itself to be laid out as square.
 	*/
	public class SquareFrameLayout : FrameLayout
	{
		public SquareFrameLayout (Context context) : base(context)
		{
		}

		public SquareFrameLayout (Context context, IAttributeSet attrs) : base(context, attrs)
		{
		}

		public SquareFrameLayout (Context context, IAttributeSet attrs, int defStyleAttr) 
			: base(context, attrs, defStyleAttr)
		{
		}

		public SquareFrameLayout (Context context, IAttributeSet attrs, int defStyleAttr, int defStyleRes) 
			: base(context, attrs, defStyleAttr, defStyleRes)
		{
		}
			
		protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
		{
			int widthSize = MeasureSpec.GetSize (widthMeasureSpec);
			int heightSize = MeasureSpec.GetSize (heightMeasureSpec);
		
			if (widthSize == 0 && heightSize == 0) {
				// If there are no constraints on size, let FrameLayout measure
				base.OnMeasure (widthMeasureSpec, heightMeasureSpec);

				// Now use the smallest of the measured dimensions for both dimensions
				int minSize = Math.Min(MeasuredWidth, MeasuredHeight);
				SetMeasuredDimension (minSize, minSize);
				return;
			}

			int size;
			if (widthSize == 0 || heightSize == 0) {
				// If one of the dimensions has no restriction on size, set both dimensions to be the
				// on that does
				size = Math.Max (widthSize, heightSize);
			} else {
				// Both dimensions have restrictions on size, set both dimensions to be the
				// smallest of the two
				size = Math.Min (widthSize, heightSize);
			}

			int newMeasureSpec = MeasureSpec.MakeMeasureSpec (size, MeasureSpecMode.Exactly);
			base.OnMeasure (newMeasureSpec, newMeasureSpec);
		}

	}
}

