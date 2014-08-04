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

import com.google.android.gms.wallet.Address;
import com.google.android.gms.wallet.LoyaltyWalletObject;
import com.google.android.gms.wallet.MaskedWallet;

import android.content.Context;
import android.text.TextUtils;

/**
 * Helper util methods.
 */
public class Util {
    /**
     * Formats the payment descriptions in a {@code MaskedWallet} for display.
     *
     * @param maskedWallet The wallet that contains the payment descriptions.
     * @return The payment descriptions in a format suitable for display to the user.
     */
    static String formatPaymentDescriptions(MaskedWallet maskedWallet) {
        StringBuilder sb = new StringBuilder();
        for (String description : maskedWallet.getPaymentDescriptions()) {
            sb.append(description);
            sb.append("\n");
        }
        if (sb.length() > 0) {
            // remove trailing newline
            sb.deleteCharAt(sb.length() - 1);
        }

        return sb.toString();
    }

    /**
     * Formats the address for display.
     *
     * @param context The context to get String resources from.
     * @param address The {@link Address} to format.
     * @return The address in a format suitable for display to the user.
     */
    static String formatAddress(Context context, Address address) {
        // different locales may need different address formats, which would be handled in
        // R.string.address_format
        String address2 = address.getAddress2().length() == 0 ?
                address.getAddress2() : address.getAddress2() + "\n";
        String address3 = address.getAddress3().length() == 0 ?
                address.getAddress3() : address.getAddress3() + "\n";
        String addressString = context.getString(R.string.address_format, address.getName(),
                address.getAddress1(), address2, address3, address.getCity(), address.getState(),
                address.getPostalCode());
        return addressString;
    }

    /**
     * Formats a price for display.
     *
     * @param context The context to get String resources from.
     * @param priceMicros The price to display, in micros.
     * @return The given price in a format suitable for display to the user.
     */
    static String formatPrice(Context context, long priceMicros) {
        return context.getString(R.string.price_format, priceMicros / 1000000d);
    }

    /**
     * Formats a {@link com.google.android.gms.wallet.LoyaltyWalletObject} for display.
     *
     * @param loyaltyWalletObject the object to format
     * @return The formatted object with the various fields of interest to the user.
     */
    public static String formatLoyaltyWalletObject(LoyaltyWalletObject loyaltyWalletObject) {
        StringBuilder sb = new StringBuilder();
        if (!TextUtils.isEmpty(loyaltyWalletObject.getAccountId())) {
            sb.append(loyaltyWalletObject.getAccountId())
                    .append(", ");
        }
        sb.append(loyaltyWalletObject.getProgramName());
        return sb.toString();
    }
}
