/*
 * Copyright (C) 2007 The Android Open Source Project
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

namespace MonoDroid.ApiDemo
{
	[Activity (Label = "Views/Layouts/TableLayout/07. Column Collapse")]
	[IntentFilter (new[] { Intent.ActionMain }, Categories = new string[] { ApiDemo.SAMPLE_CATEGORY })]		
	public class TableLayout07 : Activity
	{
	 	bool mShortcutsCollapsed;
		bool mCheckmarksCollapsed;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			SetContentView (Resource.Layout.table_layout_7);

			var table = FindViewById <TableLayout> (Resource.Id.menu);
			var button = FindViewById <Button> (Resource.Id.toggle1);
			button.Click += delegate {
				mShortcutsCollapsed = !mShortcutsCollapsed;
				table.SetColumnCollapsed (2, mShortcutsCollapsed);
			};

			var button2 = FindViewById <Button> (Resource.Id.toggle2);
			button2.Click += delegate {
				mCheckmarksCollapsed = !mCheckmarksCollapsed;
				table.SetColumnCollapsed (0, mCheckmarksCollapsed);
			};

			mCheckmarksCollapsed = table.IsColumnCollapsed (0);
			mShortcutsCollapsed = table.IsColumnCollapsed (2);

			AppendRow (table);
		}

		void AppendRow (TableLayout table)
		{
			TableRow row = new TableRow (this);

			TextView label = new TextView (this);
			label.SetText (Resource.String.table_layout_7_quit);
			label.SetPadding (3, 3, 3, 3);

			TextView shortcut = new TextView (this);
			shortcut.SetText (Resource.String.table_layout_7_ctrlq);
			shortcut.SetPadding (3, 3, 3, 3);
			shortcut.Gravity = GravityFlags.Right | GravityFlags.Top;

			row.AddView (label, new TableRow.LayoutParams (1));
			row.AddView (shortcut, new TableRow.LayoutParams ());

			table.AddView (row, new TableLayout.LayoutParams ());
		}
	}
}

