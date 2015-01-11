/*
 * Copyright (C) 2014 The Android Open Source Project
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
using Android.Graphics;
using Android.OS;
using Android.Widget;

using ActivitySceneTransitionBasic.ImageLoader;

/**
 * Our secondary Activity which is launched from MainActivity. Has a simple detail UI
 * which has a large banner image, title and body text.
 */
namespace ActivitySceneTransitionBasic
{

	[Activity (Label="@string/app_name")]
	public class DetailActivity : Activity
	{

		// Extra name for the ID parameter
		public static readonly string EXTRA_PARAM_ID = "detail:_id";

		// View name of the header image. Used for activity scene transitions
		public static readonly string VIEW_NAME_HEADER_IMAGE = "detail:header:image";

		// View name of the header title. Used for activity scene transitions
		public static readonly string VIEW_NAME_HEADER_TITLE = "detail:header:title";

		private ImageView mHeaderImageView;
		private TextView mHeaderTitle;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);
			SetContentView (Resource.Layout.details);
		
			Item item = Item.GetItem (Intent.GetIntExtra (EXTRA_PARAM_ID, 0));

			mHeaderImageView = (ImageView) FindViewById (Resource.Id.imageview_header);
			mHeaderTitle = (TextView) FindViewById (Resource.Id.textview_title);


			/**
         	* Set the name of the view's which will be transition to, using the static values above.
        	* This could be done in the layout XML, but exposing it via static variables allows easy
	        * querying from other Activities
	        */
			mHeaderImageView.TransitionName = VIEW_NAME_HEADER_IMAGE;
			mHeaderTitle.TransitionName = VIEW_NAME_HEADER_TITLE;

			LoadItem (item);

		}

		private void LoadItem (Item item)
		{
			// Set the title TextView to the item's name and author
			mHeaderTitle.Text = Java.Lang.String.Format(GetString(Resource.String.image_header), item.name, item.author);

			// Check to see if we already have the thumbnail sized image in the cache. If so, start
			// loading the full size image and display the thumbnail as a placeholder.

			#pragma warning disable 4014
			Images.SetImageFromUrlAsync (mHeaderImageView, item.thumbnailUrl);
			Images.SetImageFromUrlAsync (mHeaderImageView, item.photoUrl);
			#pragma warning restore 4014
		 
		}
	}

}

