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

package com.google.android.gms.samples.plus;

import android.app.ListActivity;
import android.content.Intent;
import android.os.Bundle;
import android.view.Menu;
import android.view.MenuItem;
import android.view.View;
import android.widget.ListView;
import android.widget.SimpleAdapter;

import java.util.ArrayList;
import java.util.HashMap;
import java.util.LinkedHashMap;
import java.util.List;
import java.util.Map;

/**
 * Displays examples of integrating with the Google+ Platform for Android.
 */
public class PlusSampleActivity extends ListActivity {
    private static final String FROM_TITLE = "title";
    private static final String TITLE_KEY = "title";
    private static final String INTENT_KEY = "intent";

    private static final Map<String, String> SAMPLES_MAP;
    static {
        SAMPLES_MAP = new LinkedHashMap<String, String>();
        SAMPLES_MAP.put("Sign in", SignInActivity.class.getName());
        SAMPLES_MAP.put("+1", PlusOneActivity.class.getName());
        SAMPLES_MAP.put("Send interactive post", ShareActivity.class.getName());
        SAMPLES_MAP.put("Write moments", MomentActivity.class.getName());
        SAMPLES_MAP.put("List & remove moments", ListMomentsActivity.class.getName());
        SAMPLES_MAP.put("List people (circled by you)", ListPeopleActivity.class.getName());
        SAMPLES_MAP.put("License info", LicenseActivity.class.getName());
    }

    @Override
    public void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setListAdapter(new SimpleAdapter(this, getSamples(),
                R.layout.main_list_item, new String[] { FROM_TITLE },
                new int[] { android.R.id.text1 }));
    }

    @Override
    public boolean onCreateOptionsMenu(Menu menu) {
        getMenuInflater().inflate(R.menu.main_activity_menu, menu);
        return true;
    }

    @Override
    public boolean onOptionsItemSelected(MenuItem item) {
        int itemId = item.getItemId();
        if (itemId == R.id.change_locale) {
            Intent intent = new Intent(Intent.ACTION_MAIN);
            intent.setAction(android.provider.Settings.ACTION_LOCALE_SETTINGS);
            intent.addCategory(Intent.CATEGORY_DEFAULT);
            startActivity(intent);
            return true;
        }
        return super.onOptionsItemSelected(item);
    }

    protected ArrayList<HashMap<String, Object>> getSamples() {
        ArrayList<HashMap<String, Object>> samples = new ArrayList<HashMap<String, Object>>();
        for (Map.Entry<String, String> sample : SAMPLES_MAP.entrySet()) {
            Intent sampleIntent = new Intent(Intent.ACTION_MAIN);
            sampleIntent.setClassName(getApplicationContext(), sample.getValue());
            addItem(samples, sample.getKey(), sampleIntent);
        }
        return samples;
    }

    private void addItem(List<HashMap<String, Object>> data, String title, Intent intent) {
        HashMap<String, Object> temp = new HashMap<String, Object>();
        temp.put(TITLE_KEY, title);
        temp.put(INTENT_KEY, intent);
        data.add(temp);
    }

    @Override
    @SuppressWarnings("unchecked")
    protected void onListItemClick(ListView listView, View view, int position, long id) {
        Map<String, Object> map = (Map<String, Object>) listView.getItemAtPosition(position);
        Intent intent = (Intent) map.get(INTENT_KEY);
        startActivity(intent);
    }
}
