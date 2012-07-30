package me.kiip.example;

import me.kiip.api.Kiip;
import me.kiip.api.Kiip.RequestListener;
import me.kiip.api.KiipException;
import me.kiip.api.Resource;
import android.app.Application;
import android.util.Log;
import android.widget.Toast;

public class ExampleApplication extends Application {
    private static final String TAG = "example";

    private static final String KP_APP_KEY =    "0b56b49f621ad7f42fd85de7e461f9dd";
    private static final String KP_APP_SECRET = "ac3abfdf5cb86ce0febba0c8afd2744e";

    /** Called when the application is first created. */
    @Override
    public void onCreate() {
        super.onCreate();
        // The application has just launched, create Kiip instance.
        Kiip.init(this, KP_APP_KEY, KP_APP_SECRET);
    }

    public RequestListener<Resource> startSessionListener = new RequestListener<Resource>() {
        @Override
        public void onFinished(Kiip manager, Resource response) {
            if (response != null) {
                toast("Start Session Finished w/ Promo");
            } else {
                toast("Start Session Finished No Promo");
            }
            manager.showResource(response);
        }

        @Override
        public void onError(Kiip manager, KiipException error) {
            toast("Start Session Failed: " + "(" + error.getCode() + ") " + error.getMessage());
        }
    };

    public RequestListener<Resource> endSessionListener = new RequestListener<Resource>() {
        @Override
        public void onFinished(Kiip manager, Resource response) {
            toast("End Session Finished");
        }

        @Override
        public void onError(Kiip manager, KiipException error) {
            toast("End Session Failed: " + "(" + error.getCode() + ") " + error.getMessage());
        }
    };

    private void toast(String message) {
        Log.v(TAG, message);
        Toast.makeText(this, message, Toast.LENGTH_SHORT).show();
    }
}
