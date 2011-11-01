package mono.samples.googlemaps;

import android.os.Bundle;

public class MyMapActivity extends com.google.android.maps.MapActivity {
    /** Called when the activity is first created. */
    @Override
    public void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.map);
    }

    @Override
    protected boolean isRouteDisplayed() {
        return false;
    }
}
