package com.example.mapdemo;

import com.google.android.gms.maps.StreetViewPanoramaOptions;
import com.google.android.gms.maps.StreetViewPanoramaView;
import com.google.android.gms.maps.model.LatLng;

import android.os.Bundle;
import android.support.v4.app.FragmentActivity;
import android.view.ViewGroup.LayoutParams;

/**
 * This shows how to create a simple activity with streetview
 */
public class StreetViewPanoramaViewDemoActivity extends FragmentActivity {

    private StreetViewPanoramaView svpView;

    // George St, Sydney
    private static final LatLng SYDNEY = new LatLng(-33.87365, 151.20689);

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);

        StreetViewPanoramaOptions options = new StreetViewPanoramaOptions();
        if (savedInstanceState == null) {
            options.position(SYDNEY);
        }

        svpView = new StreetViewPanoramaView(this, options);
        addContentView(svpView,
            new LayoutParams(LayoutParams.MATCH_PARENT, LayoutParams.MATCH_PARENT));

      svpView.onCreate(savedInstanceState);
    }

    @Override
    protected void onResume() {
        svpView.onResume();
        super.onResume();
    }

    @Override
    protected void onPause() {
        svpView.onPause();
        super.onPause();
    }

    @Override
    protected void onDestroy() {
        svpView.onDestroy();
        super.onPause();
    }

    @Override
    public void onSaveInstanceState(Bundle outState) {
        super.onSaveInstanceState(outState);
        svpView.onSaveInstanceState(outState);
    }
}
