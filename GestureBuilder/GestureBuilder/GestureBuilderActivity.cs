/*
 * Copyright (C) 2009 The Android Open Source Project
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

//
// C# port Coyright (C) 2012 Xamarin Inc.
//

using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.IO;

using Android.App;
using Android.Content;
using Android.Gestures;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Java.Interop;
using Java.Lang;
using Java.Util;

using Object = Java.Lang.Object;
using Path = System.IO.Path;
using Environment = Android.OS.Environment;

[assembly:UsesPermission (Android.Manifest.Permission.WriteExternalStorage)]

namespace GestureBuilder
{
	[Activity (Label = "@string/application_name",
               Icon="@drawable/icon")]
	[IntentFilter (new string [] {"android.intent.action.MAIN"}, Categories = new string [] {
		Intent.CategoryLauncher, Intent.CategoryDefault})]
	public class GestureBuilderActivity : ListActivity
	{
		internal enum Status {
			Success,
			Cancelled,
			NoStorage,
			NotLoaded
		}
		const int MenuIdRename = 1;
		const int MenuIdRemove = 2;
		const int DialogRenameGesture = 1;
		const int RequestNewGesture = 1;
    
		// Type: long (id)
		const string GESTURES_INFO_ID = "gestures.info_id";

		readonly string mStoreFile = Path.Combine (Environment.ExternalStorageDirectory.AbsolutePath, "gestures");

		class Comparator<T> : Object, IComparator where T : Object
		{
			Comparison<T> cmp;
			public Comparator (Comparison<T> cmp)
			{
				this.cmp = cmp;
			}

			public int Compare (Object object1, Object object2)
			{
				return cmp ((T) object1, (T) object2);
			}

			public override bool Equals (Object obj)
			{
				var o = obj as Comparator<T>;
				return o != null && cmp == o.cmp;
			}
		}
    
		Comparator<NamedGesture> mSorter = new Comparator<NamedGesture> ((o1, o2) => o1.Name.CompareTo (o2.Name));

		static GestureLibrary sStore;

		GesturesAdapter mAdapter;
		GesturesLoadTask mTask;
		TextView mEmpty;

		Dialog mRenameDialog;
		EditText mInput;
		NamedGesture mCurrentRenameGesture;

		protected override void OnCreate (Bundle savedInstanceState) 
		{
			base.OnCreate (savedInstanceState);
			SetContentView (Resource.Layout.gestures_list);

			mAdapter = new GesturesAdapter (this);
			ListAdapter = mAdapter;

			if (sStore == null)
				sStore = GestureLibraries.FromFile (mStoreFile);
			mEmpty = (TextView) FindViewById (Android.Resource.Id.Empty);
			LoadGestures ();

			RegisterForContextMenu (ListView);
		}

		internal static GestureLibrary Store {
			get { return sStore; }
		}
		
		[Export ("reloadGestures")]
		public void ReloadGestures (View v) 
		{
			LoadGestures ();
		}
    
		[Export ("addGesture")]
		public void AddGesture (View v) 
		{
			Intent intent = new Intent (this, typeof (CreateGestureActivity));
			StartActivityForResult (intent, RequestNewGesture);
		}

		protected override void OnActivityResult (int requestCode, Result resultCode, Intent data) 
		{
			base.OnActivityResult (requestCode, resultCode, data);
        
			if (resultCode == Result.Ok) {
				switch (requestCode) {
				case RequestNewGesture:
					LoadGestures ();
					break;
				}
			}
		}

		void LoadGestures ()
		{
			if (mTask != null && mTask.GetStatus () != AsyncTask.Status.Finished)
				mTask.Cancel (true);
			mTask = (GesturesLoadTask) new GesturesLoadTask (this).Execute ();
		}

		protected override void OnDestroy ()
		{
			base.OnDestroy ();

			if (mTask != null && mTask.GetStatus () != AsyncTask.Status.Finished) {
				mTask.Cancel (true);
				mTask = null;
			}

			CleanupRenameDialog ();
		}

		void CheckForEmpty () 
		{
			if (mAdapter.Count == 0)
				mEmpty.SetText (Resource.String.gestures_empty);
		}
	
		protected override void OnSaveInstanceState (Bundle outState) 
		{
			base.OnSaveInstanceState (outState);
	
			if (mCurrentRenameGesture != null)
				outState.PutLong (GESTURES_INFO_ID, mCurrentRenameGesture.Gesture.ID);
		}
	
		protected override void OnRestoreInstanceState (Bundle state) 
		{
			base.OnRestoreInstanceState (state);
	
			long id = state.GetLong (GESTURES_INFO_ID, -1);
			if (id != -1) {
				var entries = sStore.GestureEntries;
				bool breakOut = false;
				foreach (string name in entries) {
					if (breakOut)
						break;
					foreach (Gesture gesture in sStore.GetGestures (name)) {
						if (gesture.ID == id) {
							mCurrentRenameGesture = new NamedGesture ();
							mCurrentRenameGesture.Name = name;
							mCurrentRenameGesture.Gesture = gesture;
							breakOut = true;
							break;
						}
					}
				}
			}
		}

		public override void OnCreateContextMenu (IContextMenu menu, View v, IContextMenuContextMenuInfo menuInfo) 
		{

			base.OnCreateContextMenu (menu, v, menuInfo);

			var info = (AdapterView.AdapterContextMenuInfo) menuInfo;
			menu.SetHeaderTitle (((TextView) info.TargetView).Text);

			menu.Add (0, MenuIdRename, 0, Resource.String.gestures_rename);
			menu.Add (0, MenuIdRemove, 0, Resource.String.gestures_delete);
		}

		public override bool OnContextItemSelected (IMenuItem item) 
		{
			var menuInfo = (AdapterView.AdapterContextMenuInfo) item.MenuInfo;
			NamedGesture gesture = (NamedGesture) (object) menuInfo.TargetView.Tag;

			switch (item.ItemId) {
			case MenuIdRename:
				RenameGesture (gesture);
				return true;
			case MenuIdRemove:
				DeleteGesture (gesture);
				return true;
			}

			return base.OnContextItemSelected (item);
		}

		void RenameGesture (NamedGesture gesture) 
		{
			mCurrentRenameGesture = gesture;
			ShowDialog (DialogRenameGesture);
		}


		protected override Dialog OnCreateDialog (int id) 
		{
			if (id == DialogRenameGesture)
				return CreateRenameDialog ();
			return base.OnCreateDialog (id);
		}

		protected override void OnPrepareDialog (int id, Dialog dialog) 
		{
			base.OnPrepareDialog (id, dialog);
			if (id == DialogRenameGesture)
				mInput.Text = mCurrentRenameGesture.Name;
		}
		
		class DialogCancelListener : Object, IDialogInterfaceOnCancelListener
		{
			public DialogCancelListener (Action cleanup)
			{
				this.cleanup = cleanup;
			}
			Action cleanup;
			
			public void OnCancel (IDialogInterface dialog)
			{
				cleanup ();
			}
		}

		private Dialog CreateRenameDialog () 
		{
			View layout = View.Inflate (this, Resource.Layout.dialog_rename, null);
			mInput = (EditText) layout.FindViewById (Resource.Id.name);
			((TextView) layout.FindViewById (Resource.Id.label)).SetText (Resource.String.gestures_rename_label);

			AlertDialog.Builder builder = new AlertDialog.Builder (this);
			builder.SetIcon (0);
			builder.SetTitle (GetString (Resource.String.gestures_rename_title));
			builder.SetCancelable (true);
			builder.SetOnCancelListener (new DialogCancelListener (CleanupRenameDialog));
			builder.SetNegativeButton (GetString (Resource.String.cancel_action), delegate { CleanupRenameDialog (); });
			builder.SetPositiveButton (GetString (Resource.String.rename_action), delegate { ChangeGestureName (); });
			builder.SetView (layout);
			return builder.Create ();
		}

		void ChangeGestureName () 
		{
			string name = mInput.Text.ToString ();
			if (!string.IsNullOrEmpty (name)) {
				NamedGesture renameGesture = mCurrentRenameGesture;
				GesturesAdapter adapter = mAdapter;
				int count = adapter.Count;

				// Simple linear search, there should not be enough items to warrant
				// a more sophisticated search
				for (int i = 0; i < count; i++) {
					NamedGesture gesture = adapter.GetItem (i);
					if (gesture.Gesture.ID == renameGesture.Gesture.ID) {
						sStore.RemoveGesture (gesture.Name, gesture.Gesture);
						gesture.Name = mInput.Text.ToString ();
						sStore.AddGesture (gesture.Name, gesture.Gesture);
						break;
					}
				}

				adapter.NotifyDataSetChanged ();
			}
			mCurrentRenameGesture = null;
		}

		internal void CleanupRenameDialog () 
		{
			if (mRenameDialog != null) {
				mRenameDialog.Dismiss ();
				mRenameDialog = null;
			}
			mCurrentRenameGesture = null;
		}

		internal void DeleteGesture (NamedGesture gesture) 
		{
			sStore.RemoveGesture (gesture.Name, gesture.Gesture);
			sStore.Save ();

			GesturesAdapter adapter = mAdapter;
			adapter.SetNotifyOnChange (false);
			adapter.Remove (gesture);
			adapter.Sort (mSorter);
			CheckForEmpty ();
			adapter.NotifyDataSetChanged ();

			Toast.MakeText (this, Resource.String.gestures_delete_success, ToastLength.Short).Show ();
		}

		class GesturesLoadTask : AsyncTask<Object, NamedGesture, int> {
			int mThumbnailSize;
			int mThumbnailInset;
			Color mPathColor;
			
			GestureBuilderActivity parent;
			public GesturesLoadTask (GestureBuilderActivity parent)
			{
				this.parent = parent;
			}

			protected override void OnPreExecute () 
			{
				base.OnPreExecute ();
				
				mPathColor = parent.Resources.GetColor (Resource.Color.gesture_color);
				mThumbnailInset = (int) parent.Resources.GetDimension (Resource.Dimension.gesture_thumbnail_inset);
				mThumbnailSize = (int) parent.Resources.GetDimension (Resource.Dimension.gesture_thumbnail_size);
				
				parent.FindViewById (Resource.Id.addButton).Enabled = false;
				parent.FindViewById (Resource.Id.reloadButton).Enabled = false;
				
				parent.mAdapter.SetNotifyOnChange (false);
				parent.mAdapter.Clear ();
			}
			
			protected override int RunInBackground (params Object [] parms) {
				if (IsCancelled) return (int) GestureBuilderActivity.Status.Cancelled;
				if (!Environment.MediaMounted.Equals (Environment.ExternalStorageState))
					return (int) GestureBuilderActivity.Status.NoStorage;

				GestureLibrary store = sStore;

				if (store.Load ()) {
					foreach (string name in store.GestureEntries) {
						if (IsCancelled) break;

						foreach (Gesture gesture in store.GetGestures (name)) {
							Bitmap bitmap = gesture.ToBitmap (mThumbnailSize, mThumbnailSize,
                                mThumbnailInset, mPathColor);
							NamedGesture namedGesture = new NamedGesture ();
							namedGesture.Gesture = gesture;
							namedGesture.Name = name;

							parent.mAdapter.AddBitmap (namedGesture.Gesture.ID, bitmap);
							PublishProgress (namedGesture);
						}
					}

					return (int) GestureBuilderActivity.Status.Success;
				}
				
				return (int) GestureBuilderActivity.Status.NotLoaded;
			}


			protected override void OnProgressUpdate (params NamedGesture [] values) 
			{
				base.OnProgressUpdate (values);

				GesturesAdapter adapter = parent.mAdapter;
				adapter.SetNotifyOnChange (false);

				foreach (NamedGesture gesture in values)
					adapter.Add (gesture);

				adapter.Sort (parent.mSorter);
				adapter.NotifyDataSetChanged ();
			}


			protected override void OnPostExecute (int result) 
			{
				base.OnPostExecute (result);

				if (result == (int) GestureBuilderActivity.Status.NoStorage) {
					parent.ListView.Visibility = ViewStates.Gone;
					parent.mEmpty.Visibility = ViewStates.Visible;
					parent.mEmpty.Text = (parent.GetString (Resource.String.gestures_error_loading,
					                          parent.mStoreFile));
				} else {
					parent.FindViewById (Resource.Id.addButton).Enabled = true;
					parent.FindViewById (Resource.Id.reloadButton).Enabled = true;
					parent.CheckForEmpty ();
				}
			}
		}

		internal class NamedGesture : Object
		{
			public string Name;
			public Gesture Gesture;
		}

		internal class GesturesAdapter : ArrayAdapter<NamedGesture> 
		{
			LayoutInflater mInflater;
			IDictionary<long, Drawable> mThumbnails = new ConcurrentDictionary<long, Drawable> ();

			public GesturesAdapter (Context context) : base (context, 0)
			{
				mInflater = (LayoutInflater) context.GetSystemService (Context.LayoutInflaterService);
			}

			internal void AddBitmap (long id, Bitmap bitmap) 
			{
				mThumbnails [id] = new BitmapDrawable (bitmap);
			}

			public override View GetView (int position, View convertView, ViewGroup parent) 
			{
				if (convertView == null)
					convertView = mInflater.Inflate (Resource.Layout.gestures_item, parent, false);

				NamedGesture gesture = GetItem (position);
				TextView label = (TextView) convertView;

				label.Tag = gesture;
				label.Text = gesture.Name;
				label.SetCompoundDrawablesWithIntrinsicBounds (mThumbnails [gesture.Gesture.ID], null, null, null);

				return convertView;
			}
		}
	}
}
