/*
 * Copyright 2014 The Android Open Source Project
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
using System;

namespace FragmentTransition
{
	public class Meat
	{
		public int resourceId {
			get;
			set;
		}

		public string title {
			get;
			set;
		}

		public readonly static Meat[] MEATS = new Meat[] {
			new Meat (Resource.Drawable.p1, "First"),
			new Meat (Resource.Drawable.p2, "Second"),
			new Meat (Resource.Drawable.p3, "Third"),
			new Meat (Resource.Drawable.p4, "Fourth"),
			new Meat (Resource.Drawable.p5, "Fifth"),
			new Meat (Resource.Drawable.p6, "Sixth"),
			new Meat (Resource.Drawable.p7, "Seventh"),
			new Meat (Resource.Drawable.p8, "Eighth"),
			new Meat (Resource.Drawable.p9, "Ninth"),
			new Meat (Resource.Drawable.p10, "Tenth"),
			new Meat (Resource.Drawable.p11, "Eleventh"),

		};

		public Meat (int resourceId, string title)
		{
			this.resourceId = resourceId;
			this.title = title;
		}
	}
}

