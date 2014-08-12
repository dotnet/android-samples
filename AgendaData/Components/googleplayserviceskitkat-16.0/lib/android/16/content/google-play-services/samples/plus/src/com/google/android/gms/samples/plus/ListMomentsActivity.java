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
import com.google.android.gms.common.api.CommonStatusCodes;
import com.google.android.gms.common.api.GoogleApiClient;
import com.google.android.gms.common.api.ResultCallback;
import com.google.android.gms.plus.Moments;
import com.google.android.gms.plus.Moments.LoadMomentsResult;
import com.google.android.gms.plus.Plus;
import com.google.android.gms.plus.Plus.PlusOptions;
import com.google.android.gms.plus.model.moments.Moment;
import com.google.android.gms.plus.model.moments.MomentBuffer;

import android.app.Activity;
import android.app.AlertDialog;
import android.app.Dialog;
import android.content.Context;
import android.content.DialogInterface;
import android.content.Intent;
import android.content.IntentSender;
import android.net.Uri;
import android.os.Bundle;
import android.util.Log;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.AdapterView;
import android.widget.AdapterView.OnItemClickListener;
import android.widget.ArrayAdapter;
import android.widget.ListView;
import android.widget.TextView;
import android.widget.Toast;

import java.util.ArrayList;

/**
 * Example of listing the current user's moments.
 */
public class ListMomentsActivity extends Activity implements GoogleApiClient.ConnectionCallbacks,
        GoogleApiClient.OnConnectionFailedListener, ResultCallback<LoadMomentsResult>,
        OnItemClickListener, DialogInterface.OnCancelListener {

    private static final String TAG = "MomentActivity";

    private static final String STATE_RESOLVING_ERROR = "resolving_error";

    private static final int DIALOG_GET_GOOGLE_PLAY_SERVICES = 1;

    private static final int REQUEST_CODE_SIGN_IN = 1;
    private static final int REQUEST_CODE_GET_GOOGLE_PLAY_SERVICES = 2;

    private ListView mMomentListView;
    private MomentListAdapter mMomentListAdapter;
    private ArrayList<Moment> mListItems;
    private boolean mResolvingError;

    private GoogleApiClient mGoogleApiClient;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.list_moments_activity);
        PlusOptions options = PlusOptions.builder().addActivityTypes(MomentUtil.ACTIONS).build();
        mGoogleApiClient = new GoogleApiClient.Builder(this)
                .addConnectionCallbacks(this)
                .addOnConnectionFailedListener(this)
                .addApi(Plus.API, options)
                .addScope(Plus.SCOPE_PLUS_LOGIN)
                .build();

        mListItems = new ArrayList<Moment>();
        mMomentListAdapter = new MomentListAdapter(this, android.R.layout.simple_list_item_1,
                mListItems);
        mMomentListView = (ListView) findViewById(R.id.moment_list);
        mMomentListView.setOnItemClickListener(this);
        mResolvingError = savedInstanceState != null
                && savedInstanceState.getBoolean(STATE_RESOLVING_ERROR, false);

        int available = GooglePlayServicesUtil.isGooglePlayServicesAvailable(this);
        if (available != CommonStatusCodes.SUCCESS) {
            showDialog(DIALOG_GET_GOOGLE_PLAY_SERVICES);
        }
    }

    @Override
    protected Dialog onCreateDialog(int id) {
        if (id != DIALOG_GET_GOOGLE_PLAY_SERVICES) {
            return super.onCreateDialog(id);
        }

        int available = GooglePlayServicesUtil.isGooglePlayServicesAvailable(this);
        if (available == CommonStatusCodes.SUCCESS) {
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
        mGoogleApiClient.connect();
    }

    @Override
    protected void onSaveInstanceState(Bundle outState) {
        super.onSaveInstanceState(outState);
        outState.putBoolean(STATE_RESOLVING_ERROR, mResolvingError);
    }

    @Override
    protected void onStop() {
        mGoogleApiClient.disconnect();
        super.onStop();
    }

    @Override
    public void onResult(LoadMomentsResult momentData) {
        switch (momentData.getStatus().getStatusCode()) {
            case CommonStatusCodes.SUCCESS:
                MomentBuffer momentBuffer = momentData.getMomentBuffer();
                mListItems.clear();
                try {
                    int count = momentBuffer.getCount();
                    for (int i = 0; i < count; i++) {
                        mListItems.add(momentBuffer.get(i).freeze());
                    }
                } finally {
                    momentBuffer.close();
                }

                mMomentListAdapter.notifyDataSetChanged();
                break;

            case CommonStatusCodes.SIGN_IN_REQUIRED:
                mGoogleApiClient.disconnect();
                mGoogleApiClient.connect();
                break;

            default:
                Log.e(TAG, "Error when listing moments: " + momentData.getStatus());
                break;
        }
    }

    /**
     * Delete a moment when clicked.
     */
    @Override
    public void onItemClick(AdapterView<?> parent, View view, int position, long id) {
        Moment moment = mMomentListAdapter.getItem(position);
        if (moment != null) {
            if (mGoogleApiClient.isConnected()) {
                Plus.MomentsApi.remove(mGoogleApiClient, moment.getId());
                Toast.makeText(this, getString(R.string.plus_remove_moment_status),
                        Toast.LENGTH_SHORT).show();
            } else {
                Toast.makeText(this, getString(R.string.greeting_status_sign_in_required),
                        Toast.LENGTH_SHORT).show();
            }
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
            if (!mGoogleApiClient.isConnecting() && !mGoogleApiClient.isConnected()) {
                mGoogleApiClient.connect();
            }
        } else {
            Log.e(TAG, "Unable to sign the user in.");
            finish();
        }
    }

    @Override
    public void onConnected(Bundle connectionHint) {
        Plus.MomentsApi.load(mGoogleApiClient).setResultCallback(this);
        mMomentListView.setAdapter(mMomentListAdapter);
    }

    @Override
    public void onConnectionSuspended(int cause) {
        mMomentListView.setAdapter(null);
        mGoogleApiClient.connect();
    }

    @Override
    public void onConnectionFailed(ConnectionResult result) {
        mMomentListView.setAdapter(null);
        if (mResolvingError) {
            return;
        }

        try {
            result.startResolutionForResult(this, REQUEST_CODE_SIGN_IN);
            mResolvingError = true;
        } catch (IntentSender.SendIntentException e) {
            // Try connecting again.
            mGoogleApiClient.connect();
        }
    }

    @Override
    public void onCancel(DialogInterface dialogInterface) {
        Log.e(TAG, "Unable to sign the user in.");
        finish();
    }

    /**
     * Array adapter that maintains a Moment list.
     */
    private class MomentListAdapter extends ArrayAdapter<Moment> {

        private final LayoutInflater mLayoutInflater;
        private final ArrayList<Moment> mItems;

        public MomentListAdapter(Context context, int textViewResourceId,
                ArrayList<Moment> objects) {
            super(context, textViewResourceId, objects);
            mLayoutInflater = (LayoutInflater) getSystemService(Context.LAYOUT_INFLATER_SERVICE);
            mItems = objects;

        }

        @Override
        public View getView(int position, View convertView, ViewGroup parent) {
            View resultView = convertView;
            if (resultView == null) {
                resultView = mLayoutInflater.inflate(R.layout.moment_row, null);
            }

            Moment moment = mItems.get(position);
            if (moment != null) {
                TextView typeView = (TextView) resultView.findViewById(R.id.moment_type);
                TextView titleView = (TextView) resultView.findViewById(R.id.moment_title);

                String type = Uri.parse(moment.getType()).getPath().substring(1);
                typeView.setText(type);

                if (moment.getTarget() != null) {
                    titleView.setText(moment.getTarget().getName());
                }
            }

            return resultView;
        }
    }
}
