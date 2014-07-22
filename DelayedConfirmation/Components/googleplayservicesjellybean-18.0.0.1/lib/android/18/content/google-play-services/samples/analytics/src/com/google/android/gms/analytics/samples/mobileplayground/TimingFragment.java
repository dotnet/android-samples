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
import android.widget.EditText;

import com.google.android.gms.analytics.GoogleAnalytics;
import com.google.android.gms.analytics.HitBuilders;
import com.google.android.gms.analytics.Tracker;
import com.google.android.gms.analytics.samples.mobileplayground.AnalyticsSampleApp.TrackerName;

/**
 * Class to exercise Timing hits.
 */
public class TimingFragment extends Fragment {
    @Override
    public View onCreateView(LayoutInflater inflater, ViewGroup container,
            Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);

        View view = inflater.inflate(R.layout.timing, container, false);

        final Button sendButton = (Button)view.findViewById(R.id.timingSend);
        sendButton.setOnClickListener(new OnClickListener() {
            @Override
            public void onClick(View v) {
                Tracker t = ((AnalyticsSampleApp)getActivity().getApplication()).getTracker(
                        TrackerName.APP_TRACKER);
                t.send(new HitBuilders.TimingBuilder().setCategory(getTimingCategory())
                        .setValue(getTimingInterval()).setVariable(getTimingName())
                        .setLabel(getTimingLabel()).build());
            }
        });

        final Button dispatchButton = (Button)view.findViewById(R.id.timingDispatch);
        dispatchButton.setOnClickListener(new OnClickListener() {
            @Override
            public void onClick(View v) {
                // Manually start a dispatch (Unnecessary if the tracker has a dispatch interval)
                GoogleAnalytics.getInstance(getActivity().getBaseContext()).dispatchLocalHits();
            }
        });
        return view;
    }

    private String getTimingCategory() {
        return ((EditText)getView().findViewById(R.id.editTimingCategory)).getText().toString()
                .trim();
    }

    private long getTimingInterval() {
        String value = ((EditText)getView().findViewById(R.id.editTimingInterval)).getText()
                .toString().trim();
        if (value.length() == 0) {
            return 0;
        }
        return Long.valueOf(value);
    }

    private String getTimingName() {
        return ((EditText)getView().findViewById(R.id.editTimingName)).getText().toString().trim();
    }

    private String getTimingLabel() {
        return ((EditText)getView().findViewById(R.id.editTimingLabel)).getText().toString().trim();
    }
}
