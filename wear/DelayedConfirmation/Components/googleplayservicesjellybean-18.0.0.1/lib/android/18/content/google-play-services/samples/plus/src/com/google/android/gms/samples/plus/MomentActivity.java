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

import com.google.android.gms.common.api.CommonStatusCodes;
import com.google.android.gms.common.api.GoogleApiClient;
import com.google.android.gms.common.api.ResultCallback;
import com.google.android.gms.common.api.Status;
import com.google.android.gms.common.ConnectionResult;
import com.google.android.gms.common.GooglePlayServicesUtil;
import com.google.android.gms.plus.Moments;
import com.google.android.gms.plus.Plus;
import com.google.android.gms.plus.Plus.PlusOptions;
import com.google.android.gms.plus.model.moments.ItemScope;
import com.google.android.gms.plus.model.moments.Moment;

import android.app.Activity;
import android.app.AlertDialog;
import android.app.Dialog;
import android.content.DialogInterface;
import android.content.Intent;
import android.content.IntentSender;
import android.os.Build;
import android.os.Bundle;
import android.util.Log;
import android.view.MenuItem;
import android.view.View;
import android.widget.AdapterView;
import android.widget.AdapterView.OnItemClickListener;
import android.widget.ArrayAdapter;
import android.widget.ListAdapter;
import android.widget.ListView;
import android.widget.TextView;
import android.widget.Toast;

/**
 * Example of writing moments through the GoogleApiClient.
 */
public class MomentActivity extends Activity implements OnItemClickListener,
        GoogleApiClient.ConnectionCallbacks, GoogleApiClient.OnConnectionFailedListener,
        ResultCallback<Status>, DialogInterface.OnCancelListener {

    private static final String TAG = "MomentActivity";

    private static final String STATE_RESOLVING_ERROR = "resolvingError";

    private static final int DIALOG_GET_GOOGLE_PLAY_SERVICES = 1;

    private static final int REQUEST_CODE_SIGN_IN = 1;
    private static final int REQUEST_CODE_GET_GOOGLE_PLAY_SERVICES = 2;

    private GoogleApiClient mGoogleApiClient;
    private ListAdapter mListAdapter;
    private ListView mMomentListView;
    private boolean mResolvingError;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.multi_moment_activity);
        PlusOptions options = PlusOptions.builder().addActivityTypes(MomentUtil.ACTIONS).build();
        mGoogleApiClient = new GoogleApiClient.Builder(this)
                .addConnectionCallbacks(this)
                .addOnConnectionFailedListener(this)
                .addApi(Plus.API, options)
                .addScope(Plus.SCOPE_PLUS_LOGIN)
                .build();

        mListAdapter = new ArrayAdapter<String>(
                this, android.R.layout.simple_list_item_1, MomentUtil.MOMENT_LIST);
        mMomentListView = (ListView) findViewById(R.id.moment_list);
        mMomentListView.setOnItemClickListener(this);
        mResolvingError = savedInstanceState != null
                && savedInstanceState.getBoolean(STATE_RESOLVING_ERROR, false);

        int available = GooglePlayServicesUtil.isGooglePlayServicesAvailable(this);
        if (available != CommonStatusCodes.SUCCESS) {
            showDialog(DIALOG_GET_GOOGLE_PLAY_SERVICES);
        }

        if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.HONEYCOMB) {
            getActionBar().setDisplayHomeAsUpEnabled(true);
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
    protected void onStop() {
        mGoogleApiClient.disconnect();
        super.onStop();
    }

    @Override
    public void onResult(Status status) {
        switch (status.getStatusCode()) {
            case CommonStatusCodes.SUCCESS:
                Toast.makeText(this, getString(R.string.plus_write_moment_status_success),
                        Toast.LENGTH_SHORT).show();
                break;

            case CommonStatusCodes.SUCCESS_CACHE:
                Toast.makeText(this, getString(R.string.plus_write_moment_status_cached),
                        Toast.LENGTH_SHORT).show();
                break;

            case CommonStatusCodes.SIGN_IN_REQUIRED:
                Toast.makeText(this, getString(R.string.plus_write_moment_status_auth_error),
                        Toast.LENGTH_SHORT).show();
                mGoogleApiClient.disconnect();
                mGoogleApiClient.connect();
                break;

            default:
                Toast.makeText(this, getString(R.string.plus_write_moment_status_error),
                        Toast.LENGTH_SHORT).show();
                Log.e(TAG, "Error when writing moments: " + status);
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
            if (!mGoogleApiClient.isConnecting() && !mGoogleApiClient.isConnected()) {
                mGoogleApiClient.connect();
            }
        } else {
            Log.e(TAG, "Unable to sign the user in.");
            finish();
        }
    }

    @Override
    protected void onSaveInstanceState(Bundle outState) {
        super.onSaveInstanceState(outState);
        outState.putBoolean(STATE_RESOLVING_ERROR, mResolvingError);
    }

    @Override
    public void onItemClick(AdapterView<?> adapterView, View view, int i, long l) {
        if (mGoogleApiClient.isConnected()) {
            TextView textView = (TextView) view;
            String momentType = (String) textView.getText();
            String targetUrl = MomentUtil.MOMENT_TYPES.get(momentType);

            ItemScope target = new ItemScope.Builder().setUrl(targetUrl).build();

            Moment.Builder momentBuilder = new Moment.Builder();
            momentBuilder.setType("http://schemas.google.com/" + momentType);
            momentBuilder.setTarget(target);

            ItemScope result = MomentUtil.getResultFor(momentType);
            if (result != null) {
                momentBuilder.setResult(result);
            }

            Plus.MomentsApi.write(mGoogleApiClient, momentBuilder.build()).setResultCallback(this);
        }
    }

    @Override
    public void onConnected(Bundle connectionHint) {
        mMomentListView.setAdapter(mListAdapter);
    }

    @Override
    public void onConnectionSuspended(int cause) {
        mMomentListView.setAdapter(null);
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
            // Reconnect to get another intent to start.
            mGoogleApiClient.connect();
        }
    }

    @Override
    public void onCancel(DialogInterface dialogInterface) {
        Log.e(TAG, "Unable to sign the user in.");
        finish();
    }

    @Override
    public boolean onOptionsItemSelected(MenuItem item) {
        switch (item.getItemId()) {
            case android.R.id.home:
                Intent intent = new Intent(this, PlusSampleActivity.class);
                intent.addFlags(Intent.FLAG_ACTIVITY_CLEAR_TOP);
                startActivity(intent);
                finish();
                return true;

            default:
                return super.onOptionsItemSelected(item);
        }
    }
}
