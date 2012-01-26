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
using Android.Content;
using Android.Graphics;
using Android.Util;
using Android.Widget;

// A custom EditText that draws lines so it looks like a notepad
namespace Mono.Samples.Notepad
{
	class LinedEditText : EditText
	{
		private Rect rect;
		private Paint paint;

		// we need this constructor for LayoutInflater
		public LinedEditText (Context context, IAttributeSet attrs)
			: base (context, attrs)
		{
			rect = new Rect ();
			paint = new Paint ();
			paint.SetStyle (Android.Graphics.Paint.Style.Stroke);
			paint.Color = Color.LightGray;
		}

		protected override void OnDraw (Canvas canvas)
		{
			int count = LineCount;

			for (int i = 0; i < count; i++) {
				int baseline = GetLineBounds (i, rect);

				canvas.DrawLine (rect.Left, baseline + 1, rect.Right, baseline + 1, paint);
			}

			base.OnDraw (canvas);
		}
	}
}