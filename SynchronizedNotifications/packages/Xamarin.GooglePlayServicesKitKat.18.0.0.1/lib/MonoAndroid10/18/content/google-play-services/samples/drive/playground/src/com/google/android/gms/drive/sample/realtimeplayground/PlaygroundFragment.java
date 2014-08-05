// Copyright 2014 Google Inc. All Rights Reserved.

package com.google.android.gms.drive.sample.realtimeplayground;

import com.google.android.gms.drive.realtime.RealtimeDocument;

import android.support.v4.app.Fragment;
import android.view.View;

import java.util.IdentityHashMap;
import java.util.Map;
import java.util.Map.Entry;

/**
 * A fragment used in the Realtime playground which allows widgets to be disabled until the Realtime
 * document has been loaded.
 *
 * <p>Subclasses should override {@link #onLoaded(RealtimeDocument)} to wire up fields to the
 * Realtime document and also call {@link #disableViewUntilLoaded(View)} with each widget that
 * should be disabled until the document is loaded.
 */
public abstract class PlaygroundFragment extends Fragment {
    private volatile boolean mOnLoadedRequired = true;
    private Map<View, Boolean> mViewEnabledState = new IdentityHashMap<>();
    private boolean saveDisabledState;

    @Override
    public void onStop() {
        super.onStop();
        for (View view : mViewEnabledState.keySet()) {
            if (saveDisabledState) {
                mViewEnabledState.put(view, view.isEnabled());
            }
            view.setEnabled(false);
        }
        mOnLoadedRequired = true;
    }

    @Override
    public void onStart() {
        super.onStart();
        saveDisabledState = false;
        for (View view : mViewEnabledState.keySet()) {
            view.setEnabled(false);
        }
        tryOnLoaded();
    }

    /**
     * Calls {@link #onLoaded} if it can be called and it has not already been called.
     */
    public void tryOnLoaded() {
        PlaygroundDocumentActivity activity = (PlaygroundDocumentActivity) getActivity();
        if (activity == null) {
            return;
        }
        RealtimeDocument document = activity.getRealtimeDocument();
        if (mOnLoadedRequired && document != null) {
            onLoaded(document);
            mOnLoadedRequired = false;
            for (Entry<View, Boolean> viewEntry : mViewEnabledState.entrySet()) {
                viewEntry.getKey().setEnabled(viewEntry.getValue());
            }
            saveDisabledState = true;
        }
    }

    /**
     * Returns a title for this fragment.
     */
    abstract String getTitle();

    /**
     * Called when the Realtime document is loaded.
     * @param document
     */
    abstract void onLoaded(RealtimeDocument document);

    /**
     * Adds a view to the set of views which will be disabled until the document is loaded.
     */
    protected void disableViewUntilLoaded(View view) {
        mViewEnabledState.put(view, view.isEnabled());
        view.setEnabled(false);
    }
}
