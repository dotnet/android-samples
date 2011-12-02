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
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Views;
using System.IO;

namespace MonoDroid.ApiDemo
{
	[Activity (Label = "Graphics/Alpha Bitmap")]
	[IntentFilter (new[] { Intent.ActionMain }, Categories = new string[] { ApiDemo.SAMPLE_CATEGORY })]
	public class AlphaBitmap : GraphicsActivity
	{
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			// Create your application here
			SetContentView (new SampleView (this));
		}

		private class SampleView : View
		{
			private Bitmap mBitmap;
			private Bitmap mBitmap2;
			private Bitmap mBitmap3;
			private Shader mShader;

			public SampleView (Context context)
				: base (context)
			{
				Focusable = true;

				Stream input = context.Resources.OpenRawResource (Resource.Drawable.app_sample_code);
				
				mBitmap = BitmapFactory.DecodeStream (input);
				mBitmap2 = mBitmap.ExtractAlpha ();
				mBitmap3 = Bitmap.CreateBitmap (200, 200, Bitmap.Config.Alpha8);
				DrawIntoBitmap (mBitmap3);

				mShader = new LinearGradient (0, 0, 100, 70, new int[] {
                                         Color.Red, Color.Green, Color.Blue },
							     null, Shader.TileMode.Mirror);
			}

			protected override void OnDraw (Canvas canvas)
			{
				canvas.DrawColor (Color.White);

				Paint p = new Paint ();
				float y = 10;

				p.Color = Color.Red;
				canvas.DrawBitmap (mBitmap, 10, y, p);
				y += mBitmap.Height + 10;
				canvas.DrawBitmap (mBitmap2, 10, y, p);
				y += mBitmap2.Height + 10;
				p.SetShader (mShader);
				canvas.DrawBitmap (mBitmap3, 10, y, p);
			}

			private void DrawIntoBitmap (Bitmap bm)
			{
				float x = bm.Width;
				float y = bm.Height;
				Canvas c = new Canvas (bm);
				Paint p = new Paint ();
				p.AntiAlias = true;

				p.Alpha = 0x80;
				c.DrawCircle (x / 2, y / 2, x / 2, p);

				p.Alpha = 0x30;
				p.SetXfermode (new PorterDuffXfermode (PorterDuff.Mode.Src));
				p.TextSize = 60;
				p.TextAlign = Paint.Align.Center;
				Paint.FontMetrics fm = p.GetFontMetrics ();
				c.DrawText ("Alpha", x / 2, (y - fm.Ascent) / 2, p);
			}
		}
	}
}
