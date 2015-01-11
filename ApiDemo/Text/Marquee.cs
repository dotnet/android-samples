/* 
 * Copyright (C) 2007 Google Inc.
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
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace MonoDroid.ApiDemo
{
	[Activity (Label = "Text/Marquee")]
	[IntentFilter (new[] { Intent.ActionMain }, Categories = new string[] { ApiDemo.SAMPLE_CATEGORY })]
	public class Marquee : Activity
	{
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			SetContentView (Resource.Layout.marquee);

			Button m1 = FindViewById <Button> (Resource.Id.marquee1);
			Button m2 = FindViewById <Button> (Resource.Id.marquee2);
			Button m3 = FindViewById <Button> (Resource.Id.marquee3);

			m1.Click += delegate {
				m1.Selected = true;
				m2.Selected = false;
				m3.Selected = false;
			};

			m2.Click += delegate {
				m1.Selected = false;
				m2.Selected = true;
				m3.Selected = false;
			};

			m3.Click += delegate {
				m1.Selected = false;
				m2.Selected = false;
				m3.Selected = true;
			};


		}
	}
}

