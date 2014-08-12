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
import com.google.android.gms.common.SignInButton;
import com.google.android.gms.common.api.GoogleApiClient;
import com.google.android.gms.common.api.GoogleApiClient.ConnectionCallbacks;
import com.google.android.gms.common.api.GoogleApiClient.OnConnectionFailedListener;
import com.google.android.gms.common.api.Scope;
import com.google.android.gms.plus.Plus;
import com.google.android.gms.plus.Plus.PlusOptions;
import com.google.android.gms.plus.model.people.Person;

import android.app.Activity;
import android.app.ProgressDialog;
import android.content.Intent;
import android.content.IntentSender.SendIntentException;
import android.os.Bundle;
import android.support.v4.app.Fragment;
import android.view.LayoutInflater;
import android.view.View;
import android.view.View.OnClickListener;
import android.view.ViewGroup;
import android.widget.Toast;

public class LoginFragment extends Fragment implements
        OnClickListener, ConnectionCallbacks, OnConnectionFailedListener {

    private static final String WALLET_SANDBOX_SCOPE =
            "https://www.googleapis.com/auth/paymentssandbox.make_payments";

    private static final String KEY_SIGNIN_BUTTON_CLICKED = "KEY_SIGNIN_BUTTON_CLICKED";
    private ProgressDialog mProgressDialog;
    private boolean mSignInButtonClicked = false;
    private GoogleApiClient mGoogleApiClient;
    private ConnectionResult mConnectionResult;
    private int mLoginAction;

    public static LoginFragment newInstance(int loginAction) {
        LoginFragment fragment = new LoginFragment();
        Bundle bundle = new Bundle();
        bundle.putInt(LoginActivity.EXTRA_ACTION, loginAction);
        fragment.setArguments(bundle);
        return fragment;
    }

    @Override
    public void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);

        if (savedInstanceState != null) {
            mSignInButtonClicked = savedInstanceState.getBoolean(KEY_SIGNIN_BUTTON_CLICKED);
        }
        Bundle args = getArguments();
        if (args != null) {
            mLoginAction = args.getInt(LoginActivity.EXTRA_ACTION);
        }
        PlusOptions options = PlusOptions.builder().build();
        mGoogleApiClient = new GoogleApiClient.Builder(getActivity())
                .addApi(Plus.API, options)
                .addConnectionCallbacks(this)
                .addOnConnectionFailedListener(this)
                .addScope(Plus.SCOPE_PLUS_PROFILE)
                .addScope(new Scope(WALLET_SANDBOX_SCOPE))
                .build();
    }

    @Override
    public void onSaveInstanceState(Bundle outState) {
        super.onSaveInstanceState(outState);
        outState.putBoolean(KEY_SIGNIN_BUTTON_CLICKED, mSignInButtonClicked);
    }

    @Override
    public View onCreateView(LayoutInflater inflater, ViewGroup container,
            Bundle savedInstanceState) {
        View view = inflater.inflate(R.layout.fragment_login, container, false);
        mProgressDialog = initializeProgressDialog();

        SignInButton signInButton = (SignInButton) view.findViewById(R.id.sign_in_button);
        signInButton.setSize(SignInButton.SIZE_WIDE);
        signInButton.setOnClickListener(this);

        view.findViewById(R.id.button_login_bikestore).setOnClickListener(this);
        return view;
    }

    @Override
    public void onStart() {
        super.onStart();
        mGoogleApiClient.connect();
    }

    @Override
    public void onStop() {
        super.onStop();
        if (mGoogleApiClient.isConnecting() || mGoogleApiClient.isConnected()) {
            mGoogleApiClient.disconnect();
        }
    }

    @Override
    public void onPause() {
        super.onPause();
        dismissProgressDialog();
    }

    @Override
    public void onActivityResult(int requestCode, int resultCode, Intent data) {
        switch (requestCode) {
            case BikestoreWalletFragment.REQUEST_CODE_RESOLVE_ERR:
                if (resultCode == Activity.RESULT_OK) {
                    if (!mGoogleApiClient.isConnecting() || !mGoogleApiClient.isConnected()) {
                        mGoogleApiClient.connect();
                        showProgressDialog();
                    }
                }
                break;
            default:
                super.onActivityResult(requestCode, resultCode, data);
                dismissProgressDialog();
                break;
        }
    }

    @Override
    public void onClick(View view) {
        switch (view.getId()) {
            case R.id.sign_in_button:
                showProgressDialog();
                if (mConnectionResult != null) {
                    resolveConnection();
                } else {
                    // for cases when button is clicked before any connection result
                    mSignInButtonClicked = true;
                }
                break;
            case R.id.button_login_bikestore:
                Toast.makeText(getActivity(), R.string.login_bikestore_message, Toast.LENGTH_LONG)
                        .show();
                break;
        }
    }

    @Override
    public void onConnected(Bundle connectionHint) {
        dismissProgressDialog();
        if (mLoginAction == LoginActivity.Action.LOGOUT) {
            logOut();
        } else {
            mSignInButtonClicked = false;
            logIn();
        }
    }

    @Override
    public void onConnectionFailed(ConnectionResult result) {
        // Save the intent so that we can start an activity when the user clicks
        // the sign-in button.
        mConnectionResult = result;
        if (mSignInButtonClicked) {
            resolveConnection();
        }
    }

    private void resolveConnection() {
        try {
            if (mConnectionResult != null && mConnectionResult.hasResolution()) {
                mConnectionResult.startResolutionForResult(getActivity(),
                        BikestoreWalletFragment.REQUEST_CODE_RESOLVE_ERR);
            }
        }  catch (SendIntentException e) {
            mConnectionResult = null;
            mGoogleApiClient.connect();
        }
    }

    private void logIn() {
        if (mGoogleApiClient.isConnected()) {
            Person user = Plus.PeopleApi.getCurrentPerson(mGoogleApiClient);
            String accountName = Plus.AccountApi.getAccountName(mGoogleApiClient);
            if (user == null) {
                Toast.makeText(getActivity(), R.string.network_error, Toast.LENGTH_LONG).show();
            } else {
                Toast.makeText(getActivity(), getString(R.string.welcome_user,
                        user.getDisplayName()), Toast.LENGTH_LONG).show();

                ((BikestoreApplication) getActivity().getApplication()).login(accountName);
                getActivity().setResult(Activity.RESULT_OK);
                getActivity().finish();
            }
        }
    }

    private void logOut() {
        if (mGoogleApiClient.isConnected()) {
            Plus.AccountApi.clearDefaultAccount(mGoogleApiClient);
            ((BikestoreApplication) getActivity().getApplication()).logout();
            mGoogleApiClient.disconnect();
            Toast.makeText(getActivity(), getString(R.string.logged_out), Toast.LENGTH_LONG)
                    .show();
            getActivity().setResult(Activity.RESULT_OK);
            getActivity().finish();
        }
    }

    private ProgressDialog initializeProgressDialog() {
        ProgressDialog dialog = new ProgressDialog(getActivity());
        dialog.setIndeterminate(true);
        dialog.setMessage(getString(R.string.loading));
        return dialog;
    }

    private void showProgressDialog() {
        if (mProgressDialog != null && !mProgressDialog.isShowing()) {
            mProgressDialog.show();
        }
    }

    private void dismissProgressDialog() {
        if (mProgressDialog != null && mProgressDialog.isShowing()) {
            mProgressDialog.dismiss();
        }
    }

    @Override
    public void onConnectionSuspended(int cause) {
        // nothing specifically required here, onConnected will be called when connection resumes
    }
}
