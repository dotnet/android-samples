package com.example.android.preview.support.wearable.notifications;

import android.content.Context;
import android.preview.support.wearable.notifications.RemoteInput;
import android.preview.support.wearable.notifications.WearableNotifications;

/**
 * Collection of notification actions presets.
 */
public class ActionsPresets {
    public static final ActionsPreset[] PRESETS = new ActionsPreset[] {
            new NoActionsPreset(),
            new SingleActionPreset(),
            new ReplyActionPreset(),
            new ReplyWithChoicesActionPreset()
    };

    private static class NoActionsPreset extends ActionsPreset {
        public NoActionsPreset() {
            super(R.string.no_actions);
        }

        @Override
        public void apply(Context context, WearableNotifications.Builder builder) {
        }
    }

    private static class SingleActionPreset extends ActionsPreset {
        public SingleActionPreset() {
            super(R.string.single_action);
        }

        @Override
        public void apply(Context context, WearableNotifications.Builder builder) {
            WearableNotifications.Action action = new WearableNotifications.Action.Builder(
                    R.drawable.ic_full_action,
                    context.getString(R.string.example_action),
                    NotificationUtil.getExamplePendingIntent(context,
                            R.string.example_action_clicked))
                    .build();
            builder.addAction(action);
        }
    }

    private static class ReplyActionPreset extends ActionsPreset {
        public ReplyActionPreset() {
            super(R.string.reply_action);
        }

        @Override
        public void apply(Context context, WearableNotifications.Builder builder) {
            RemoteInput remoteInput = new RemoteInput.Builder(NotificationUtil.EXTRA_REPLY)
                    .setLabel(context.getString(R.string.example_reply_label))
                    .build();
            WearableNotifications.Action action = new WearableNotifications.Action.Builder(
                    R.drawable.ic_full_reply,
                    context.getString(R.string.example_reply_action),
                    NotificationUtil.getExamplePendingIntent(context,
                            R.string.example_reply_action_clicked))
                    .addRemoteInput(remoteInput)
                    .build();
            builder.addAction(action);
        }
    }

    private static class ReplyWithChoicesActionPreset extends ActionsPreset {
        public ReplyWithChoicesActionPreset() {
            super(R.string.reply_action_with_choices);
        }

        @Override
        public void apply(Context context, WearableNotifications.Builder builder) {
            RemoteInput remoteInput = new RemoteInput.Builder(NotificationUtil.EXTRA_REPLY)
                    .setLabel(context.getString(R.string.example_reply_answer_label))
                    .setChoices(new String[] { context.getString(R.string.yes),
                            context.getString(R.string.no), context.getString(R.string.maybe) })
                    .build();
            WearableNotifications.Action action = new WearableNotifications.Action.Builder(
                    R.drawable.ic_full_reply,
                    context.getString(R.string.example_reply_action),
                    NotificationUtil.getExamplePendingIntent(context,
                            R.string.example_reply_action_clicked))
                    .addRemoteInput(remoteInput)
                    .build();
            builder.addAction(action);
        }
    }
}
