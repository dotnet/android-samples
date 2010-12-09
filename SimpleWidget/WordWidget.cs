/*
 * Copyright (C) 2009 The Android Open Source Project
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
using Android.Appwidget;
using Android.Content;

namespace SimpleWidget
{
	[BroadcastReceiver (Label = "@string/widget_name")]
	[IntentFilter (new string [] { "android.appwidget.action.APPWIDGET_UPDATE" })]
	[MetaData ("android.appwidget.provider", Resource = "@xml/widget_word")]
	public class WordWidget : AppWidgetProvider
	{
		public override void OnUpdate (Context context, AppWidgetManager appWidgetManager, int[] appWidgetIds)
		{
			// To prevent any ANR timeouts, we perform the update in a service
			context.StartService (new Intent (context, typeof (UpdateService)));
		}
	}
}
