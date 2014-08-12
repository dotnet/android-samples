package com.example.android.preview.support.wearable.notifications;

import android.content.Context;
import android.preview.support.wearable.notifications.WearableNotifications;

/**
 * Base class for notification actions presets.
 */
public abstract class ActionsPreset extends NamedPreset {
    public ActionsPreset(int nameResId) {
        super(nameResId);
    }

    /** Apply the priority to a notification builder */
    public abstract void apply(Context context, WearableNotifications.Builder builder);
}
