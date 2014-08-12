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

import android.os.Bundle;

import com.google.android.gms.common.api.ResultCallback;
import com.google.android.gms.drive.Drive;
import com.google.android.gms.drive.DriveApi.DriveIdResult;
import com.google.android.gms.drive.DriveFile;
import com.google.android.gms.drive.DriveResource.MetadataResult;
import com.google.android.gms.drive.Metadata;

/**
 * An activity to retrieve the metadata of a file.
 */
public class RetrieveMetadataActivity extends BaseDemoActivity {

    @Override
    public void onConnected(Bundle connectionHint) {
      Drive.DriveApi.fetchDriveId(getGoogleApiClient(), EXISTING_FILE_ID)
              .setResultCallback(idCallback);
    }

    final private ResultCallback<DriveIdResult> idCallback = new ResultCallback<DriveIdResult>() {
        @Override
        public void onResult(DriveIdResult result) {
            if (!result.getStatus().isSuccess()) {
                showMessage("Cannot find DriveId. Are you authorized to view this file?");
                return;
            }
            DriveFile file = Drive.DriveApi.getFile(getGoogleApiClient(), result.getDriveId());
            file.getMetadata(getGoogleApiClient())
                    .setResultCallback(metadataCallback);
        }
    };

    final private ResultCallback<MetadataResult> metadataCallback = new
            ResultCallback<MetadataResult>() {
        @Override
        public void onResult(MetadataResult result) {
            if (!result.getStatus().isSuccess()) {
                showMessage("Problem while trying to fetch metadata");
                return;
            }
            Metadata metadata = result.getMetadata();
            showMessage("Metadata successfully fetched. Title: " + metadata.getTitle());
        }
    };
}
