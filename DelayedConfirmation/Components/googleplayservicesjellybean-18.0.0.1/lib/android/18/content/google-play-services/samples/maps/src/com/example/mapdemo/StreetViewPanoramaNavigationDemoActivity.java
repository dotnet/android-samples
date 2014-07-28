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

package com.example.mapdemo;

import com.google.android.gms.maps.StreetViewPanorama;
import com.google.android.gms.maps.SupportStreetViewPanoramaFragment;
import com.google.android.gms.maps.model.LatLng;
import com.google.android.gms.maps.model.StreetViewPanoramaCamera;
import com.google.android.gms.maps.model.StreetViewPanoramaLink;
import com.google.android.gms.maps.model.StreetViewPanoramaLocation;

import android.os.Bundle;
import android.support.v4.app.FragmentActivity;
import android.util.FloatMath;
import android.view.View;
import android.widget.SeekBar;
import android.widget.Toast;

/**
 * This shows how to create an activity with access to all the options in Panorama
 * which can be adjusted dynamically
 */

public class StreetViewPanoramaNavigationDemoActivity extends FragmentActivity {

    // George St, Sydney
    private static final LatLng SYDNEY = new LatLng(-33.87365, 151.20689);

    // Cole St, San Fran
    private static final String SAN_FRAN = "jc-W9NSgV9pQ_66IFu0YFw";

    // Bondi Beach, Bondi
    private static final LatLng BONDI = new LatLng(-33.891614, 151.276417);

    // LatLng with no panorama
    private static final LatLng INVALID = new LatLng(-45.125783, 151.276417);


    private static final long DEFAULT_ANIMATION_DURATION = 1000;

    /**
     * The amount in degrees by which to scroll the camera
     */
    private static final int PAN_BY_DEG = 30;

    private static final float ZOOM_BY = 0.5f;

    private StreetViewPanorama svp;

    private SeekBar customDurationBar;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.street_view_panorama_navigation_demo);

        setUpStreetViewPanoramaIfNeeded(savedInstanceState);
        customDurationBar = (SeekBar) findViewById(R.id.duration_bar);
    }

    private void setUpStreetViewPanoramaIfNeeded(Bundle savedInstanceState) {
        if (svp == null) {
            svp = ((SupportStreetViewPanoramaFragment)
                getSupportFragmentManager().findFragmentById(R.id.streetviewpanorama))
                    .getStreetViewPanorama();
            if (svp != null) {
                if (savedInstanceState == null) {
                    svp.setPosition(SYDNEY);
                }
            }
        }
    }

    /**
     * When the panorama is not ready the PanoramaView cannot be used. This should be called on
     * all entry points that call methods on the Panorama API.
     */
    private boolean checkReady() {
        if (svp == null) {
            Toast.makeText(this, R.string.panorama_not_ready, Toast.LENGTH_SHORT).show();
            return false;
        }
        return true;
    }

    /**
     * Called when the Go To San Fran button is clicked.
     */
    public void onGoToSanFran(View view) {
        if (!checkReady()) {
            return;
        }
        svp.setPosition(SAN_FRAN);
    }

    /**
     * Called when the Animate To Sydney button is clicked.
     */
    public void onGoToSydney(View view) {
        if (!checkReady()) {
            return;
        }
        svp.setPosition(SYDNEY);
    }

    /**
     * Called when the Animate To Bondi button is clicked.
     */
    public void onGoToBondi(View view) {
        if (!checkReady()) {
            return;
        }
        svp.setPosition(BONDI, 500);
    }

    /**
     * Called when the Animate To Invalid button is clicked.
     */
    public void onGoToInvalid(View view) {
        if (!checkReady()) {
            return;
        }
        svp.setPosition(INVALID);
    }

    public void onZoomIn(View view) {
        if (!checkReady()) {
            return;
        }

        svp.animateTo(
            new StreetViewPanoramaCamera.Builder().zoom(svp.getPanoramaCamera().zoom + ZOOM_BY)
            .tilt(svp.getPanoramaCamera().tilt)
            .bearing(svp.getPanoramaCamera().bearing).build(), getDuration());
    }

    public void onZoomOut(View view) {
        if (!checkReady()) {
            return;
        }

        svp.animateTo(
            new StreetViewPanoramaCamera.Builder().zoom(svp.getPanoramaCamera().zoom - ZOOM_BY)
            .tilt(svp.getPanoramaCamera().tilt)
            .bearing(svp.getPanoramaCamera().bearing).build(), getDuration());
    }

    public void onPanLeft(View view) {
        if (!checkReady()) {
            return;
        }

        svp.animateTo(
            new StreetViewPanoramaCamera.Builder().zoom(svp.getPanoramaCamera().zoom)
            .tilt(svp.getPanoramaCamera().tilt)
            .bearing(svp.getPanoramaCamera().bearing - PAN_BY_DEG).build(), getDuration());
    }

    public void onPanRight(View view) {
        if (!checkReady()) {
            return;
        }

        svp.animateTo(
            new StreetViewPanoramaCamera.Builder().zoom(svp.getPanoramaCamera().zoom)
            .tilt(svp.getPanoramaCamera().tilt)
            .bearing(svp.getPanoramaCamera().bearing + PAN_BY_DEG).build(), getDuration());

    }

    public void onPanUp(View view) {
        if (!checkReady()) {
            return;
        }

        float currentTilt = svp.getPanoramaCamera().tilt;
        float newTilt = currentTilt + PAN_BY_DEG;

        newTilt = (newTilt > 90) ? 90 : newTilt;

        svp.animateTo(
            new StreetViewPanoramaCamera.Builder().zoom(svp.getPanoramaCamera().zoom)
            .tilt(newTilt)
            .bearing(svp.getPanoramaCamera().bearing).build(), getDuration());
    }

    public void onPanDown(View view) {
        if (!checkReady()) {
            return;
        }

        float currentTilt = svp.getPanoramaCamera().tilt;
        float newTilt = currentTilt - PAN_BY_DEG;

        newTilt = (newTilt < -90) ? -90 : newTilt;

        svp.animateTo(
            new StreetViewPanoramaCamera.Builder().zoom(svp.getPanoramaCamera().zoom)
            .tilt(newTilt)
            .bearing(svp.getPanoramaCamera().bearing).build(), getDuration());
    }

    public void onRequestPosition(View view) {
        if (!checkReady()){
            return;
        }
        if (svp.getLocation() != null) {
          Toast.makeText(view.getContext(), svp.getLocation().position.toString(),
              Toast.LENGTH_SHORT).show();
        }
    }

    public void onMovePosition(View view) {
        StreetViewPanoramaLocation location = svp.getLocation();
        StreetViewPanoramaCamera camera = svp.getPanoramaCamera();
        if (location != null && location.links != null) {
            StreetViewPanoramaLink link = findClosestLinkToBearing(location.links, camera.bearing);
            svp.setPosition(link.panoId);
        }
    }

    public static StreetViewPanoramaLink findClosestLinkToBearing(StreetViewPanoramaLink[] links,
        float bearing) {
        float minBearingDiff = 360;
        StreetViewPanoramaLink closestLink = links[0];
        for (StreetViewPanoramaLink link : links) {
            if (minBearingDiff > findNormalizedDifference(bearing, link.bearing)) {
                minBearingDiff = findNormalizedDifference(bearing, link.bearing);
                closestLink = link;
            }
        }
        return closestLink;
    }

    // Find the difference between angle a and b as a value between 0 and 180
    public static float findNormalizedDifference(float a, float b) {
        float diff = a - b;
        float normalizedDiff = diff - (360.0f * FloatMath.floor(diff / 360.0f));
        return (normalizedDiff < 180.0f) ? normalizedDiff : 360.0f - normalizedDiff;
    }

    private long getDuration() {
        return customDurationBar.getProgress();
    }
}
