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
using Android.Views;
using Android.Content;

namespace MonoDroid.ApiDemo
{
	[Activity (Label = "App/Translucent Blur Background")]
	[IntentFilter (new[] { Intent.ActionMain }, Categories = new string[] { ApiDemo.SAMPLE_CATEGORY })]
	public class TranslucentBlurBackground : Activity
	{
		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);

			// Have the system blur any windows behind this one.
			Window.SetFlags (WindowManagerFlags.BlurBehind, WindowManagerFlags.BlurBehind);

			SetContentView (Resource.Layout.translucent_background);
		}
	}
}
