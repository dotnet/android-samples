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
import com.google.android.gms.common.GooglePlayServicesClient.ConnectionCallbacks;
import com.google.android.gms.common.GooglePlayServicesClient.OnConnectionFailedListener;
import com.google.android.gms.common.Scopes;
import com.google.android.gms.common.SignInButton;
import com.google.android.gms.plus.PlusClient;
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

    // UI elements
    private ProgressDialog mProgressDialog;

    private PlusClient mPlusClient;
    private ConnectionResult mConnectionResult;

    @Override
    public void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);

        mPlusClient = new PlusClient.Builder(getActivity(), this, this)
                .setScopes(Scopes.PLUS_LOGIN,
                        "https://www.googleapis.com/auth/paymentssandbox.make_payments")
                .build();

        setRetainInstance(true);
    }

    @Override
    public View onCreateView(LayoutInflater inflater, ViewGroup container,
            Bundle savedInstanceState) {
        View view = inflater.inflate(R.layout.fragment_login, container, false);
        mProgressDialog = initializeProgressDialog();

        SignInButton signInButton = (SignInButton) view.findViewById(R.id.sign_in_button);
        signInButton.setSize(SignInButton.SIZE_WIDE);
        signInButton.setOnClickListener(this);

        view.findViewById(R.id.button_login_xyz).setOnClickListener(this);
        return view;
    }

    @Override
    public void onStart() {
        super.onStart();
    }

    @Override
    public void onStop() {
        super.onStop();
        if (mPlusClient.isConnecting() || mPlusClient.isConnected()) {
            mPlusClient.disconnect();
        }
    }

    @Override
    public void onActivityResult(int requestCode, int resultCode, Intent data) {
        switch (requestCode) {
            case XyzWalletFragment.REQUEST_CODE_RESOLVE_ERR:
                mConnectionResult = null;
                if (resultCode == Activity.RESULT_OK) {
                    if (!mPlusClient.isConnecting() || !mPlusClient.isConnected()) {
                        mPlusClient.connect();
                    }
                } else {
                    if (mProgressDialog.isShowing()) {
                        mProgressDialog.dismiss();
                    }
                }
                break;
            default:
                super.onActivityResult(requestCode, resultCode, data);
                break;
        }
    }

    @Override
    public void onClick(View view) {
        switch (view.getId()) {
            case R.id.sign_in_button:
                if (!mPlusClient.isConnected() || !mPlusClient.isConnecting()) {
                    mPlusClient.connect();
                    mProgressDialog.show();
                }
                break;
            case R.id.button_login_xyz:
                Toast.makeText(getActivity(), R.string.login_xyz_message, Toast.LENGTH_LONG)
                        .show();
                break;
        }
    }

    @Override
    public void onConnected(Bundle connectionHint) {
        if (mProgressDialog.isShowing()) {
            mProgressDialog.dismiss();
            // User was waiting for connection to complete. Continue login
            logIn();
        }
    }

    @Override
    public void onDisconnected() {
        // don't need to do anything here
    }

    @Override
    public void onConnectionFailed(ConnectionResult result) {
        // Save the intent so that we can start an activity when the user clicks
        // the sign-in button.
        mConnectionResult = result;

        if (mProgressDialog.isShowing()) {
            // The user clicked the sign-in button already.
            // Dismiss the progress dialog and start to resolve connection errors.
            mProgressDialog.dismiss();
            logIn();
        }
    }

    private void logIn() {
        if (mPlusClient.isConnected()) {
            Person user = mPlusClient.getCurrentPerson();
            if (user == null) {
                Toast.makeText(getActivity(), R.string.network_error, Toast.LENGTH_LONG).show();
            } else {
                Toast.makeText(getActivity(), getString(R.string.welcome_user,
                        user.getDisplayName()), Toast.LENGTH_LONG).show();

                ((XyzApplication) getActivity().getApplication())
                        .login(mPlusClient.getAccountName());

                getActivity().setResult(Activity.RESULT_OK);
                getActivity().finish();
            }
        } else if (mConnectionResult.hasResolution()) {
            mProgressDialog.show();
            try {
                mConnectionResult.startResolutionForResult(getActivity(),
                        XyzWalletFragment.REQUEST_CODE_RESOLVE_ERR);
            } catch (SendIntentException e) {
                // Try connecting again.
                mConnectionResult = null;
                mPlusClient.connect();
            }
        }
    }

    private ProgressDialog initializeProgressDialog() {
        ProgressDialog dialog = new ProgressDialog(getActivity());
        dialog.setIndeterminate(true);
        dialog.setMessage(getString(R.string.loading));
        return dialog;
    }
}
