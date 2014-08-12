/*
 * Copyright (C) 2013 The Android Open Source Project
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

package com.google.android.gms.samples.ads;

import android.app.Activity;
import android.app.ListActivity;
import android.content.Intent;
import android.content.res.Resources;
import android.os.Bundle;
import android.view.View;
import android.widget.ArrayAdapter;
import android.widget.ListView;

/**
 * Displays examples of integrating different ad formats with the Google AdMob SDK for
 * Android.
 */
public class GoogleAdsSampleActivity extends ListActivity {

    private static class Sample {
        private String mTitle;
        private Class<? extends Activity> mActivityClass;

        private Sample(String title, Class<? extends Activity> activityClass) {
            mTitle = title;
            mActivityClass = activityClass;
        }

        @Override
        public String toString() {
            return mTitle;
        }

        public Class<? extends Activity> getActivityClass() {
            return mActivityClass;
        }
    }

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);

        Resources res = getResources();
        Sample[] samples = {
            new Sample(res.getString(R.string.banner_in_xml), BannerXmlActivity.class),
            new Sample(res.getString(R.string.banner_in_code), BannerCodeActivity.class),
            new Sample(res.getString(R.string.interstitial), InterstitialActivity.class)
        };
        setListAdapter(
                new ArrayAdapter<Sample>(this, android.R.layout.simple_list_item_1, samples));
    }

    @Override
    protected void onListItemClick(ListView listView, View view, int position, long id) {
        Sample sample = (Sample) listView.getItemAtPosition(position);
        Intent intent = new Intent(this.getApplicationContext(), sample.getActivityClass());
        startActivity(intent);
    }
}
