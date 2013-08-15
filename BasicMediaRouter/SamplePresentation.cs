/*
 * Copyright 2013 The Android Open Source Project
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *       http://www.apache.org/licenses/LICENSE-2.0
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
 * <p>
 * A {@link Presentation} used to demonstrate interaction between primary and
 * secondary screens.
 * </p>
 * <p>
 * It displays the name of the display in which it has been embedded (see
 * {@link Presentation#getDisplay()}) and exposes a facility to change its
 * background color and display its text.
 * </p>
 */
using Android.Graphics;

namespace BasicMediaRouter
{
	public class SamplePresentation : Presentation
	{
		LinearLayout mLayout;
		TextView mText;

		public SamplePresentation (Context outerContext, Display display) : base (outerContext, display)
		{
		}

		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);
			SetContentView (Resource.Layout.display);

			// Get the Views
			mLayout = FindViewById <LinearLayout> (Resource.Id.display_layout);
			mText = FindViewById <TextView> (Resource.Id.display_text);

			/*
        	 * Show the name of the display this presentation was embedded in.
         	*/
			var smallText = (TextView) FindViewById (Resource.Id.display_smalltext);
			var name = Display.Name;
			smallText.Text = Resources.GetString (Resource.String.display_name, name);
		}

		/**
     	* Set the background color of the layout and display the color as a String.
     	*
     	* @param color The background color
    	 */
		public void SetColor (int color)
		{
			mLayout.SetBackgroundColor (new Color (color));

			// Display the color as a string on screen
			var s = Resources.GetString (Resource.String.display_color, color);
			mText.Text = s;
		}
	}
}

