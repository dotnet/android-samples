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
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Views;

namespace MonoDroid.ApiDemo
{
	[Activity (Label = "Graphics/Layers")]
	[IntentFilter (new[] { Intent.ActionMain }, Categories = new string[] { ApiDemo.SAMPLE_CATEGORY })]
	public class Layers : Activity
	{
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			// Create your application here
			SetContentView (new SampleView (this));
		}

		private class SampleView : View
		{
			private Paint mPaint;

			public SampleView (Context context) : base (context)
			{
				Focusable = true;

				mPaint = new Paint ();
				mPaint.AntiAlias = true;
			}

			protected override void OnDraw (Canvas canvas)
			{
				canvas.DrawColor (Color.White);

				canvas.Translate (10, 10);

				canvas.SaveLayerAlpha (0, 0, 200, 200, 0x88, SaveFlags.All);

				mPaint.Color = Color.Red;
				canvas.DrawCircle (75, 75, 75, mPaint);
				mPaint.Color = Color.Blue;
				canvas.DrawCircle (125, 125, 75, mPaint);

				canvas.Restore ();
			}
		}
	}
}
