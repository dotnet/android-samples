/*
* Copyright (C) 2012 The Android Open Source Project
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
using CommonSampleLibrary;
using Android.Database;
using Android.Provider;
using Android.Graphics;
using Java.IO;
using System.Threading.Tasks;

namespace StorageClient
{
	public class StorageClientFragment : Android.Support.V4.App.Fragment
	{
		// A request code's purpose is to match the result of a "startActivityForResult" with
		// the type of the original request.  Choose any value.
		static readonly int READ_REQUEST_CODE = 1337;

		public static readonly String TAG = "StorageClientFragment";

		public override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);
			SetHasOptionsMenu (true);
		}

		public override bool OnOptionsItemSelected (IMenuItem item)
		{
			if (item.ItemId == Resource.Id.sample_action) {
				PerformFileSearch ();
			}
			return true;
		}

		/**
     	* Fires an intent to spin up the "file chooser" UI and select an image.
     	*/
		public void PerformFileSearch ()
		{

			// ACTION_OPEN_DOCUMENT is the intent to choose a file via the system's file browser.
			Intent intent = new Intent (Intent.ActionOpenDocument);

			// Filter to only show results that can be "opened", such as a file (as opposed to a list
			// of contacts or timezones)
			intent.AddCategory (Intent.CategoryOpenable);

			// Filter to show only images, using the image MIME data type.
			// If one wanted to search for ogg vorbis files, the type would be "audio/ogg".
			// To search for all documents available via installed storage providers, it would be
			// "*/*".
			intent.SetType ("image/*");

			StartActivityForResult (intent, READ_REQUEST_CODE);
		}

		public override void OnActivityResult (int requestCode, int resultCode, Intent data)
		{
			Log.Info(TAG, "Received an \"Activity Result\"");
			// The ACTION_OPEN_DOCUMENT intent was sent with the request code READ_REQUEST_CODE.
			// If the request code seen here doesn't match, it's the response to some other intent,
			// and the below code shouldn't run at all.

			if (requestCode == READ_REQUEST_CODE && resultCode == (int)Result.Ok) {
				// The document selected by the user won't be returned in the intent.
				// Instead, a URI to that document will be contained in the return intent
				// provided to this method as a parameter.  Pull that uri using "resultData.getData()"
				if (data != null) {
					Android.Net.Uri uri = data.Data;
					Log.Info (TAG, "Uri: " + uri.ToString ());
					ShowImage (uri);
				}
			}		
		}

		/**
     	* Given the URI of an image, shows it on the screen using a DialogFragment.
     	*/
		public void ShowImage (Android.Net.Uri uri)
		{
			if (uri != null) {
				// Since the URI is to an image, create and show a DialogFragment to display the
				// image to the user.
				Android.Support.V4.App.FragmentManager fm = Activity.SupportFragmentManager;
				var imageDialog = new ImageDialogFragment (uri);
				imageDialog.Show ((Android.Support.V4.App.FragmentManager)fm, "image_dialog");
			}
		}

		/**
    	 * DialogFragment which displays an image, given a URI.
     	*/
		class ImageDialogFragment : Android.Support.V4.App.DialogFragment
		{
			Dialog mDialog;
			Android.Net.Uri mUri;

			public ImageDialogFragment (Android.Net.Uri uri) : base ()
			{
				mUri = uri;
			}

			public override Dialog OnCreateDialog (Bundle savedInstanceState)
			{
				mDialog = base.OnCreateDialog (savedInstanceState);

				// To optimize for the "lightbox" style layout.  Since we're not actually displaying a
				// title, remove the bar along the top of the fragment where a dialog title would
				// normally go.
				mDialog.Window.RequestFeature (WindowFeatures.NoTitle);
				var imageView = new ImageView (Activity);
				mDialog.SetContentView (imageView);

				DumpImageMetaData (mUri);
				imageView.SetImageBitmap (GetBitmapFromUri (mUri));

				return mDialog;
			}

			public override void OnStop ()
			{
				base.OnStop ();
				if (Dialog!= null) {
					Dialog.Dismiss ();
				}
			}

			/** 
			 * Create a Bitmap from the URI for that image and return it.
            */
			Bitmap GetBitmapFromUri (Android.Net.Uri uri)
			{
				ParcelFileDescriptor parcelFileDescriptor = null;
				try {
					parcelFileDescriptor = Activity.ContentResolver.OpenFileDescriptor (uri, "r");
					var fileDescriptor = parcelFileDescriptor.FileDescriptor;
					var image = BitmapFactory.DecodeFileDescriptor (fileDescriptor);
					parcelFileDescriptor.Close ();
					return image;
				} catch (Java.Lang.Exception e) {
					Log.Error (TAG, "Failed to load image.", e);
					return null;
				} finally {
					try {
						if (parcelFileDescriptor != null) {
							parcelFileDescriptor.Close ();
						}
					} catch (IOException e) {
						e.PrintStackTrace ();
						Log.Error (TAG, "Error closing ParcelFile Descriptor");
					}
				}
			}

			/**
     		* Grabs metadata for a document specified by URI, logs it to the screen.
     		*/
			public void DumpImageMetaData (Android.Net.Uri uri)
			{

				// The query, since it only applies to a single document, will only return one row.
				// no need to filter, sort, or select fields, since we want all fields for one
				// document.
				ICursor cursor = Activity.ContentResolver.Query (uri, null, null, null, null, null);

				try {
					// moveToFirst() returns false if the cursor has 0 rows.  Very handy for
					// "if there's anything to look at, look at it" conditionals.
					if (cursor != null && cursor.MoveToFirst ()) {

						// Note it's called "Display Name".  This is provider-specific, and
						// might not necessarily be the file name.
						string displayName = cursor.GetString (
							cursor.GetColumnIndex (OpenableColumns.DisplayName));
						Log.Info (TAG, "Display Name: " + displayName);

						int sizeIndex = cursor.GetColumnIndex (OpenableColumns.Size);
						// If the size is unknown, the value stored is null.  But since an int can't be
						// null in java, the behavior is implementation-specific, which is just a fancy
						// term for "unpredictable".  So as a rule, check if it's null before assigning
						// to an int.  This will happen often:  The storage API allows for remote
						// files, whose size might not be locally known.
						String size = null;
						if (!cursor.IsNull (sizeIndex)) {
							// Technically the column stores an int, but cursor.getString will do the
							// conversion automatically.
							size = cursor.GetString (sizeIndex);
						} else {
							size = "Unknown";
						}
						Log.Info (TAG, "Size: " + size);
					}
				} finally {
					cursor.Close ();
				}
			}
		}
	}
}

 