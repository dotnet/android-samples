/*
 * Copyright 2014 The Android Open Source Project
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

using Android.Support.V4.App;
using Android.Content;
using Android.OS;
using Android.Animation;
using Android.Views.Animations;
using Android.Runtime;
using Android.Views;
using Android.Widget;

using CommonSampleLibrary;

namespace FragmentTransition
{
	public class FragmentTransitionFragment : Android.Support.V4.App.Fragment, GridView.IOnItemClickListener
	{
		private const string TAG = "FragmentTransitionFragment";

		private MeatAdapter mAdapter;

		public static FragmentTransitionFragment NewInstance ()
		{
			return new FragmentTransitionFragment ();
		}

		public FragmentTransitionFragment ()
		{
		}

		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			// This is the adapter we use to populate the grid.
			mAdapter = new MeatAdapter (inflater, Resource.Layout.item_meat_grid);
			// Inflate the layout with a GridView in it.
			return inflater.Inflate (Resource.Layout.fragment_fragment_transition, container, false);
		}

		public override void OnViewCreated (View view, Bundle savedInstanceState)
		{
			var grid = view.FindViewById<GridView> (Resource.Id.grid);
			grid.Adapter = mAdapter;
			grid.OnItemClickListener = this;
		}
		public void OnItemClick(AdapterView parent, View view, int position, long id)
		{
			Meat meat = mAdapter [position];
			Log.Info (TAG, meat.title + " clicked. Replacing fragment.");
			// We start the fragment transaction here. It is just an ordinary fragment transaction.
			Activity.SupportFragmentManager
				.BeginTransaction ()
				.Replace (Resource.Id.sample_content_fragment,
					DetailFragment.NewInstance (meat.resourceId, meat.title,
						(int)view.GetX (), (int)view.GetY (),
						view.Width, view.Height)
				)
				// We push the fragment transaction to back stack. User can go back to the
				// previous fragment by pressing back button.
				.AddToBackStack ("detail")
				.Commit ();
		}

		public override Animation OnCreateAnimation (int transit, bool enter, int nextAnim)
		{
			return AnimationUtils.LoadAnimation (Activity,
				enter ? Android.Resource.Animation.FadeIn : Android.Resource.Animation.FadeOut);
		}
	}
}