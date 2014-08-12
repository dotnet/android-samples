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

import com.google.android.gms.wallet.WalletConstants;

import android.annotation.TargetApi;
import android.content.Intent;
import android.os.Build;
import android.os.Bundle;
import android.view.Menu;
import android.view.MenuInflater;
import android.view.MenuItem;

/**
 * The checkout page.
 *
 * Handles login and logout, but most of the interesting things happen in {@link CheckoutFragment}.
 * Other pages further in the checkout process will send users back to this page if an error occurs,
 * so {@link #onNewIntent(Intent)} needs to check to see if an error code has been passed in.
 */
public class CheckoutActivity extends BikestoreFragmentActivity {
    private Menu mMenu;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_checkout);
    }

    @Override
    public void onActivityResult(int requestCode, int resultCode, Intent data) {
        if (requestCode == REQUEST_USER_LOGIN) {
            if (resultCode == RESULT_OK) {
                invalidateOptionsMenu();
            }
        } else {
            super.onActivityResult(requestCode, resultCode, data);
        }
    }

    @Override
    public boolean onCreateOptionsMenu(Menu menu) {
        super.onCreateOptionsMenu(menu);

        mMenu = menu;
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

    /**
     * If the confirmation page encounters an error it can't handle, it will send the customer back
     * to this page.  The intent should include the error code as an {@code int} in the field
     * {@link WalletConstants#EXTRA_ERROR_CODE}.
     */
    @Override
    protected void onNewIntent(Intent intent) {
        if (intent.hasExtra(WalletConstants.EXTRA_ERROR_CODE)) {
            int errorCode = intent.getIntExtra(WalletConstants.EXTRA_ERROR_CODE, 0);

            CheckoutFragment fragment =
                    (CheckoutFragment) getSupportFragmentManager().findFragmentById(R.id.frag);
            fragment.setErrorCode(errorCode);
        }
    }

    @Override
    @TargetApi(11)
    public void invalidateOptionsMenu() {
        // Activity.invalidateOptionsMenu() is a useful method to rebuild the menu object but it
        // was not added until Honeycomb. We'll use it if it's available on the platform.
        if (Build.VERSION.SDK_INT >= 11) {
            super.invalidateOptionsMenu();
        } else if (mMenu != null) {
            MenuInflater inflater = getMenuInflater();
            if (((BikestoreApplication) getApplication()).isLoggedIn()) {
                inflater.inflate(R.menu.menu_logout, mMenu);
            } else {
                inflater.inflate(R.menu.menu_login, mMenu);
            }
        }
    }
}
