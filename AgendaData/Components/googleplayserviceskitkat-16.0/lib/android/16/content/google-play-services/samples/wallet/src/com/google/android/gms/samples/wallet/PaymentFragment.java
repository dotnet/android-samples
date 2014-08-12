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
import com.google.android.gms.wallet.MaskedWallet;
import com.google.android.gms.wallet.Wallet;
import com.google.android.gms.wallet.WalletConstants;

import android.app.Activity;
import android.content.Intent;
import android.os.Bundle;
import android.view.LayoutInflater;
import android.view.View;
import android.view.View.OnClickListener;
import android.view.ViewGroup;
import android.widget.Button;
import android.widget.CompoundButton;
import android.widget.CompoundButton.OnCheckedChangeListener;
import android.widget.RadioButton;
import android.widget.RadioGroup;
import android.widget.TextView;
import android.widget.Toast;

/**
 * Payment entry page of the sample storefront.
 *
 * <p>When the customer visits this page, a {@link GoogleApiClient} connection should be made in order
 * to interact with the Google Wallet service.
 *
 * <p>The user may arrive at this page one of two ways:
 * <ul>
 *   <li>The user has logged in, but has not selected any payment option.<br />
 *   In this case, the Google Wallet logo will be shown as a payment option, and selecting it will
 *   behave similarly to clicking on the Buy with Google Wallet button on the checkout page.<br />
 *
 *   <li>The user clicked the Change button on the confirmation page.<br />
 *   The user's previously selected masked wallet information is shown, along with a Change button.
 *   If the change button is clicked, then
 *   {@link Wallet#changeMaskedWallet(GoogleApiClient, String, String, int)} will be
 *   called, which will bring up the wallet chooser interface again.  If the user chooses a new
 *   masked wallet, it will be returned in {@link #onActivityResult(int, int, Intent)}.
 * </ul>
 */
public class PaymentFragment extends BikestoreWalletFragment implements OnClickListener,
        OnCheckedChangeListener {

    private MaskedWallet mMaskedWallet;

    private RadioGroup mRadioGroup;
    private RadioButton mWalletRadioButton;
    private RadioButton mNewCardRadioButton;

    private View mEnterCardDetails;
    private View mMaskedWalletInfo;
    private TextView mEditButton;
    private Button mSubmitButton;
    private int mRequestCode;

    @Override
    public void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        mMaskedWallet = getActivity().getIntent().getParcelableExtra(Constants.EXTRA_MASKED_WALLET);
    }

    @Override
    public View onCreateView(
            LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState) {

        initializeProgressDialog();

        View view = inflater.inflate(R.layout.fragment_payment, container, false);
        mRadioGroup = (RadioGroup) view.findViewById(R.id.radio_group);
        mWalletRadioButton = (RadioButton) view.findViewById(R.id.radio_button_google_wallet);
        mWalletRadioButton.setOnCheckedChangeListener(this);

        mMaskedWalletInfo = view.findViewById(R.id.masked_wallet_info);
        mEditButton = (TextView) view.findViewById(R.id.button_change_google_wallet);

        // the customer can reach this page two ways:
        // if the user clicked on buy with GW on the checkout page, and then clicked on the change
        // button on the confirmation page, then the previously selected masked wallet info will be
        // displayed, along with a change button
        if (mMaskedWallet != null) {
            TextView email = (TextView) view.findViewById(R.id.text_username);
            email.setText(mMaskedWallet.getEmail());

            TextView paymentDescriptions =
                    (TextView) view.findViewById(R.id.text_payment_descriptions);
            paymentDescriptions.setText(Util.formatPaymentDescriptions(mMaskedWallet));

            mEditButton.setOnClickListener(this);
            mMaskedWalletInfo.setOnClickListener(this);

            mRequestCode = REQUEST_CODE_RESOLVE_CHANGE_MASKED_WALLET;
        } else {
            // the other possible way to reach this page is if the user hasn't selected any payment
            // info before, in which case the Buy with Google Wallet button will be displayed
            mWalletRadioButton.setCompoundDrawablesWithIntrinsicBounds(R.drawable.wallet_logo, 0, 0,
                    0);

            mMaskedWalletInfo.setVisibility(View.GONE);
            mEditButton.setVisibility(View.GONE);

            mRequestCode = REQUEST_CODE_RESOLVE_LOAD_MASKED_WALLET;
        }

        // if the user is logged in, show their saved credit cards
        if (getApplication().isLoggedIn()) {
            RadioButton saved = (RadioButton) inflater.inflate(R.layout.payment_option,
                    mRadioGroup, false);
            mRadioGroup.addView(saved, 1);
            saved.setOnCheckedChangeListener(this);
        }

        mEnterCardDetails = view.findViewById(R.id.enter_credit_card_details);
        mNewCardRadioButton = (RadioButton) view.findViewById(R.id.radio_button_add_card);
        mNewCardRadioButton.setOnCheckedChangeListener(this);

        mSubmitButton = (Button) view.findViewById(R.id.button_submit);
        mSubmitButton.setOnClickListener(this);

        mWalletRadioButton.setChecked(true);

        return view;
    }

    @Override
    public void onClick(View v) {
        if (v == mMaskedWalletInfo) {
            mWalletRadioButton.setChecked(true);
        } else if (v == mEditButton) {
            changeMaskedWallet();
        } else if (v == mSubmitButton) {
            submitForm();
        }
    }

    private void changeMaskedWallet() {
        if (mConnectionResult != null) {
            resolveUnsuccessfulConnectionResult();
        } else {
            getMaskedWallet();
            mProgressDialog.show();
            mHandleMaskedWalletWhenReady = true;
        }
        mWalletRadioButton.setChecked(true);
    }

    private void submitForm() {
        if (mWalletRadioButton.isChecked()) {
            if (mMaskedWallet != null) {
                launchConfirmationPage();
            } else {
                // wait for Wallet.loadMaskedWallet() or
                // Wallet.changeMaskedWallet() to be called
                mProgressDialog.show();
                mHandleMaskedWalletWhenReady = true;
            }
        } else {
            Toast.makeText(getActivity(), R.string.not_implemented, Toast.LENGTH_LONG).show();
        }
    }

    @Override
    public void onCheckedChanged(CompoundButton buttonView, boolean isChecked) {
        if (isChecked) {
            if (buttonView == mWalletRadioButton) {
                mRadioGroup.clearCheck();
            } else {
                mWalletRadioButton.setChecked(false);
            }
        }

        // show or hide the enter credit card fields depending on if the associated radio button is
        // selected
        if (buttonView == mNewCardRadioButton) {
            if (isChecked) {
                mEnterCardDetails.setVisibility(View.VISIBLE);
            } else {
                mEnterCardDetails.setVisibility(View.GONE);
            }
        }
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
            case REQUEST_CODE_RESOLVE_CHANGE_MASKED_WALLET:
            case REQUEST_CODE_RESOLVE_LOAD_MASKED_WALLET:
                if (mProgressDialog.isShowing()) {
                    mProgressDialog.dismiss();
                }
                switch (resultCode) {
                    case Activity.RESULT_OK:
                        mMaskedWallet =
                                data.getParcelableExtra(WalletConstants.EXTRA_MASKED_WALLET);
                        launchConfirmationPage();
                        break;
                    case Activity.RESULT_CANCELED:
                        // nothing to do here
                        break;
                    default:
                        handleError(errorCode);
                        break;
                }
                break;
            default:
                break;
        }
    }

    @Override
    protected void handleError(int errorCode) {
        switch (errorCode) {
            case WalletConstants.ERROR_CODE_SPENDING_LIMIT_EXCEEDED:
                Toast.makeText(getActivity(),
                        getString(R.string.spending_limit_exceeded, errorCode),
                        Toast.LENGTH_LONG).show();
                break;
            case WalletConstants.ERROR_CODE_INVALID_PARAMETERS:
            case WalletConstants.ERROR_CODE_AUTHENTICATION_FAILURE:
            case WalletConstants.ERROR_CODE_BUYER_ACCOUNT_ERROR:
            case WalletConstants.ERROR_CODE_MERCHANT_ACCOUNT_ERROR:
            case WalletConstants.ERROR_CODE_SERVICE_UNAVAILABLE:
            case WalletConstants.ERROR_CODE_UNSUPPORTED_API_VERSION:
            case WalletConstants.ERROR_CODE_UNKNOWN:
            default:
                // unrecoverable error
                handleUnrecoverableGoogleWalletError(errorCode);
                break;
        }
    }

    private void getMaskedWallet() {
        if (mRequestCode == REQUEST_CODE_RESOLVE_CHANGE_MASKED_WALLET) {
            Wallet.changeMaskedWallet(mGoogleApiClient, mMaskedWallet.getGoogleTransactionId(),
                    mMaskedWallet.getMerchantTransactionId(), mRequestCode);
        } else {
            ItemInfo itemInfo = Constants.ITEMS_FOR_SALE[mItemId];
            Wallet.loadMaskedWallet(mGoogleApiClient,
                    WalletUtil.createMaskedWalletRequest(itemInfo),
                    REQUEST_CODE_RESOLVE_LOAD_MASKED_WALLET);
        }
    }

    private void launchConfirmationPage() {
        Intent intent = new Intent(getActivity(), ConfirmationActivity.class);
        intent.putExtra(Constants.EXTRA_ITEM_ID, mItemId);
        intent.putExtra(Constants.EXTRA_MASKED_WALLET, mMaskedWallet);
        intent.addFlags(Intent.FLAG_ACTIVITY_CLEAR_TOP);
        startActivity(intent);
    }
}
