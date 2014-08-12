/**
 * Copyright 2013 Google Inc. All Rights Reserved.
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

package com.google.android.gms.drive.sample.quickeditor;

import com.google.android.gms.common.api.PendingResult;
import com.google.android.gms.common.api.ResultCallback;
import com.google.android.gms.common.api.Status;
import com.google.android.gms.common.ConnectionResult;
import com.google.android.gms.drive.Contents;
import com.google.android.gms.drive.Drive;
import com.google.android.gms.drive.DriveApi.ContentsResult;
import com.google.android.gms.drive.DriveFile;
import com.google.android.gms.drive.DriveId;
import com.google.android.gms.drive.DriveResource.MetadataResult;
import com.google.android.gms.drive.Metadata;
import com.google.android.gms.drive.MetadataChangeSet;
import com.google.android.gms.drive.OpenFileActivityBuilder;
import com.google.android.gms.drive.sample.quickeditor.tasks.EditDriveFileAsyncTask;

import android.content.Intent;
import android.content.IntentSender;
import android.content.IntentSender.SendIntentException;
import android.os.Bundle;
import android.util.Log;
import android.view.Menu;
import android.view.MenuInflater;
import android.view.MenuItem;
import android.view.View;
import android.view.View.OnClickListener;
import android.widget.Button;
import android.widget.EditText;
import android.widget.Toast;

import java.io.IOException;

/**
 * An activity lets you open/create a Drive text file and modify.
 */
public class HomeActivity extends BaseDriveActivity {

    private static final String TAG = "MainActivity";

    /**
     * Request code for creator activity.
     */
    private static final int REQUEST_CODE_CREATOR = NEXT_AVAILABLE_REQUEST_CODE;
    /**
     * Request code for the opener activity.
     */
    private static final int REQUEST_CODE_OPENER = NEXT_AVAILABLE_REQUEST_CODE + 1;

    /**
     * Text file mimetype.
     */
    private static final String MIME_TYPE_TEXT = "text/plain";

    /**
     * Title edit text field.
     */
    private EditText mTitleEditText;
    /**
     * Body edit text field.
     */
    private EditText mContentsEditText;
    /**
     * Save button. Invokes the upsert tasks on click.
     */
    private Button mSaveButton;

    /**
     * Drive ID of the currently opened Drive file.
     */
    private DriveId mCurrentDriveId;
    /**
     * Currently opened file's metadata.
     */
    private Metadata mMetadata;
    /**
     * Currently opened file's contents.
     */
    private Contents mContents;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);

        mTitleEditText = (EditText) findViewById(R.id.editTextTitle);
        mContentsEditText = (EditText) findViewById(R.id.editTextContents);
        mSaveButton = (Button) findViewById(R.id.buttonSave);
        mSaveButton.setOnClickListener(new OnClickListener() {
            @Override
            public void onClick(View v) {
                save();
            }
        });
        refresh();
    }

    @Override
    public boolean onCreateOptionsMenu(Menu menu) {
        MenuInflater inflater = getMenuInflater();
        inflater.inflate(R.menu.activity_main, menu);
        return super.onCreateOptionsMenu(menu);
    }

    @Override
    public void onConnected(Bundle connectionHint) {
        super.onConnected(connectionHint);
        refresh();
    }

    @Override
    public void onConnectionFailed(ConnectionResult result) {
        super.onConnectionFailed(result);
        showToast(R.string.msg_errconnect);
    }

    @Override
    public boolean onMenuItemSelected(int featureId, MenuItem item) {
        if (item.getItemId() == R.id.menu_new) {
            ResultCallback<ContentsResult> onContentsCallback =
                    new ResultCallback<ContentsResult>() {
                @Override
                public void onResult(ContentsResult result) {
                    // TODO: error handling in case of failure
                    MetadataChangeSet metadataChangeSet = new MetadataChangeSet.Builder()
                            .setMimeType(MIME_TYPE_TEXT).build();
                    IntentSender createIntentSender = Drive.DriveApi
                            .newCreateFileActivityBuilder()
                            .setInitialMetadata(metadataChangeSet)
                            .setInitialContents(result.getContents())
                            .build(mGoogleApiClient);
                    try {
                        startIntentSenderForResult(createIntentSender, REQUEST_CODE_CREATOR, null,
                                0, 0, 0);
                    } catch (SendIntentException e) {
                        Log.w(TAG, "Unable to send intent", e);
                    }
                }
            };
            Drive.DriveApi.newContents(mGoogleApiClient).setResultCallback(onContentsCallback);
        } else if (item.getItemId() == R.id.menu_open) {
            IntentSender i = Drive.DriveApi
                    .newOpenFileActivityBuilder()
                    .setMimeType(new String[] { MIME_TYPE_TEXT })
                    .build(mGoogleApiClient);
            try {
                startIntentSenderForResult(i, REQUEST_CODE_OPENER, null, 0, 0, 0);
            } catch (SendIntentException e) {
                Log.w(TAG, "Unable to send intent", e);
            }
        }
        return super.onMenuItemSelected(featureId, item);
    }

    /**
     * Handles activity results for creator and the opener activities.
     */
    @Override
    protected void onActivityResult(int requestCode, int resultCode,
            Intent data) {
        switch (requestCode) {
            case REQUEST_CODE_CREATOR:
                if (resultCode == RESULT_OK) {
                    mCurrentDriveId = (DriveId) data.getParcelableExtra(
                            OpenFileActivityBuilder.EXTRA_RESPONSE_DRIVE_ID);
                    refresh();
                }
                break;
            case REQUEST_CODE_OPENER:
                if (resultCode == RESULT_OK) {
                    mCurrentDriveId = (DriveId) data.getParcelableExtra(
                            OpenFileActivityBuilder.EXTRA_RESPONSE_DRIVE_ID);
                    get();
                }
                break;
            default:
                super.onActivityResult(requestCode, resultCode, data);
        }
    }

    /**
     * Refreshes the main content view with the current activity state.
     */
    private void refresh() {
        Log.d(TAG, "Refreshing...");
        if (mCurrentDriveId == null) {
            mSaveButton.setEnabled(false);
            return;
        }
        mSaveButton.setEnabled(true);

        if (mMetadata == null || mContents == null) {
            return;
        }

        mTitleEditText.setText(mMetadata.getTitle());
        try {
            String contents = Utils.readFromInputStream(mContents.getInputStream());
            mContentsEditText.setText(contents);
        } catch (IOException e) {
            // TODO: handle it better, at least an error message
            Log.e(TAG, "IOException while reading from contents input stream", e);
        }
    }

    /**
     * Retrieves the currently selected Drive file's meta data and contents.
     */
    private void get() {
        Log.d(TAG, "Retrieving...");
        DriveFile file = Drive.DriveApi.getFile(mGoogleApiClient, mCurrentDriveId);
        final PendingResult<MetadataResult>
                metadataResult = file.getMetadata(mGoogleApiClient);
        final PendingResult<ContentsResult>
                contentsResult = file.openContents(mGoogleApiClient,
                DriveFile.MODE_READ_ONLY | DriveFile.MODE_WRITE_ONLY, null);
    }

    /**
     * Saves meta data and content changes.
     */
    private void save() {
        Log.d(TAG, "Saving...");
        if (mCurrentDriveId == null) {
            return;
        }
        new EditDriveFileAsyncTask(mGoogleApiClient) {
            @Override
            public Changes edit(Contents contents) {
                MetadataChangeSet metadataChangeSet = new MetadataChangeSet.Builder()
                        .setTitle(mTitleEditText.getText().toString()).build();
                try {
                    byte[] body = mContentsEditText.getText().toString().getBytes();
                    contents.getOutputStream().write(body);
                } catch (IOException e) {
                    Log.e(TAG, "IOException while reading from contents output stream", e);
                }
                return new Changes(metadataChangeSet, contents);
            }

            @Override
            protected void onPostExecute(com.google.android.gms.common.api.Status status) {
                if (!status.getStatus().isSuccess()) {
                    showToast(R.string.msg_errsaving);
                    return;
                }
                showToast(R.string.msg_saved);
            }
        }.execute(mCurrentDriveId);
    }

    /**
     * Shows a toast with the given message.
     */
    private void showToast(int id) {
        Toast.makeText(this, id, Toast.LENGTH_LONG).show();
    }

}
