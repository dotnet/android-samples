/**
 * Copyright 2013 Google Inc. All Rights Reserved.
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

package com.google.android.gms.drive.sample.querying;

import com.google.android.gms.common.api.ResultCallback;
import com.google.android.gms.drive.Drive;
import com.google.android.gms.drive.DriveApi.MetadataBufferResult;
import com.google.android.gms.drive.Metadata;
import com.google.android.gms.drive.MetadataBuffer;
import com.google.android.gms.drive.query.Filters;
import com.google.android.gms.drive.query.Query;
import com.google.android.gms.drive.query.SearchableField;
import com.google.android.gms.drive.widget.DataBufferAdapter;

import android.content.Context;
import android.os.Bundle;
import android.support.v4.widget.DrawerLayout;
import android.util.Log;
import android.view.Gravity;
import android.view.MenuItem;
import android.view.View;
import android.view.ViewGroup;
import android.widget.AdapterView;
import android.widget.AdapterView.OnItemClickListener;
import android.widget.ArrayAdapter;
import android.widget.ListView;
import android.widget.TextView;
import android.widget.Toast;

/**
 * An activity that demonstrates sample queries to filter the files on the
 * currently authenticated user's Google Drive. Application is only authorized
 * to query the files it has opened or created.
 */
public class HomeActivity extends BaseDriveActivity implements
        OnItemClickListener, ResultCallback<MetadataBufferResult> {

    private static final String TAG = "HomeActivity";

    private static Query[] sQueries = new Query[] {
            // files not shared with me
            new Query.Builder().addFilter(Filters.not(
                    Filters.eq(SearchableField.MIME_TYPE, "text/plain")))
                    .build(),

            // files shared with me
            new Query.Builder().addFilter(Filters.sharedWithMe()).build(),

            // files with text/plain mimetype
            new Query.Builder().addFilter(
                    Filters.eq(SearchableField.MIME_TYPE, "text/plain"))
                    .build(),

            // files with a title containing 'a'
            new Query.Builder().addFilter(
                    Filters.contains(SearchableField.TITLE, "a")).build(),

            // files starred and with text/plain mimetype
            new Query.Builder().addFilter(Filters.and(
                    Filters.eq(SearchableField.MIME_TYPE, "text/plain"),
                    Filters.eq(SearchableField.STARRED, true))).build(),

            // files with text/plain or text/html mimetype
            new Query.Builder().addFilter(Filters.or(
                    Filters.eq(SearchableField.MIME_TYPE, "text/html"),
                    Filters.eq(SearchableField.MIME_TYPE, "text/plain")))
                    .build()
    };

    /**
     * User friendly titles for available queries.
     */
    private String[] mTitles;

    /**
     * Main drawer layout.
     */
    private DrawerLayout mMainDrawerLayout;

    /**
     * List view that displays the available queries.
     */
    private ListView mListViewQueries;

    /**
     * List view that displays the query results.
     */
    private ListView mListViewFiles;

    /**
     * Index of the selected query.
     */
    private int mSelectedIndex = 0;

    /**
     * Retrieved metadata results buffer or {@code null} if there are no results
     * retrieved yet.
     */
    private MetadataBuffer mMetadataBuffer;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_home);

        mTitles = getResources().getStringArray(R.array.titles_array);

        mListViewQueries = (ListView) findViewById(R.id.listViewQueries);
        mListViewQueries.setAdapter(
                new ArrayAdapter<String>(this, R.layout.row_query, mTitles));
        mListViewQueries.setOnItemClickListener(this);

        mListViewFiles = (ListView) findViewById(R.id.listViewFiles);
        mListViewFiles.setAdapter(new ResultsAdapter(HomeActivity.this));
        mListViewFiles.setEmptyView(findViewById(R.id.viewEmpty));

        // enable action bar for home button, so we can open
        // the left list view for navigation.
        getActionBar().setDisplayHomeAsUpEnabled(true);
        getActionBar().setHomeButtonEnabled(true);
        mMainDrawerLayout = (DrawerLayout) findViewById(R.id.drawerLayoutMain);
    }

    /**
     * Called when {@code GoogleApiClient} is connected, no querying or client
     * related actions other than disconnection should be invoked before.
     */
    @Override
    public void onConnected(Bundle connectionHint) {
        super.onConnected(connectionHint);
        refresh();
    }

    /**
     * Invokes calls to query user's Google Drive root folder's children with
     * the currently selected query.
     */
    private void refresh() {
        Drive.DriveApi.query(mGoogleApiClient, sQueries[mSelectedIndex]).setResultCallback(this);
    }

    /**
     * Called when user interacts with the action bar. Handles home clicks to
     * open the navigation drawer for the query selection list view.
     */
    @Override
    public boolean onOptionsItemSelected(MenuItem item) {
        if (item.getItemId() == android.R.id.home) {
            mMainDrawerLayout.openDrawer(Gravity.LEFT);
        }
        return super.onOptionsItemSelected(item);
    }

    /**
     * Called when user clicks on one of the items on the queries list. Keep the
     * query's index and invoke to filter the user's Drive with the selected
     * query.
     */
    @Override
    public void onItemClick(AdapterView<?> arg0, View arg1, int i, long arg3) {
        mMainDrawerLayout.closeDrawers();
        mSelectedIndex = i;
        refresh();
    }

    /**
     * Called when query has executed and a result has been retrieved. Files
     * list view should be re-rendered with the new results.
     */
    @Override
    public void onResult(MetadataBufferResult result) {
        if (!result.getStatus().isSuccess()) {
            Toast.makeText(this, R.string.msg_errorretrieval, Toast.LENGTH_SHORT).show();
            return;
        }
        Log.d(TAG, "Retrieved file count: " + result.getMetadataBuffer().getCount());
        mMetadataBuffer = result.getMetadataBuffer();
        ((ResultsAdapter) mListViewFiles.getAdapter()).notifyDataSetChanged();
    }

    /**
     * List adapter to provide data to the files list view. If there are no
     * results yet retrieved, it shows no items.
     */
    private class ResultsAdapter extends DataBufferAdapter<MetadataBuffer> {

        /**
         * Constructor.
         */
        public ResultsAdapter(Context context) {
            super(context, R.layout.row_file);
        }

        /**
         * Inflates the row view for the item at the ith position, renders it
         * with the corresponding item.
         */
        @Override
        public View getView(int i, View convertView, ViewGroup arg2) {
            if (convertView == null) {
                convertView = View.inflate(getBaseContext(), R.layout.row_file, null);
            }
            TextView titleTextView = (TextView) convertView.findViewById(R.id.textViewTitle);
            TextView descTextView = (TextView) convertView.findViewById(R.id.textViewDescription);
            Metadata metadata = mMetadataBuffer.get(i);

            titleTextView.setText(metadata.getTitle());
            descTextView.setText(metadata.getModifiedDate().toString());
            return convertView;
        }
    }
}
