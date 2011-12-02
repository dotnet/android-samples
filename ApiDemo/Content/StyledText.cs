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
using System.Collections.Generic;

using Android.App;
using Android.OS;
using Android.Widget;
using Java.Lang;
using Android.Content;

namespace MonoDroid.ApiDemo
{
	[Activity (Label = "Content/Styled Text")]
	[IntentFilter (new[] { Intent.ActionMain }, Categories = new string[] { ApiDemo.SAMPLE_CATEGORY })]
	public class StyledText : Activity
	{
		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);

			SetContentView (Resource.Layout.styled_text);

			// Programmatically retrieve a string resource with style
			// information and apply it to the second text view.  Note the
			// use of CharSequence instead of String so we don't lose
			// the style info.
			var str = GetTextFormatted (Resource.String.styled_text);
			TextView tv = (TextView)FindViewById (Resource.Id.text);
			tv.TextFormatted = str;
		}
	}
}
