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

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Net;
using Android.OS;
using Android.Views;
using Android.Widget;
using Java.IO;
using Java.Lang;
using Java.Util;

using System;
using String = System.String;
using File = Java.IO.File;
using Uri = Android.Net.Uri;

namespace com.example.monodroid.hcgallery
{
	public class ContentFragment : Fragment 
	{
		private View mContentView;
		
		// The bitmap currently used by ImageView
		private Bitmap mBitmap = null;
		
		// Current action mode (contextual action bar, a.k.a. CAB)
		private ActionMode mCurrentActionMode;
			
		public override void OnActivityCreated (Bundle savedInstanceState) 
		{
			base.OnActivityCreated (savedInstanceState);
		}

		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState) 
		{
			mContentView = inflater.Inflate (Resource.Layout.content_welcome, null);
			ImageView imageView = (ImageView) mContentView.FindViewById (Resource.Id.image);
			mContentView.DrawingCacheEnabled = false;
			
			mContentView.Drag += (o, e) => {
				switch (e.Event.Action) {
				case DragAction.Entered:
					mContentView.SetBackgroundColor (
						Resources.GetColor (Resource.Color.drag_active_color));
					break;
				
				case DragAction.Exited:
					mContentView.SetBackgroundColor (Color.Transparent);
					break;
				
				case DragAction.Started:
					e.Handled = ProcessDragStarted (e.Event);
					break;
				
				case DragAction.Drop:
					mContentView.SetBackgroundColor (Color.Transparent);
					e.Handled = ProcessDrop (e.Event, imageView);
					break;
				}
				e.Handled = false;
			};
			
			// Keep the action bar visibility in sync with the system status bar. That is, when entering
			// 'lights out mode,' hide the action bar, and when exiting this mode, show the action bar.
			
			Activity activity = Activity;
			mContentView.SystemUiVisibilityChange += delegate (object sender, View.SystemUiVisibilityChangeEventArgs e) {
				ActionBar actionBar = Activity.ActionBar;
				if (actionBar != null) {
					mContentView.SystemUiVisibility = e.Visibility;
					if (e.Visibility == StatusBarVisibility.Visible) {
						actionBar.Show ();
					} else {
						actionBar.Hide ();
					}
				}
			};
			
			// Show/hide the system status bar when single-clicking a photo. This is also called
			// 'lights out mode.' Activating and deactivating this mode also invokes the listener
			// defined above, which will show or hide the action bar accordingly.
			
			mContentView.Click += delegate {
				if (mContentView.SystemUiVisibility == StatusBarVisibility.Visible) {
					mContentView.SystemUiVisibility = StatusBarVisibility.Hidden;
				} else {
					mContentView.SystemUiVisibility = StatusBarVisibility.Visible;
				}
			};
				
			// When long-pressing a photo, activate the action mode for selection, showing the
			// contextual action bar (CAB).
			
			mContentView.LongClick += (o, e) => {
				if (mCurrentActionMode != null) {
					e.Handled = false;
				}
				
				mCurrentActionMode = Activity.StartActionMode (
					mContentSelectionActionModeCallback);
				mContentView.Selected = true;
			};
			
			return mContentView;
		}
			
		bool ProcessDragStarted (DragEvent evt) 
		{
			// Determine whether to continue processing drag and drop based on the
			// plain text mime type.
			ClipDescription clipDesc = evt.ClipDescription;
			if (clipDesc != null) {
			    return clipDesc.HasMimeType (ClipDescription.MimetypeTextPlain);
			}
			return false;
		}
			
		bool ProcessDrop (DragEvent evt, ImageView imageView) 
		{
			// Attempt to parse clip data with expected format: category||entry_id.
			// Ignore event if data does not conform to this format.
			ClipData data = evt.ClipData;
			if (data != null) {
				if (data.ItemCount > 0) {
					ClipData.Item item = data.GetItemAt (0);
					String textData = (String) item.Text;
					if (textData != null) {
						StringTokenizer tokenizer = new StringTokenizer (textData, "||");
						if (tokenizer.CountTokens () != 2) {
							return false;
						}
						int category = -1;
						int entryId = -1;
						try {
							category = Java.Lang.Integer.ParseInt (tokenizer.NextToken ());
							entryId = Java.Lang.Integer.ParseInt (tokenizer.NextToken ());
						} catch (NumberFormatException exception) {
							return false;
						}
						UpdateContentAndRecycleBitmap (category, entryId);
						// Update list fragment with selected entry.
						TitlesFragment titlesFrag = (TitlesFragment)
						FragmentManager.FindFragmentById (Resource.Id.frag_title);
						titlesFrag.SelectPosition (entryId);
						return true;
					}
				}
			}
			return false;
		}
			
		internal void UpdateContentAndRecycleBitmap (int category, int position)
		{
			if (mCurrentActionMode != null) {
				mCurrentActionMode.Finish ();
			}
			
			if (mBitmap != null) {
				// This is an advanced call and should be used if you
				// are working with a lot of bitmaps. The bitmap is dead
				// after this call.
				mBitmap.Recycle ();
			}
			
			// Get the bitmap that needs to be drawn and update the ImageView
			mBitmap = Directory.GetCategory (category).GetEntry (position).GetBitmap (Resources);
			((ImageView) View.FindViewById (Resource.Id.image)).SetImageBitmap (mBitmap);
		}
		
		void ShareCurrentPhoto () 
		{
			File externalCacheDir = Activity.ExternalCacheDir;
			if (externalCacheDir == null) {
				Toast.MakeText (Activity, "Error writing to USB/external storage.",
					ToastLength.Short).Show ();
				return;
			}
			
			// Prevent media scanning of the cache directory.
			File noMediaFile = new File (externalCacheDir, ".nomedia");
			try {
				noMediaFile.CreateNewFile ();
			} catch (IOException e) {
			}
			
			// Write the bitmap to temporary storage in the external storage directory (e.g. SD card).
			// We perform the actual disk write operations on a separate thread using the
			// {@link AsyncTask} class, thus avoiding the possibility of stalling the main (UI) thread.
			
			File tempFile = new File (externalCacheDir, "tempfile.jpg");
			
			new AsyncTaskImpl (delegate (Java.Lang.Object [] parms) {
				/**
				* Compress and write the bitmap to disk on a separate thread.
				* @return TRUE if the write was successful, FALSE otherwise.
				*/
				try {
					var fo = System.IO.File.OpenWrite (tempFile.AbsolutePath);
					if (!mBitmap.Compress (Bitmap.CompressFormat.Jpeg, 60, fo)) {
						Toast.MakeText (Activity, "Error writing bitmap data.", ToastLength.Short).Show ();
						return false;
					}
					return true;
					
				} catch (FileNotFoundException e) {
					Toast.MakeText (Activity, "Error writing to USB/external storage.", ToastLength.Short).Show ();
					return false;
				}
			}, delegate {
				throw new System.NotImplementedException ();
			}, delegate (bool result) {
				/**
				* After doInBackground completes (either successfully or in failure), we invoke an
				* intent to share the photo. This code is run on the main (UI) thread.
				*/
				if (result != true) {
					return;
				}
					
				Intent shareIntent = new Intent (Intent.ActionSend);
				shareIntent.PutExtra (Intent.ExtraStream, Uri.FromFile (tempFile));
				shareIntent.SetType ("image/jpeg");
				StartActivity (Intent.CreateChooser (shareIntent, "Share photo"));
			}).Execute ();
		}
		
		class AsyncTaskImpl : AsyncTask<object, object, bool>
		{
			Func<Java.Lang.Object[],Java.Lang.Object> bgtask_java;
			Func<object[],bool> bgtask_managed;
			Action<bool> post_execute;

			public AsyncTaskImpl (Func<Java.Lang.Object[],Java.Lang.Object> doInBackgroundJava,
				Func<object[],bool> doInBackGroundManaged,
				Action<bool> postExecute)
			{
				bgtask_java = doInBackgroundJava;
				bgtask_managed = doInBackGroundManaged;
				post_execute = postExecute;
			}
			
			protected override Java.Lang.Object DoInBackground (params Java.Lang.Object [] native_params)
			{
				return bgtask_java (native_params);
			}
			
			protected override bool RunInBackground (params object[] parms)
			{
				return bgtask_managed (parms);
			}
			
			protected override void OnPostExecute (bool result)
			{
				post_execute (result);
			}
		}
		
		class CreateActionModeImpl : Java.Lang.Object, ActionMode.ICallback
		{
			Func<ActionMode,IMenu,bool> create;
			Func<ActionMode,IMenu,bool> prepare;
			Func<ActionMode,IMenuItem,bool> clicked;
			Action<ActionMode> destroy;
			
			public CreateActionModeImpl (Func<ActionMode,IMenu,bool> create,
				Func<ActionMode,IMenu,bool> prepare,
				Func<ActionMode,IMenuItem,bool> clicked,
				Action<ActionMode> destroy)
			{
				this.create = create;
				this.prepare = prepare;
				this.clicked = clicked;
				this.destroy = destroy;
			}

			public bool OnActionItemClicked (Android.Views.ActionMode mode, Android.Views.IMenuItem item)
			{
				return clicked (mode, item);
			}
	
			public bool OnCreateActionMode (Android.Views.ActionMode mode, Android.Views.IMenu menu)
			{
				return create (mode, menu);
			}
	
			public void OnDestroyActionMode (Android.Views.ActionMode mode)
			{
				destroy (mode);
			}
	
			public bool OnPrepareActionMode (Android.Views.ActionMode mode, Android.Views.IMenu menu)
			{
				return prepare (mode, menu);
			}
		}
			
		public ContentFragment ()
		{
			mContentSelectionActionModeCallback = new CreateActionModeImpl (delegate (ActionMode actionMode, IMenu menu) {
				actionMode.SetTitle (Resource.String.photo_selection_cab_title);
				
				MenuInflater inflater = Activity.MenuInflater;
				inflater.Inflate (Resource.Menu.photo_context_menu, menu);
				return true;
			}, delegate (ActionMode actionMode, IMenu menu) {
				return false;
			}, delegate (ActionMode actionMode, IMenuItem menuItem) {
				switch (menuItem.ItemId) {
				case Resource.Id.share:
					ShareCurrentPhoto ();
					actionMode.Finish ();
					return true;
				}
				return false;
			}, delegate (ActionMode actionMode) {
				mContentView.Selected = false;
				mCurrentActionMode = null;
			});
		}

		/**
		* The callback for the 'photo selected' {@link ActionMode}. In this action mode, we can
		* provide contextual actions for the selected photo. We currently only provide the 'share'
		* action, but we could also add clipboard functions such as cut/copy/paste here as well.
		*/
		private ActionMode.ICallback mContentSelectionActionModeCallback;
	}
}
