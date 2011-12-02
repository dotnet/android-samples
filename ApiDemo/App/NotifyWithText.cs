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
using Android.Widget;
using Android.Content;

namespace MonoDroid.ApiDemo
{
	[Activity (Label = "App/Text Notify")]
	[IntentFilter (new[] { Intent.ActionMain }, Categories = new string[] { ApiDemo.SAMPLE_CATEGORY })]
	public class NotifyWithText : Activity
	{
		/**
		 * When you push the button on this Activity, it creates a {@link Toast} object and
		 * using the Toast method.
		 * @see Toast
		 * @see Toast#makeText(android.content.Context,int,int)
		 * @see Toast#makeText(android.content.Context,java.lang.CharSequence,int)
		 * @see Toast#LENGTH_SHORT
		 * @see Toast#LENGTH_LONG
		 */
		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);

			SetContentView (Resource.Layout.notify_with_text);

			Button button;

			// short notification
			button = (Button)FindViewById (Resource.Id.short_notify);

			button.Click += delegate {
				// Note that we create the Toast object and call the show() method
				// on it all on one line.  Most uses look like this, but there
				// are other methods on Toast that you can call to configure how
				// it appears.
				//
				// Note also that we use the version of makeText that takes a
				// resource id (Resource.string.short_notification_text).  There is also
				// a version that takes a CharSequence if you must construct
				// the text yourself.
				Toast.MakeText (this, Resource.String.short_notification_text, ToastLength.Short).Show ();
			};

			// long notification
			// The only difference here is that the notification stays up longer.
			// You might want to use this if there is more text that they're going
			// to read.
			button = (Button)FindViewById (Resource.Id.long_notify);

			button.Click += delegate {
				Toast.MakeText (this, Resource.String.long_notification_text, ToastLength.Long).Show ();
			};
		}
	}
}
