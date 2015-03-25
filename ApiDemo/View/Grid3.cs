/*
 * Copyright (C) 2010 The Android Open Source Project
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
using Android.Content.PM;

namespace MonoDroid.ApiDemo
{
	/**
 	* This demo illustrates the use of CHOICE_MODE_MULTIPLE_MODAL, a.k.a. selection mode on GridView.
 	*/
	[Activity (Label = "Views/Grid/3. Selection Mode", Name = "monodroid.apidemo.Grid3")]
	[IntentFilter (new[] { Intent.ActionMain }, Categories = new string[] { ApiDemo.SAMPLE_CATEGORY })]				
	public class Grid3 : Activity
	{
		public GridView mGrid;
		public IList<ResolveInfo> mApps;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			Window.RequestFeature (WindowFeatures.ActionBar);

			LoadApps ();

			SetContentView (Resource.Layout.grid_1);
			mGrid = FindViewById <GridView> (Resource.Id.myGrid);
			mGrid.Adapter = new AppsAdapter (this);
			mGrid.ChoiceMode = (ChoiceMode)AbsListViewChoiceMode.MultipleModal;
			mGrid.SetMultiChoiceModeListener (new MultiChoiceModeListener (this));
		}

		protected override void OnPostCreate (Bundle savedInstanceState)
		{
			base.OnPostCreate (savedInstanceState);
			ActionBar.Subtitle = "Long press to start selection";
		}

		private void LoadApps ()
		{
			Intent mainIntent = new Intent (Intent.ActionMain, null);
			mainIntent.AddCategory (Intent.CategoryLauncher);

			mApps = PackageManager.QueryIntentActivities (mainIntent, 0);
		}

		class AppsAdapter : BaseAdapter
		{
			Context self;

			public AppsAdapter (Context s)
			{
				self = s;
			}

			public override View GetView (int position, View convertView, ViewGroup parent)
			{
				CheckableLayout l;
				ImageView i;

				if (convertView == null) {
					i = new ImageView (self);
					i.SetScaleType (ImageView.ScaleType.FitCenter);
					i.LayoutParameters = new ViewGroup.LayoutParams (50, 50);
					l = new CheckableLayout (self);
					l.LayoutParameters = new GridView.LayoutParams (GridView.LayoutParams.WrapContent,
					                                              	GridView.LayoutParams.WrapContent);
					l.AddView (i);
				} else {
					l = (CheckableLayout) convertView;
					i = (ImageView) l.GetChildAt (0);
				}

				ResolveInfo info = ((Grid3)self).mApps[position];
				i.SetImageDrawable (info.ActivityInfo.LoadIcon (self.PackageManager));

				return l;
			}

			public override int Count {
				get {
					return ((Grid3)self).mApps.Count;
				}
			}

			public override Java.Lang.Object GetItem (int position)
			{
				return ((Grid3)self).mApps [position];
			}

			public override long GetItemId (int position)
			{
				return position;
			}
		}

		class CheckableLayout : FrameLayout, ICheckable
		{
			bool _checked;

			public CheckableLayout (Context context) : base (context)
			{
			}

			public bool Checked {
				get {return _checked; } 
				set { 
					_checked = value;
					SetBackgroundDrawable (value ? Resources.GetDrawable (Resource.Drawable.blue) : null);
				}
			}

			public void Toggle ()
			{
				Checked = !_checked;
			}
		}

		class MultiChoiceModeListener : Java.Lang.Object, GridView.IMultiChoiceModeListener
		{
			Context self;

			public MultiChoiceModeListener (Context s)
			{
				self = s;
			}

			public bool OnCreateActionMode (ActionMode mode, IMenu menu)
			{
				mode.Title = "Select Items";
				mode.Subtitle = "One item selected";
				return true;
			}

			public bool OnPrepareActionMode (ActionMode mode, IMenu menu)
			{
				return true;
			}

			public bool OnActionItemClicked (ActionMode mode, IMenuItem item)
			{
				return true;
			}

			public void OnDestroyActionMode (ActionMode mode)
			{
			}

			public void OnItemCheckedStateChanged (ActionMode mode, int position, long id, bool isChecked) {
				int selectCount = ((Grid3)self).mGrid.CheckedItemCount;

				switch (selectCount) {
				case 1:
					mode.Subtitle = "One item selected";
					break;
				
				default:
					mode.Subtitle = "" + selectCount + " items selected";
					break;
				}
			}
		}
	}
}

