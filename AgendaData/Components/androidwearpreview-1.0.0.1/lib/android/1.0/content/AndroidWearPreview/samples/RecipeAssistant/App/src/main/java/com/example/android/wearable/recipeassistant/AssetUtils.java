package com.example.android.wearable.recipeassistant;


import android.content.Context;
import android.graphics.Bitmap;
import android.graphics.BitmapFactory;
import android.graphics.drawable.Drawable;
import android.util.Log;

import org.json.JSONException;
import org.json.JSONObject;

import java.io.IOException;
import java.io.InputStream;

final class AssetUtils {
    private static final String TAG = "RecipeAssistant";

    public static byte[] loadAsset(Context context, String asset) {
        byte[] buffer = null;
        try {
            InputStream is = context.getAssets().open(asset);
            int size = is.available();
            buffer = new byte[size];
            is.read(buffer);
            is.close();
        } catch (IOException e) {
            Log.e(TAG, "Failed to load asset " + asset + ": " + e);
        }
        return buffer;
    }

    public static JSONObject loadJSONAsset(Context context, String asset) {
        String jsonString = new String(loadAsset(context, asset));
        JSONObject jsonObject = null;
        try {
            jsonObject = new JSONObject(jsonString);
        } catch (JSONException e) {
            Log.e(TAG, "Failed to parse JSON asset " + asset + ": " + e);
        }
        return jsonObject;
    }

    public static Bitmap loadBitmapAsset(Context context, String asset) {
        InputStream is = null;
        Bitmap bitmap = null;
        try {
            is = context.getAssets().open(asset);
        } catch (IOException e) {
            e.printStackTrace();
        }
        if (is != null) {
            bitmap = BitmapFactory.decodeStream(is);
        }
        return bitmap;
    }
}
