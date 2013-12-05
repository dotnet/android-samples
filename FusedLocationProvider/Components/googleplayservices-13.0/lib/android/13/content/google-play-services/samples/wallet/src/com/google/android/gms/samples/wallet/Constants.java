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

/**
 * Constants used by Google Wallet SDK Sample.
 */
public class Constants {

    // Environment to use when creating an instance of WalletClient
    public static final int WALLET_ENVIRONMENT = WalletConstants.ENVIRONMENT_SANDBOX;

    public static final String MERCHANT_NAME = "XYZ Inc";

    // Intent extra keys
    public static final String EXTRA_ITEM_ID = "EXTRA_ITEM_ID";
    public static final String EXTRA_MASKED_WALLET = "EXTRA_MASKED_WALLET";
    public static final String EXTRA_FULL_WALLET = "EXTRA_FULL_WALLET";

    public static final String CURRENCY_CODE_USD = "USD";

    // values to use with KEY_DESCRIPTION
    public static final String DESCRIPTION_LINE_ITEM_SHIPPING = "Shipping";
    public static final String DESCRIPTION_LINE_ITEM_TAX = "Tax";

    /**
     * Sample list of items for sale. The list would normally be fetched from
     * the merchant's servers.
     */
    public static final ItemInfo[] ITEMS_FOR_SALE = {
            new ItemInfo("Camera XY000", "Features", 800000000, 0, CURRENCY_CODE_USD,
                    "seller data 0", R.drawable.xy000),
            new ItemInfo("Camera XY001", "More features", 950000000, 9990000, CURRENCY_CODE_USD,
                    "seller data 1", R.drawable.xy001),
            new ItemInfo("Camera XY002", "Even more features", 1500000000, 9990000,
                    CURRENCY_CODE_USD, "seller data 2", R.drawable.xy002)
    };
}
