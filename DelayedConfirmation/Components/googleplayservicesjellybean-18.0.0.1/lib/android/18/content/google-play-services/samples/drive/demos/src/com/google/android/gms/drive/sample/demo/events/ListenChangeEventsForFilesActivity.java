/**
 * Copyright 2014 Google Inc. All Rights Reserved.
 *
 *  Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except
 * in compliance with the License. You may obtain a copy of the License at
 *
 *  http://www.apache.org/licenses/LICENSE-2.0
 *
 *  Unless required by applicable law or agreed to in writing, software distributed under the
 * License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either
 * express or implied. See the License for the specific language governing permissions and
 * limitations under the License.
 */

package com.google.android.gms.drive.sample.demo.events;

import android.content.Intent;
import android.content.IntentSender;
import android.content.IntentSender.SendIntentException;
import android.os.Bundle;
import android.util.Log;
import android.view.View;
import android.view.View.OnClickListener;
import android.widget.Button;
import android.widget.TextView;

import com.google.android.gms.drive.Drive;
import com.google.android.gms.drive.DriveFile;
import com.google.android.gms.drive.DriveId;
import com.google.android.gms.drive.OpenFileActivityBuilder;
import com.google.android.gms.drive.events.ChangeEvent;
import com.google.android.gms.drive.events.DriveEvent.Listener;
import com.google.android.gms.drive.sample.demo.BaseDemoActivity;
import com.google.android.gms.drive.sample.demo.R;

/**
 * An activity that listens change events on a user-picked file.
 */
public class ListenChangeEventsForFilesActivity extends BaseDemoActivity {

    private static final String TAG = "ListenChangeEventsForFiles";

    private static final int REQUEST_CODE_OPENER = NEXT_AVAILABLE_REQUEST_CODE;

    /*
     * Toggles file change event listening.
     */
    private Button mActionButton;

    /**
     * Displays the change event on the screen.
     */
    private TextView mLogTextView;

    /**
     * Represents the file picked by the user.
     */
    private DriveId mSelectedFileId;

    /**
     * Keeps the status whether change events are being listened to or not.
     */
    private boolean isSubscribed = false;

    final private Object mSubscriptionStatusLock = new Object();

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_changeevents);

        mLogTextView = (TextView) findViewById(R.id.textViewLog);
        mActionButton = (Button) findViewById(R.id.buttonAction);
        mActionButton.setOnClickListener(new OnClickListener() {
            @Override
            public void onClick(View v) {
                toggle();
            }
        });
        refresh();
    }

    /**
     * Refreshes the status of UI elements. Enables/disables subscription button
     * depending on whether there is file picked by the user.
     */
    private void refresh() {
        if (mSelectedFileId == null) {
            mActionButton.setEnabled(false);
        } else {
            mActionButton.setEnabled(true);
        }

        synchronized (mSubscriptionStatusLock) {
            if (!isSubscribed) {
                mActionButton.setText(R.string.button_subscribe);
            } else {
                mActionButton.setText(R.string.button_unsubscribe);
            }
        }
    }

    /**
     * Toggles the subscription status. If there is no selected file, returns
     * immediately.
     */
    private void toggle() {
        if (mSelectedFileId == null) {
            return;
        }
        synchronized (mSubscriptionStatusLock) {
            DriveFile file = Drive.DriveApi.getFile(getGoogleApiClient(),
                    mSelectedFileId);
            if (!isSubscribed) {
                Log.d(TAG, "Starting to listen to the file changes.");
                file.addChangeListener(getGoogleApiClient(), changeListener);
                isSubscribed = true;
            } else {
                Log.d(TAG, "Stopping to listen to the file changes.");
                file.removeChangeListener(getGoogleApiClient(), changeListener);
                isSubscribed = false;
            }
        }
        refresh();
    }

    /**
     * Forces user to pick a file on connection.
     */
    @Override
    public void onConnected(Bundle connectionHint) {
        if (mSelectedFileId == null) {
            IntentSender intentSender = Drive.DriveApi
                    .newOpenFileActivityBuilder()
                    .setMimeType(new String[] { "text/plain" })
                    .build(getGoogleApiClient());
            try {
                startIntentSenderForResult(intentSender, REQUEST_CODE_OPENER,
                        null, 0, 0, 0);
            } catch (SendIntentException e) {
                Log.w(TAG, "Unable to send intent", e);
            }
        }
    }

    /**
     * Handles response from file picker.
     */
    @Override
    protected void onActivityResult(int requestCode, int resultCode, Intent data) {
        switch (requestCode) {
        case REQUEST_CODE_OPENER:
            if (resultCode == RESULT_OK) {
                mSelectedFileId = (DriveId) data.getParcelableExtra(
                        OpenFileActivityBuilder.EXTRA_RESPONSE_DRIVE_ID);
                refresh();
            } else {
                finish();
            }
            break;
        default:
            super.onActivityResult(requestCode, resultCode, data);
        }
    }

    /**
     * A listener to handle file change events.
     */
    final private Listener<ChangeEvent> changeListener = new Listener<ChangeEvent>() {
        @Override
        public void onEvent(ChangeEvent event) {
            mLogTextView.setText(String.format("File change event: %s", event));
        }
    };

}
