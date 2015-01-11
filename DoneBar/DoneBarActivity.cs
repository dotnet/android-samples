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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

/**
 * A sample activity demonstrating the "done bar" alternative action bar presentation. For a more
 * detailed description see {@link R.string.done_bar_description}.
 */

namespace DoneBar
{
	[Activity (Label = "DoneBarActivity", Theme="@style/Theme.Sample")]			
	public class DoneBarActivity : Activity
	{
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			// Inflate a "Done/Cancel" custom action bar view.
			var inflater = (LayoutInflater) ActionBar.ThemedContext.GetSystemService (Context.LayoutInflaterService);
			View customActionBarView = inflater.Inflate (Resource.Layout.actionbar_custom_view_done_cancel, null);
		
			customActionBarView.FindViewById (Resource.Id.actionbar_done).Click += delegate {
				// "Done"
				Finish();
			};

			customActionBarView.FindViewById (Resource.Id.actionbar_cancel).Click += delegate {
				// "Cancel"
				Finish();
			};

			// Show the custom action bar view and hide the normal Home icon and title.
			ActionBar.SetDisplayOptions (ActionBarDisplayOptions.ShowCustom, ActionBarDisplayOptions.ShowCustom | 
			                              ActionBarDisplayOptions.ShowHome | ActionBarDisplayOptions.ShowTitle);

			ActionBar.SetCustomView (customActionBarView, new ActionBar.LayoutParams (
				ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent));

			SetContentView (Resource.Layout.activity_done_bar);
		}
	}
}

