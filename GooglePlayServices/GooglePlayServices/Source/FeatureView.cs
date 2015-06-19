/*
 * Copyright (C) 2012 The Android Open Source Project
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

// C# Port by Atsushi Eno
// Copyright (C) 2012 Xamarin, Inc. (Apache License, Version 2.0 too)

using System;

using Android.Content;
using Android.Views;
using Android.Widget;

namespace GooglePlayServicesTest
{
	public class FeatureView : FrameLayout
	{
		/**
     * Constructs a feature view by inflating layout/feature.xml.
     */
		public FeatureView (Context context) 
			: base (context)
		{
			LayoutInflater layoutInflater =
				(LayoutInflater) context.GetSystemService (Context.LayoutInflaterService);
			layoutInflater.Inflate (Resource.Layout.feature, this);
		}

		object lock_obj = new object ();

		/**
     * Set the resource id of the title of the demo.
     *
     * @param titleId the resource id of the title of the demo
     */
		public void SetTitleId (int titleId) 
		{
			lock (lock_obj)
				((TextView) (FindViewById (Resource.Id.title))).SetText (titleId);
		}
		
		/**
     * Set the resource id of the description of the demo.
     *
     * @param descriptionId the resource id of the description of the demo
     */
		public void SetDescriptionId(int descriptionId) 
		{
			lock (lock_obj)
				((TextView) (FindViewById(Resource.Id.description))).SetText (descriptionId);
		}
	}
}

