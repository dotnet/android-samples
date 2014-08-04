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

import com.google.android.gms.wallet.MaskedWallet;
import com.google.android.gms.wallet.MaskedWalletRequest;
import com.google.android.gms.wallet.WalletConstants;
import com.google.android.gms.wallet.fragment.BuyButtonText;
import com.google.android.gms.wallet.fragment.Dimension;
import com.google.android.gms.wallet.fragment.SupportWalletFragment;
import com.google.android.gms.wallet.fragment.WalletFragmentInitParams;
import com.google.android.gms.wallet.fragment.WalletFragmentMode;
import com.google.android.gms.wallet.fragment.WalletFragmentOptions;
import com.google.android.gms.wallet.fragment.WalletFragmentStyle;

import android.app.Activity;
import android.content.Intent;
import android.os.Bundle;
import android.support.v4.app.Fragment;
import android.view.View;
import android.widget.Button;
import android.widget.Toast;

/**
 * The checkout page.
 *
 * Handles login and logout, but most of the interesting things happen in the WalletFragment
 * that is hosted by this activity.
 * Other pages further in the checkout process will send users back to this page if an error occurs,
 * so {@link #onNewIntent(Intent)} needs to check to see if an error code has been passed in.
 */
public class CheckoutActivity extends BikestoreFragmentActivity
        implements android.view.View.OnClickListener {

    private static final int REQUEST_CODE_MASKED_WALLET = 1001;

    private Button mReturnToShopping;
    private SupportWalletFragment mWalletFragment;
    private int mItemId;
    private Button mContinueCheckout;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_checkout);

        mItemId = getIntent().getIntExtra(Constants.EXTRA_ITEM_ID, 0);
        createAndAddWalletFragment();
        mReturnToShopping = (Button) findViewById(R.id.button_return_to_shopping);
        mReturnToShopping.setOnClickListener(this);
        mContinueCheckout = (Button) findViewById(R.id.button_regular_checkout);
        mContinueCheckout.setOnClickListener(this);
    }

    @Override
    public void onActivityResult(int requestCode, int resultCode, Intent data) {
        // retrieve the error code, if available
        int errorCode = -1;
        if (data != null) {
            errorCode = data.getIntExtra(WalletConstants.EXTRA_ERROR_CODE, -1);
        }
        switch (requestCode) {
            case REQUEST_CODE_MASKED_WALLET:
                switch (resultCode) {
                    case Activity.RESULT_OK:
                        MaskedWallet maskedWallet =
                                data.getParcelableExtra(WalletConstants.EXTRA_MASKED_WALLET);
                        launchConfirmationPage(maskedWallet);
                        break;
                    case Activity.RESULT_CANCELED:
                        break;
                    default:
                        handleError(errorCode);
                        break;
                }
                break;
            case WalletConstants.RESULT_ERROR:
                handleError(errorCode);
                break;
            default:
                super.onActivityResult(requestCode, resultCode, data);
                break;
        }
    }

    /**
     * If the confirmation page encounters an error it can't handle, it will send the customer back
     * to this page.  The intent should include the error code as an {@code int} in the field
     * {@link WalletConstants#EXTRA_ERROR_CODE}.
     */
    @Override
    protected void onNewIntent(Intent intent) {
        if (intent.hasExtra(WalletConstants.EXTRA_ERROR_CODE)) {
            int errorCode = intent.getIntExtra(WalletConstants.EXTRA_ERROR_CODE, 0);
            handleError(errorCode);
        }
    }

    private void goToItemListActivity() {
        Intent intent = new Intent(this, ItemListActivity.class);
        intent.addFlags(Intent.FLAG_ACTIVITY_CLEAR_TOP);
        startActivity(intent);
    }

    private void continueCheckout() {
        Toast.makeText(this, R.string.checkout_bikestore_message, Toast.LENGTH_LONG).show();
    }

    @Override
    public void onClick(View v) {
        if (v == mReturnToShopping) {
            goToItemListActivity();
        } else if (v == mContinueCheckout) {
            continueCheckout();
        }
    }

    private void createAndAddWalletFragment() {
        WalletFragmentStyle walletFragmentStyle = new WalletFragmentStyle()
                .setBuyButtonText(BuyButtonText.BUY_WITH_GOOGLE)
                .setBuyButtonWidth(Dimension.MATCH_PARENT);

        WalletFragmentOptions walletFragmentOptions = WalletFragmentOptions.newBuilder()
                .setEnvironment(WalletConstants.ENVIRONMENT_SANDBOX)
                .setFragmentStyle(walletFragmentStyle)
                .setTheme(WalletConstants.THEME_HOLO_LIGHT)
                .setMode(WalletFragmentMode.BUY_BUTTON)
                .build();
        mWalletFragment = SupportWalletFragment.newInstance(walletFragmentOptions);

        // Now initialize the Wallet Fragment
        String accountName = ((BikestoreApplication) getApplication()).getAccountName();
        MaskedWalletRequest maskedWalletRequest =
                WalletUtil.createMaskedWalletRequest(Constants.ITEMS_FOR_SALE[mItemId]);
        WalletFragmentInitParams.Builder startParamsBuilder = WalletFragmentInitParams.newBuilder()
                .setMaskedWalletRequest(maskedWalletRequest)
                .setMaskedWalletRequestCode(REQUEST_CODE_MASKED_WALLET)
                .setAccountName(accountName);
        mWalletFragment.initialize(startParamsBuilder.build());

        // add Wallet fragment to the UI
        getSupportFragmentManager().beginTransaction()
                .replace(R.id.dynamic_wallet_button_fragment, mWalletFragment)
                .commit();
    }

    private void launchConfirmationPage(MaskedWallet maskedWallet) {
        Intent intent = new Intent(this, ConfirmationActivity.class);
        intent.putExtra(Constants.EXTRA_ITEM_ID, mItemId);
        intent.putExtra(Constants.EXTRA_MASKED_WALLET, maskedWallet);
        startActivity(intent);
    }

    @Override
    public Fragment getResultTargetFragment() {
        return null;
    }

}
