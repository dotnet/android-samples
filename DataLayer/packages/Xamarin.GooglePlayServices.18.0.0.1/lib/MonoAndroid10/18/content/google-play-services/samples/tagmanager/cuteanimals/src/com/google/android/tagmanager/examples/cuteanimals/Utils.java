package com.google.android.tagmanager.examples.cuteanimals;

import android.content.Context;

import com.google.android.gms.tagmanager.DataLayer;
import com.google.android.gms.tagmanager.TagManager;

/**
 * Utility class.
 */
public class Utils {
    private Utils() {
        // private constructor.
    }

    /**
     * Push an "openScreen" event with the given screen name. Tags that match that event will fire.
     */
    public static void pushOpenScreenEvent(Context context, String screenName) {
        DataLayer dataLayer = TagManager.getInstance(context).getDataLayer();
        dataLayer.pushEvent("openScreen", DataLayer.mapOf("screenName", screenName));
    }

    /**
     * Push a "closeScreen" event with the given screen name. Tags that match that event will fire.
     */
    public static void pushCloseScreenEvent(Context context, String screenName) {
        DataLayer dataLayer = TagManager.getInstance(context).getDataLayer();
        dataLayer.pushEvent("closeScreen", DataLayer.mapOf("screenName", screenName));
    }
}
