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
import com.google.android.gms.wallet.FullWalletRequest;
import com.google.android.gms.wallet.Wallet;
import com.google.android.gms.wallet.WalletConstants;

import android.app.Activity;
import android.content.Intent;
import android.support.v4.app.ActivityCompat;
import android.support.v4.app.Fragment;
import android.support.v4.app.FragmentActivity;
import android.view.Menu;
import android.view.MenuInflater;
import android.view.MenuItem;
import android.widget.Toast;

/**
 * Common base class for the {@link FragmentActivity}s that will contain a Login options menu
 * and some common functionality to handle Wallet related errors.
 */
public abstract class BikestoreFragmentActivity extends FragmentActivity {

    /**
     *  Request code used to launch LoginActivity
     */
    protected static final int REQUEST_USER_LOGIN = 1000;

    /**
     * When calling {@link Wallet#loadFullWallet(GoogleApiClient, FullWalletRequest, int)} or
     * resolving connection errors with
     * {@link ConnectionResult#startResolutionForResult(android.app.Activity, int)},
     * the given {@link Activity}'s callback is called. Since in this case, the caller is a
     * {@link Fragment}, and not {@link Activity} that is passed in, this callback is forwarded to
     * {@link FullWalletConfirmationButtonFragment}, {@link PromoAddressLookupFragment} or
     * {@link LoginFragment}.
     * If the requestCode is one of the predefined codes to handle
     * the API calls, pass it to the fragment or else treat it normally.
     */
    @Override
    protected void onActivityResult(int requestCode, int resultCode, Intent data) {
        switch (requestCode) {
            case FullWalletConfirmationButtonFragment.REQUEST_CODE_RESOLVE_LOAD_FULL_WALLET:
            case FullWalletConfirmationButtonFragment.REQUEST_CODE_RESOLVE_ERR:
            case PromoAddressLookupFragment.REQUEST_CODE_RESOLVE_ADDRESS_LOOKUP:
            case PromoAddressLookupFragment.REQUEST_CODE_RESOLVE_ERR:
            case LoginFragment.REQUEST_CODE_RESOLVE_ERR:
                Fragment fragment = getResultTargetFragment();
                if (fragment != null) {
                    fragment.onActivityResult(requestCode, resultCode, data);
                }
                break;
            case REQUEST_USER_LOGIN:
                if (resultCode == RESULT_OK) {
                    ActivityCompat.invalidateOptionsMenu(this);
                }
                break;
            default:
                super.onActivityResult(requestCode, resultCode, data);
                break;
        }
    }

    @Override
    public boolean onCreateOptionsMenu(Menu menu) {
        super.onCreateOptionsMenu(menu);
        MenuInflater inflater = getMenuInflater();
        if (((BikestoreApplication) getApplication()).isLoggedIn()) {
            inflater.inflate(R.menu.menu_logout, menu);
        } else {
            inflater.inflate(R.menu.menu_login, menu);
        }
        return true;
    }

    @Override
    public boolean onOptionsItemSelected(MenuItem item) {
        switch (item.getItemId()) {
            case R.id.login:
                Intent loginIntent = new Intent(this, LoginActivity.class);
                loginIntent.putExtra(LoginActivity.EXTRA_ACTION, LoginActivity.Action.LOGIN);
                startActivityForResult(loginIntent, REQUEST_USER_LOGIN);
                return true;
            case R.id.logout:
                Intent logoutIntent = new Intent(this, LoginActivity.class);
                logoutIntent.putExtra(LoginActivity.EXTRA_ACTION, LoginActivity.Action.LOGOUT);
                startActivityForResult(logoutIntent, REQUEST_USER_LOGIN);
                return true;
            default:
                return false;
        }
    }

    protected void handleError(int errorCode) {
        switch (errorCode) {
            case WalletConstants.ERROR_CODE_SPENDING_LIMIT_EXCEEDED:
                Toast.makeText(this, getString(R.string.spending_limit_exceeded, errorCode),
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
                String errorMessage = getString(R.string.google_wallet_unavailable) + "\n" +
                    getString(R.string.error_code, errorCode);
                Toast.makeText(this, errorMessage, Toast.LENGTH_LONG).show();
                break;
        }
    }
    /**
     * Implemented by Activities like {@link ConfirmationActivity}, {@link LoginActivity},
     * {@link ItemListActivity}
     * This is called from {@link BikestoreFragmentActivity#onActivityResult(int, int, Intent)}
     * to forward the callback to the appropriate {@link Fragment}
     *
     * @return The Fragment that should handle result. Some implementations can return null.
     */
    protected abstract Fragment getResultTargetFragment();

}
