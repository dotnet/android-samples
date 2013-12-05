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

import com.google.android.gms.common.ConnectionResult;
import com.google.android.gms.common.GooglePlayServicesClient.ConnectionCallbacks;
import com.google.android.gms.common.GooglePlayServicesClient.OnConnectionFailedListener;
import com.google.android.gms.plus.PlusOneButton;

import android.app.Activity;
import android.os.Bundle;

/**
 * Example usage of the +1 button.
 */
public class PlusOneActivity extends Activity
        implements ConnectionCallbacks, OnConnectionFailedListener {
    private static final String URL = "https://developers.google.com/+";

    // The request code must be 0 or higher.
    private static final int PLUS_ONE_REQUEST_CODE = 0;

    private PlusOneButton mPlusOneSmallButton;
    private PlusOneButton mPlusOneMediumButton;
    private PlusOneButton mPlusOneTallButton;
    private PlusOneButton mPlusOneStandardButton;
    private PlusOneButton mPlusOneStandardButtonWithAnnotation;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.plus_one_activity);

        /*
         * The {@link PlusOneButton} can be configured in code, but in this example we
         * have set the parameters in the layout.
         *
         * Example:
         * mPlusOneSmallButton.setAnnotation(PlusOneButton.ANNOTATION_INLINE);
         * mPlusOneSmallButton.setSize(PlusOneButton.SIZE_MEDIUM);
         */
        mPlusOneSmallButton = (PlusOneButton) findViewById(R.id.plus_one_small_button);
        mPlusOneMediumButton = (PlusOneButton) findViewById(R.id.plus_one_medium_button);
        mPlusOneTallButton = (PlusOneButton) findViewById(R.id.plus_one_tall_button);
        mPlusOneStandardButton = (PlusOneButton) findViewById(R.id.plus_one_standard_button);
        mPlusOneStandardButtonWithAnnotation = (PlusOneButton) findViewById(
                R.id.plus_one_standard_ann_button);
    }

    @Override
    protected void onResume() {
        super.onResume();
        // Refresh the state of the +1 button each time we receive focus.
        mPlusOneSmallButton.initialize(URL, PLUS_ONE_REQUEST_CODE);
        mPlusOneMediumButton.initialize(URL, PLUS_ONE_REQUEST_CODE);
        mPlusOneTallButton.initialize(URL, PLUS_ONE_REQUEST_CODE);
        mPlusOneStandardButton.initialize(URL, PLUS_ONE_REQUEST_CODE);
        mPlusOneStandardButtonWithAnnotation.initialize(URL, PLUS_ONE_REQUEST_CODE);
    }

    @Override
    public void onConnectionFailed(ConnectionResult status) {
        // Nothing to do.
    }

    @Override
    public void onConnected(Bundle connectionHint) {
        // Nothing to do.
    }

    @Override
    public void onDisconnected() {
        // Nothing to do.
    }
}
