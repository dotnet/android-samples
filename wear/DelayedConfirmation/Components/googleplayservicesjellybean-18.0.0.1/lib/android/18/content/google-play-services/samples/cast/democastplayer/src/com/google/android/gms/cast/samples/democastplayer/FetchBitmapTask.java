// Copyright 2013 Google Inc. All Rights Reserved.

package com.google.android.gms.cast.samples.democastplayer;

import android.graphics.Bitmap;
import android.graphics.BitmapFactory;
import android.net.Uri;
import android.os.AsyncTask;

import java.io.BufferedInputStream;
import java.io.IOException;
import java.io.InputStream;
import java.net.HttpURLConnection;
import java.net.MalformedURLException;
import java.net.URL;

/**
 * An AsyncTask to fetch an image over HTTP and scale it to a desired size.
 */
public abstract class FetchBitmapTask extends AsyncTask<Uri, Void, Bitmap> {
    private final int mPreferredWidth;
    private final int mPreferredHeight;

    /**
     * Constructs a new FetchBitmapTask that will do scaling.
     *
     * @param preferredWidth The preferred image width.
     * @param preferredHeight The preferred image height.
     */
    public FetchBitmapTask(int preferredWidth, int preferredHeight) {
        mPreferredWidth = preferredWidth;
        mPreferredHeight = preferredHeight;
    }

    /**
     * Constructs a new FetchBitmapTask.
     */
    public FetchBitmapTask() {
        this(0, 0);
    }

    @Override
    protected Bitmap doInBackground(Uri... uris) {
        if (uris.length != 1) {
            return null;
        }

        Bitmap bitmap = null;
        URL url = null;
        try {
            url = new URL(uris[0].toString());
        } catch (MalformedURLException e) {
            return null;
        }
        HttpURLConnection urlConnection = null;
        try {
            urlConnection = (HttpURLConnection) url.openConnection();
            urlConnection.setDoInput(true);

            if (urlConnection.getResponseCode() == HttpURLConnection.HTTP_OK) {
                InputStream stream = new BufferedInputStream(urlConnection.getInputStream());
                bitmap = BitmapFactory.decodeStream(stream);
                if ((mPreferredWidth > 0) && (mPreferredHeight > 0)) {
                    bitmap = scaleBitmap(bitmap);
                }
            }
        } catch (IOException e) { /* ignore */
        } finally {
            if (urlConnection != null) {
                urlConnection.disconnect();
            }
        }

        return bitmap;
    }

    /*
     * Scales the bitmap to the preferred width and height.
     *
     * @param bitmap The bitmap to scale.
     * @return The scaled bitmap.
     */
    private Bitmap scaleBitmap(Bitmap bitmap) {
        int width = bitmap.getWidth();
        int height = bitmap.getHeight();

        // Calculate deltas.
        int dw = width - mPreferredWidth;
        int dh = height - mPreferredHeight;

        if ((dw == 0) && (dh == 0)) {
            return bitmap;
        }

        float scaleFactor = 0.0f;
        if ((dw > 0) || (dh > 0)) {
            // Icon is too big; scale down.
            float scaleWidth = (float) mPreferredWidth / width;
            float scaleHeight = (float) mPreferredHeight / height;
            scaleFactor = Math.min(scaleHeight, scaleWidth);
        } else {
            // Icon is too small; scale up.
            float scaleWidth = width / (float) mPreferredWidth;
            float scaleHeight = height / (float) mPreferredHeight;
            scaleFactor = Math.min(scaleHeight, scaleWidth);
        }

        int finalWidth = (int) ((width * scaleFactor) + 0.5f);
        int finalHeight = (int) ((height * scaleFactor) + 0.5f);

        return Bitmap.createScaledBitmap(bitmap, finalWidth, finalHeight, false);
    }

}
