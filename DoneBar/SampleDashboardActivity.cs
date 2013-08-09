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

namespace DoneBar
{
	/*
	 * A simple launcher activity offering access to the individual samples in this project.
	 */
	[Activity (Label = "DoneBar", MainLauncher = true, Theme="@style/Theme.Sample")]
	public class SampleDashboardActivity : Activity
	{
		/*
		 * The collection of samples that will be used to populate the 'dashboard' grid.
		 */
		Sample[] mSamples;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			SetContentView (Resource.Layout.activity_sample_dashboard);

			// Prepare list of samples in this dashboard.
			mSamples = new Sample[] {
				new Sample (Resource.String.done_bar_title, Resource.String.done_bar_description, 
				            new Intent (this, typeof (DoneBarActivity))),

				new Sample (Resource.String.done_button_title, Resource.String.done_button_description, 
				            new Intent (this, typeof (DoneButtonActivity))),
			};

			// Use the custom adapter in the GridView and hook up a click listener to handle
			// selection of individual samples.
			GridView gridView = (GridView)FindViewById (Android.Resource.Id.List);
			gridView.Adapter = new SampleAdapter (this, mSamples);

			gridView.ItemClick += delegate(object sender, AdapterView.ItemClickEventArgs e) {
				StartActivity (mSamples[e.Position].Intent);
			};
		}
	}

	/**
     * A custom array-based adapter, designed for use with a {@link GridView}.
     */
	class SampleAdapter : BaseAdapter
	{
		Context context;
		Sample[] mSamples;

		public SampleAdapter (Context c, Sample[] samples)
		{
			context = c;
			mSamples = samples;
		}

		public override int Count {
			get {
				return mSamples.Length;
			}
		}

		public override Java.Lang.Object GetItem (int position)
		{
			return (Java.Lang.Object) mSamples [position];
		}

		public override long GetItemId (int position)
		{
			// The title string ID should be unique per sample, so use it as an ID.
			return mSamples[position].TitleResId;
		}

		public override View GetView (int position, View convertView, ViewGroup parent)
		{
			if (convertView == null) {
				// If there was no re-usable view that can be simply repopulated, create
				// a new root view for this grid item.
				var inflater = (LayoutInflater) context.GetSystemService (Context.LayoutInflaterService);
				convertView = inflater.Inflate (Resource.Layout.sample_dashboard_item, parent, false);
			}

			// Populate the view's children with real data about this sample.
			convertView.FindViewById <TextView> (Android.Resource.Id.Text1).SetText (mSamples[position].TitleResId);
			convertView.FindViewById <TextView> (Android.Resource.Id.Text2).SetText (mSamples[position].DescriptionResId);
			return convertView;
		}
	}

	/**
     * A simple class that stores information about a sample: a title, description, and
     * the intent to call
     * {@link android.content.Context#startActivity(android.content.Intent) startActivity}
     * with in order to open the sample.
     */
	public class Sample : Java.Lang.Object
	{
		public int TitleResId;
		public int DescriptionResId;
		public Intent Intent;

		/*
         * Instantiate a new sample object with a title, description, and intent.
         */
		public Sample (int titleResId, int descriptionResId, Intent intent) 
		{
			Intent = intent;
			TitleResId = titleResId;
			DescriptionResId = descriptionResId;
		}
	}
}


