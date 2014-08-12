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

package com.google.android.gms.samples.wallet;

import com.google.android.gms.common.api.GoogleApiClient;
import com.google.android.gms.common.api.GoogleApiClient.ConnectionCallbacks;
import com.google.android.gms.common.api.GoogleApiClient.OnConnectionFailedListener;
import com.google.android.gms.common.ConnectionResult;
import com.google.android.gms.common.GooglePlayServicesUtil;
import com.google.android.gms.wallet.FullWalletRequest;
import com.google.android.gms.wallet.MaskedWalletRequest;
import com.google.android.gms.wallet.Wallet;
import com.google.android.gms.wallet.WalletConstants;

import android.app.Activity;
import android.app.Dialog;
import android.app.ProgressDialog;
import android.content.DialogInterface;
import android.content.DialogInterface.OnCancelListener;
import android.content.DialogInterface.OnDismissListener;
import android.content.Intent;
import android.content.IntentSender;
import android.os.Bundle;
import android.os.Handler;
import android.os.Message;
import android.support.v4.app.Fragment;

import java.lang.ref.WeakReference;

/**
 * Base class for common functionality for Fragments that use {@code GoogleApiClient}.
 */
public abstract class BikestoreWalletFragment extends Fragment implements ConnectionCallbacks,
        OnConnectionFailedListener {

    /**
     * Request code used when attempting to resolve issues with connecting to Google Play Services.
     * Only use this request code when calling {@link ConnectionResult#startResolutionForResult(
     * android.app.Activity, int)}.
     */
    protected static final int REQUEST_CODE_RESOLVE_ERR = 1000;

    /**
     * Request code used when loading a masked wallet. Only use this request code when calling
     * {@link Wallet#loadMaskedWallet(GoogleApiClient, MaskedWalletRequest, int)}.
     */
    protected static final int REQUEST_CODE_RESOLVE_LOAD_MASKED_WALLET = 1001;

    /**
     * Request code used when loading a full wallet. Only use this request code when calling
     * {@link Wallet#loadFullWallet(GoogleApiClient, FullWalletRequest, int)}.
     */
    protected static final int REQUEST_CODE_RESOLVE_LOAD_FULL_WALLET = 1002;

    /**
     * Request code used when changing a masked wallet. Only use this request code when calling
     * {@link Wallet#changeMaskedWallet(GoogleApiClient, String, String, int)}.
     */
    protected static final int REQUEST_CODE_RESOLVE_CHANGE_MASKED_WALLET = 1003;

    // Maximum number of times to try to connect to GoogleApiClient if the connection is failing
    private static final int MAX_RETRIES = 3;
    private static final long INITIAL_RETRY_DELAY = 3000;
    private static final int MESSAGE_RETRY_CONNECTION = 1010;
    private static final String KEY_RETRY_COUNTER = "KEY_RETRY_COUNTER";
    private static final String KEY_HANDLE_MASKED_WALLET_WHEN_READY =
            "KEY_HANDLE_MASKED_WALLET_WHEN_READY";
    private static final String KEY_HANDLE_FULL_WALLET_WHEN_READY =
            "KEY_HANDLE_FULL_WALLET_WHEN_READY";

    private int mRetryCounter = 0;
    // handler for processing retry attempts
    private RetryHandler mRetryHandler;

    protected GoogleApiClient mGoogleApiClient;
    protected ProgressDialog mProgressDialog;
    // whether the user tried to do an action that requires a masked wallet (i.e.: loadMaskedWallet)
    // before a masked wallet was acquired (i.e. still waiting for mGoogleApiClient to connect)
    protected boolean mHandleMaskedWalletWhenReady = false;
    // whether the user tried to do an action that requires a full wallet (i.e.: loadFullWallet)
    // before a full wallet was acquired (i.e.: still waiting for mGoogleApiClient to connect)
    protected boolean mHandleFullWalletWhenReady = false;
    protected int mItemId;

    // Cached connection result for resolving connection failures on user action.
    protected ConnectionResult mConnectionResult;

    @Override
    public void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);

        if (savedInstanceState != null) {
            mRetryCounter = savedInstanceState.getInt(KEY_RETRY_COUNTER);
            mHandleMaskedWalletWhenReady =
                    savedInstanceState.getBoolean(KEY_HANDLE_MASKED_WALLET_WHEN_READY);
            mHandleFullWalletWhenReady =
                    savedInstanceState.getBoolean(KEY_HANDLE_FULL_WALLET_WHEN_READY);
        }
        mItemId = getActivity().getIntent().getIntExtra(Constants.EXTRA_ITEM_ID, 0);

        String accountName = getApplication().getAccountName();

        // Set up an API client;
        mGoogleApiClient = new GoogleApiClient.Builder(getActivity())
                .addConnectionCallbacks(this)
                .addOnConnectionFailedListener(this)
                .setAccountName(accountName)
                .addApi(Wallet.API, new Wallet.WalletOptions.Builder()
                    .setEnvironment(Constants.WALLET_ENVIRONMENT)
                    .setTheme(WalletConstants.THEME_HOLO_LIGHT)
                    .build())
                .build();

        mRetryHandler = new RetryHandler(this);
    }

    @Override
    public void onStart() {
        super.onStart();

        // Connect to Google Play Services
        mGoogleApiClient.connect();
    }

    @Override
    public void onSaveInstanceState(Bundle outState) {
        super.onSaveInstanceState(outState);
        outState.putInt(KEY_RETRY_COUNTER, mRetryCounter);
        outState.putBoolean(KEY_HANDLE_MASKED_WALLET_WHEN_READY, mHandleFullWalletWhenReady);
        outState.putBoolean(KEY_HANDLE_FULL_WALLET_WHEN_READY, mHandleFullWalletWhenReady);
    }

    @Override
    public void onStop() {
        super.onStop();

        // Disconnect from Google Play Services
        mGoogleApiClient.disconnect();

        if (mProgressDialog != null) {
            mProgressDialog.dismiss();
        }

        mRetryHandler.removeMessages(MESSAGE_RETRY_CONNECTION);
    }

    @Override
    public void onConnected(Bundle connectionHint) {
        // don't need to do anything here
        // subclasses may override if they need to do anything
    }

    @Override
    public void onConnectionSuspended(int cause) {
        // don't need to do anything here
        // subclasses may override if they need to do anything
    }

    @Override
    public void onConnectionFailed(ConnectionResult result) {
        mConnectionResult = result;

        // Handle the user's tap by dismissing the progress dialog and attempting to resolve the
        // connection result.
        if (mHandleMaskedWalletWhenReady || mHandleFullWalletWhenReady) {
            mProgressDialog.dismiss();
            resolveUnsuccessfulConnectionResult();
        }
    }

    public BikestoreApplication getApplication() {
        return (BikestoreApplication) getActivity().getApplication();
    }

    /**
     * Helper to try to resolve a user recoverable error (i.e. the user has an out of date version
     * of Google Play Services installed), via an error dialog provided by
     * {@link GooglePlayServicesUtil#getErrorDialog(int, Activity, int, OnCancelListener)}. If an,
     * error is not user recoverable then the error will be handled in {@link #handleError(int)}.
     */
    protected void resolveUnsuccessfulConnectionResult() {
        // Additional user input is needed
        if (mConnectionResult.hasResolution()) {
            try {
                mConnectionResult.startResolutionForResult(getActivity(), REQUEST_CODE_RESOLVE_ERR);
            } catch (IntentSender.SendIntentException e) {
                reconnect();
            }
        } else {
            int errorCode = mConnectionResult.getErrorCode();
            if (GooglePlayServicesUtil.isUserRecoverableError(errorCode)) {
                Dialog dialog = GooglePlayServicesUtil.getErrorDialog(errorCode, getActivity(),
                        REQUEST_CODE_RESOLVE_ERR, new OnCancelListener() {

                    @Override
                    public void onCancel(DialogInterface dialog) {
                        // get a new connection result
                        mGoogleApiClient.connect();
                    }
                });

                // the dialog will either be dismissed, which will invoke the OnCancelListener, or
                // the dialog will be addressed, which will result in a callback to
                // OnActivityResult()
                dialog.show();
            } else {
                switch (errorCode) {
                    case ConnectionResult.INTERNAL_ERROR:
                    case ConnectionResult.NETWORK_ERROR:
                        reconnect();
                        break;
                    default:
                        handleError(errorCode);
                }
            }
        }

        mConnectionResult = null;
    }

    private void reconnect() {
        if (mRetryCounter < MAX_RETRIES) {
            mProgressDialog.show();
            Message m = mRetryHandler.obtainMessage(MESSAGE_RETRY_CONNECTION);
            // back off exponentially
            long delay = (long) (INITIAL_RETRY_DELAY * Math.pow(2, mRetryCounter));
            mRetryHandler.sendMessageDelayed(m, delay);
            mRetryCounter++;
        } else {
            handleError(WalletConstants.ERROR_CODE_SERVICE_UNAVAILABLE);
        }
    }

    /**
     * For unrecoverable Google Wallet errors, send the user back to the checkout page to handle the
     * problem.
     *
     * @param errorCode
     */
    protected void handleUnrecoverableGoogleWalletError(int errorCode) {
        Intent intent = new Intent(getActivity(), CheckoutActivity.class);
        intent.addFlags(Intent.FLAG_ACTIVITY_CLEAR_TOP);
        intent.putExtra(WalletConstants.EXTRA_ERROR_CODE, errorCode);
        intent.putExtra(Constants.EXTRA_ITEM_ID, mItemId);
        startActivity(intent);
    }

    protected abstract void handleError(int errorCode);

    protected void initializeProgressDialog() {
        mProgressDialog = new ProgressDialog(getActivity());
        mProgressDialog.setMessage(getString(R.string.loading));
        mProgressDialog.setIndeterminate(true);
        mProgressDialog.setOnDismissListener(new OnDismissListener() {

            @Override
            public void onDismiss(DialogInterface dialog) {
                mHandleMaskedWalletWhenReady = false;
                mHandleFullWalletWhenReady = false;
            }
        });
    }

    private static class RetryHandler extends Handler {

        private WeakReference<BikestoreWalletFragment> mWeakReference;

        protected RetryHandler(BikestoreWalletFragment walletFragment) {
            mWeakReference = new WeakReference<BikestoreWalletFragment>(walletFragment);
        }

        @Override
        public void handleMessage(Message msg) {
            switch (msg.what) {
                case MESSAGE_RETRY_CONNECTION:
                    BikestoreWalletFragment walletFragment = mWeakReference.get();
                    if (walletFragment != null) {
                        walletFragment.mGoogleApiClient.connect();
                    }
                    break;
            }
        }
    }
}
