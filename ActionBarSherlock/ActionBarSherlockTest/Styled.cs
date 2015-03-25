/*
 * Copyright (C) 2011 Jake Wharton
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

// C# port by Atsushi Eno
// Copyright (C) 2013 Xamarin Inc.

using System;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Widget;
using Xamarin.ActionbarSherlockBinding.App;
using Xamarin.ActionbarSherlockBinding.Views;

namespace Mono.ActionbarsherlockTest
{
	[Activity (Label = "@string/styled")]
	[IntentFilter (new string [] { Intent.ActionMain },
		Categories = new string [] { Constants.DemoCategory })]
	public class Styled : SherlockActivity 
	{
			protected override void OnCreate(Bundle savedInstanceState) {
			base.OnCreate(savedInstanceState);
			SetContentView(Resource.Layout.text);
			FindViewById<TextView> (Resource.Id.text).SetText(Resource.String.styled_content);

			//This is a workaround for http://b.android.com/15340 from http://stackoverflow.com/a/5852198/132047
			if (Build.VERSION.SdkInt < Android.OS.BuildVersionCodes.IceCreamSandwich) {
				BitmapDrawable bg = (BitmapDrawable) Resources.GetDrawable (Resource.Drawable.bg_striped);
				bg.SetTileModeXY (Shader.TileMode.Repeat, Shader.TileMode.Repeat);
				SupportActionBar.SetBackgroundDrawable (bg);

				BitmapDrawable bgSplit = (BitmapDrawable) Resources.GetDrawable (Resource.Drawable.bg_striped_split_img);
				bgSplit.SetTileModeXY (Shader.TileMode.Repeat, Shader.TileMode.Repeat);
				SupportActionBar.SetSplitBackgroundDrawable (bgSplit);
			}
		}

			public override bool OnCreateOptionsMenu(IMenu menu) {
			menu.Add ("Save")
				.SetIcon (Resource.Drawable.ic_compose)
					.SetShowAsAction (MenuItem.ShowAsActionIfRoom | MenuItem.ShowAsActionWithText);

			menu.Add ("Search")
				.SetIcon (Resource.Drawable.ic_search)
					.SetShowAsAction (MenuItem.ShowAsActionIfRoom | MenuItem.ShowAsActionWithText);

			menu.Add ("Refresh")
				.SetIcon (Resource.Drawable.ic_refresh)
					.SetShowAsAction (MenuItem.ShowAsActionIfRoom | MenuItem.ShowAsActionWithText);

			return base.OnCreateOptionsMenu(menu);
		}
	}
}

