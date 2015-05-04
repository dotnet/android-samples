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
using Android.Content.Res;

namespace MonoDroid.ApiDemo
{
	/**
 	* A form, showing use of the GridLayout API. Here we demonstrate use of the row/column order
 	* preserved property which allows rows and or columns to pass over each other when needed.
 	* The two buttons in the bottom right corner need to be separated from the other UI elements.
 	* This can either be done by separating rows or separating columns - but we don't need
 	* to do both and may only have enough space to do one or the other.
 	*/
	[Activity (Label = "Views/Layouts/GridLayout/3. Form (Java)", Name = "monodroid.apidemo.GridLayout3")]
	[IntentFilter (new[] { Intent.ActionMain }, Categories = new string[] { ApiDemo.SAMPLE_CATEGORY })]			
	public class GridLayout3 : Activity
	{
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			SetContentView (Create (this));
		}

		public static View Create (Context context)
		{
			GridLayout p = new GridLayout(context);
			p.UseDefaultMargins = true;
			p.AlignmentMode = GridAlign.Bounds;
			Configuration configuration = context.Resources.Configuration;
			if ((configuration.Orientation == Android.Content.Res.Orientation.Portrait)) {
				p.ColumnOrderPreserved = false;
			} else {
				p.RowOrderPreserved = false;
			}

			GridLayout.Spec titleRow              = GridLayout.InvokeSpec (0);
			GridLayout.Spec introRow              = GridLayout.InvokeSpec (1);
			GridLayout.Spec emailRow              = GridLayout.InvokeSpec (2, GridLayout.BaselineAlighment);
			GridLayout.Spec passwordRow           = GridLayout.InvokeSpec (3, GridLayout.BaselineAlighment);
			GridLayout.Spec button1Row            = GridLayout.InvokeSpec (5);
			GridLayout.Spec button2Row            = GridLayout.InvokeSpec (6);

			GridLayout.Spec centerInAllColumns    = GridLayout.InvokeSpec (0, 4, GridLayout.Center);
			GridLayout.Spec leftAlignInAllColumns = GridLayout.InvokeSpec (0, 4, GridLayout.LeftAlighment);
			GridLayout.Spec labelColumn           = GridLayout.InvokeSpec (0, GridLayout.RightAlighment);
			GridLayout.Spec fieldColumn           = GridLayout.InvokeSpec (1, GridLayout.LeftAlighment);
			GridLayout.Spec defineLastColumn      = GridLayout.InvokeSpec (3);
			GridLayout.Spec fillLastColumn        = GridLayout.InvokeSpec (3, GridLayout.Fill);

			{
				TextView c = new TextView (context);
				c.TextSize = 32;
				c.Text = "Email setup";
				p.AddView (c, new GridLayout.LayoutParams (titleRow, centerInAllColumns));
			}
			{
				TextView c = new TextView (context);
				c.TextSize = 16;
				c.Text = "You can configure email in a few simple steps:";
				p.AddView (c, new GridLayout.LayoutParams (introRow, leftAlignInAllColumns));
			}
			{
				TextView c = new TextView (context);
				c.Text = "Email address:";
				p.AddView (c, new GridLayout.LayoutParams (emailRow, labelColumn));
			}
			{
				EditText c = new EditText (context);
				c.SetEms (10);
				c.InputType = Android.Text.InputTypes.ClassText | Android.Text.InputTypes.TextVariationEmailAddress;
				p.AddView (c, new GridLayout.LayoutParams (emailRow, fieldColumn));
			}
			{
				TextView c = new TextView (context);
				c.Text = "Password:";
				p.AddView (c, new GridLayout.LayoutParams (passwordRow, labelColumn));
			}
			{
				TextView c = new EditText (context);
				c.SetEms (8);
				c.InputType = Android.Text.InputTypes.ClassText | Android.Text.InputTypes.TextVariationPassword;
				p.AddView (c, new GridLayout.LayoutParams (passwordRow, fieldColumn));
			}
			{
				Button c = new Button (context);
				c.Text = "Manual setup";
				p.AddView (c, new GridLayout.LayoutParams (button1Row, defineLastColumn));
			}
			{
				Button c = new Button (context);
				c.Text ="Next";
				p.AddView (c, new GridLayout.LayoutParams (button2Row, fillLastColumn));
			}

			return p;
		}
	}
}