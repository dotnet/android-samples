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
import com.google.android.gms.wallet.MaskedWalletRequest;
import com.google.android.gms.wallet.Wallet;
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
import android.widget.ImageButton;
import android.widget.TextView;
import android.widget.Toast;

/**
 * Checkout page of the sample storefront.  Will fetch a masked wallet to pay for the item in the
 * user's shopping cart.  If the user decides to buy the item, they will be taken to
 * {@link ConfirmationActivity}, which handles requesting a full wallet and submitting the order.
 *
 * <p>When the customer visits this page, a {@link GoogleApiClient} connection should be made in order
 * to interact with the Google Wallet service.  The client builds a {@link MaskedWalletRequest}
 * object, which is needed to request a {@link MaskedWallet} from the {@link Wallet} API when
 * the user taps the Buy with Google Wallet button.
 *
 * <p>
 * {@link Wallet#loadMaskedWallet(GoogleApiClient, MaskedWalletRequest, int)}
 * will return a {@link MaskedWallet} to the Fragment's <code>onActivityResult</code> using the
 * specified <code>requestCode</code>.
 */
public class CheckoutFragment extends BikestoreWalletFragment implements OnClickListener {

    /**
     *  Request code for logging in a user before continuing with the Google Wallet flow.
     */
    private static final int REQUEST_CODE_USER_LOGIN_WALLET = 1006;

    private Button mContinueCheckout;
    private ImageButton mBuyWithGoogleWallet;
    private Button mReturnToShopping;
    // the user is unable to use Google Wallet
    private boolean mGoogleWalletDisabled = false;
    private int mErrorCode;

    @Override
    public void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        mErrorCode = getActivity().getIntent().getIntExtra(WalletConstants.EXTRA_ERROR_CODE, 0);
        setHasOptionsMenu(true);
    }

    @Override
    public View onCreateView(
            LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState) {

        initializeProgressDialog();

        View view = inflater.inflate(R.layout.fragment_checkout, container, false);

        mContinueCheckout = (Button) view.findViewById(R.id.button_regular_checkout);
        mBuyWithGoogleWallet = (ImageButton) view.findViewById(R.id.button_wallet);
        mReturnToShopping = (Button) view.findViewById(R.id.button_return_to_shopping);

        ItemInfo itemInfo = Constants.ITEMS_FOR_SALE[mItemId];

        TextView itemName = (TextView) view.findViewById(R.id.text_item_name);
        itemName.setText(itemInfo.name);

        Drawable itemImage = getResources().getDrawable(itemInfo.imageResourceId);
        int imageSize = getResources().getDimensionPixelSize(R.dimen.image_thumbnail_size);
        int actualWidth = itemImage.getIntrinsicWidth();
        int actualHeight = itemImage.getIntrinsicHeight();
        int scaledHeight = imageSize;
        int scaledWidth = (int) (((float) actualWidth / actualHeight) * scaledHeight);
        itemImage.setBounds(0, 0, scaledWidth, scaledHeight);
        itemName.setCompoundDrawables(itemImage, null, null, null);

        TextView itemPrice = (TextView) view.findViewById(R.id.text_item_price);
        itemPrice.setText(Util.formatPrice(getActivity(), itemInfo.priceMicros));

        // display estimated shipping and tax because the shipping address is unknown
        TextView shippingLabel = (TextView) view.findViewById(R.id.text_shipping);
        shippingLabel.setText(R.string.estimated_shipping);
        TextView estimatedShipping =
                (TextView) view.findViewById(R.id.text_shipping_price);
        if ((mItemId == Constants.PROMOTION_ITEM) && getApplication().isAddressValidForPromo()) {
            estimatedShipping.setText(Util.formatPrice(getActivity(), 0L));
        } else {
            estimatedShipping.setText(Util.formatPrice(getActivity(),
                    itemInfo.shippingPriceMicros));
        }
        TextView tax = (TextView) view.findViewById(R.id.text_tax_price);
        tax.setText(Util.formatPrice(getActivity(), itemInfo.taxMicros));
        TextView total = (TextView) view.findViewById(R.id.text_total_price);
        total.setText(Util.formatPrice(getActivity(), itemInfo.getTotalPrice()));

        mContinueCheckout.setOnClickListener(this);
        mBuyWithGoogleWallet.setOnClickListener(this);
        mReturnToShopping.setOnClickListener(this);

        return view;
    }

    @Override
    public void onClick(View v) {
        if (v == mContinueCheckout) {
            continueCheckout();
        } else if (v == mBuyWithGoogleWallet) {
            buyWithGoogleWallet();
        } else if (v == mReturnToShopping) {
            goToItemListActivity();
        }
    }

    private void continueCheckout() {
        if (getApplication().isLoggedIn()) {
            buyWithGoogleWallet();
        } else {
            Intent intent = new Intent(getActivity(), LoginActivity.class);
            startActivityForResult(intent, REQUEST_CODE_USER_LOGIN_WALLET);
        }
    }

    private void buyWithGoogleWallet() {
        if (mGoogleWalletDisabled) {
            displayGoogleWalletUnavailableToast();
        } else if (mConnectionResult != null) {
            // If there was a connection failure, attempt to resolve the ConnectionResult
            // when the user taps the button
            resolveUnsuccessfulConnectionResult();
        } else {
            loadMaskedWallet();
        }
    }

    private void loadMaskedWallet() {
        if (mGoogleApiClient.isConnected()) {
            mProgressDialog.show();
            Wallet.loadMaskedWallet(mGoogleApiClient, createMaskedWalletRequest(),
                    REQUEST_CODE_RESOLVE_LOAD_MASKED_WALLET);
        } else {
            if (!mGoogleApiClient.isConnected() && !mGoogleApiClient.isConnecting()) {
                mGoogleApiClient.connect();
            }
            mHandleMaskedWalletWhenReady = true;
        }
    }

    private void goToItemListActivity() {
        Intent intent = new Intent(getActivity(), ItemListActivity.class);
        intent.addFlags(Intent.FLAG_ACTIVITY_CLEAR_TOP);
        startActivity(intent);
    }

    void setErrorCode(int errorCode) {
        mErrorCode = errorCode;
    }

    @Override
    public void onResume() {
        super.onResume();
        // if there was an error, display it to the user
        if (mErrorCode > 0) {
            handleError(mErrorCode);
            // clear it out so it only gets displayed once
            mErrorCode = 0;
        }
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
            case REQUEST_CODE_RESOLVE_LOAD_MASKED_WALLET:
                if (mProgressDialog.isShowing()) {
                    mProgressDialog.dismiss();
                }
                switch (resultCode) {
                    case Activity.RESULT_OK:
                        MaskedWallet maskedWallet =
                                data.getParcelableExtra(WalletConstants.EXTRA_MASKED_WALLET);
                        launchConfirmationPage(maskedWallet);
                        break;
                    case Activity.RESULT_CANCELED:
                        // nothing to do here
                        break;
                    default:
                        int errorCode = data.getIntExtra(WalletConstants.EXTRA_ERROR_CODE, 0);
                        handleError(errorCode);
                        break;
                }
                break;
            case REQUEST_CODE_USER_LOGIN_WALLET:
                // User successfully logged in, time to continue their checkout flow
                // If the user canceled out of the login screen don't do anything.
                if (resultCode == Activity.RESULT_OK) {
                    // Recreating the menu so it now shows Logout
                    getActivity().invalidateOptionsMenu();
                    buyWithGoogleWallet();
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
                mGoogleWalletDisabled = true;
                displayGoogleWalletErrorToast(errorCode);
                break;
        }
    }

    private void launchConfirmationPage(MaskedWallet maskedWallet) {
        Intent intent = new Intent(getActivity(), ConfirmationActivity.class);
        intent.putExtra(Constants.EXTRA_ITEM_ID, mItemId);
        intent.putExtra(Constants.EXTRA_MASKED_WALLET, maskedWallet);
        startActivity(intent);
    }

    private MaskedWalletRequest createMaskedWalletRequest() {
        ItemInfo itemInfo = Constants.ITEMS_FOR_SALE[mItemId];
        return WalletUtil.createMaskedWalletRequest(itemInfo);
    }

    @Override
    public void onConnected(Bundle connectionHint) {
        if (mHandleMaskedWalletWhenReady) {
            loadMaskedWallet();
        }
    }

    private void displayGoogleWalletUnavailableToast() {
        Toast.makeText(getActivity(), R.string.google_wallet_unavailable, Toast.LENGTH_LONG).show();
    }

    private void displayGoogleWalletErrorToast(int errorCode) {
        String errorMessage = getString(R.string.google_wallet_unavailable) + "\n" +
                getString(R.string.error_code, errorCode);
        Toast.makeText(getActivity(), errorMessage, Toast.LENGTH_LONG).show();
    }
}
