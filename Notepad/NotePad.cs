/*
 * Copyright (C) 2007 The Android Open Source Project
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

using Android.Provider;

namespace Mono.Samples.Notepad
{
	/**
	 * Convenience definitions for NotePadProvider
	 */
	class Notes : BaseColumns
	{
		public static String AUTHORITY = "com.google.provider.NotePad";

		// This class cannot be instantiated
		private Notes () { }
			/**
			 * The content:// style URL for this table
			 */
			public static Android.Net.Uri CONTENT_URI = Android.Net.Uri.Parse ("content://" + AUTHORITY + "/notes");

			/**
			 * The MIME type of {@link #CONTENT_URI} providing a directory of notes.
			 */
			public static String CONTENT_TYPE = "vnd.android.cursor.dir/vnd.google.note";

			/**
			 * The MIME type of a {@link #CONTENT_URI} sub-directory of a single note.
			 */
			public static String CONTENT_ITEM_TYPE = "vnd.android.cursor.item/vnd.google.note";

			/**
			 * The default sort order for this table
			 */
			public static String DEFAULT_SORT_ORDER = "modified DESC";

			/**
			 * The title of the note
			 * <P>Type: TEXT</P>
			 */
			public static String TITLE = "title";

			/**
			 * The note itself
			 * <P>Type: TEXT</P>
			 */
			public static String NOTE = "note";

			/**
			 * The timestamp for when the note was created
			 * <P>Type: INTEGER (long from System.curentTimeMillis())</P>
			 */
			public static String CREATED_DATE = "created";

			/**
			 * The timestamp for when the note was last modified
			 * <P>Type: INTEGER (long from System.curentTimeMillis())</P>
			 */
			public static String MODIFIED_DATE = "modified";
	}
}

