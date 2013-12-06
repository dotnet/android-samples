package com.google.android.gms.samples.wallet;

import com.google.android.gms.common.ConnectionResult;
import com.google.android.gms.wallet.FullWalletRequest;
import com.google.android.gms.wallet.MaskedWalletRequest;
import com.google.android.gms.wallet.WalletClient;
import com.google.android.gms.wallet.WalletConstants;

import android.app.Activity;
import android.content.Intent;
import android.support.v4.app.Fragment;
import android.support.v4.app.FragmentActivity;

/**
 * Common base class for the {@link FragmentActivity}s that will contain an
 * {@link XyzWalletFragment}.
 */
public class XyzWalletFragmentActivity extends FragmentActivity {

    /**
     * When calling one of the API methods on WalletClient such as
     * {@link WalletClient#loadMaskedWallet(MaskedWalletRequest, int)},
     * {@link WalletClient#loadFullWallet(FullWalletRequest, int)},
     * {@link WalletClient#changeMaskedWallet(String, String, int)} or resolving connection errors
     * with {@link ConnectionResult#startResolutionForResult(android.app.Activity, int)},
     * the given {@link Activity}'s callback is called.  Since in this case, the caller is a
     * {@link Fragment}, and not {@link Activity} that is passed in, this callback is forwarded to
     * {@link XyzWalletFragment}. If the requestCode is one of the predefined codes to handle
     * the API calls, pass it to the fragment or else treat it normally.
     */
    @Override
    protected void onActivityResult(int requestCode, int resultCode, Intent data) {
        switch (requestCode) {
            case XyzWalletFragment.REQUEST_CODE_RESOLVE_CHANGE_MASKED_WALLET:
            case XyzWalletFragment.REQUEST_CODE_RESOLVE_LOAD_MASKED_WALLET:
            case XyzWalletFragment.REQUEST_CODE_RESOLVE_LOAD_FULL_WALLET:
            case XyzWalletFragment.REQUEST_CODE_RESOLVE_ERR:
                Fragment fragment = getSupportFragmentManager().findFragmentById(R.id.frag);
                fragment.onActivityResult(requestCode, resultCode, data);
                break;
            default:
                super.onActivityResult(requestCode, resultCode, data);
                break;
        }
    }
}
