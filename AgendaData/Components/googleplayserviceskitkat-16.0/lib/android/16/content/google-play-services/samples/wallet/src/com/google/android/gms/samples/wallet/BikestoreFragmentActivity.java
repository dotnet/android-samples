package com.google.android.gms.samples.wallet;

import com.google.android.gms.common.ConnectionResult;
import com.google.android.gms.common.api.GoogleApiClient;
import com.google.android.gms.wallet.FullWalletRequest;
import com.google.android.gms.wallet.MaskedWalletRequest;
import com.google.android.gms.wallet.Wallet;

import android.app.Activity;
import android.content.Intent;
import android.support.v4.app.Fragment;
import android.support.v4.app.FragmentActivity;

/**
 * Common base class for the {@link FragmentActivity}s that will contain a
 * {@link BikestoreWalletFragment}.
 */
public class BikestoreFragmentActivity extends FragmentActivity {

    /**
     *  Request code used to launch LoginActivity
     */
    protected static final int REQUEST_USER_LOGIN = 1000;

    /**
     * When calling one of the API methods on Wallet such as
     * {@link Wallet#loadMaskedWallet(GoogleApiClient, MaskedWalletRequest, int)},
     * {@link Wallet#loadFullWallet(GoogleApiClient, FullWalletRequest, int)},
     * {@link Wallet#changeMaskedWallet(GoogleApiClient, String, String, int)} or
     * resolving connection errors with
     * {@link ConnectionResult#startResolutionForResult(android.app.Activity, int)},
     * the given {@link Activity}'s callback is called.  Since in this case, the caller is a
     * {@link Fragment}, and not {@link Activity} that is passed in, this callback is forwarded to
     * {@link BikestoreWalletFragment} or {@link PromoAddressLookupFragment}
     * If the requestCode is one of the predefined codes to handle
     * the API calls, pass it to the fragment or else treat it normally.
     */
    @Override
    protected void onActivityResult(int requestCode, int resultCode, Intent data) {
        switch (requestCode) {
            case BikestoreWalletFragment.REQUEST_CODE_RESOLVE_CHANGE_MASKED_WALLET:
            case BikestoreWalletFragment.REQUEST_CODE_RESOLVE_LOAD_MASKED_WALLET:
            case BikestoreWalletFragment.REQUEST_CODE_RESOLVE_LOAD_FULL_WALLET:
            case BikestoreWalletFragment.REQUEST_CODE_RESOLVE_ERR:
            case PromoAddressLookupFragment.REQUEST_CODE_RESOLVE_ERR:
            case PromoAddressLookupFragment.REQUEST_CODE_RESOLVE_ADDRESS_LOOKUP:
                Fragment fragment = getSupportFragmentManager().findFragmentById(R.id.frag);
                fragment.onActivityResult(requestCode, resultCode, data);
                break;
            default:
                super.onActivityResult(requestCode, resultCode, data);
                break;
        }
    }
}
