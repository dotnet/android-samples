// Copyright 2013 Google Inc. All Rights Reserved.

package com.google.android.gms.drive.sample.realtimeplayground;

import com.google.android.gms.common.ConnectionResult;
import com.google.android.gms.common.GooglePlayServicesUtil;
import com.google.android.gms.common.api.GoogleApiClient;
import com.google.android.gms.drive.Drive;

import android.accounts.Account;
import android.accounts.AccountManager;
import android.content.Intent;
import android.content.IntentSender.SendIntentException;
import android.os.Bundle;
import android.support.v7.app.ActionBarActivity;
import android.util.Log;

/**
 * The BaseDriveActivity handles authentication and the connection to the Drive services.  Each
 * activity that interacts with Drive should extend this class.
 * <p>The connection is requested in onStart, and disconnected in onStop unless you override
 * {@link #disconnectOnStop()} to return false, in which case disconnection will happen in
 * onDestroy.
 * <p>Extend {@link #onClientConnected()} to be notified when the connection is active.
 */
public abstract class BaseDriveActivity extends ActionBarActivity
        implements GoogleApiClient.ConnectionCallbacks, GoogleApiClient.OnConnectionFailedListener {

    private static final String TAG = "BaseDriveActivity";

    protected static final String EXTRA_ACCOUNT_NAME = "accountName";

    // Magic value indicating use the GMS Core default account
    protected static final String DEFAULT_ACCOUNT = "DEFAULT ACCOUNT";

    protected static final int RESOLVE_CONNECTION_REQUEST_CODE = 1;
    protected static final int NEXT_AVAILABLE_REQUEST_CODE = 2;

    // This variable can only be accessed from the UI thread.
    protected GoogleApiClient mGoogleApiClient;

    protected String mAccountName;

    @Override
    protected void onCreate(Bundle b) {
        super.onCreate(b);
        // Determine the active account:
        // In the saved instance bundle?
        // In the intent?
        // If not found, use the default account.
        if (b != null) {
            mAccountName = b.getString(EXTRA_ACCOUNT_NAME);
        }
        if (mAccountName == null) {
            mAccountName = getIntent().getStringExtra(EXTRA_ACCOUNT_NAME);
        }
        if (mAccountName == null) {
            Account[] accounts = AccountManager.get(this).getAccountsByType("com.google");
            if (accounts.length > 0) {
                mAccountName = accounts[0].name;
                Log.d(TAG, "No account specified, selecting " + mAccountName);
            } else {
                mAccountName = DEFAULT_ACCOUNT;
                Log.d(TAG, "No enabled accounts, changing to DEFAULT ACCOUNT");
            }
        }

        setupGoogleApiClient();
    }

    /**
     * Switches the account name to be used for the GoogleApiClient. This will disconnect any
     * existing client connection and try to connect to the new one. Caller should wait for the
     * {@link #onClientConnected()} callback before trying to use the client.
     *
     * @param accountName The new account name to be used.
     */
    protected void switchAccount(String accountName) {
        disconnectGoogleApiClient();
        cleanupGoogleApiClient();
        mAccountName = accountName;
        setupGoogleApiClient();
        connectGoogleApiClient();
    }

    protected void setupGoogleApiClient() {
        Log.d(TAG, "API client setup");
        cleanupGoogleApiClient();

        GoogleApiClient.Builder builder = new GoogleApiClient.Builder(this)
                .addApi(Drive.API)
                .addConnectionCallbacks(this)
                .addOnConnectionFailedListener(this)
                .addScope(Drive.SCOPE_APPFOLDER)
                .addScope(Drive.SCOPE_FILE);
        // If account name is unset in the builder, the default is used
        if (!DEFAULT_ACCOUNT.equals(mAccountName)) {
            builder.setAccountName(mAccountName);
        } else {
            Log.d(TAG, "No account specified, selecting default account.");
        }
        mGoogleApiClient = builder.build();
    }

    private void cleanupGoogleApiClient() {
        if (mGoogleApiClient != null) {
            mGoogleApiClient.unregisterConnectionCallbacks(this);
            mGoogleApiClient.unregisterConnectionFailedListener(this);
            mGoogleApiClient = null;
        }
    }

    /**
     * Invoked when the drive client has successfully connected.  This can be used by extending
     * activities to perform actions once the client is fully initialized.
     */
    protected abstract void onClientConnected();

    protected String getAccountName() {
        return mAccountName;
    }

    @Override
    protected void onSaveInstanceState(Bundle outState) {
        super.onSaveInstanceState(outState);
        outState.putString(EXTRA_ACCOUNT_NAME, mAccountName);
    }

    @Override
    protected void onStart() {
        Log.d(TAG, "onResume");
        super.onStart();
        disconnectGoogleApiClient();
        cleanupGoogleApiClient();
        setupGoogleApiClient();
        connectGoogleApiClient();
    }

    @Override
    protected void onActivityResult(int requestCode, int resultCode, Intent data) {
        switch (requestCode) {
            case RESOLVE_CONNECTION_REQUEST_CODE:
                handleResultConnectionResult(resultCode);
                break;
            default:
                Log.w(TAG, "Unexpected activity request code" + requestCode);
        }
    }

    private void handleResultConnectionResult(int resultCode) {
        switch (resultCode) {
            case RESULT_OK:
                connectGoogleApiClient();
                break;
            default:
                Log.w(TAG, "Canceled request to connect to Google Play Services: " + resultCode);
        }
    }


    @Override
    protected void onPause() {
        Log.d(TAG, "onPause");
        super.onPause();
    }

    @Override
    protected void onStop() {
        Log.d(TAG, "onStop");
        disconnectGoogleApiClient();
        cleanupGoogleApiClient();
        super.onStop();
    }

    /**
     * Initiates a connection request (if not already connected) that will result in a call to
     * {@link #onClientConnected}.
     */
    protected void connectGoogleApiClient() {
        if (!mGoogleApiClient.isConnected()) {
            Log.i(TAG, "Connecting to GoogleApiClient");
            mGoogleApiClient.connect();
        }
    }

    /**
     * Disconnects the client if currently connected.  Provided for components that want to
     * override the default behavior of disconnecting in {@link #onStop}.
     */
    private void disconnectGoogleApiClient() {
        if (mGoogleApiClient != null) {
            Log.i(TAG, "Disconnecting from GoogleApiClient");
            mGoogleApiClient.disconnect();
        }
    }

    @Override
    public void onConnected(Bundle connectionHint) {
        Log.i(TAG, "GoogleApiClient connected");
        onClientConnected();
    }

    @Override
    public void onConnectionSuspended(int cause) {
        Log.i(TAG, "GoogleApiClient connections suspended");
    }

    @Override
    public void onConnectionFailed(ConnectionResult result) {
        Log.i(TAG, "Connection failed: " + result.getErrorCode());
        if (!result.hasResolution()) {
            GooglePlayServicesUtil.showErrorDialogFragment(result.getErrorCode(), this, 0);
            return;
        }
        // If user interaction is required to resolve the connection failure, the result will
        // contain a resolution.  This will launch a UI that allows the user to resolve the issue.
        // (E.g., authorize your app.)
        try {
            result.startResolutionForResult(this, RESOLVE_CONNECTION_REQUEST_CODE);
        } catch (SendIntentException e) {
            Log.i(TAG, "Send intent failed", e);
        }
    }
}
