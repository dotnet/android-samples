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

/**
 * Represents an Item in our application. Each item has a name, id, full size image url and
 * thumbnail url.
 */
namespace ActivitySceneTransitionBasic
{

	public class Item : Java.Lang.Object
	{

		private static readonly string LARGE_BASE_URL = "http://storage.googleapis.com/androiddevelopers/sample_data/activity_transition/large/";
		private static readonly string THUMB_BASE_URL = "http://storage.googleapis.com/androiddevelopers/sample_data/activity_transition/thumbs/";

		public static readonly Item[] ITEMS = {
			new Item("Flying in the Light", "Romain Guy", "flying_in_the_light.jpg"),
			new Item("Caterpillar", "Romain Guy", "caterpillar.jpg"),
			new Item("Look Me in the Eye", "Romain Guy", "look_me_in_the_eye.jpg"),
			new Item("Flamingo", "Romain Guy", "flamingo.jpg"),
			new Item("Rainbow", "Romain Guy", "rainbow.jpg"),
			new Item("Over there", "Romain Guy", "over_there.jpg"),
			new Item("Jelly Fish 2", "Romain Guy", "jelly_fish_2.jpg"),
			new Item("Lone Pine Sunset", "Romain Guy", "lone_pine_sunset.jpg"),
		};

		public static Item GetItem(int id)
		{
			foreach (Item item in ITEMS) {
				if (item.id == id)
					return item;
			}
			return null;
		}

		private readonly string mName;
		private readonly string mAuthor;
		private readonly string mFileName;

		public int id {
			get { return mName.GetHashCode () + mFileName.GetHashCode (); }
		} 

		public string author {
			get { return mAuthor; }
		}

		public string name {
			get { return mName; }
		}

		public string photoUrl {
			get { return LARGE_BASE_URL + mFileName;
				}
		}

		public string thumbnailUrl {
			get { return THUMB_BASE_URL + mFileName; }
		}

		public Item (string name, string author, string filename)
		{
			mName = name;
			mAuthor = author;
			mFileName = filename;
		}

	

	}
}