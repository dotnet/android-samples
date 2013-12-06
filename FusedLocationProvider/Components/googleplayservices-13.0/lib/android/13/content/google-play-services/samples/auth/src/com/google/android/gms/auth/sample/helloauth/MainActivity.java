/*
 * Copyright 2012 The Android Open Source Project
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

package com.google.android.gms.auth.sample.helloauth;

import android.app.ListActivity;
import android.content.Intent;
import android.os.Bundle;
import android.view.View;
import android.widget.ArrayAdapter;
import android.widget.ListView;

import com.google.android.gms.auth.sample.helloauth.R;

public class MainActivity extends ListActivity {

    @Override
    public void onCreate(Bundle icicle) {
        super.onCreate(icicle);
        String[] items = getResources().getStringArray(R.array.main_activity_items);
        setTitle(getPackageName());
        this.setListAdapter(
                new ArrayAdapter<String>(this, android.R.layout.simple_list_item_1, items));
    }

    @Override
    protected void onListItemClick(ListView l, View v, int position, long id) {
        // Highly coupled with the order of contents in main_activity_items
        Intent intent = new Intent(this, HelloActivity.class);
        if (position == 0) {
            intent.putExtra(HelloActivity.TYPE_KEY, HelloActivity.Type.FOREGROUND.name());
        } else if (position == 1) {
            intent.putExtra(HelloActivity.TYPE_KEY, HelloActivity.Type.BACKGROUND.name());
        } else if (position == 2) {
            intent.putExtra(HelloActivity.TYPE_KEY, HelloActivity.Type.BACKGROUND_WITH_SYNC.name());
        }
        startActivity(intent);
    }
}
