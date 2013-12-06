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

import com.google.android.gms.common.ConnectionResult;
import com.google.android.gms.common.GooglePlayServicesUtil;
import com.google.android.gms.plus.PlusClient;
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
        PlusClient.ConnectionCallbacks, PlusClient.OnConnectionFailedListener,
        DialogInterface.OnCancelListener {
    protected static final String TAG = "ShareActivity";

    private static final String STATE_SHARING = "state_sharing";

    private static final int DIALOG_GET_GOOGLE_PLAY_SERVICES = 1;

    private static final int REQUEST_CODE_SIGN_IN = 1;
    private static final int REQUEST_CODE_INTERACTIVE_POST = 2;
    private static final int REQUEST_CODE_GET_GOOGLE_PLAY_SERVICES = 3;

    /** The button should say "View item" in English. */
    private static final String LABEL_VIEW_ITEM = "VIEW_ITEM";

    private EditText mEditSendText;
    private boolean mSharing;
    private PlusClient mPlusClient;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.share_activity);

        mPlusClient = new PlusClient.Builder(this, this, this)
                .setActions(MomentUtil.ACTIONS)
                .build();

        Button sendButton = (Button) findViewById(R.id.send_interactive_button);
        sendButton.setOnClickListener(this);

        mEditSendText = (EditText) findViewById(R.id.share_prefill_edit);
        mSharing = savedInstanceState != null
                && savedInstanceState.getBoolean(STATE_SHARING, false);

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
    protected void onSaveInstanceState(Bundle outState) {
        super.onSaveInstanceState(outState);
        outState.putBoolean(STATE_SHARING, mSharing);
    }

    @Override
    public void onClick(View view) {
        switch (view.getId()) {
            case R.id.send_interactive_button:
                if (!mPlusClient.isConnected()) {
                    // Set sharing so that the share is started in onConnected.
                    mSharing = true;

                    if (!mPlusClient.isConnecting()) {
                        mPlusClient.connect();
                    }
                } else {
                    startActivityForResult(
                            getInteractivePostIntent(), REQUEST_CODE_INTERACTIVE_POST);
                }
        }
    }

    @Override
    protected void onActivityResult(int requestCode, int resultCode, Intent intent) {
        switch (requestCode) {
            case REQUEST_CODE_SIGN_IN:
            case REQUEST_CODE_GET_GOOGLE_PLAY_SERVICES:
                handleResult(resultCode);
                break;

            case REQUEST_CODE_INTERACTIVE_POST:
                mSharing = false;
                if (resultCode != RESULT_OK) {
                    Log.e(TAG, "Failed to create interactive post");
                }
                break;
        }
    }

    private void handleResult(int resultCode) {
        if (resultCode == RESULT_OK) {
            // onActivityResult is called after onStart (but onStart is not
            // guaranteed to be called while signing in), so we should make
            // sure we're not already connecting before we call connect again.
            if (!mPlusClient.isConnecting() && !mPlusClient.isConnected()) {
                mPlusClient.connect();
            }
        } else {
            Log.e(TAG, "Unable to sign the user in.");
            finish();
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
        PlusShare.Builder builder = new PlusShare.Builder(this, mPlusClient);

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
    public void onConnected(Bundle connectionHint) {
        if (!mSharing) {
            // The share button hasn't been clicked yet.
            return;
        }

        mSharing = false;
        startActivityForResult(getInteractivePostIntent(), REQUEST_CODE_INTERACTIVE_POST);
    }

    @Override
    public void onDisconnected() {
        // Do nothing.
    }

    @Override
    public void onConnectionFailed(ConnectionResult result) {
        if (!mSharing) {
            return;
        }

        try {
            result.startResolutionForResult(this, REQUEST_CODE_SIGN_IN);
        } catch (IntentSender.SendIntentException e) {
            // Try to connect again and get another intent to start.
            mPlusClient.connect();
        }
    }

    @Override
    public void onCancel(DialogInterface dialogInterface) {
        Log.e(TAG, "Unable to sign the user in.");
        finish();
    }
}
