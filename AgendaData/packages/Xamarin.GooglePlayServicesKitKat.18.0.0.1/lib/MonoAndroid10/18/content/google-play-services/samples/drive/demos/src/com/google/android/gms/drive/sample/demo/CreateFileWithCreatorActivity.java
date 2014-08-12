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

package com.google.android.gms.drive.sample.demo;

import android.content.Intent;
import android.content.IntentSender;
import android.content.IntentSender.SendIntentException;
import android.os.Bundle;
import android.util.Log;

import com.google.android.gms.common.api.ResultCallback;
import com.google.android.gms.drive.Drive;
import com.google.android.gms.drive.DriveApi.ContentsResult;
import com.google.android.gms.drive.DriveId;
import com.google.android.gms.drive.MetadataChangeSet;
import com.google.android.gms.drive.OpenFileActivityBuilder;

/**
 * An activity that illustrates how to use the creator
 * intent to create a new file. The creator intent allows the user
 * to select the parent folder and the title of the newly
 * created file.
 */
public class CreateFileWithCreatorActivity extends BaseDemoActivity {

    private static final String TAG = "CreateFileWithCreatorActivity";

    protected static final int REQUEST_CODE_CREATOR = 1;

    @Override
    public void onConnected(Bundle connectionHint) {
        super.onConnected(connectionHint);
        Drive.DriveApi.newContents(getGoogleApiClient())
                .setResultCallback(contentsCallback);
    }

    @Override
    protected void onActivityResult(int requestCode, int resultCode, Intent data) {
        switch (requestCode) {
        case REQUEST_CODE_CREATOR:
            if (resultCode == RESULT_OK) {
                DriveId driveId = (DriveId) data.getParcelableExtra(
                        OpenFileActivityBuilder.EXTRA_RESPONSE_DRIVE_ID);
                showMessage("File created with ID: " + driveId);
            }
            finish();
            break;
        default:
            super.onActivityResult(requestCode, resultCode, data);
            break;
        }
    }

    final ResultCallback<ContentsResult> contentsCallback = new ResultCallback<ContentsResult>() {
        @Override
        public void onResult(ContentsResult result) {
            MetadataChangeSet metadataChangeSet = new MetadataChangeSet.Builder()
                    .setMimeType("text/html").build();
            IntentSender intentSender = Drive.DriveApi
                    .newCreateFileActivityBuilder()
                    .setInitialMetadata(metadataChangeSet)
                    .setInitialContents(result.getContents())
                    .build(getGoogleApiClient());
            try {
                startIntentSenderForResult(
                        intentSender, REQUEST_CODE_CREATOR, null, 0, 0, 0);
            } catch (SendIntentException e) {
                Log.w(TAG, "Unable to send intent", e);
            }
        }
    };
}
