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

import com.google.android.gms.common.ConnectionResult;
import com.google.android.gms.common.GooglePlayServicesUtil;
import com.google.android.gms.common.api.GoogleApiClient;
import com.google.android.gms.common.api.GoogleApiClient.ConnectionCallbacks;
import com.google.android.gms.common.api.GoogleApiClient.OnConnectionFailedListener;
import com.google.android.gms.wallet.FullWallet;
import com.google.android.gms.wallet.FullWalletRequest;
import com.google.android.gms.wallet.MaskedWallet;
import com.google.android.gms.wallet.NotifyTransactionStatusRequest;
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
import android.view.LayoutInflater;
import android.view.View;
import android.view.View.OnClickListener;
import android.view.ViewGroup;
import android.widget.Button;

import java.lang.ref.WeakReference;

/**
 * This is a fragment that handles the creating and sending of a {@link FullWalletRequest} using
 * {@link Wallet#loadFullWallet(GoogleApiClient, FullWalletRequest, int)}. This fragment renders
 * a button which hides the complexity of managing Google Play Services connection states,
 * creation and sending of requests and handling responses. Applications may use this fragment as
 * a drop in replacement of a confirmation button in case the user has chosen to use Google Wallet.
 */
public class FullWalletConfirmationButtonFragment extends Fragment implements ConnectionCallbacks,
        OnConnectionFailedListener, OnClickListener {

    /**
     * Request code used when attempting to resolve issues with connecting to Google Play Services.
     * Only use this request code when calling {@link ConnectionResult#startResolutionForResult(
     * android.app.Activity, int)}.
     */
    public static final int REQUEST_CODE_RESOLVE_ERR = 1003;

    /**
     * Request code used when loading a full wallet. Only use this request code when calling
     * {@link Wallet#loadFullWallet(GoogleApiClient, FullWalletRequest, int)}.
     */
    public static final int REQUEST_CODE_RESOLVE_LOAD_FULL_WALLET = 1004;

    // Maximum number of times to try to connect to GoogleApiClient if the connection is failing
    private static final int MAX_RETRIES = 3;
    private static final long INITIAL_RETRY_DELAY_MILLISECONDS = 3000;
    private static final int MESSAGE_RETRY_CONNECTION = 1010;
    private static final String KEY_RETRY_COUNTER = "KEY_RETRY_COUNTER";
    private static final String KEY_HANDLE_FULL_WALLET_WHEN_READY =
            "KEY_HANDLE_FULL_WALLET_WHEN_READY";

    // No. of times to retry loadFullWallet on receiving a ConnectionResult.INTERNAL_ERROR
    private static final int MAX_FULL_WALLET_RETRIES = 1;
    private static final String KEY_RETRY_FULL_WALLET_COUNTER = "KEY_RETRY_FULL_WALLET_COUNTER";

    private int mRetryCounter = 0;
    // handler for processing retry attempts
    private RetryHandler mRetryHandler;

    protected GoogleApiClient mGoogleApiClient;
    protected ProgressDialog mProgressDialog;
    // whether the user tried to do an action that requires a full wallet (i.e.: loadFullWallet)
    // before a full wallet was acquired (i.e.: still waiting for mGoogleApiClient to connect)
    protected boolean mHandleFullWalletWhenReady = false;
    protected int mItemId;

    // Cached connection result for resolving connection failures on user action.
    protected ConnectionResult mConnectionResult;

    private ItemInfo mItemInfo;
    private Button mConfirmButton;
    private MaskedWallet mMaskedWallet;
    private int mRetryLoadFullWalletCount = 0;
    private Intent mActivityLaunchIntent;

    @Override
    public void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);

        if (savedInstanceState != null) {
            mRetryCounter = savedInstanceState.getInt(KEY_RETRY_COUNTER);
            mRetryLoadFullWalletCount = savedInstanceState.getInt(KEY_RETRY_FULL_WALLET_COUNTER);
            mHandleFullWalletWhenReady =
                    savedInstanceState.getBoolean(KEY_HANDLE_FULL_WALLET_WHEN_READY);
        }
        mActivityLaunchIntent = getActivity().getIntent();
        mItemId = mActivityLaunchIntent.getIntExtra(Constants.EXTRA_ITEM_ID, 0);
        mMaskedWallet = mActivityLaunchIntent.getParcelableExtra(Constants.EXTRA_MASKED_WALLET);

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
        outState.putBoolean(KEY_HANDLE_FULL_WALLET_WHEN_READY, mHandleFullWalletWhenReady);
        outState.putInt(KEY_RETRY_FULL_WALLET_COUNTER, mRetryLoadFullWalletCount);
    }

    @Override
    public View onCreateView(
            LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState) {

        initializeProgressDialog();
        View view = inflater.inflate(R.layout.fragment_full_wallet_confirmation_button, container,
                false);
        mItemInfo = Constants.ITEMS_FOR_SALE[mItemId];

        mConfirmButton = (Button) view.findViewById(R.id.button_place_order);
        mConfirmButton.setOnClickListener(this);
        return view;
    }

    @Override
    public void onClick(View v) {
        confirmPurchase();
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
    }

    @Override
    public void onConnectionSuspended(int cause) {
        // don't need to do anything here
    }

    @Override
    public void onConnectionFailed(ConnectionResult result) {
        mConnectionResult = result;

        // Handle the user's tap by dismissing the progress dialog and attempting to resolve the
        // connection result.
        if (mHandleFullWalletWhenReady) {
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

    @Override
    public void onActivityResult(int requestCode, int resultCode, Intent data) {
        mProgressDialog.hide();

        // retrieve the error code, if available
        int errorCode = -1;
        if (data != null) {
            errorCode = data.getIntExtra(WalletConstants.EXTRA_ERROR_CODE, -1);
        }

        switch (requestCode) {
            case REQUEST_CODE_RESOLVE_ERR:
                if (resultCode == Activity.RESULT_OK) {
                    mGoogleApiClient.connect();
                } else {
                    handleUnrecoverableGoogleWalletError(errorCode);
                }
                break;
            case REQUEST_CODE_RESOLVE_LOAD_FULL_WALLET:
                switch (resultCode) {
                    case Activity.RESULT_OK:
                        if (data.hasExtra(WalletConstants.EXTRA_FULL_WALLET)) {
                            FullWallet fullWallet =
                                    data.getParcelableExtra(WalletConstants.EXTRA_FULL_WALLET);
                            // the full wallet can now be used to process the customer's payment
                            // send the wallet info up to server to process, and to get the result
                            // for sending a transaction status
                            fetchTransactionStatus(fullWallet);
                        } else if (data.hasExtra(WalletConstants.EXTRA_MASKED_WALLET)) {
                            // re-launch the activity with new masked wallet information
                            mMaskedWallet =
                                    data.getParcelableExtra(WalletConstants.EXTRA_MASKED_WALLET);
                            mActivityLaunchIntent.putExtra(Constants.EXTRA_MASKED_WALLET,
                                    mMaskedWallet);
                            startActivity(mActivityLaunchIntent);
                        }
                        break;
                    case Activity.RESULT_CANCELED:
                        // nothing to do here
                        break;
                    default:
                        handleError(errorCode);
                        break;
                }
                break;
        }
    }

    /*package*/ void updateMaskedWallet(MaskedWallet maskedWallet) {
        mMaskedWallet = maskedWallet;
    }

    private void reconnect() {
        if (mRetryCounter < MAX_RETRIES) {
            mProgressDialog.show();
            Message m = mRetryHandler.obtainMessage(MESSAGE_RETRY_CONNECTION);
            // back off exponentially
            long delay = (long) (INITIAL_RETRY_DELAY_MILLISECONDS * Math.pow(2, mRetryCounter));
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

    private void handleError(int errorCode) {
        if (checkAndRetryFullWallet(errorCode)) {
            // handled by retrying
            return;
        }
        switch (errorCode) {
            case WalletConstants.ERROR_CODE_SPENDING_LIMIT_EXCEEDED:
                // may be recoverable if the user tries to lower their charge
                // take the user back to the checkout page to try to handle
            case WalletConstants.ERROR_CODE_INVALID_PARAMETERS:
            case WalletConstants.ERROR_CODE_AUTHENTICATION_FAILURE:
            case WalletConstants.ERROR_CODE_BUYER_ACCOUNT_ERROR:
            case WalletConstants.ERROR_CODE_MERCHANT_ACCOUNT_ERROR:
            case WalletConstants.ERROR_CODE_SERVICE_UNAVAILABLE:
            case WalletConstants.ERROR_CODE_UNSUPPORTED_API_VERSION:
            case WalletConstants.ERROR_CODE_UNKNOWN:
            default:
                // unrecoverable error
                // take the user back to the checkout page to handle these errors
                handleUnrecoverableGoogleWalletError(errorCode);
        }
    }

    private void confirmPurchase() {
        if (mConnectionResult != null) {
            // The user needs to resolve an issue before GoogleApiClient can connect
            resolveUnsuccessfulConnectionResult();
        } else {
            getFullWallet();
            mProgressDialog.setCancelable(false);
            mProgressDialog.show();
            mHandleFullWalletWhenReady = true;
        }
    }

    private void getFullWallet() {
        Wallet.Payments.loadFullWallet(mGoogleApiClient,
                WalletUtil.createFullWalletRequest(mItemInfo,
                        mMaskedWallet.getGoogleTransactionId()),
                REQUEST_CODE_RESOLVE_LOAD_FULL_WALLET);
    }

    /**
     * Here the client should connect to their server, process the credit card/instrument
     * and get back a status indicating whether charging the card was successful or not
     */
    private void fetchTransactionStatus(FullWallet fullWallet) {
        if (mProgressDialog.isShowing()) {
            mProgressDialog.dismiss();
        }
        // Send back details such as fullWallet.getProxyCard() and fullWallet.getBillingAddress()
        // and get back success or failure
        // The following code assumes a successful response and calls notifyTransactionStatus
        Wallet.Payments.notifyTransactionStatus(mGoogleApiClient,
                WalletUtil.createNotifyTransactionStatusRequest(fullWallet.getGoogleTransactionId(),
                        NotifyTransactionStatusRequest.Status.SUCCESS));

        Intent intent = new Intent(getActivity(), OrderCompleteActivity.class);
        intent.setFlags(Intent.FLAG_ACTIVITY_CLEAR_TASK | Intent.FLAG_ACTIVITY_NEW_TASK);
        intent.putExtra(Constants.EXTRA_FULL_WALLET, fullWallet);
        startActivity(intent);
    }

    /**
     * Retries {@link Wallet#loadFullWallet(GoogleApiClient, FullWalletRequest, int)} if
     * {@link #MAX_FULL_WALLET_RETRIES} has not been reached.
     *
     * @return {@code true} if {@link FullWalletConfirmationButtonFragment#getFullWallet()} is retried,
     *         {@code false} otherwise.
     */
    private boolean checkAndRetryFullWallet(int errorCode) {
        if ((errorCode == WalletConstants.ERROR_CODE_SERVICE_UNAVAILABLE ||
                errorCode == WalletConstants.ERROR_CODE_UNKNOWN) &&
                mRetryLoadFullWalletCount < MAX_FULL_WALLET_RETRIES) {
            mRetryLoadFullWalletCount++;
            getFullWallet();
            return true;
        }
        return false;
    }

    protected void initializeProgressDialog() {
        mProgressDialog = new ProgressDialog(getActivity());
        mProgressDialog.setMessage(getString(R.string.loading));
        mProgressDialog.setIndeterminate(true);
        mProgressDialog.setOnDismissListener(new OnDismissListener() {

            @Override
            public void onDismiss(DialogInterface dialog) {
                mHandleFullWalletWhenReady = false;
            }
        });
    }

    private static class RetryHandler extends Handler {

        private WeakReference<FullWalletConfirmationButtonFragment> mWeakReference;

        protected RetryHandler(FullWalletConfirmationButtonFragment fragment) {
            mWeakReference = new WeakReference<FullWalletConfirmationButtonFragment>(fragment);
        }

        @Override
        public void handleMessage(Message msg) {
            switch (msg.what) {
                case MESSAGE_RETRY_CONNECTION:
                    FullWalletConfirmationButtonFragment fragment = mWeakReference.get();
                    if (fragment != null) {
                        fragment.mGoogleApiClient.connect();
                    }
                    break;
            }
        }
    }
}
