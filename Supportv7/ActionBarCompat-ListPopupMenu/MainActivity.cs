/*
 * Copyright (C) 2013 The Android Open Source Project
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

using Android.App;
using Android.OS;
using Android.Support.V7.App;

namespace ActionBarCompatListPopupMenu
{
	[Activity (Label = "@string/app_name", MainLauncher = true, Icon = "@drawable/icon")]
	/// <summary>
	/// This sample shows you how to use <see cref="Android.Support.V7.Widget.PopupMenu"/> from
	/// ActionBarCompat to create a list, with each item having a dropdown menu.
	/// 
	/// The interesting part of this sample is in <see cref="ActionBarCompatListPopupMenu.PopupListFragment"/> 
	/// 
	/// This Activity extends from <see cref="Android.Support.V7.App.AppCompatActivity"/> , 
	/// which provides all of the function necessary to display a compatible Action Bar on devices 
	/// running Android v2.1+.
	/// </summary>
	public class MainActivity : AppCompatActivity
	{
		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);

			// Set content view (which contains a PopupListFragment)
			SetContentView (Resource.Layout.sample_main);
		}
	}
}

