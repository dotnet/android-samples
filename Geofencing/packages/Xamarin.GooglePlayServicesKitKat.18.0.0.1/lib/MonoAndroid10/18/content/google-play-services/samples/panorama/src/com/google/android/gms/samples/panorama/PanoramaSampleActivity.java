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

package com.google.android.gms.samples.panorama;

import com.google.android.gms.common.ConnectionResult;
import com.google.android.gms.common.api.GoogleApiClient;
import com.google.android.gms.common.api.GoogleApiClient.ConnectionCallbacks;
import com.google.android.gms.common.api.GoogleApiClient.OnConnectionFailedListener;
import com.google.android.gms.common.api.ResultCallback;
import com.google.android.gms.panorama.Panorama;
import com.google.android.gms.panorama.PanoramaApi.PanoramaResult;

import android.app.Activity;
import android.content.Intent;
import android.net.Uri;
import android.os.Bundle;
import android.util.Log;

/**
 * Displays examples of integrating with the panorama viewer API.
 */
public class PanoramaSampleActivity extends Activity implements ConnectionCallbacks,
        OnConnectionFailedListener {

    public static final String TAG = PanoramaSampleActivity.class.getSimpleName();

    private GoogleApiClient mClient;

    @Override
    public void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        mClient = new GoogleApiClient.Builder(this, this, this)
                .addApi(Panorama.API)
                .build();
    }

    @Override
    public void onStart() {
        super.onStart();
        mClient.connect();
    }

    @Override
    public void onConnected(Bundle connectionHint) {
        Uri uri = Uri.parse("android.resource://" + getPackageName() + "/" + R.raw.pano1);
        Panorama.PanoramaApi.loadPanoramaInfo(mClient, uri).setResultCallback(
                new ResultCallback<PanoramaResult>() {
            @Override
            public void onResult(PanoramaResult result) {
                if (result.getStatus().isSuccess()) {
                    Intent viewerIntent = result.getViewerIntent();
                    Log.i(TAG, "found viewerIntent: " + viewerIntent);
                    if (viewerIntent != null) {
                        startActivity(viewerIntent);
                    }
                } else {
                    Log.e(TAG, "error: " + result);
                }
            }
        });
    }

    @Override
    public void onConnectionSuspended(int cause) {
        Log.i(TAG, "connection suspended: " + cause);
    }

    @Override
    public void onConnectionFailed(ConnectionResult status) {
        Log.e(TAG, "connection failed: " + status);
        // TODO fill in
    }

    @Override
    public void onStop() {
        super.onStop();
        mClient.disconnect();
    }
}
