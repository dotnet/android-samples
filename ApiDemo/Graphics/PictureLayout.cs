/*
 * Copyright (C) 2008 The Android Open Source Project
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
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Util;
using Android.Views;

namespace MonoDroid.ApiDemo.Graphics
{
	public class PictureLayout : ViewGroup
	{
		private Picture mPicture = new Picture ();

		public PictureLayout (Context context)
			: base (context)
		{
		}

		public PictureLayout (Context context, IAttributeSet attrs)
			: base (context, attrs)
		{
		}

		public override void AddView (View child)
		{
			if (ChildCount > 1)
				throw new InvalidOperationException ("PictureLayout can only host one direct child.");

			base.AddView (child);
		}

		public override void AddView (View child, int index)
		{
			if (ChildCount > 1)
				throw new InvalidOperationException ("PictureLayout can only host one direct child.");

			base.AddView (child, index);
		}

		public override void AddView (View child, int index, LayoutParams @params)
		{
			if (ChildCount > 1)
				throw new InvalidOperationException ("PictureLayout can only host one direct child.");

			base.AddView (child, index, @params);
		}

		public override void AddView (View child, LayoutParams @params)
		{
			if (ChildCount > 1)
				throw new InvalidOperationException ("PictureLayout can only host one direct child.");

			base.AddView (child, @params);
		}

		protected override LayoutParams GenerateDefaultLayoutParams ()
		{
			return new LayoutParams (LayoutParams.MatchParent, LayoutParams.MatchParent);
		}

		protected override void OnMeasure (int widthMeasureSpec, int heightMeasureSpec)
		{
			int count = ChildCount;

			int maxHeight = 0;
			int maxWidth = 0;

			for (int i = 0; i < count; i++) {
				View child = GetChildAt (i);

				if (child.Visibility != ViewStates.Gone)
					MeasureChild (child, widthMeasureSpec, heightMeasureSpec);
			}

			maxWidth += PaddingLeft + PaddingRight;
			maxHeight += PaddingTop + PaddingBottom;

			Drawable drawable = Background;

			if (drawable != null) {
				maxHeight = Math.Max (maxHeight, drawable.MinimumHeight);
				maxWidth = Math.Max (maxWidth, drawable.MinimumWidth);
			}

			SetMeasuredDimension (ResolveSize (maxWidth, widthMeasureSpec),
				ResolveSize (maxHeight, heightMeasureSpec));
		}

		protected override void DispatchDraw (Canvas canvas)
		{
			base.DispatchDraw (mPicture.BeginRecording (Width, Height));

			mPicture.EndRecording ();

			int x = Width / 2;
			int y = Height / 2;

			if (false)
				canvas.DrawPicture (mPicture);
			else {
				DrawPict (canvas, 0, 0, x, y, 1, 1);
				DrawPict (canvas, x, 0, x, y, -1, 1);
				DrawPict (canvas, 0, y, x, y, 1, -1);
				DrawPict (canvas, x, y, x, y, -1, -1);
			}
		}

		public override IViewParent InvalidateChildInParent (int[] location, Rect dirty)
		{
			location[0] = Left;
			location[1] = Top;
			dirty.Set (0, 0, Width, Height);
			return Parent;
		}

		protected override void OnLayout (bool changed, int l, int t, int r, int b)
		{
			int count = base.ChildCount;

			for (int i = 0; i < count; i++) {
				View child = GetChildAt (i);
				if (child.Visibility != ViewStates.Gone) {
					int childLeft = PaddingLeft;
					int childTop = PaddingTop;
					child.Layout (childLeft, childTop,
						childLeft + child.MeasuredWidth,
						childTop + child.MeasuredHeight);

				}
			}
		}

		private void DrawPict (Canvas canvas, int x, int y, int w, int h, float sx, float sy)
		{
			canvas.Save ();
			canvas.Translate (x, y);
			canvas.ClipRect (0, 0, w, h);
			canvas.Scale (0.5f, 0.5f);
			canvas.Scale (sx, sy, w, h);
			canvas.DrawPicture (mPicture);
			canvas.Restore ();
		}
	}

}