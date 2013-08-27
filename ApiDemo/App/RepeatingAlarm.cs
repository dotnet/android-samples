/*
 * Copyright (C) 2007 The Android Open Source Project
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

using Android.App;
using Android.Content;
using Android.Widget;

namespace MonoDroid.ApiDemo
{
	/**
 	* This is an example of implement an {@link BroadcastReceiver} for an alarm that
 	* should occur once.
 	*/
	[BroadcastReceiver]
	public class RepeatingAlarm : BroadcastReceiver
	{
		public override void OnReceive (Context context, Intent intent)
		{
			Toast.MakeText (context, Resource.String.repeating_received, ToastLength.Short).Show ();
		}
	}
}

