package com.example.android.wearable.elizachat;

import android.app.Activity;
import android.app.Notification;
import android.app.PendingIntent;
import android.content.BroadcastReceiver;
import android.content.Context;
import android.content.Intent;
import android.content.IntentFilter;
import android.graphics.BitmapFactory;
import android.os.Bundle;
import android.preview.support.v4.app.NotificationManagerCompat;
import android.preview.support.wearable.notifications.RemoteInput;
import android.preview.support.wearable.notifications.WearableNotifications;
import android.support.v4.app.NotificationCompat;
import android.widget.TextView;

public class MainActivity extends Activity {
    @SuppressWarnings("unused")
    private static final String TAG = "MainActivity";

    public static final String EXTRA_REPLY = "reply";

    private static final String ACTION_RESPONSE = "com.example.android.wearable.elizachat.REPLY";

    private String mLastResponse = null;

    private BroadcastReceiver mReceiver;

    private ElizaResponder mResponder;

    private TextView mHistoryView;

    @Override
    protected void onCreate(Bundle saved) {
        super.onCreate(saved);
        setContentView(R.layout.activity_main);
        mReceiver = new BroadcastReceiver() {
            @Override
            public void onReceive(Context context, Intent intent) {
                processResponse(intent);
            }
        };
        mResponder = new ElizaResponder();
        mHistoryView = (TextView) findViewById(R.id.history);
    }

    @Override
    protected void onResume() {
        super.onResume();
        registerReceiver(mReceiver, new IntentFilter(ACTION_RESPONSE));
        if (mLastResponse == null) {
            mLastResponse = mResponder.elzTalk("");
            mHistoryView.setText(mLastResponse);
        }
        showNotification();
    }

    @Override
    protected void onPause() {
        NotificationManagerCompat.from(this).cancel(0);
        unregisterReceiver(mReceiver);
        super.onPause();
    }

    private void showNotification() {
        NotificationCompat.Builder builder = new NotificationCompat.Builder(this)
                .setContentTitle(getString(R.string.eliza))
                .setContentText(mLastResponse)
                .setLargeIcon(BitmapFactory.decodeResource(getResources(), R.drawable.bg_eliza));

        Intent intent = new Intent(ACTION_RESPONSE);
        PendingIntent pendingIntent = PendingIntent.getBroadcast(this, 0, intent,
                PendingIntent.FLAG_ONE_SHOT | PendingIntent.FLAG_CANCEL_CURRENT);
        builder.setContentIntent(pendingIntent);
        Notification notification = new WearableNotifications.Builder(builder)
                .setMinPriority()
                .addRemoteInputForContentIntent(
                        new RemoteInput.Builder(EXTRA_REPLY)
                                .setLabel(getString(R.string.reply)).build())
                .build();
        NotificationManagerCompat.from(this).notify(0, notification);
    }

    private void processResponse(Intent intent) {
        String text = intent.getStringExtra(EXTRA_REPLY);
        if (text != null && !text.equals("")) {
            mLastResponse = mResponder.elzTalk(text);
            mHistoryView.append("\n" + text + "\n" + mLastResponse);
            showNotification();
        }
    }
}
