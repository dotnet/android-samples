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
import com.google.android.gms.common.api.GoogleApiClient;
import com.google.android.gms.common.api.GoogleApiClient.ConnectionCallbacks;
import com.google.android.gms.common.api.GoogleApiClient.OnConnectionFailedListener;
import com.google.android.gms.identity.intents.Address;
import com.google.android.gms.identity.intents.Address.AddressOptions;
import com.google.android.gms.identity.intents.AddressConstants;
import com.google.android.gms.identity.intents.UserAddressRequest;
import com.google.android.gms.identity.intents.model.UserAddress;
import com.google.android.gms.wallet.WalletConstants;

import android.app.Activity;
import android.app.ProgressDialog;
import android.content.Intent;
import android.content.IntentSender.SendIntentException;
import android.os.Bundle;
import android.support.v4.app.Fragment;
import android.text.Html;
import android.view.LayoutInflater;
import android.view.View;
import android.view.View.OnClickListener;
import android.view.ViewGroup;
import android.widget.TextView;
import android.widget.Toast;

public class PromoAddressLookupFragment extends Fragment implements
        OnClickListener, ConnectionCallbacks, OnConnectionFailedListener {

    private static final String KEY_PROMO_CLICKED = "KEY_PROMO_CLICKED";
    public static final int REQUEST_CODE_RESOLVE_ADDRESS_LOOKUP = 4000;
    public static final int REQUEST_CODE_RESOLVE_ERR = 4001;
    private ProgressDialog mProgressDialog;
    private GoogleApiClient mGoogleApiClient;
    private ConnectionResult mConnectionResult;
    private boolean mPromoWasSelected = false;

    @Override
    public void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        if (savedInstanceState != null) {
            mPromoWasSelected = savedInstanceState.getBoolean(KEY_PROMO_CLICKED);
        }
        String accountName =
                ((BikestoreApplication) getActivity().getApplication()).getAccountName();
        AddressOptions options = new AddressOptions(WalletConstants.THEME_HOLO_LIGHT);
        mGoogleApiClient = new GoogleApiClient.Builder(getActivity())
                .addApi(Address.API, options)
                .setAccountName(accountName)
                .addConnectionCallbacks(this)
                .addOnConnectionFailedListener(this)
                .build();
    }

    @Override
    public void onSaveInstanceState(Bundle outState) {
        super.onSaveInstanceState(outState);
        outState.putBoolean(KEY_PROMO_CLICKED, mPromoWasSelected);
    }

    @Override
    public View onCreateView(LayoutInflater inflater, ViewGroup container,
            Bundle savedInstanceState) {
        View view = inflater.inflate(R.layout.fragment_promo_address_lookup, container, false);
        // Styling the header with HTML elements in TextView
        TextView promoTitle = (TextView) view.findViewById(R.id.promo_title);
        promoTitle.setText(Html.fromHtml(getString(R.string.promo)));
        mProgressDialog = initializeProgressDialog();
        view.setOnClickListener(this);
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
            case REQUEST_CODE_RESOLVE_ERR:
                // call connect regardless of success or failure
                // if the result was success, the connect should succeed
                // if the result was not success, this should get a new connection result
                mGoogleApiClient.connect();
                break;
            case REQUEST_CODE_RESOLVE_ADDRESS_LOOKUP:
                dismissProgressDialog();
                mPromoWasSelected = false;
                switch (resultCode) {
                    case Activity.RESULT_OK:
                        ((BikestoreApplication) getActivity().getApplication())
                                .setAddressValidForPromo(true);
                        UserAddress userAddress = UserAddress.fromIntent(data);
                        Toast.makeText(getActivity(), getString(R.string.promo_eligible,
                                formatUsAddress(userAddress)), Toast.LENGTH_LONG).show();
                        break;
                    case Activity.RESULT_CANCELED:
                        break;
                    default:
                        Toast.makeText(getActivity(), getString(R.string.no_address),
                                Toast.LENGTH_LONG).show();
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
        if (mConnectionResult != null) {
            // If there was a connection failure, attempt to resolve the ConnectionResult
            // when the user taps the button
            resolveConnection();
        } else {
            lookupAddress();
        }
    }

    @Override
    public void onConnected(Bundle connectionHint) {
        if(mPromoWasSelected) {
            lookupAddress();
        }
    }

    @Override
    public void onConnectionFailed(ConnectionResult result) {
        // Save the intent so that we can start an lookup when the user clicks the promo.
        mConnectionResult = result;
        if (mPromoWasSelected) {
            resolveConnection();
        }
    }

    private void resolveConnection() {
        try {
            if (mConnectionResult != null && mConnectionResult.hasResolution()) {
                mConnectionResult.startResolutionForResult(getActivity(),
                        REQUEST_CODE_RESOLVE_ERR);
            } else {
                mGoogleApiClient.connect();
            }
        }  catch (SendIntentException e) {
            mConnectionResult = null;
            mGoogleApiClient.connect();
        }
    }

    private void lookupAddress() {
        if (mGoogleApiClient.isConnected()) {
            showProgressDialog();
            UserAddressRequest request = UserAddressRequest.newBuilder().build();
            Address.requestUserAddress(mGoogleApiClient, request,
                    REQUEST_CODE_RESOLVE_ADDRESS_LOOKUP);
        } else {
            if (!mGoogleApiClient.isConnecting()) {
                mGoogleApiClient.connect();
            }
            mPromoWasSelected = true;
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

    // Address formatting specific to the US, depending upon the countries supported you may
    // have different address formatting
    private static String formatUsAddress(UserAddress address) {
        StringBuilder builder = new StringBuilder();
        builder.append("\n");
        if (appendIfValid(address.getAddress1(), builder)) builder.append(", ");
        if (appendIfValid(address.getLocality(), builder)) builder.append(", ");
        if (appendIfValid(address.getAdministrativeArea(), builder)) builder.append(", ");
        appendIfValid(address.getCountryCode(), builder);
        return builder.toString();
    }

    private static boolean appendIfValid(String string, StringBuilder builder) {
        if (string != null && string.length() > 0) {
            builder.append(string);
            return true;
        }
        return false;
    }

    @Override
    public void onConnectionSuspended(int cause) {
        // nothing specifically required here, onConnected will be called when connection resumes
    }
}
