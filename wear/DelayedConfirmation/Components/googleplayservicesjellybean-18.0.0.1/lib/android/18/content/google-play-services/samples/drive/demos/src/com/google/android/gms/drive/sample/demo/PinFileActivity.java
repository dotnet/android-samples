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

package com.google.android.gms.drive.sample.demo;

import android.content.Intent;
import android.content.IntentSender;
import android.content.IntentSender.SendIntentException;
import android.os.Bundle;
import android.util.Log;

import com.google.android.gms.common.api.ResultCallback;
import com.google.android.gms.drive.Drive;
import com.google.android.gms.drive.DriveFile;
import com.google.android.gms.drive.DriveId;
import com.google.android.gms.drive.DriveResource.MetadataResult;
import com.google.android.gms.drive.MetadataChangeSet;
import com.google.android.gms.drive.OpenFileActivityBuilder;

/**
 * An activity that pins a file to the device. Pinning allows
 * a file's latest version to be available locally all the time.
 * Your users should be informed about the extra bandwidth
 * and storage requirements of pinning.
 */
public class PinFileActivity extends BaseDemoActivity {

    private static final int REQUEST_CODE_OPENER = NEXT_AVAILABLE_REQUEST_CODE;

    private static final String TAG = "PinFileActivity";

    private DriveId mFileId;

    /**
     * Starts a file opener intent to pick a file.
     */
    @Override
    public void onConnected(Bundle connectionHint) {
        super.onConnected(connectionHint);
        if (mFileId == null) {
            IntentSender intentSender = Drive.DriveApi
                    .newOpenFileActivityBuilder()
                    .setMimeType(new String[] {"application/octet-stream"})
                    .build(getGoogleApiClient());
            try {
                startIntentSenderForResult(intentSender, REQUEST_CODE_OPENER,
                        null, 0, 0, 0);
            } catch (SendIntentException e) {
                Log.w(TAG, "Unable to send intent", e);
            }
        } else {
            DriveFile file = Drive.DriveApi.getFile(getGoogleApiClient(), mFileId);
            file.getMetadata(getGoogleApiClient()).setResultCallback(metadataCallback);
        }
    }

    /**
     * Handles response from the file picker.
     */
    @Override
    protected void onActivityResult(int requestCode, int resultCode, Intent data) {
        switch (requestCode) {
        case REQUEST_CODE_OPENER:
            if (resultCode == RESULT_OK) {
                mFileId = (DriveId) data.getParcelableExtra(
                        OpenFileActivityBuilder.EXTRA_RESPONSE_DRIVE_ID);
            } else {
                finish();
            }
            break;
        default:
            super.onActivityResult(requestCode, resultCode, data);
        }
    }

    /**
     * Handles the metadata response. If file is pinnable and not
     * already pinned, makes a request to pin the file.
     */
    final ResultCallback<MetadataResult> metadataCallback = new ResultCallback<MetadataResult>() {
        @Override
        public void onResult(MetadataResult result) {
            if (!result.getStatus().isSuccess()) {
                showMessage("Problem while trying to retrieve the file metadata");
                return;
            }
            if (result.getMetadata().isPinnable()) {
                showMessage("File is not pinnable");
                return;
            }
            if (result.getMetadata().isPinned()) {
                showMessage("File is already pinned");
                return;
            }
            DriveFile file = Drive.DriveApi.getFile(getGoogleApiClient(), mFileId);
            MetadataChangeSet changeSet = new MetadataChangeSet.Builder()
                    .setPinned(true)
                    .build();
            file.updateMetadata(getGoogleApiClient(), changeSet)
                    .setResultCallback(pinningCallback);
        }
    };

    /**
     * Handles the pinning request's response.
     */
    final ResultCallback<MetadataResult> pinningCallback = new ResultCallback<MetadataResult>() {
        @Override
        public void onResult(MetadataResult result) {
            if (!result.getStatus().isSuccess()) {
                showMessage("Problem while trying to pin the file");
                return;
            }
            showMessage("File successfully pinned to the device");
        }
    };
}
