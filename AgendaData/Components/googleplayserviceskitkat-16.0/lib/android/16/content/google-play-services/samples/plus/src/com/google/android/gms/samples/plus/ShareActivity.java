/*
 * Copyright (C) 2012 The Android Open Source Project
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

package com.google.android.gms.samples.plus;

import com.google.android.gms.common.api.GoogleApiClient;
import com.google.android.gms.common.ConnectionResult;
import com.google.android.gms.common.GooglePlayServicesUtil;
import com.google.android.gms.plus.Plus;
import com.google.android.gms.plus.PlusShare;

import android.app.Activity;
import android.app.AlertDialog;
import android.app.Dialog;
import android.content.DialogInterface;
import android.content.Intent;
import android.content.IntentSender;
import android.net.Uri;
import android.os.Bundle;
import android.util.Log;
import android.view.View;
import android.widget.Button;
import android.widget.EditText;

/**
 * Example of sharing with Google+ through the ACTION_SEND intent.
 */
public class ShareActivity extends Activity implements View.OnClickListener,
        DialogInterface.OnCancelListener {
    protected static final String TAG = "ShareActivity";

    private static final String STATE_SHARING = "state_sharing";

    private static final int DIALOG_GET_GOOGLE_PLAY_SERVICES = 1;

    private static final int REQUEST_CODE_INTERACTIVE_POST = 1;
    private static final int REQUEST_CODE_GET_GOOGLE_PLAY_SERVICES = 2;

    /** The button should say "View item" in English. */
    private static final String LABEL_VIEW_ITEM = "VIEW_ITEM";

    private EditText mEditSendText;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.share_activity);

        Button sendButton = (Button) findViewById(R.id.send_interactive_button);
        sendButton.setOnClickListener(this);

        mEditSendText = (EditText) findViewById(R.id.share_prefill_edit);
        int available = GooglePlayServicesUtil.isGooglePlayServicesAvailable(this);
        if (available != ConnectionResult.SUCCESS) {
            showDialog(DIALOG_GET_GOOGLE_PLAY_SERVICES);
        }
    }

    @Override
    protected Dialog onCreateDialog(int id) {
        if (id != DIALOG_GET_GOOGLE_PLAY_SERVICES) {
            return super.onCreateDialog(id);
        }

        int available = GooglePlayServicesUtil.isGooglePlayServicesAvailable(this);
        if (available == ConnectionResult.SUCCESS) {
            return null;
        }
        if (GooglePlayServicesUtil.isUserRecoverableError(available)) {
            return GooglePlayServicesUtil.getErrorDialog(
                    available, this, REQUEST_CODE_GET_GOOGLE_PLAY_SERVICES, this);
        }
        return new AlertDialog.Builder(this)
                .setMessage(R.string.plus_generic_error)
                .setCancelable(true)
                .setOnCancelListener(this)
                .create();
    }

    @Override
    public void onClick(View view) {
        switch (view.getId()) {
            case R.id.send_interactive_button:
                startActivityForResult(getInteractivePostIntent(), REQUEST_CODE_INTERACTIVE_POST);
                return;
        }
    }

    @Override
    protected void onActivityResult(int requestCode, int resultCode, Intent intent) {
        switch (requestCode) {
            case REQUEST_CODE_GET_GOOGLE_PLAY_SERVICES:
                if (resultCode != RESULT_OK) {
                    Log.e(TAG, "Unable to sign the user in.");
                    finish();
                }
                break;

            case REQUEST_CODE_INTERACTIVE_POST:
                if (resultCode != RESULT_OK) {
                    Log.e(TAG, "Failed to create interactive post");
                }
                break;
        }
    }

    private Intent getInteractivePostIntent() {
        // Create an interactive post with the "VIEW_ITEM" label. This will
        // create an enhanced share dialog when the post is shared on Google+.
        // When the user clicks on the deep link, ParseDeepLinkActivity will
        // immediately parse the deep link, and route to the appropriate resource.
        String action = "/?view=true";
        Uri callToActionUrl = Uri.parse(getString(R.string.plus_example_deep_link_url) + action);
        String callToActionDeepLinkId = getString(R.string.plus_example_deep_link_id) + action;

        // Create an interactive post builder.
        PlusShare.Builder builder = new PlusShare.Builder(this);

        // Set call-to-action metadata.
        builder.addCallToAction(LABEL_VIEW_ITEM, callToActionUrl, callToActionDeepLinkId);

        // Set the target url (for desktop use).
        builder.setContentUrl(Uri.parse(getString(R.string.plus_example_deep_link_url)));

        // Set the target deep-link ID (for mobile use).
        builder.setContentDeepLinkId(getString(R.string.plus_example_deep_link_id),
                null, null, null);

        // Set the pre-filled message.
        builder.setText(mEditSendText.getText().toString());

        return builder.getIntent();
    }

    @Override
    public void onCancel(DialogInterface dialogInterface) {
        Log.e(TAG, "Unable to sign the user in.");
        finish();
    }
}
