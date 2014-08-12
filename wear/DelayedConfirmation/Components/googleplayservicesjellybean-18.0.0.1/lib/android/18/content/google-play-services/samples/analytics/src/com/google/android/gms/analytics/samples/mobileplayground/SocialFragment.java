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
import com.google.android.gms.analytics.samples.mobileplayground.MobilePlayground.UserInputException;

/**
 * Class to exercise Social hits.
 */
public class SocialFragment extends Fragment {

    @Override
    public View onCreateView(LayoutInflater inflater, ViewGroup container,
            Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        View view = inflater.inflate(R.layout.social, container, false);

        final Button sendButton = (Button)view.findViewById(R.id.socialSend);
        sendButton.setOnClickListener(new OnClickListener() {
            @Override
            public void onClick(View v) {
                try {
                    Tracker t = ((AnalyticsSampleApp)getActivity().getApplication()).getTracker(
                            TrackerName.APP_TRACKER);
                    t.send(new HitBuilders.SocialBuilder().setNetwork(getSocialNetwork())
                            .setAction(getSocialAction()).setTarget(getSocialTarget()).build());
                } catch (UserInputException e) {
                    e.printStackTrace();
                }
            }
        });

        final Button dispatchButton = (Button)view.findViewById(R.id.socialDispatch);
        dispatchButton.setOnClickListener(new OnClickListener() {
            @Override
            public void onClick(View v) {
                // Manually start a dispatch (Unnecessary if the tracker has a dispatch interval)
                GoogleAnalytics.getInstance(getActivity().getBaseContext()).dispatchLocalHits();
            }
        });
        return view;
    }

    private String getSocialNetwork() throws UserInputException {
        String result = ((EditText)getView().findViewById(R.id.editSocialNetwork)).getText()
                .toString().trim();
        if (result.length() == 0) {
            throw new UserInputException(getString(R.string.socialNetworkWarning));
        }
        return result;
    }

    private String getSocialAction() throws UserInputException {
        String result = ((EditText)getView().findViewById(R.id.editSocialAction)).getText()
                .toString().trim();
        if (result.length() == 0) {
            throw new UserInputException(getString(R.string.socialActionWarning));
        }
        return result;
    }

    private String getSocialTarget() {
        return ((EditText)getView().findViewById(R.id.editSocialTarget)).getText().toString()
                .trim();
    }
}
