/*
 * Copyright (C) 2011 The Android Open Source Project
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
using Android.OS;
using Android.Views;
using Android.Widget;
using Xamarin.ActionbarSherlockBinding.App;
using SherlockActionBar = Xamarin.ActionbarSherlockBinding.App.ActionBar;
using Xamarin.ActionbarSherlockBinding.Views;
using Tab = Xamarin.ActionbarSherlockBinding.App.ActionBar.Tab;
using FragmentTransaction = Android.Support.V4.App.FragmentTransaction;
using ActionProvider = Xamarin.ActionbarSherlockBinding.Views.ActionProvider;
using Android.Provider;

namespace Mono.ActionbarsherlockTest
{
	/**
 * This activity demonstrates how to implement an {@link android.view.ActionProvider}
 * for adding functionality to the Action Bar. In particular this demo creates an
 * ActionProvider for launching the system settings and adds a menu item with that
 * provider.
 */
	[Activity (Label = "@string/action_providers")]
	[IntentFilter (new string [] { Intent.ActionMain },
		Categories = new string [] { Constants.DemoCategory })]
	public class ActionProviders : SherlockActivity
	{

		protected override void OnCreate (Bundle savedInstanceState)
		{
			SetTheme (SampleList.THEME); //Used for theme switching in samples
			base.OnCreate (savedInstanceState);
			SetContentView (Resource.Layout.text);
			FindViewById<TextView> (Resource.Id.text).SetText (Resource.String.action_providers_content);
		}

		/**
     * {@inheritDoc}
     */
		public override bool OnCreateOptionsMenu (Xamarin.ActionbarSherlockBinding.Views.IMenu menu)
		{
			base.OnCreateOptionsMenu (menu);
			SupportMenuInflater.Inflate (Resource.Menu.settings_action_provider, menu);
			return true;
		}

		/**
     * {@inheritDoc}
     */
		public override bool OnOptionsItemSelected (Xamarin.ActionbarSherlockBinding.Views.IMenuItem item)
		{
			// If this callback does not handle the item click, onPerformDefaultAction
			// of the ActionProvider is invoked. Hence, the provider encapsulates the
			// complete functionality of the menu item.
			Toast.MakeText (this, "Handling in onOptionsItemSelected avoided",
			               ToastLength.Short).Show ();
			return false;
		}

		public class SettingsActionProvider : ActionProvider
		{

			/** An intent for launching the system settings. */
			private static readonly Intent sSettingsIntent = new Intent (Settings.ActionSettings);
			/** Context for accessing resources. */
			private readonly Context mContext;

			/**
         * Creates a new instance.
         *
         * @param context Context for accessing resources.
         */
			public SettingsActionProvider (Context context) 
				: base (context)
			{
				mContext = context;
			}

			/**
         * {@inheritDoc}
         */
			public override View OnCreateActionView ()
			{
				// Inflate the action view to be shown on the action bar.
				LayoutInflater layoutInflater = LayoutInflater.From (mContext);
				View view = layoutInflater.Inflate (Resource.Layout.settings_action_provider, null);
				ImageButton button = view.FindViewById<ImageButton> (Resource.Id.button);
				// Attach a click listener for launching the system settings.
				button.Click += delegate {
					mContext.StartActivity (sSettingsIntent);
				};
				return view;
			}

			/**
         * {@inheritDoc}
         */
			public override bool OnPerformDefaultAction ()
			{
				// This is called if the host menu item placed in the overflow menu of the
				// action bar is clicked and the host activity did not handle the click.
				mContext.StartActivity (sSettingsIntent);
				return true;
			}
		}
	}
}
