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
using System.Collections.Generic;
using Android.Support.V7.Widget;
using Android.Widget;
using Android.Views;

namespace DirectorySelection
{
	/// <summary>
	/// Provide views to RecyclerView with the directory entries.
	/// </summary>
	public class DirectoryEntryAdapter : RecyclerView.Adapter
	{
		protected static readonly string DIRECTORY_MIME_TYPE = "vnd.android.document/directory";
		List<DirectoryEntry> mDirectoryEntries;

		/// <summary>
		/// Initialize the directory entries of the Adapter.
		/// </summary>
		/// <param name="directoryEntries">A list of <see cref="DirectorySelection.DirectoryEntry"/></param>
		public DirectoryEntryAdapter (List<DirectoryEntry> directoryEntries)
		{
			mDirectoryEntries = directoryEntries;
		}

		public override RecyclerView.ViewHolder OnCreateViewHolder (ViewGroup parent, int viewType)
		{
			View v = LayoutInflater.From (parent.Context)
				.Inflate (Resource.Layout.directory_item, parent, false);
			return new DirectoryEntryAdapter.ViewHolder (v);
		}

		public override void OnBindViewHolder (RecyclerView.ViewHolder holder, int position)
		{
			var myHolder = (ViewHolder)holder;
			myHolder.mFileName.Text = mDirectoryEntries [position].fileName;
			//myHolder.mImageView.setText(mDirectoryEntries[position].mimeType);

			if (DIRECTORY_MIME_TYPE == (mDirectoryEntries [position].mimeType)) {
				myHolder.mImageView.SetImageResource (Resource.Drawable.ic_folder_grey600_36dp);
			} else {
				myHolder.mImageView.SetImageResource (Resource.Drawable.ic_description_grey600_36dp);
			}
		}

		public override int ItemCount {
			get {
				return mDirectoryEntries.Count;
			}
		}

		public void SetDirectoryEntries (List<DirectoryEntry> directoryEntries)
		{
			mDirectoryEntries = directoryEntries;
		}

		public class ViewHolder : RecyclerView.ViewHolder
		{
			public readonly TextView mFileName;
			public readonly TextView mMimeType;
			public readonly ImageView mImageView;

			public ViewHolder (View v) : base (v)
			{
				mFileName = (TextView)v.FindViewById (Resource.Id.textview_filename);
				mMimeType = (TextView)v.FindViewById (Resource.Id.textview_mimetype);
				mImageView = (ImageView)v.FindViewById (Resource.Id.entry_image);
			}
		}
	}
}

