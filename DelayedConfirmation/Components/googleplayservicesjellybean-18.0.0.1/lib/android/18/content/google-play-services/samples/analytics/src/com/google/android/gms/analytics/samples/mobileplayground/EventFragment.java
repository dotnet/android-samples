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
 * Class to exercise Event hits.
 */
public class EventFragment extends Fragment {

    @Override
    public View onCreateView(LayoutInflater inflater, ViewGroup container,
            Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);

        View view = inflater.inflate(R.layout.event, container, false);

        setupEvent(view, R.id.video1Play, R.string.videoCategory, R.string.videoPlay,
                R.string.video1);
        setupEvent(view, R.id.video1Pause, R.string.videoCategory, R.string.videoPause,
                R.string.video1);
        setupEvent(view, R.id.video2Play, R.string.videoCategory, R.string.videoPlay,
                R.string.video2);
        setupEvent(view, R.id.video2Pause, R.string.videoCategory, R.string.videoPause,
                R.string.video2);

        setupEvent(view, R.id.book1View, R.string.bookCategory, R.string.bookView, R.string.book1);
        setupEvent(view, R.id.book1Share, R.string.bookCategory, R.string.bookShare,
                R.string.book1);

        final Button dispatchButton = (Button)view.findViewById(R.id.eventDispatch);
        dispatchButton.setOnClickListener(new OnClickListener() {
            @Override
            public void onClick(View v) {
                // Manually start a dispatch (Unnecessary if the tracker has a dispatch interval)
                GoogleAnalytics.getInstance(getActivity().getApplicationContext())
                        .dispatchLocalHits();
            }
        });
        return view;
    }

    private void setupEvent(View v, int buttonId, final int categoryId, final int actionId,
            final int labelId) {
        final Button pageviewButton = (Button)v.findViewById(buttonId);
        pageviewButton.setOnClickListener(new OnClickListener() {
            @Override
            public void onClick(View v) {
                Tracker t = ((AnalyticsSampleApp)getActivity().getApplication()).getTracker(
                        TrackerName.APP_TRACKER);
                t.send(new HitBuilders.EventBuilder().setCategory(getString(categoryId))
                        .setAction(getString(actionId)).setLabel(getString(labelId)).build());
            }
        });
    }
}
