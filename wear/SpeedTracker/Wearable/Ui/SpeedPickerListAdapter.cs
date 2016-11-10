/*
 * Copyright (C) 2014 Google Inc. All Rights Reserved.
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

using Android.Content;
using Android.Support.V7.Widget;
using Android.Support.Wearable.Views;
using Android.Views;
using Android.Widget;

namespace Wearable.Ui
{
	/**
	 * A {@link android.support.wearable.view.WearableListView.Adapter} that is used to populate the
	 * list of speeds.
	 */
	public class SpeedPickerListAdapter : WearableListView.Adapter
	{
		private int[] mDataSet;
		private Context mContext;
		private LayoutInflater mInflater;

		public SpeedPickerListAdapter(Context context, int[] dataset)
		{
			mContext = context;
			mInflater = LayoutInflater.From(context);
			mDataSet = dataset;
		}

		/**
		 * Displays all possible speed limit choices.
		 */
		public class ItemViewHolder : WearableListView.ViewHolder
		{
			public TextView mTextView;

			public ItemViewHolder(View itemView) : base(itemView)
			{
				// find the text view within the custom item's layout
				mTextView = (TextView)itemView.FindViewById(Resource.Id.name);
			}
		}

		/**
		 * Create new views for list items (invoked by the WearableListView's layout manager)
		 */
		public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
		{
			// Inflate our custom layout for list items
			return new ItemViewHolder(mInflater.Inflate(Resource.Layout.ItemSpeedPickerLayout, null));
		}

		/**
		 * Replaces the contents of a list item. Instead of creating new views, the list tries to
		 * recycle existing ones. This is invoked by the WearableListView's layout manager.
		 */
		public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
		{
			// retrieve the text view
			var itemHolder = (ItemViewHolder)holder;
			var view = itemHolder.mTextView;
			// replace text contents
			view.Text = mContext.GetString(Resource.String.speed_for_list, mDataSet[position]);
			// replace list item's metadata
			holder.ItemView.Tag = position;
		}

		/**
		 * Return the size of the data set (invoked by the WearableListView's layout manager).
		 */
		public override int ItemCount => mDataSet.Length;
	}
}