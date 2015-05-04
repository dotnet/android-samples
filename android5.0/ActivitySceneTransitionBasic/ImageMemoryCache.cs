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
using System.Collections.Generic;
using System.Threading.Tasks;

using Android.Graphics;
using Android.Util;

/**
 * An image memory cache implementation which allows the retrieval of entires via a URL.
 *
 * This class provide the method getBitmapFromUrl which allows the retrieval of
 * a bitmap solely on the URL.
 */
namespace ActivitySceneTransitionBasic
{
	public class ImageMemoryCache : LruCache
	{
		/**
     	* Singleton instance which has it's maximum size set to be 1/8th of the allowed memory size.
     	*/
		public static readonly ImageMemoryCache INSTANCE =
			new ImageMemoryCache ((int)Java.Lang.Runtime.GetRuntime().MaxMemory() / 8);

		private IDictionary<string, Bitmap> mLastSnapshot;

		private ImageMemoryCache (int maxSize) : base(maxSize)
		{
		}

		public Bitmap getBitmapFromUrl (string url)
		{
			if (mLastSnapshot == null) {
				mLastSnapshot = (IDictionary<string, Bitmap>)Snapshot ();
			}

			// Iterate through the snapshot to find any entries which match our url
			foreach (KeyValuePair<string, Bitmap> entry in mLastSnapshot) {
				if (url == entry.Key) {
					// We've found an entry with the same url, return the bitmap
					return entry.Value;
				}
			}
			
			// We didn't find an entry, so return null
			return null;
		}

		public Bitmap getBitmap (string key) {
			return (Bitmap) Get (key);
		}

		public void putBitmap (string key, Bitmap bitmap)
		{
			Put (key, bitmap);

			// An entry has been added, so invalidate the snapshot
			mLastSnapshot = null;
		}

		protected void EntryRemoved(Boolean evicted, String key, Bitmap oldVal){
		}
	}
}

