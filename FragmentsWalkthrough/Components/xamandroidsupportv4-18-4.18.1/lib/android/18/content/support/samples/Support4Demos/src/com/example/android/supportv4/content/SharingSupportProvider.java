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

package com.example.android.supportv4.content;

import android.content.ContentProvider;
import android.content.ContentValues;
import android.database.Cursor;
import android.net.Uri;
import android.os.ParcelFileDescriptor;
import android.util.Log;

import java.io.File;
import java.io.FileNotFoundException;

/**
 * This simple ContentProvider provides access to the two example files shared
 * by the ShareCompat example {@link com.example.android.supportv4.app.SharingSupport}.
 */
public class SharingSupportProvider extends ContentProvider {
    public static final Uri CONTENT_URI =
            Uri.parse("content://com.example.supportv4.content.sharingsupportprovider");

    private static final String TAG = "SharingSupportProvider";

    @Override
    public boolean onCreate() {
        return true;
    }

    @Override
    public Cursor query(Uri uri, String[] projection, String selection, String[] selectionArgs,
            String sortOrder) {
        return null;
    }

    @Override
    public String getType(Uri uri) {
        if (uri.equals(Uri.withAppendedPath(CONTENT_URI, "foo.txt")) ||
                uri.equals(Uri.withAppendedPath(CONTENT_URI, "bar.txt"))) {
            return "text/plain";
        }
        return null;
    }

    @Override
    public Uri insert(Uri uri, ContentValues values) {
        return null;
    }

    @Override
    public int delete(Uri uri, String selection, String[] selectionArgs) {
        return 0;
    }

    @Override
    public int update(Uri uri, ContentValues values, String selection, String[] selectionArgs) {
        return 0;
    }

    @Override
    public ParcelFileDescriptor openFile(Uri uri, String mode) {
        String path = uri.getPath();
        if (mode.equals("r") &&
                (path.equals("/foo.txt") || path.equals("/bar.txt"))) {
            try {
                return ParcelFileDescriptor.open(
                        new File(getContext().getFilesDir() + path),
                        ParcelFileDescriptor.MODE_READ_ONLY);
            } catch (FileNotFoundException e) {
                Log.e(TAG, "Bad file " + uri);
            }
        }
        return null;
    }
}
