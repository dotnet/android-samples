/**
 * Copyright 2013 Google Inc. All Rights Reserved.
 *
 *  Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except
 * in compliance with the License. You may obtain a copy of the License at
 *
 *  http://www.apache.org/licenses/LICENSE-2.0
 *
 *  Unless required by applicable law or agreed to in writing, software distributed under the
 * License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either
 * express or implied. See the License for the specific language governing permissions and
 * limitations under the License.
 */

package com.google.android.gms.drive.sample.demo;

import android.app.Activity;
import android.content.Intent;
import android.os.Bundle;
import android.view.View;
import android.widget.AdapterView;
import android.widget.AdapterView.OnItemClickListener;
import android.widget.ArrayAdapter;
import android.widget.ListView;

import com.google.android.gms.drive.sample.demo.events.ListenChangeEventsForFilesActivity;

/**
 * An activity to list all available demo activities.
 */
public class HomeActivity extends Activity {

    @SuppressWarnings("rawtypes")
    private final Class[] sActivities = new Class[] {
            ListFilesActivity.class,
            QueryFilesActivity.class,
            CreateFileActivity.class,
            CreateFileInAppFolderActivity.class,
            CreateFolderActivity.class,
            RetrieveMetadataActivity.class,
            RetrieveContentsActivity.class,
            RetrieveContentsWithProgressDialogActivity.class,
            EditMetadataActivity.class,
            EditContentsActivity.class,
            PinFileActivity.class,
            SyncRequestsActivity.class,
            CreateFileWithCreatorActivity.class,
            PickFileWithOpenerActivity.class,
            PickFolderWithOpenerActivity.class,
            CreateFileInFolderActivity.class,
            CreateFolderInFolderActivity.class,
            ListFilesInFolderActivity.class,
            QueryFilesInFolderActivity.class,
            ListenChangeEventsForFilesActivity.class
    };

    private ListView mListViewSamples;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_home);

        String[] titles = getResources().getStringArray(R.array.titles_array);
        mListViewSamples = (ListView) findViewById(R.id.listViewSamples);
        mListViewSamples.setAdapter(
                new ArrayAdapter<String>(this, android.R.layout.simple_list_item_1, titles));
        mListViewSamples.setOnItemClickListener(new OnItemClickListener() {

                @Override
            public void onItemClick(AdapterView<?> arg0, View arg1, int i, long arg3) {
                Intent intent = new Intent(getBaseContext(), sActivities[i]);
                startActivity(intent);
            }
        });
    }

}
