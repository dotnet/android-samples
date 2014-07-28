package com.example.android.preview.support.wearable.notifications;

import android.app.Activity;
import android.app.Notification;
import android.content.Context;
import android.content.Intent;
import android.os.Bundle;
import android.os.Handler;
import android.os.Message;
import android.preview.support.v4.app.NotificationManagerCompat;
import android.view.View;
import android.view.ViewGroup;
import android.widget.AdapterView;
import android.widget.ArrayAdapter;
import android.widget.CheckBox;
import android.widget.CompoundButton;
import android.widget.Spinner;
import android.widget.TextView;

import java.util.Arrays;

/**
 * Main activity which posts a notification when resumed, and allows customization
 * of that notification via controls.
 */
public class MainActivity extends Activity implements Handler.Callback {
    private static final int MSG_POST_NOTIFICATIONS = 0;
    private static final long POST_NOTIFICATIONS_DELAY_MS = 100;

    private Handler mHandler;
    private Spinner mPresetSpinner;
    private Spinner mPrioritySpinner;
    private Spinner mActionsSpinner;
    private CheckBox mIncludeLargeIconCheckbox;
    private CheckBox mLocalOnlyCheckbox;
    private CheckBox mIncludeContentIntentCheckbox;
    private CheckBox mIncludeContentIntentRequiredCheckbox;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);

        mHandler = new Handler(this);

        initPresetSpinner();
        initPrioritySpinner();
        initActionsSpinner();
        initIncludeLargeIconCheckbox();
        initLocalOnlyCheckbox();
        initIncludeContentIntentCheckbox();
    }

    @Override
    protected void onResume() {
        super.onResume();
        updateNotifications();
    }

    private void initPresetSpinner() {
        mPresetSpinner = (Spinner) findViewById(R.id.preset_spinner);
        mPresetSpinner.setAdapter(new NamedPresetSpinnerArrayAdapter(this,
                NotificationPresets.PRESETS));
        mPresetSpinner.setOnItemSelectedListener(new UpdateNotificationsOnItemSelectedListener());
    }

    private void initPrioritySpinner() {
        mPrioritySpinner = (Spinner) findViewById(R.id.priority_spinner);
        mPrioritySpinner.setAdapter(new NamedPresetSpinnerArrayAdapter(this,
                PriorityPresets.PRESETS));
        mPrioritySpinner.setSelection(Arrays.asList(PriorityPresets.PRESETS)
                .indexOf(PriorityPresets.DEFAULT));
        mPrioritySpinner.setOnItemSelectedListener(new UpdateNotificationsOnItemSelectedListener());
    }

    private void initActionsSpinner() {
        mActionsSpinner = (Spinner) findViewById(R.id.actions_spinner);
        mActionsSpinner.setAdapter(new NamedPresetSpinnerArrayAdapter(this,
                ActionsPresets.PRESETS));
        mActionsSpinner.setOnItemSelectedListener(new UpdateNotificationsOnItemSelectedListener());
    }

    private void initIncludeLargeIconCheckbox() {
        mIncludeLargeIconCheckbox = (CheckBox) findViewById(R.id.include_large_icon_checkbox);
        mIncludeLargeIconCheckbox.setOnCheckedChangeListener(
                new UpdateNotificationsOnCheckedChangeListener());
    }

    private void initLocalOnlyCheckbox() {
        mLocalOnlyCheckbox = (CheckBox) findViewById(R.id.local_only_checkbox);
        mLocalOnlyCheckbox.setOnCheckedChangeListener(
                new UpdateNotificationsOnCheckedChangeListener());
    }

    private void initIncludeContentIntentCheckbox() {
        mIncludeContentIntentCheckbox = (CheckBox) findViewById(
                R.id.include_content_intent_checkbox);
        mIncludeContentIntentRequiredCheckbox = (CheckBox) findViewById(
                R.id.include_content_intent_required_checkbox);
        mIncludeContentIntentCheckbox.setOnCheckedChangeListener(
                new UpdateNotificationsOnCheckedChangeListener());
    }

    /**
     * Begin to re-post the sample notification(s).
     */
    private void updateNotifications() {
        // Disable messages to skip notification deleted messages during cancel.
        sendBroadcast(new Intent(NotificationIntentReceiver.ACTION_DISABLE_MESSAGES)
                .setClass(this, NotificationIntentReceiver.class));

        // Cancel all existing notifications to trigger fresh-posting behavior: For example,
        // switching from HIGH to LOW priority does not cause a reordering in Notification Shade.
        NotificationManagerCompat.from(this).cancelAll();

        // Post the updated notifications on a delay to avoid a cancel+post race condition
        // with notification manager.
        mHandler.removeMessages(MSG_POST_NOTIFICATIONS);
        mHandler.sendEmptyMessageDelayed(MSG_POST_NOTIFICATIONS, POST_NOTIFICATIONS_DELAY_MS);
    }

    /**
     * Post the sample notification(s) using current options.
     */
    private void postNotifications() {
        sendBroadcast(new Intent(NotificationIntentReceiver.ACTION_ENABLE_MESSAGES)
                .setClass(this, NotificationIntentReceiver.class));

        NotificationPreset preset = NotificationPresets.PRESETS[
                mPresetSpinner.getSelectedItemPosition()];
        PriorityPreset priorityPreset = PriorityPresets.PRESETS[
                mPrioritySpinner.getSelectedItemPosition()];
        ActionsPreset actionsPreset = ActionsPresets.PRESETS[
                mActionsSpinner.getSelectedItemPosition()];
        mIncludeContentIntentCheckbox.setVisibility(preset.contentIntentRequired() ?
                View.GONE : View.VISIBLE);
        mIncludeContentIntentRequiredCheckbox.setVisibility(preset.contentIntentRequired() ?
                View.VISIBLE : View.GONE);
        NotificationPreset.BuildOptions options = new NotificationPreset.BuildOptions(
                priorityPreset,
                actionsPreset,
                mIncludeLargeIconCheckbox.isChecked(),
                mLocalOnlyCheckbox.isChecked(),
                mIncludeContentIntentCheckbox.isChecked());
        Notification[] notifications = preset.buildNotifications(this, options);

        // Post new notifications
        for (int i = 0; i < notifications.length; i++) {
            NotificationManagerCompat.from(this).notify(i, notifications[i]);
        }
    }

    @Override
    public boolean handleMessage(Message message) {
        switch (message.what) {
            case MSG_POST_NOTIFICATIONS:
                postNotifications();
                return true;
        }
        return false;
    }

    private class UpdateNotificationsOnItemSelectedListener
            implements AdapterView.OnItemSelectedListener {
        @Override
        public void onItemSelected(AdapterView<?> parent, View view, int position, long id) {
            updateNotifications();
        }

        @Override
        public void onNothingSelected(AdapterView<?> adapterView) {
        }
    }

    private class UpdateNotificationsOnCheckedChangeListener
            implements CompoundButton.OnCheckedChangeListener {
        @Override
        public void onCheckedChanged(CompoundButton compoundButton, boolean checked) {
            updateNotifications();
        }
    }

    private class NamedPresetSpinnerArrayAdapter extends ArrayAdapter<NamedPreset> {
        public NamedPresetSpinnerArrayAdapter(Context context, NamedPreset[] presets) {
            super(context, R.layout.simple_spinner_item, presets);
        }

        @Override
        public View getDropDownView(int position, View convertView, ViewGroup parent) {
            TextView view = (TextView) super.getDropDownView(position, convertView, parent);
            view.setText(getString(getItem(position).nameResId));
            return view;
        }

        @Override
        public View getView(int position, View convertView, ViewGroup parent) {
            TextView view = (TextView) getLayoutInflater().inflate(
                    android.R.layout.simple_spinner_item, parent, false);
            view.setText(getString(getItem(position).nameResId));
            return view;
        }
    }
}
