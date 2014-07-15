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

using Android.Widget;
using Android.App;
using Android.Views;

namespace FragmentTransition
{
	public class MeatAdapter : BaseAdapter<Meat>
	{
		private LayoutInflater mLayoutInflater;
		private int mResourceId;

		public MeatAdapter (LayoutInflater mLayoutInflater, int resourceId) : base ()
		{
			this.mLayoutInflater = mLayoutInflater;
			mResourceId = resourceId;
		}

		public override long GetItemId (int pos)
		{
			return this [pos].resourceId;
		}

		public override int Count {
			get { return Meat.MEATS.Length; }
		}

		public override Meat this [int position] {
			get { return Meat.MEATS [position]; }
		}

		public override View GetView (int position, View convertView, ViewGroup parent)
		{
			View view;
			ViewHolder holder;
			if (null == convertView) {
				view = mLayoutInflater.Inflate (mResourceId, parent, false);
				System.Diagnostics.Debug.Assert (view != null);
				holder = new ViewHolder ();
				holder.image = view.FindViewById<ImageView> (Resource.Id.image);
				holder.title = view.FindViewById<TextView> (Resource.Id.title);
				view.Tag = holder;
			} else {
				view = convertView;
				holder = (ViewHolder)view.Tag;
			}
			BindView (holder, position);
			return view;
		}

		public void BindView (ViewHolder holder, int position)
		{
			Meat meat = this [position];
			holder.image.SetImageResource (meat.resourceId);
			holder.title.Text = meat.title;
		}

	}
	//This allows for setting and retrieving of the view.Tag field
	public class ViewHolder : Java.Lang.Object
	{
		public ImageView image;
		public TextView title;
	}
}

