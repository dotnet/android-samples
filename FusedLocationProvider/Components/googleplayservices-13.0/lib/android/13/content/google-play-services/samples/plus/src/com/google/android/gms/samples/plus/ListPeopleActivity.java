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
import com.google.android.gms.plus.model.people.PersonBuffer;

import android.app.Activity;
import android.app.AlertDialog;
import android.app.Dialog;
import android.content.DialogInterface;
import android.content.Intent;
import android.content.IntentSender;
import android.os.Bundle;
import android.util.Log;
import android.widget.ArrayAdapter;
import android.widget.ListView;

import java.util.ArrayList;

/**
 * Example of listing people with the Google+ APIs.
 */
public class ListPeopleActivity extends Activity implements PlusClient.ConnectionCallbacks,
        PlusClient.OnPeopleLoadedListener, PlusClient.OnConnectionFailedListener,
        DialogInterface.OnCancelListener {

    private static final String TAG = "ListPeopleActivity";

    private static final String STATE_RESOLVING_ERROR = "resolving_error";

    private static final int DIALOG_GET_GOOGLE_PLAY_SERVICES = 1;

    private static final int REQUEST_CODE_SIGN_IN = 1;
    private static final int REQUEST_CODE_GET_GOOGLE_PLAY_SERVICES = 2;

    private ArrayAdapter mListAdapter;
    private ListView mPersonListView;
    private ArrayList<String> mListItems;
    private PlusClient mPlusClient;
    private boolean mResolvingError;

    @Override
    public void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.person_list_activity);

        mPlusClient = new PlusClient.Builder(this, this, this)
                .setActions(MomentUtil.ACTIONS)
                .build();

        mListItems = new ArrayList<String>();
        mListAdapter = new ArrayAdapter<String>(this,
                android.R.layout.simple_list_item_1, mListItems);
        mPersonListView = (ListView) findViewById(R.id.person_list);
        mResolvingError = savedInstanceState != null
                && savedInstanceState.getBoolean(STATE_RESOLVING_ERROR, false);

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
    protected void onStart() {
        super.onStart();
        mPlusClient.connect();
    }

    @Override
    protected void onSaveInstanceState(Bundle outState) {
        super.onSaveInstanceState(outState);
        outState.putBoolean(STATE_RESOLVING_ERROR, mResolvingError);
    }

    @Override
    protected void onStop() {
        super.onStop();
        mPlusClient.disconnect();
    }

    @Override
    public void onPeopleLoaded(ConnectionResult status, PersonBuffer personBuffer,
            String nextPageToken) {
        switch (status.getErrorCode()) {
            case ConnectionResult.SUCCESS:
                mListItems.clear();
                try {
                    int count = personBuffer.getCount();
                    for (int i = 0; i < count; i++) {
                        mListItems.add(personBuffer.get(i).getDisplayName());
                    }
                } finally {
                    personBuffer.close();
                }

                mListAdapter.notifyDataSetChanged();
                break;

            case ConnectionResult.SIGN_IN_REQUIRED:
                mPlusClient.disconnect();
                mPlusClient.connect();
                break;

            default:
                Log.e(TAG, "Error when listing people: " + status);
                break;
        }
    }

    @Override
    public void onActivityResult(int requestCode, int resultCode, Intent data) {
        switch (requestCode) {
            case REQUEST_CODE_SIGN_IN:
                mResolvingError = false;
                handleResult(resultCode);
                break;
            case REQUEST_CODE_GET_GOOGLE_PLAY_SERVICES:
                handleResult(resultCode);
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

    @Override
    public void onConnected(Bundle connectionHint) {
        mPersonListView.setAdapter(mListAdapter);
        mPlusClient.loadVisiblePeople(this, null);
    }

    @Override
    public void onDisconnected() {
        mPersonListView.setAdapter(null);
        mPlusClient.connect();
    }

    @Override
    public void onConnectionFailed(ConnectionResult result) {
        if (mResolvingError) {
            return;
        }

        mPersonListView.setAdapter(null);
        try {
            result.startResolutionForResult(this, REQUEST_CODE_SIGN_IN);
            mResolvingError = true;
        } catch (IntentSender.SendIntentException e) {
            // Get another pending intent to run.
            mPlusClient.connect();
        }
    }

    @Override
    public void onCancel(DialogInterface dialogInterface) {
        Log.e(TAG, "Unable to sign the user in.");
        finish();
    }
}
