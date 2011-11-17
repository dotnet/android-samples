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
using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Text;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace com.example.monodroid.hcgallery
{

	public class TitlesFragment : ListFragment 
	{
		private int mCategory = 0;
		private int mCurPosition = 0;
			
		public override void OnActivityCreated (Bundle savedInstanceState) 
		{
			base.OnActivityCreated (savedInstanceState);
			
			//Current position should survive screen rotations.
			if (savedInstanceState != null) {
				mCategory = savedInstanceState.GetInt ("category");
				mCurPosition = savedInstanceState.GetInt ("listPosition");
			}
			
			PopulateTitles (mCategory);
			ListView lv = ListView;
			lv.ChoiceMode = ChoiceMode.Single;
			lv.CacheColorHint = Color.Transparent;
			lv.ItemLongClick += (o, e) => {
				String title = (String) ((TextView) e.View).Text;
			
				// Set up clip data with the category||entry_id format.
				String textData = String.Format ("{0}||{1}", mCategory, e.Position);
				ClipData data = ClipData.NewPlainText (title, textData);
				e.View.StartDrag (data, new MyDragShadowBuilder (e.View), null, 0);
			};
			
			SelectPosition (mCurPosition);
		}
			
		private class MyDragShadowBuilder : View.DragShadowBuilder
		{
			private Drawable mShadow;
			
			public MyDragShadowBuilder(View v)
				: base (v)
			{
			
				TypedArray a = v.Context.ObtainStyledAttributes (Resource.Styleable.AppTheme);
				mShadow = a.GetDrawable (Resource.Styleable.AppTheme_listDragShadowBackground);
				mShadow.Callback = v;
				mShadow.SetBounds (0, 0, v.Width, v.Height);
				a.Recycle ();
			}
			
			public override void OnDrawShadow (Canvas canvas) 
			{
				base.OnDrawShadow (canvas);
				mShadow.Draw (canvas);
				View.Draw (canvas);
			}
		}
			
		public void PopulateTitles (int category) 
		{
			DirectoryCategory cat = Directory.GetCategory (category);
			String[] items = new String [cat.EntryCount];
			for (int i = 0; i < cat.EntryCount; i++)
				items [i] = cat.GetEntry (i).Name;
			ListAdapter = new ArrayAdapter<String>(Activity,
				Resource.Layout.title_list_item, items);
			mCategory = category;
		}
			
		public override void OnListItemClick (ListView l, View v, int position, long id)
		{
			UpdateImage (position);
		}
			
		private void UpdateImage (int position) 
		{
			ContentFragment frag = (ContentFragment) FragmentManager
				.FindFragmentById (Resource.Id.frag_content);
			frag.UpdateContentAndRecycleBitmap (mCategory, position);
			mCurPosition = position;
		}
			
		public void SelectPosition (int position) 
		{
			ListView lv = ListView;
			lv.SetItemChecked (position, true);
			UpdateImage (position);
		}
			
		public override void OnSaveInstanceState (Bundle outState)
		{
			base.OnSaveInstanceState (outState);
			outState.PutInt ("listPosition", mCurPosition);
			outState.PutInt ("category", mCategory);
		}
	}
}
