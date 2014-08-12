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

package com.google.android.gms.analytics.samples.mobileplayground;

import android.os.Bundle;
import android.support.v4.app.Fragment;
import android.view.LayoutInflater;
import android.view.View;
import android.view.View.OnClickListener;
import android.view.ViewGroup;
import android.widget.Button;

import com.google.android.gms.analytics.GoogleAnalytics;
import com.google.android.gms.analytics.HitBuilders;
import com.google.android.gms.analytics.Tracker;
import com.google.android.gms.analytics.samples.mobileplayground.AnalyticsSampleApp.TrackerName;

/**
 * Class to exercise Screen Views.
 */
public class AppviewFragment extends Fragment {

    @Override
    public View onCreateView(LayoutInflater inflater, ViewGroup container,
            Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);

        View v = inflater.inflate(R.layout.screenview, container, false);

        setupAppview(v, R.id.homePageview, R.string.homePath);
        setupAppview(v, R.id.helpPageview, R.string.helpPath);

        final Button dispatchButton = (Button) v.findViewById(R.id.pageviewDispatch);
        dispatchButton.setOnClickListener(new OnClickListener() {
            @Override
            public void onClick(View v) {
                // Manually start a dispatch (Unnecessary if the tracker has a dispatch interval)
                GoogleAnalytics.getInstance(getActivity().getBaseContext()).dispatchLocalHits();
            }
        });
        return v;
    }

    private void setupAppview(View v, int buttonId, int pathId) {
        final Button pageviewButton = (Button) v.findViewById(buttonId);
        // Set button text
        final String path = getString(pathId);
        final String sendText = getString(R.string.sendPrefix) + path;
        pageviewButton.setText(sendText);
        // Set listener to track a pageview.
        pageviewButton.setOnClickListener(new OnClickListener() {
            @Override
            public void onClick(View v) {
                Tracker t = ((AnalyticsSampleApp) getActivity().getApplication()).getTracker(
                        TrackerName.APP_TRACKER);
                t.setScreenName(path);
                t.send(new HitBuilders.AppViewBuilder().build());
            }
        });
    }
}
