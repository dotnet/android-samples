/*
 * Copyright (C) 2011 Jake Wharton
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
using Android.Content.PM;
using Android.OS;
using Android.Widget;
using Xamarin.ActionbarSherlockBinding;
using Xamarin.ActionbarSherlockBinding.App;
using Xamarin.ActionbarSherlockBinding.Views;
using SherlockActionBar = Xamarin.ActionbarSherlockBinding.App.ActionBar;

namespace Mono.ActionbarsherlockTest
{
	[Activity (Label = "@string/static_attach")]
	[IntentFilter (new string [] { Intent.ActionMain },
		Categories = new string [] { Constants.DemoCategory })]
	public class StaticAttachment : Activity, ActionBarSherlock.IOnCreateOptionsMenuListener
	{
		ActionBarSherlock mSherlock;

		public StaticAttachment ()
		{
			mSherlock = ActionBarSherlock.Wrap (this);
		}

		protected override void OnCreate (Bundle savedInstanceState)
		{
			SetTheme (SampleList.THEME); //Used for theme switching in samples
			base.OnCreate (savedInstanceState);

			/*
         * Most interactions with what would otherwise be the system UI should
         * now be done through this instance. Content, title, action bar, and
         * menu inflation can all be done.
         *
         * All of the base activities use this class to provide the normal
         * action bar functionality so everything that they can do is possible
         * using this static attachment method.
         *
         * Calling something like setContentView or getActionBar on this
         * instance is required in order to properly set up the wrapped layout
         * and dispatch menu events (if they are needed).
         */
			mSherlock.SetUiOptions ((int)UiOptions.SplitActionBarWhenNarrow);
			mSherlock.SetContentView (Resource.Layout.text);

			FindViewById<TextView> (Resource.Id.text).SetText (Resource.String.static_attach_content);
		}
		/*
     * In order to use action items properly with static attachment you
     * need to dispatch create, prepare, and selected events for the
     * native type to the ActionBarSherlock instance. If for some reason
     * you need to use static attachment you should probably create a
     * common base activity that does this for all three methods.
     */
		public override bool OnCreateOptionsMenu (Android.Views.IMenu menu)
		{
			return mSherlock.DispatchCreateOptionsMenu (menu);
		}
		/*
     * In order to receive these events you need to implement an interface
     * from ActionBarSherlock so it knows to dispatch to this callback.
     * There are three possible interface you can implement, one for each
     * menu event.
     *
     * Remember, there are no superclass implementations of these methods so
     * you must return a value with meaning.
     */
		public bool OnCreateOptionsMenu (IMenu menu)
		{
			//Used to put dark icons on light action bar
			bool isLight = SampleList.THEME == Resource.Style.Theme_Sherlock_Light;

			menu.Add ("Save")
				.SetIcon (isLight ? Resource.Drawable.ic_compose_inverse : Resource.Drawable.ic_compose)
					.SetShowAsAction (MenuItem.ShowAsActionIfRoom | MenuItem.ShowAsActionWithText);

			menu.Add ("Search")
				.SetIcon (isLight ? Resource.Drawable.ic_search_inverse : Resource.Drawable.ic_search)
					.SetShowAsAction (MenuItem.ShowAsActionIfRoom | MenuItem.ShowAsActionWithText);

			menu.Add ("Refresh")
				.SetIcon (isLight ? Resource.Drawable.ic_refresh_inverse : Resource.Drawable.ic_refresh)
					.SetShowAsAction (MenuItem.ShowAsActionIfRoom | MenuItem.ShowAsActionWithText);

			return true;
		}
	}
}
