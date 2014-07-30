package com.google.android.gms.drive.sample.realtimeplayground;

import com.google.android.gms.drive.Drive;
import com.google.android.gms.drive.DriveId;
import com.google.android.gms.drive.MetadataChangeSet;
import com.google.android.gms.drive.OpenFileActivityBuilder;

import android.app.Activity;
import android.content.Intent;
import android.content.IntentSender;
import android.os.Bundle;
import android.util.Log;
import android.view.View;
import android.widget.Button;

public class RealtimePlaygroundHomeActivity extends BaseDriveActivity {

    private static final String TAG = "RealtimePlaygroundHomeActivity";

    private static final int OPEN_CREATE_REQUEST_CODE =
            BaseDriveActivity.NEXT_AVAILABLE_REQUEST_CODE;

    private Button mCreateButton;
    private Button mOpenButton;

    @Override
    public void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_realtime_playground_home);

        mCreateButton = (Button) findViewById(R.id.create_button);
        mCreateButton.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {
                doCreate();
            }
        });
        mCreateButton.setEnabled(false);
        mOpenButton = (Button) findViewById(R.id.open_button);
        mOpenButton.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {
                doOpen();
            }
        });
        mOpenButton.setEnabled(false);
    }

    @Override
    protected void onClientConnected() {
        mCreateButton.setEnabled(true);
        mOpenButton.setEnabled(true);
    }

    private void doCreate() {
        MetadataChangeSet metadataChangeSet = new MetadataChangeSet.Builder()
                .setTitle("New Realtime Playground").build();
        IntentSender intentSender = Drive.DriveApi
                .newCreateShortcutFileActivityBuilder()
                .setInitialMetadata(metadataChangeSet)
                .build(mGoogleApiClient);
        try {
            startIntentSenderForResult(intentSender, OPEN_CREATE_REQUEST_CODE, null, 0, 0, 0);
        } catch (IntentSender.SendIntentException e) {
            Log.w(TAG, "Unable to send intent", e);
        }
    }

    private void doOpen() {
        IntentSender intentSender = Drive.DriveApi
            .newOpenFileActivityBuilder()
            .setMimeType(new String[] { "application/vnd.google-apps.drive-sdk.840867953062" })
            .build(mGoogleApiClient);
        try {
            startIntentSenderForResult(
                    intentSender, OPEN_CREATE_REQUEST_CODE, null, 0, 0, 0);
        } catch (IntentSender.SendIntentException e) {
            Log.w(TAG, "Unable to send intent", e);
        }
    }

    @Override
    protected void onActivityResult(int requestCode, int resultCode, Intent data) {
        super.onActivityResult(requestCode, resultCode, data);
        if (requestCode == OPEN_CREATE_REQUEST_CODE && resultCode == Activity.RESULT_OK) {
            openRealtimePlaygroundFile(data);
        } else {
            Log.d(TAG, "Not OK activity result");
        }
    }

    private void openRealtimePlaygroundFile(Intent data) {
        DriveId id = data.getParcelableExtra(
                OpenFileActivityBuilder.EXTRA_RESPONSE_DRIVE_ID);
        Intent intent = new Intent(this, PlaygroundDocumentActivity.class);
        intent.putExtra(PlaygroundDocumentActivity.EXTRA_DRIVE_ID, id);
        intent.putExtra(PlaygroundDocumentActivity.EXTRA_ACCOUNT_NAME, getAccountName());
        startActivity(intent);
    }
}
