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

import com.google.android.gms.maps.CameraUpdateFactory;
import com.google.android.gms.maps.GoogleMap;
import com.google.android.gms.maps.GoogleMap.OnCameraChangeListener;
import com.google.android.gms.maps.SupportMapFragment;
import com.google.android.gms.maps.model.CameraPosition;
import com.google.android.gms.maps.model.LatLng;
import com.google.android.gms.maps.model.LatLngBounds;
import com.google.android.gms.maps.model.MarkerOptions;

import android.os.Bundle;
import android.os.Handler;
import android.os.SystemClock;
import android.support.v4.app.FragmentActivity;
import android.view.View;
import android.view.animation.Interpolator;
import android.view.animation.OvershootInterpolator;
import android.widget.TextView;

/**
 * This shows how to use setPadding to allow overlays that obscure part of the map without
 * obscuring the map UI or copyright notices.
 */
public class VisibleRegionDemoActivity extends FragmentActivity {
    /**
     * Note that this may be null if the Google Play services APK is not available.
     */
    private GoogleMap mMap;

    private static final LatLng SOH = new LatLng(-33.85704, 151.21522);
    private static final LatLng SFO = new LatLng(37.614631, -122.385153);
    private static final LatLngBounds AUS = new LatLngBounds(
            new LatLng(-44, 113), new LatLng(-10, 154));

    private TextView mMessageView;

    /** Keep track of current values for padding, so we can animate from them. */
    int currentLeft = 150;
    int currentTop = 0;
    int currentRight = 0;
    int currentBottom = 0;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.visible_region_demo);
        mMessageView = (TextView) findViewById(R.id.message_text);
        setUpMapIfNeeded();
    }

    @Override
    protected void onResume() {
        super.onResume();
        setUpMapIfNeeded();
    }

    /**
     * Sets up the map if it is possible to do so (i.e., the Google Play services APK is correctly
     * installed) and the map has not already been instantiated.. This will ensure that we only ever
     * call {@link #setUpMap()} once when {@link #mMap} is not null.
     * <p>
     * If it isn't installed {@link SupportMapFragment} (and
     * {@link com.google.android.gms.maps.MapView MapView}) will show a prompt for the user to
     * install/update the Google Play services APK on their device.
     * <p>
     * A user can return to this FragmentActivity after following the prompt and correctly
     * installing/updating/enabling the Google Play services. Since the FragmentActivity may not
     * have been completely destroyed during this process (it is likely that it would only be
     * stopped or paused), {@link #onCreate(Bundle)} may not be called again so we should call this
     * method in {@link #onResume()} to guarantee that it will be called.
     */
    private void setUpMapIfNeeded() {
        // Do a null check to confirm that we have not already instantiated the map.
        if (mMap == null) {
            // Try to obtain the map from the SupportMapFragment.
            mMap = ((SupportMapFragment) getSupportFragmentManager().findFragmentById(R.id.map))
                   .getMap();
            // Check if we were successful in obtaining the map.
            if (mMap != null) {
                // turn MyLocation on and move to a place with indoor (SFO airport)
                mMap.setMyLocationEnabled(true);
                mMap.setPadding(currentLeft, currentTop, currentRight, currentBottom);
                mMap.moveCamera(CameraUpdateFactory.newLatLngZoom(SFO, 18));
                // Add a marker to the Opera House
                mMap.addMarker(new MarkerOptions().position(SOH).title("Sydney Opera House"));
                // Add a camera change listener.
                mMap.setOnCameraChangeListener(new OnCameraChangeListener() {
                    public void onCameraChange(CameraPosition pos) {
                      mMessageView.setText("CameraChangeListener: " + pos);
                    }
                  });
            }
        }
    }

    public void moveToOperaHouse(View view) {
        mMap.moveCamera(CameraUpdateFactory.newLatLngZoom(SOH, 16));
    }

    public void moveToSFO(View view) {
        mMap.moveCamera(CameraUpdateFactory.newLatLngZoom(SFO, 18));
    }

    public void moveToAUS(View view) {
        mMap.moveCamera(CameraUpdateFactory.newLatLngBounds(AUS, 0));
    }

    public void setNoPadding(View view) {
        animatePadding(150, 0, 0, 0);
    }

    public void setMorePadding(View view) {
        View mapView = ((SupportMapFragment)
            getSupportFragmentManager().findFragmentById(R.id.map)).getView();
        int left = 150;
        int top = 0;
        int right = mapView.getWidth() / 3;
        int bottom = mapView.getHeight() / 4;
        animatePadding(left, top, right, bottom);
    }

    public void animatePadding(
        final int toLeft, final int toTop, final int toRight, final int toBottom) {

        final Handler handler = new Handler();
        final long start = SystemClock.uptimeMillis();
        final long duration = 1000;

        final Interpolator interpolator = new OvershootInterpolator();

        final int startLeft = currentLeft;
        final int startTop = currentTop;
        final int startRight = currentRight;
        final int startBottom = currentBottom;

        currentLeft = toLeft;
        currentTop = toTop;
        currentRight = toRight;
        currentBottom = toBottom;

        handler.post(new Runnable() {
            @Override
            public void run() {
                long elapsed = SystemClock.uptimeMillis() - start;
                float t = interpolator.getInterpolation((float) elapsed / duration);

                int left = (int) (startLeft + ((toLeft - startLeft) * t));
                int top = (int) (startTop + ((toTop - startTop) * t));
                int right = (int) (startRight + ((toRight - startRight) * t));
                int bottom = (int) (startBottom + ((toBottom - startBottom) * t));

                mMap.setPadding(left, top, right, bottom);

                if (elapsed < duration) {
                    // Post again 16ms later.
                    handler.postDelayed(this, 16);
                }
            }
        });
    }
}
