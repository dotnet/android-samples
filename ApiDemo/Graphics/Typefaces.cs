//
// Copyright (C) 2007 The Android Open Source Project
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//      http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//

using System;

using Android.App;
using Android.OS;
using Android.Widget;
using Android.Content;
using Android.Runtime;
using Java.IO;
using Android.Views;
using Android.Graphics;

namespace MonoDroid.ApiDemo
{
	[Activity (Label = "Graphics/Typefaces")]
	[IntentFilter (new[] { Intent.ActionMain }, Categories = new string[] { ApiDemo.SAMPLE_CATEGORY })]
	public class Typefaces : Activity
	{
		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);

			SetContentView (new SampleView (this));
		}

		private class SampleView : View
		{
			private Paint mPaint = new Paint (PaintFlags.AntiAlias);
			private Typeface mFace;

			public SampleView (Context context) : base (context)
			{
				mFace = Typeface.CreateFromAsset (Context.Assets, "fonts/samplefont.ttf");
				mPaint.TextSize = 64;
			}

			protected override void OnDraw (Canvas canvas)
			{
				canvas.DrawColor (Color.White);

				mPaint.SetTypeface (null);
				canvas.DrawText ("Default", 10, 100, mPaint);
				mPaint.SetTypeface (mFace);
				canvas.DrawText ("Custom", 10, 200, mPaint);
			}
		}
	}
}
