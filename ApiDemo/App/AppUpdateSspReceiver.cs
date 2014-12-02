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
	[BroadcastReceiver (Name = "monodroid.apidemo.AppUpdateSspReceiver")]
	public class AppUpdateSspReceiver : BroadcastReceiver
	{
		public override void OnReceive (Context context, Intent intent)
		{
			var msg = "Ssp update received: " + intent.Data;
			Toast.MakeText (context, msg, ToastLength.Short).Show ();
		}
	}
}

