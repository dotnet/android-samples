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

namespace com.example.monodroid.hcgallery
{
	public class Directory 
	{
		private static DirectoryCategory [] mCategories;
		
		public static void InitializeDirectory () 
		{
			mCategories = new DirectoryCategory [] {
				new DirectoryCategory ("Balloons", new DirectoryEntry [] {
					new DirectoryEntry ("Red Balloon", Resource.Drawable.red_balloon),
					new DirectoryEntry ("Green Balloon", Resource.Drawable.green_balloon),
					new DirectoryEntry ("Blue Balloon", Resource.Drawable.blue_balloon)
					}),
				new DirectoryCategory ("Bikes", new DirectoryEntry [] {
					new DirectoryEntry ("Old school huffy", Resource.Drawable.blue_bike),
					new DirectoryEntry ("New Bikes", Resource.Drawable.rainbow_bike),
					new DirectoryEntry ("Chrome Fast", Resource.Drawable.chrome_wheel)
					}),
				new DirectoryCategory ("Androids", new DirectoryEntry[] {
					new DirectoryEntry ("Steampunk Android", Resource.Drawable.punk_droid),
					new DirectoryEntry ("Stargazing Android", Resource.Drawable.stargazer_droid),
					new DirectoryEntry ("Big Android", Resource.Drawable.big_droid)
					}),
					new DirectoryCategory ("Pastries", new DirectoryEntry[] {
						new DirectoryEntry ("Cupcake", Resource.Drawable.cupcake),
						new DirectoryEntry ("Donut", Resource.Drawable.donut),
						new DirectoryEntry ("Eclair", Resource.Drawable.eclair),
						new DirectoryEntry ("Froyo", Resource.Drawable.froyo),
					}),
				};
		}
		
		public static int CategoryCount {
			get { return mCategories.Length; }
		}
		
		public static DirectoryCategory GetCategory (int i)
		{
			return mCategories[i];
		}
	}
}