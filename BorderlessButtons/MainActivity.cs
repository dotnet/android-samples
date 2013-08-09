/*
 * Copyright 2013 The Android Open Source Project
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
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
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

/*
 * This activity demonstrates the <b>borderless button</b> styling from the Holo visual language.
 * The most interesting bits in this sample are in the layout files (res/layout/).
 * <p>
 * See <a href="http://developer.android.com/design/building-blocks/buttons.html#borderless">
 * borderless buttons</a> at the Android Design guide for a discussion of this visual style.
 */

namespace BorderlessButtons
{
	[Activity (Label = "BorderlessButtons", MainLauncher = true, Theme = "@style/Theme.Sample")]
	public class MainActivity : ListActivity
	{
		static readonly Android.Net.Uri DOCS_URI = Android.Net.Uri.Parse ("http://developer.android.com/design/building-blocks/buttons.html#borderless");

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			SetContentView (Resource.Layout.Main);

			ListAdapter = new MyListAdapter (this);

			FindViewById (Resource.Id.cancel_button).Click += delegate {
				Finish();
			};

			FindViewById (Resource.Id.ok_button).Click += delegate {
				Finish();
			};
		}

		public override bool OnCreateOptionsMenu (IMenu menu)
		{
			base.OnCreateOptionsMenu (menu);
			MenuInflater.Inflate (Resource.Layout.MainMenu, menu);
			return true;
		}

		public override bool OnOptionsItemSelected (IMenuItem item)
		{
			switch (item.ItemId) {
			case Resource.Id.docs_link:
				try {
					StartActivity (new Intent (Intent.ActionView, DOCS_URI));
				} catch (ActivityNotFoundException ignored) {
					Console.WriteLine (ignored);
				}
				return true;
			}
			return base.OnOptionsItemSelected (item);
		}

		class MyListAdapter : BaseAdapter
		{
			Context context;

			public MyListAdapter (Context c)
			{
				context = c;
			}

			public override int Count {
				get {
					return 10;
				}
			}

			public override Java.Lang.Object GetItem (int position)
			{
				return null;
			}

			public override long GetItemId (int position)
			{
				return position + 1;
			}

			public override View GetView (int position, View convertView, ViewGroup parent)
			{
				if (convertView == null)
					convertView = LayoutInflater.From (context).Inflate (Resource.Layout.ListItem, parent, false);

				// Because the list item contains multiple touch targets, you should not override
				// onListItemClick. Instead, set a click listener for each target individually.
				convertView.FindViewById (Resource.Id.primary_target).Click += delegate {
					Toast.MakeText (context, Resource.String.touched_primary_message, ToastLength.Short).Show ();
				};

				convertView.FindViewById (Resource.Id.secondary_action).Click += delegate {
					Toast.MakeText (context, Resource.String.touched_secondary_message, ToastLength.Short).Show ();
				};

				return convertView;
			}
		}
	}
}
