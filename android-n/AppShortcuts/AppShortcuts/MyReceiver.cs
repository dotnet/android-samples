/*
 * Copyright (C) 2016 The Android Open Source Project
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
using Android.Content;
using Android.Util;

namespace AppShortcuts
{
	[BroadcastReceiver(Enabled = true)]
	[IntentFilter(new[] {Intent.ActionLocaleChanged})]
	public class MyReceiver : BroadcastReceiver
	{
		static string TAG = Main.TAG;

		public override void OnReceive(Context context, Intent intent)
		{
			Log.Info(TAG, "onReceive: " + intent);
			if (Intent.ActionLocaleChanged.Equals(intent.Action))
			{
				// Refresh all shortcut to update the labels.
				// (Right now shortcut labels don't contain localized strings though.)
				new ShortcutHelper(context).RefreshShortcuts(/*force=*/ true);
			}
		}
	}
}