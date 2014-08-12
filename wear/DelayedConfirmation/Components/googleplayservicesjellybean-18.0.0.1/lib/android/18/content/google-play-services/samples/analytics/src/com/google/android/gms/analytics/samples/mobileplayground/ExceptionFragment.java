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
import android.widget.CheckBox;
import android.widget.EditText;

import com.google.android.gms.analytics.GoogleAnalytics;
import com.google.android.gms.analytics.HitBuilders;
import com.google.android.gms.analytics.Tracker;
import com.google.android.gms.analytics.samples.mobileplayground.AnalyticsSampleApp.TrackerName;

/**
 * Class to exercise Exception hits.
 */
public class ExceptionFragment extends Fragment {

    @Override
    public View onCreateView(LayoutInflater inflater, ViewGroup container,
            Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);

        View view = inflater.inflate(R.layout.exception, container, false);

        final Button dispatchBtn = (Button) view.findViewById(R.id.btnDispatch);
        dispatchBtn.setOnClickListener(new OnClickListener() {
            @Override
            public void onClick(View v) {
                dispatch();
            }
        });

        final Button trackBtn = (Button) view.findViewById(R.id.trackBtn);
        trackBtn.setOnClickListener(new OnClickListener() {
            @Override
            public void onClick(View v) {
                track();
            }
        });
        return view;
    }

    private void dispatch() {
        GoogleAnalytics.getInstance(getActivity().getApplicationContext()).dispatchLocalHits();
    }

    private void track() {
        Tracker t = ((AnalyticsSampleApp) getActivity().getApplication()).getTracker(
                TrackerName.APP_TRACKER);
        t.send(new HitBuilders.ExceptionBuilder()
                .setDescription(getExceptionMethod() + ":" + getExceptionLocation())
                .setFatal(getExceptionFatal()).build());
    }

    private boolean getExceptionFatal() {
        return ((CheckBox) getView().findViewById(R.id.isFatalChk)).isChecked();
    }

    private String getExceptionLocation() {
        return ((EditText) getView().findViewById(R.id.exceptionLocationEdit)).getText().toString();
    }

    private String getExceptionMethod() {
        return ((EditText) getView().findViewById(R.id.exceptionMethodEdit)).getText().toString();
    }
}
