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
import com.google.android.gms.wallet.FullWallet;
import com.google.android.gms.wallet.FullWalletRequest;
import com.google.android.gms.wallet.MaskedWallet;
import com.google.android.gms.wallet.MaskedWalletRequest;
import com.google.android.gms.wallet.NotifyTransactionStatusRequest;
import com.google.android.gms.wallet.WalletClient;
import com.google.android.gms.wallet.WalletConstants;

import android.app.Activity;
import android.content.Intent;
import android.graphics.drawable.Drawable;
import android.os.Bundle;
import android.view.LayoutInflater;
import android.view.View;
import android.view.View.OnClickListener;
import android.view.ViewGroup;
import android.widget.Button;
import android.widget.TextView;

/**
 * Confirmation page of the sample storefront.  Handles fetching a {@link MaskedWallet} if the
 * customer decides to change their shipping address.  If the customer wants to change their payment
 * method, they will go to {@link PaymentFragment}.  If the customer confirms their order, a
 * {@link FullWallet} will be obtained, which will contain the actual payment info to charge
 * against.
 *
 * <p>When the customer visits this page, a {@link WalletClient} connection should be made in order
 * to interact with the Google Wallet service.  If the user wants to change their shipping address
 * then {@link WalletClient#changeMaskedWallet(String, String, int)} is called.
 * The client creates an instance of {@link FullWalletRequest} to call
 * {@link WalletClient#loadFullWallet(FullWalletRequest, int)} if the user confirms their
 * selections and places an order.
 */
public class ConfirmationFragment extends XyzWalletFragment implements OnClickListener {

    // No. of times to retry loadFullWallet on receiving a ConnectionResult.INTERNAL_ERROR
    private static final int MAX_FULL_WALLET_RETRIES = 1;

    private MaskedWallet mMaskedWallet;
    private FullWallet mFullWallet;
    private ItemInfo mItemInfo;

    // UI components
    private Button mChangePaymentButton;
    private Button mChangeAddressButton;
    private Button mConfirmButton;

    private TextView mEmail;
    private TextView mShipping;
    private TextView mTax;
    private TextView mTotal;
    private TextView mPaymentDescriptions;
    private TextView mShippingAddress;

    private int mRetryLoadFullWalletCount = 0;

    @Override
    public void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);

        Intent intent = getActivity().getIntent();
        processIntent(intent);
    }

    @Override
    public View onCreateView(
            LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState) {

        initializeProgressDialog();

        View view = inflater.inflate(R.layout.fragment_confirmation, container, false);

        mItemInfo = Constants.ITEMS_FOR_SALE[mItemId];

        TextView itemName = (TextView) view.findViewById(R.id.text_item_name);
        itemName.setText(mItemInfo.name);

        Drawable itemImage = getResources().getDrawable(mItemInfo.imageResourceId);
        int imageSize = getResources().getDimensionPixelSize(R.dimen.image_thumbnail_size);
        itemImage.setBounds(0, 0, imageSize, imageSize);
        itemName.setCompoundDrawables(itemImage, null, null, null);

        TextView itemPrice = (TextView) view.findViewById(R.id.text_item_price);
        itemPrice.setText(Util.formatPrice(getActivity(), mItemInfo.priceMicros));

        TextView shippingLabel = (TextView) view.findViewById(R.id.text_shipping);
        shippingLabel.setText(R.string.shipping);

        mEmail = (TextView) view.findViewById(R.id.text_username);
        mShipping = (TextView) view.findViewById(R.id.text_shipping_price);
        mTax = (TextView) view.findViewById(R.id.text_tax_price);
        mTotal = (TextView) view.findViewById(R.id.text_total_price);
        mPaymentDescriptions = (TextView) view.findViewById(R.id.text_payment_descriptions);
        mShippingAddress = (TextView) view.findViewById(R.id.text_shipping_address);

        updateUiForNewMaskedWallet();

        mChangePaymentButton = (Button) view.findViewById(R.id.button_change_google_wallet);
        mChangePaymentButton.setOnClickListener(this);

        mChangeAddressButton = (Button) view.findViewById(R.id.button_change_shipping_address);
        mChangeAddressButton.setOnClickListener(this);

        mConfirmButton = (Button) view.findViewById(R.id.confirm_button);
        mConfirmButton.setOnClickListener(this);

        return view;
    }

    @Override
    public void onClick(View v) {
        if (v == mChangePaymentButton) {
            goToPaymentActivity();
        } else if (v == mChangeAddressButton) {
            changeMaskedWallet();
        } else if (v == mConfirmButton) {
            confirmPurchase();
        }
    }

    /**
     * Helper method to retrieve relevant data out of an intent.  If there is new data, the member
     * fields will be updated.
     *
     * @param intent The intent to retrieve data from.
     * @return {@code true} if the given {@code Intent} contained new data.
     */
    protected boolean processIntent(Intent intent) {
        // the masked wallet contains the customer's payment info and should be displayed on the
        // confirmation page
        MaskedWallet maskedWallet = intent.getParcelableExtra(Constants.EXTRA_MASKED_WALLET);

        if (maskedWallet != null) {
            mMaskedWallet = maskedWallet;
            return true;
        } else {
            return false;
        }
    }

    protected void onNewIntent(Intent intent) {
        if (processIntent(intent)) {
            updateUiForNewMaskedWallet();
        }
    }

    /**
     * Handles updating the UI when the masked wallet has changed.  This can happen if the customer
     * decides to change their payment instrument or their shipping address.
     * {@link ConfirmationFragment#mMaskedWallet} is expected to be updated and not {@code null}
     * before this method is called.
     */
    private void updateUiForNewMaskedWallet() {
        // email may change if the user changes to a different Wallet account
        mEmail.setText(mMaskedWallet.getEmail());

        ItemInfo itemInfo = Constants.ITEMS_FOR_SALE[mItemId];
        // calculating exact shipping and tax should now be possible because there is a shipping
        // address in mMaskedWallet
        mShipping.setText(Util.formatPrice(getActivity(), itemInfo.shippingPriceMicros));
        mTax.setText(Util.formatPrice(getActivity(), itemInfo.taxMicros));
        mTotal.setText(Util.formatPrice(getActivity(), itemInfo.getTotalPrice()));

        // display the payment descriptions of all of the payment instruments being used
        mPaymentDescriptions.setText(Util.formatPaymentDescriptions(mMaskedWallet));

        mShippingAddress.setText(Util.formatAddress(getActivity(),
                mMaskedWallet.getShippingAddress()));
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
                    mWalletClient.connect();
                } else {
                    handleUnrecoverableGoogleWalletError(errorCode);
                }
                break;
            case REQUEST_CODE_RESOLVE_CHANGE_MASKED_WALLET:
                switch (resultCode) {
                    case Activity.RESULT_OK:
                        mMaskedWallet =
                                data.getParcelableExtra(WalletConstants.EXTRA_MASKED_WALLET);
                        updateUiForNewMaskedWallet();
                        break;
                    case Activity.RESULT_CANCELED:
                        // nothing to do here
                        break;
                    default:
                        handleError(errorCode);
                        break;
                }
                break;
            case REQUEST_CODE_RESOLVE_LOAD_FULL_WALLET:
                switch (resultCode) {
                    case Activity.RESULT_OK:
                        if (data.hasExtra(WalletConstants.EXTRA_FULL_WALLET)) {
                            mFullWallet =
                                    data.getParcelableExtra(WalletConstants.EXTRA_FULL_WALLET);
                            // the full wallet can now be used to process the customer's payment
                            // send the wallet info up to server to process, and to get the result
                            // for sending a transaction status
                            fetchTransactionStatus(mFullWallet);
                        } else if (data.hasExtra(WalletConstants.EXTRA_MASKED_WALLET)) {
                            mMaskedWallet =
                                    data.getParcelableExtra(WalletConstants.EXTRA_MASKED_WALLET);
                            updateUiForNewMaskedWallet();
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
            default:
                break;
        }
    }

    @Override
    protected void handleError(int errorCode) {
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

    private void goToPaymentActivity() {
        Intent intent = new Intent(getActivity(), PaymentActivity.class);
        intent.putExtra(Constants.EXTRA_ITEM_ID, mItemId);
        intent.putExtra(Constants.EXTRA_MASKED_WALLET, mMaskedWallet);
        startActivity(intent);
    }

    private void changeMaskedWallet() {
        if (mConnectionResult != null) {
            resolveUnsuccessfulConnectionResult();
        } else {
            mWalletClient.changeMaskedWallet(mMaskedWallet.getGoogleTransactionId(),
                    mMaskedWallet.getMerchantTransactionId(),
                    REQUEST_CODE_RESOLVE_CHANGE_MASKED_WALLET);
        }
    }

    private void confirmPurchase() {
        if (mConnectionResult != null) {
            // The user needs to resolve an issue before WalletClient can connect
            resolveUnsuccessfulConnectionResult();
        } else {
            getFullWallet();
            mProgressDialog.setCancelable(false);
            mProgressDialog.show();
            mHandleFullWalletWhenReady = true;
        }
    }

    private void getFullWallet() {
        mWalletClient.loadFullWallet(WalletUtil.createFullWalletRequest(mItemInfo,
                mMaskedWallet.getGoogleTransactionId()), REQUEST_CODE_RESOLVE_LOAD_FULL_WALLET);
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
        mWalletClient.notifyTransactionStatus(WalletUtil.createNotifyTransactionStatusRequest(
                fullWallet.getGoogleTransactionId(),
                NotifyTransactionStatusRequest.Status.SUCCESS));

        Intent intent = new Intent(getActivity(), OrderCompleteActivity.class);
        intent.setFlags(Intent.FLAG_ACTIVITY_CLEAR_TASK | Intent.FLAG_ACTIVITY_NEW_TASK);
        intent.putExtra(Constants.EXTRA_FULL_WALLET, mFullWallet);
        startActivity(intent);
    }

    /**
     * Retries {@link WalletClient#loadFullWallet(FullWalletRequest, int)} if
     * {@link #MAX_FULL_WALLET_RETRIES} has not been reached.
     *
     * @return {@code true} if {@link ConfirmationFragment#getFullWallet()} is retried,
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
}
