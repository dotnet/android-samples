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

import com.google.android.gms.ads.AdRequest;
import com.google.android.gms.ads.InterstitialAd;

import android.app.Activity;
import android.os.Bundle;
import android.view.View;
import android.widget.Button;

/**
 * Example of requesting and displaying an interstitial ad.
 */
public class InterstitialActivity extends Activity {
    private InterstitialAd mInterstitial;
    private Button mShowButton;

    /** Called when the activity is first created. */
    @Override
    public void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_interstitial);

        mInterstitial = new InterstitialAd(this);
        mInterstitial.setAdUnitId(getResources().getString(R.string.ad_unit_id));
        mInterstitial.setAdListener(new ToastAdListener(this) {
            @Override
            public void onAdLoaded() {
                super.onAdLoaded();
                mShowButton.setText("Show Interstitial");
                mShowButton.setEnabled(true);
            }

            @Override
            public void onAdFailedToLoad(int errorCode) {
                super.onAdFailedToLoad(errorCode);
                mShowButton.setText("Ad Failed to Load");
                mShowButton.setEnabled(false);
            }
        });

        mShowButton = (Button) findViewById(R.id.showButton);
        mShowButton.setEnabled(false);
    }

    public void loadInterstitial(View unusedView) {
        mShowButton.setText("Loading Interstitial...");
        mShowButton.setEnabled(false);
        mInterstitial.loadAd(new AdRequest.Builder().build());
    }

    public void showInterstitial(View unusedView) {
        if (mInterstitial.isLoaded()) {
            mInterstitial.show();
        }

        mShowButton.setText("Interstitial Not Ready");
        mShowButton.setEnabled(false);
    }
}
