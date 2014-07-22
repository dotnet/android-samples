package com.example.android.preview.support.wearable.notifications;

import android.app.Notification;
import android.content.Context;

/**
 * Base class for notification preset generators.
 */
public abstract class NotificationPreset extends NamedPreset {
    public NotificationPreset(int nameResId) {
        super(nameResId);
    }

    public static class BuildOptions {
        public final PriorityPreset priorityPreset;
        public final ActionsPreset actionsPreset;
        public final boolean includeLargeIcon;
        public final boolean isLocalOnly;
        public final boolean hasContentIntent;

        public BuildOptions(PriorityPreset priorityPreset, ActionsPreset actionsPreset,
                boolean includeLargeIcon, boolean isLocalOnly, boolean hasContentIntent) {
            this.priorityPreset = priorityPreset;
            this.actionsPreset = actionsPreset;
            this.includeLargeIcon = includeLargeIcon;
            this.isLocalOnly = isLocalOnly;
            this.hasContentIntent = hasContentIntent;
        }
    }

    /** Build a notification with this preset and the provided options */
    public abstract Notification[] buildNotifications(Context context, BuildOptions options);

    /** Whether the content for this preset is required. */
    public boolean contentIntentRequired() {
        return false;
    }
}
