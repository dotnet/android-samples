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

/**
 * Hard coded info about items for sale.
 */
public class ItemInfo {

    public final String name;
    public final String description;
    // Micros are used for prices to avoid rounding errors when converting between currencies
    public final long priceMicros;
    // The estimated tax used to calculate the estimated total price for a Masked Wallet request.
    public final long estimatedTaxMicros;
    // The estimated shipping price used with a Masked Wallet request.
    public final long estimatedShippingPriceMicros;
    // Actual tax and shipping price that should be calculated based on the shipping address
    // received in a MaskedWallet and used when fetching a Full Wallet.
    public final long taxMicros;
    public final long shippingPriceMicros;
    public final String currencyCode;
    public final String sellerData;
    public final int imageResourceId;

    public ItemInfo(String name, String description, long price, long shippingPrice,
            String currencyCode, String sellerData, int imageResourceId) {
        this.name = name;
        this.description = description;
        this.priceMicros = price;
        this.estimatedTaxMicros = (int) (price * 0.10);
        this.taxMicros = (int) (price * 0.10);
        // put in an estimated shipping price
        this.estimatedShippingPriceMicros = 10000000L;
        this.shippingPriceMicros = shippingPrice;
        this.currencyCode = currencyCode;
        this.sellerData = sellerData;
        this.imageResourceId = imageResourceId;
    }

    @Override
    public String toString() {
        return name;
    }

    public long getTotalPrice() {
        return priceMicros + taxMicros + shippingPriceMicros;
    }
}
