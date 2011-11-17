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
using System;
using Android.Content.Res;
using Android.Graphics;
using Android.Graphics.Drawables;

namespace com.example.monodroid.hcgallery
{
	public class DirectoryEntry 
	{
		private String name;
		private int resID;
			
		public DirectoryEntry (String name, int resID) 
		{
			this.name = name;
			this.resID = resID;
		}
			
		public String Name {
			get { return name; }
		}
			
		public Drawable GetDrawable (Resources res) 
		{
			return res.GetDrawable (resID);
		}
			
		public Bitmap GetBitmap (Resources res) 
		{
			return BitmapFactory.DecodeResource (res, resID);
		}
	}
}
