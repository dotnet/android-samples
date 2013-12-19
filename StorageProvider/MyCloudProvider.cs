/*
* Copyright 2013 The Android Open Source Project
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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.Database;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Provider;
using Java.IO;
using CommonSampleLibrary;
using Android.Webkit;

namespace StorageProvider
{
	/**
 	* Manages documents and exposes them to the Android system for sharing.
 	*/
	public class MyCloudProvider : DocumentsProvider
	{
		static readonly string TAG = typeof (MyCloudProvider).Name;

		// Use these as the default columns to return information about a root if no specific
		// columns are requested in a query.
		static readonly string[] DEFAULT_ROOT_PROJECTION = new String[] {
			DocumentsContract.Root.ColumnRootId,
			DocumentsContract.Root.ColumnMimeTypes,
			DocumentsContract.Root.ColumnFlags,
			DocumentsContract.Root.ColumnIcon,
			DocumentsContract.Root.ColumnTitle,
			DocumentsContract.Root.ColumnSummary,
			DocumentsContract.Root.ColumnDocumentId,
			DocumentsContract.Root.ColumnAvailableBytes
		};

		// Use these as the default columns to return information about a document if no specific
		// columns are requested in a query.
		static readonly String[] DEFAULT_DOCUMENT_PROJECTION = new String[] {
			DocumentsContract.Document.ColumnDocumentId,
			DocumentsContract.Document.ColumnMimeType,
			DocumentsContract.Document.ColumnDisplayName,
			DocumentsContract.Document.ColumnLastModified,
			DocumentsContract.Document.ColumnFlags,
			DocumentsContract.Document.ColumnSize
		};

		// No official policy on how many to return, but make sure you do limit the number of recent
		// and search results.
		static readonly int MAX_SEARCH_RESULTS = 20;
		static readonly int MAX_LAST_MODIFIED = 5;

		static readonly string ROOT = "root";

		// A file object at the root of the file hierarchy.  Depending on your implementation, the root
		// does not need to be an existing file system directory.  For example, a tag-based document
		// provider might return a directory containing all tags, represented as child directories.
		File mBaseDir;

		public override bool OnCreate ()
		{
			Log.Verbose (TAG, "onCreate");

			mBaseDir = Context.FilesDir;

			WriteDummyFilesToStorage ();

			return true;
		}

		public override ICursor QueryRoots (string[] projection)
		{
			Log.Verbose (TAG, "queryRoots");

			// Create a cursor with either the requested fields, or the default projection.  This
			// cursor is returned to the Android system picker UI and used to display all roots from
			// this provider.
			var result = new MatrixCursor (ResolveRootProjection (projection));

			// If user is not logged in, return an empty root cursor.  This removes our provider from
			// the list entirely.
			if (!IsUserLoggedIn ()) {
				return result;
			}

			// It's possible to have multiple roots (e.g. for multiple accounts in the same app) -
			// just add multiple cursor rows.
			// Construct one row for a root called "MyCloud".
			MatrixCursor.RowBuilder row = result.NewRow ();

			row.Add (DocumentsContract.Root.ColumnRootId, ROOT);
			row.Add (DocumentsContract.Root.ColumnSummary, Context.GetString (Resource.String.root_summary));

			// FLAG_SUPPORTS_CREATE means at least one directory under the root supports creating
			// documents.  FLAG_SUPPORTS_RECENTS means your application's most recently used
			// documents will show up in the "Recents" category.  FLAG_SUPPORTS_SEARCH allows users
			// to search all documents the application shares.
			row.Add (DocumentsContract.Root.ColumnFlags, (int)DocumentRootFlags.SupportsCreate |
				(int)DocumentRootFlags.SupportsRecents | (int)DocumentRootFlags.SupportsSearch);

			// COLUMN_TITLE is the root title (e.g. what will be displayed to identify your provider).
			row.Add (DocumentsContract.Root.ColumnTitle, Context.GetString (Resource.String.app_name));

			// This document id must be unique within this provider and consistent across time.  The
			// system picker UI may save it and refer to it later.
			row.Add (DocumentsContract.Root.ColumnDocumentId, GetDocIdForFile (mBaseDir));

			// The child MIME types are used to filter the roots and only present to the user roots
			// that contain the desired type somewhere in their file hierarchy.
			row.Add (DocumentsContract.Root.ColumnMimeTypes, GetChildMimeTypes (mBaseDir));
			row.Add (DocumentsContract.Root.ColumnAvailableBytes, mBaseDir.FreeSpace);
			row.Add (DocumentsContract.Root.ColumnIcon, Resource.Drawable.icon);

			return result;
		}

		public override ICursor QueryRecentDocuments (string rootId, string[] projection)
		{
			Log.Verbose (TAG, "queryRecentDocuments");

			// This example implementation walks a local file structure to find the most recently
			// modified files.  Other implementations might include making a network call to query a
			// server.

			// Create a cursor with the requested projection, or the default projection.
			var result = new MatrixCursor (ResolveDocumentProjection (projection));

			File parent = GetFileForDocId (rootId);

			// Create a list to store the most recent documents, which orders by last modified.
			var lastModifiedFiles = new List<File> (); 

			// Iterate through all files and directories in the file structure under the root.  If
			// the file is more recent than the least recently modified, add it to the queue,
			// limiting the number of results.
			var pending = new List<File> ();

			// Start by adding the parent to the list of files to be processed
			pending.Add (parent);

			// Do while we still have unexamined files
			while (pending.Any ()) {
				// Take a file from the front of the list of unprocessed files
				File file = pending [0];
				pending.RemoveAt (0);
				if (file.IsDirectory) {
					// If it's a directory, add all its children to the unprocessed list
					pending.AddRange (file.ListFiles ().AsEnumerable ());
				} else {
					// If it's a file, add it to the ordered list.
					lastModifiedFiles.Add (file);
				}
			}

			// Sort our list by last modified.
			lastModifiedFiles.Sort (new Comparison<File> (delegate (File i, File j) {
				return (i.LastModified().CompareTo (j.LastModified ()));
			}));

			// Add the most recent files to the cursor, not exceeding the max number of results.
			for (int i = 0; i < Math.Min (MAX_LAST_MODIFIED + 1, lastModifiedFiles.Count); i++) {
				File file = lastModifiedFiles[0];
				lastModifiedFiles.RemoveAt (0);
				IncludeFile (result, null, file);
			}

			return result;
		}

		public override ICursor QuerySearchDocuments (string rootId, string query, string[] projection)
		{
			// Create a cursor with the requested projection, or the default projection.
			var result = new MatrixCursor (ResolveDocumentProjection (projection));
			File parent = GetFileForDocId (rootId);

			// This example implementation searches file names for the query and doesn't rank search
			// results, so we can stop as soon as we find a sufficient number of matches.  Other
			// implementations might use other data about files, rather than the file name, to
			// produce a match; it might also require a network call to query a remote server.

			// Iterate through all files in the file structure under the root until we reach the
			// desired number of matches.
			var pending = new List<File>();

			// Start by adding the parent to the list of files to be processed
			pending.Add (parent);

			// Do while we still have unexamined files, and fewer than the max search results
			while (pending.Any () && result.Count < MAX_SEARCH_RESULTS) {
				// Take a file from the list of unprocessed files
				File file = pending [0];
				pending.RemoveAt (0);
				if (file.IsDirectory) {
					// If it's a directory, add all its children to the unprocessed list
					pending.AddRange (file.ListFiles ().AsEnumerable ());
				} else {
					// If it's a file and it matches, add it to the result cursor.
					if (file.Name.ToLower().Contains (query)) {
						IncludeFile (result, null, file);
					}
				}
			}
			return result;
		}

		public override AssetFileDescriptor OpenDocumentThumbnail (string documentId, Android.Graphics.Point sizeHint, CancellationSignal signal)
		{
			Log.Verbose (TAG, "openDocumentThumbnail");

			File file = GetFileForDocId (documentId);
			var pfd = ParcelFileDescriptor.Open (file, ParcelFileMode.ReadOnly);
			return new AssetFileDescriptor (pfd, 0, AssetFileDescriptor.UnknownLength);
		}

		public override ICursor QueryDocument (string documentId, string[] projection)
		{
			Log.Verbose (TAG, "queryDocument");

			// Create a cursor with the requested projection, or the default projection.
			var result = new MatrixCursor (ResolveDocumentProjection (projection));
			IncludeFile (result, documentId, null);
			return result;
		}

		public override ICursor QueryChildDocuments (string parentDocumentId, string[] projection, string sortOrder)
		{
			Log.Verbose (TAG, "queryChildDocuments, parentDocumentId: " + parentDocumentId + " sortOrder: " + sortOrder);

			var result = new MatrixCursor (ResolveDocumentProjection (projection));
			File parent = GetFileForDocId (parentDocumentId);
			foreach (File file in parent.ListFiles ()) {
				IncludeFile (result, null, file);
			}
			return result;
		}

		public override ParcelFileDescriptor OpenDocument (string documentId, string mode, CancellationSignal signal)
		{
			Log.Verbose (TAG, "openDocument, mode: " + mode);
			// It's OK to do network operations in this method to download the document, as long as you
			// periodically check the CancellationSignal.  If you have an extremely large file to
			// transfer from the network, a better solution may be pipes or sockets
			// (see ParcelFileDescriptor for helper methods).

			File file = GetFileForDocId (documentId);
			var accessMode = ParcelFileDescriptor.ParseMode (mode);

			bool isWrite = (mode.IndexOf ('w') != -1);
			if (isWrite) {
				// Attach a close listener if the document is opened in write mode.
				try {
					var handler = new Handler (Context.MainLooper);
					return ParcelFileDescriptor.Open (file, accessMode, handler, new MyOnCloseListener (documentId));
				} catch (IOException) {
					throw new FileNotFoundException ("Failed to open document with id " + documentId + " and mode " + mode);
				}
			} else {
				return ParcelFileDescriptor.Open (file, accessMode);
			}
		}

		class MyOnCloseListener : Java.Lang.Object, ParcelFileDescriptor.IOnCloseListener
		{
			string documentID;

			public MyOnCloseListener (string documentId)
			{
				documentID = documentId;
			}

			public void OnClose (Java.IO.IOException e) 
			{
				// Update the file with the cloud server.  The client is done writing.
				Log.Info (TAG, "A file with id " + documentID + " has been closed! Time to update the server.");
			}
		}

		public override string CreateDocument (string parentDocumentId, string mimeType, string displayName)
		{
			Log.Verbose(TAG, "createDocument");

			File parent = GetFileForDocId (parentDocumentId);
			var file = new File (parent, displayName);
			try {
				file.CreateNewFile ();
				file.SetWritable (true);
				file.SetReadable (true);
			} catch (IOException) {
				throw new FileNotFoundException ("Failed to create document with name " +
					displayName +" and documentId " + parentDocumentId);
			}
			return GetDocIdForFile (file);
		}

		public override void DeleteDocument (string documentId)
		{
			Log.Verbose (TAG, "deleteDocument");
			File file = GetFileForDocId (documentId);
			if (file.Delete ()) {
				Log.Info (TAG, "Deleted file with id " + documentId);
			} else {
				throw new FileNotFoundException ("Failed to delete document with id " + documentId);
			}
		}

		public override string GetDocumentType (string documentId)
		{
			File file = GetFileForDocId (documentId);
			return GetTypeForFile (file);
		}

		/**
     	* @param projection the requested root column projection
     	* @return either the requested root column projection, or the default projection if the
     	* requested projection is null.
     	*/
		static String[] ResolveRootProjection (String[] projection)
		{
			return projection != null ? projection : DEFAULT_ROOT_PROJECTION;
		}

		static String[] ResolveDocumentProjection (String[] projection)
		{
			return projection != null ? projection : DEFAULT_DOCUMENT_PROJECTION;
		}

		/**
     	* Get a file's MIME type
     	*
     	* @param file the File object whose type we want
     	* @return the MIME type of the file
     	*/
		static string GetTypeForFile (File file)
		{
			if (file.IsDirectory) {
				return  DocumentsContract.Document.MimeTypeDir;
			} else {
				return GetTypeForName (file.Name);
			}
		}

		/**
     	* Get the MIME data type of a document, given its filename.
     	*
     	* @param name the filename of the document
     	* @return the MIME data type of a document
     	*/
		static string GetTypeForName (string name)
		{
			int lastDot = name.LastIndexOf ('.');
			if (lastDot >= 0) {
				string extension = name.Substring (lastDot + 1);
				string mime = MimeTypeMap.Singleton.GetMimeTypeFromExtension (extension);
				if (mime != null) {
					return mime;
				}
			}
			return "application/octet-stream";
		}
			
		/**
     	* Gets a string of unique MIME data types a directory supports, separated by newlines.  This
     	* should not change.
     	*
     	* @param parent the File for the parent directory
     	* @return a string of the unique MIME data types the parent directory supports
     	*/
		string GetChildMimeTypes (File parent)
		{
			return "image/*\n" + "text/*\n"
				+ "application/vnd.openxmlformats-officedocument.wordprocessingml.document\n";
		}

		/**
     	* Get the document ID given a File.  The document id must be consistent across time.  Other
     	* applications may save the ID and use it to reference documents later.
     	* <p/>
     	* This implementation is specific to this demo.  It assumes only one root and is built
     	* directly from the file structure.  However, it is possible for a document to be a child of
     	* multiple directories (for example "android" and "images"), in which case the file must have
     	* the same consistent, unique document ID in both cases.
     	*
     	* @param file the File whose document ID you want
     	* @return the corresponding document ID
     	*/
		string GetDocIdForFile (File file)
		{
			string path = file.AbsolutePath;

			// Start at first char of path under root
			string rootPath = mBaseDir.Path;
			if (rootPath == path) {
				path = "";
			} else if (rootPath.EndsWith ("/")) {
				path = path.Substring (rootPath.Length);
			} else {
				path = path.Substring (rootPath.Length + 1);
			}

			return "root" + ':' + path;
		}

		/**
     	* Add a representation of a file to a cursor.
     	*
     	* @param result the cursor to modify
     	* @param docId  the document ID representing the desired file (may be null if given file)
     	* @param file   the File object representing the desired file (may be null if given docID)
     	*/
		void IncludeFile (MatrixCursor result, string docId, File file)
		{
			if (docId == null) {
				docId = GetDocIdForFile (file);
			} else {
				file = GetFileForDocId (docId);
			}

			DocumentContractFlags flags = (DocumentContractFlags)0;

			if (file.IsDirectory) {
				// Request the folder to lay out as a grid rather than a list. This also allows a larger
				// thumbnail to be displayed for each image.
				//            flags |= Document.FLAG_DIR_PREFERS_GRID;

				// Add FLAG_DIR_SUPPORTS_CREATE if the file is a writable directory.
				if (file.IsDirectory && file.CanWrite ()) {
					flags |=  DocumentContractFlags.DirSupportsCreate;
				}
			} else if (file.CanWrite ()) {
				// If the file is writable set FLAG_SUPPORTS_WRITE and
				// FLAG_SUPPORTS_DELETE
				flags |= DocumentContractFlags.SupportsWrite;
				flags |= DocumentContractFlags.SupportsDelete;
			}

			string displayName = file.Name;
			string mimeType = GetTypeForFile (file);

			if (mimeType.StartsWith ("image/")) {
				// Allow the image to be represented by a thumbnail rather than an icon
				flags |= DocumentContractFlags.SupportsThumbnail;
			}

			MatrixCursor.RowBuilder row = result.NewRow ();
			row.Add (DocumentsContract.Document.ColumnDocumentId, docId);
			row.Add (DocumentsContract.Document.ColumnDisplayName, displayName);
			row.Add (DocumentsContract.Document.ColumnSize, file.Length ());
			row.Add (DocumentsContract.Document.ColumnMimeType, mimeType);
			row.Add (DocumentsContract.Document.ColumnLastModified, file.LastModified ());
			row.Add (DocumentsContract.Document.ColumnFlags, (int)flags);

			// Add a custom icon
			row.Add (DocumentsContract.Document.ColumnIcon, Resource.Drawable.icon);
		}

		/**
     	* Translate your custom URI scheme into a File object.
     	*
     	* @param docId the document ID representing the desired file
     	* @return a File represented by the given document ID
     	*/
		File GetFileForDocId (String docId)
		{
			File target = mBaseDir;
			if (docId == ROOT) {
				return target;
			}

			int splitIndex = docId.IndexOf (':', 1);
			if (splitIndex < 0) {
				throw new FileNotFoundException ("Missing root for " + docId);
			} else {
				string path = docId.Substring (splitIndex + 1);
				target = new File (target, path);
				if (!target.Exists ()) {
					throw new FileNotFoundException ("Missing file for " + docId + " at " + target);
				}
				return target;
			}
		}


		/**
     	* Preload sample files packaged in the apk into the internal storage directory.  This is a
     	* dummy function specific to this demo.  The MyCloud mock cloud service doesn't actually
     	* have a backend, so it simulates by reading content from the device's internal storage.
     	*/
		void WriteDummyFilesToStorage ()
		{
			if (mBaseDir.List().Length > 0) {
				return;
			}

			int[] imageResIds = GetResourceIdArray (Resource.Array.image_res_ids);
			foreach (int resId in imageResIds) {
				WriteFileToInternalStorage (resId, ".jpeg");
			}

			int[] textResIds = GetResourceIdArray (Resource.Array.text_res_ids);
			foreach (int resId in textResIds) {
				WriteFileToInternalStorage (resId, ".txt");
			}

			int[] docxResIds = GetResourceIdArray (Resource.Array.docx_res_ids);
			foreach (int resId in docxResIds) {
				WriteFileToInternalStorage (resId, ".docx");
			}
		}

		/**
     	* Write a file to internal storage.  Used to set up our dummy "cloud server".
     	*
     	* @param resId     the resource ID of the file to write to internal storage
     	* @param extension the file extension (ex. .png, .mp3)
     	*/
		void WriteFileToInternalStorage (int resId, String extension)
		{
			var ins = Context.Resources.OpenRawResource (resId);
			var outputStream = new ByteArrayOutputStream ();
			int size;
			byte[] buffer = new byte [1024];
			try {
				while ((size = ins.Read (buffer, 0, 1024)) >= 0) {
					outputStream.Write (buffer, 0, size);
				}
				ins.Close ();
				buffer = outputStream.ToByteArray ();
				String filename = Context.Resources.GetResourceEntryName (resId) + extension;
				var fos = Context.OpenFileOutput (filename, FileCreationMode.Private);
				fos.Write (buffer, 0, buffer.Length);
				fos.Close ();

			} catch (IOException e) {
				e.PrintStackTrace ();
			}
		}

		int[] GetResourceIdArray (int arrayResId)
		{
			TypedArray ar = Context.Resources.ObtainTypedArray (arrayResId);
			int len = ar.Length ();
			int[] resIds = new int[len];
			for (int i = 0; i < len; i++) {
				resIds[i] = ar.GetResourceId (i, 0);
			}
			ar.Recycle ();
			return resIds;
		}

		/**
     	* Dummy function to determine whether the user is logged in.
     	*/
		bool IsUserLoggedIn ()
		{
			ISharedPreferences sharedPreferences = 
				Context.GetSharedPreferences (Context.GetString (Resource.String.app_name),
					FileCreationMode.Private);

			return sharedPreferences.GetBoolean (Context.GetString (Resource.String.key_logged_in), false);
		}


	}
}

