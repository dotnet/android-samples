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
using Android.Widget;
using Android.Support.V7.Widget;
using Android.Support.V4.App;
using Android.OS;
using Android.Views;
using Android.Content;
using Android.App;
using Android.Util;
using Android.Provider;
using Java.Lang;

namespace DirectorySelection
{
	/// <summary>
	/// Fragment that demonstrates how to use Directory Selection API.
	/// </summary>
	public class DirectorySelectionFragment : Android.Support.V4.App.Fragment
	{
		static readonly string TAG = typeof(DirectorySelectionFragment).Name;
		
		public static readonly int REQUEST_CODE_OPEN_DIRECTORY = 1;

		Android.Net.Uri mCurrentDirectoryUri;
		TextView mCurrentDirectoryTextView;
		Button mCreateDirectoryButton;
		RecyclerView mRecyclerView;
		DirectoryEntryAdapter mAdapter;
		RecyclerView.LayoutManager mLayoutManager;

		/// <summary>
		/// Use this factory method to create a new instance of
		/// this fragment using the provided parameters.
		/// </summary>
		/// <returns>A new instance of fragment <see cref="DirectorySelection.DirectorySelectionFragment"/>.</returns>
		public static DirectorySelectionFragment NewInstance ()
		{
			DirectorySelectionFragment fragment = new DirectorySelectionFragment ();
			return fragment;
		}

		public override View OnCreateView (LayoutInflater inflater, ViewGroup container,
		                                   Bundle savedInstanceState)
		{
			// Inflate the layout for this fragment
			return inflater.Inflate (Resource.Layout.fragment_directory_selection, container, false);
		}

		public override void OnViewCreated (View rootView, Bundle savedInstanceState)
		{
			base.OnViewCreated (rootView, savedInstanceState);

			var openDir = rootView.FindViewById (Resource.Id.button_open_directory);
			openDir.Click += (sender, e) => {
				Intent intent = new Intent (Intent.ActionOpenDocumentTree);
				StartActivityForResult (intent, REQUEST_CODE_OPEN_DIRECTORY);
			};

			mCurrentDirectoryTextView = (TextView)rootView
				.FindViewById (Resource.Id.textview_current_directory);
			mCreateDirectoryButton = (Button)rootView.FindViewById (Resource.Id.button_create_directory);

			mCreateDirectoryButton.Click += (sender, e) => {
				EditText editView = new EditText (Activity);
				var builder = new AlertDialog.Builder (Activity)
					.SetTitle (Resource.String.create_directory)
					.SetView (editView)
					.SetPositiveButton (Resource.String.ok, delegate {
					CreateDirectory (mCurrentDirectoryUri,
						editView.Text);
					UpdateDirectoryEntries (mCurrentDirectoryUri);
				})
					.SetNegativeButton (Resource.String.ok, delegate {
				});
				builder.Show ();
			};

			mRecyclerView = rootView.FindViewById<RecyclerView> (Resource.Id.recyclerview_directory_entries);
			mLayoutManager = new LinearLayoutManager (Activity);
			mRecyclerView.SetLayoutManager (mLayoutManager);
			mRecyclerView.ScrollToPosition (0);
			mAdapter = new DirectoryEntryAdapter (new List<DirectoryEntry> ());
			mRecyclerView.SetAdapter (mAdapter);
		}

		public override void OnActivityResult (int requestCode, int resultCode, Intent data)
		{
			base.OnActivityResult (requestCode, resultCode, data);
			if (requestCode == REQUEST_CODE_OPEN_DIRECTORY && resultCode == (int)Result.Ok) {
				Log.Debug (TAG, string.Format ("Open Directory result Uri : {0}", data.Data));
				UpdateDirectoryEntries (data.Data);
				mAdapter.NotifyDataSetChanged ();
			}
		}

		/// <summary>
		/// Updates the current directory of the uri passed as an argument and its children directories.
		/// And updates the instance's <see cref="Android.Support.V7.Widget.RecyclerView"/>  depending 
		/// on the contents of the children.
		/// </summary>
		/// <param name="uri">The uri of the current directory.</param>
		void UpdateDirectoryEntries (Android.Net.Uri uri)
		{
			var contentResolver = Activity.ContentResolver;
			var docUri = DocumentsContract.BuildDocumentUriUsingTree (uri,
				             DocumentsContract.GetTreeDocumentId (uri));
			var childrenUri = DocumentsContract.BuildChildDocumentsUriUsingTree (uri,
				                  DocumentsContract.GetTreeDocumentId (uri));

			var docCursor = contentResolver.Query (docUri, new [] {
				DocumentsContract.Document.ColumnDisplayName, 
				DocumentsContract.Document.ColumnMimeType
			}, null, null, null);
			try {
				while (docCursor.MoveToNext ()) {
					Log.Debug (TAG, "found doc =" + docCursor.GetString (0) + ", mime=" + docCursor
						.GetString (1));
					mCurrentDirectoryUri = uri;
					mCurrentDirectoryTextView.Text = docCursor.GetString (0);
					mCreateDirectoryButton.Enabled = true;
				}
			} finally {
				CloseQuietly (docCursor);
			}

			var childCursor = contentResolver.Query (childrenUri, new [] {
				DocumentsContract.Document.ColumnDisplayName, 
				DocumentsContract.Document.ColumnMimeType
			}, null, null, null);
			try {
				List<DirectoryEntry> directoryEntries = new List<DirectoryEntry> ();
				while (childCursor.MoveToNext ()) {
					Log.Debug (TAG, "found child=" + childCursor.GetString (0) + ", mime=" + childCursor
						.GetString (1));
					DirectoryEntry entry = new DirectoryEntry ();
					entry.fileName = childCursor.GetString (0);
					entry.mimeType = childCursor.GetString (1);
					directoryEntries.Add (entry);
				}
				mAdapter.SetDirectoryEntries (directoryEntries);
				mAdapter.NotifyDataSetChanged ();
			} finally {
				CloseQuietly (childCursor);
			}
		}

		/// <summary>
		/// Creates a directory under the directory represented as the uri in the argument.
		/// </summary>
		/// <param name="uri">The uri of the directory under which a new directory is created.</param>
		/// <param name="directoryName">The directory name of a new directory.</param>
		void CreateDirectory (Android.Net.Uri uri, string directoryName)
		{
			var contentResolver = Activity.ContentResolver;
			var docUri = DocumentsContract.BuildDocumentUriUsingTree (uri,
				             DocumentsContract.GetTreeDocumentId (uri));
			var directoryUri = DocumentsContract
				.CreateDocument (contentResolver, docUri, DocumentsContract.Document.MimeTypeDir, directoryName);
			if (directoryUri != null) {
				Log.Info (TAG, string.Format (
					"Created directory : {0}, Document Uri : {1}, Created directory Uri : {2}",
					directoryName, docUri, directoryUri));
				Toast.MakeText (Activity, string.Format ("Created a directory [{0}]",
					directoryName), ToastLength.Short).Show ();
			} else {
				Log.Warn (TAG, string.Format ("Failed to create a directory : {0}, Uri {1}", directoryName,
					docUri));
				Toast.MakeText (Activity, string.Format ("Failed to created a directory [{0}] : ",
					directoryName), ToastLength.Short).Show ();
			}
		}

		public void CloseQuietly (Java.IO.ICloseable closeable)
		{
			if (closeable != null) {
				try {
					closeable.Close ();
				} catch (RuntimeException rethrown) {
					throw rethrown;
				} catch (Java.Lang.Exception) {
				}
			}
		}
	}
}