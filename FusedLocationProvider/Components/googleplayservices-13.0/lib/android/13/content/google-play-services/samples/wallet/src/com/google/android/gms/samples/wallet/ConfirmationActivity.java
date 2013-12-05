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

import android.content.Intent;
import android.os.Bundle;

/**
 * Activity that displays the user's Google Wallet checkout confirmation page.
 *
 * @see ConfirmationFragment
 */
public class ConfirmationActivity extends XyzWalletFragmentActivity {

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);

        setContentView(R.layout.activity_confirmation);
    }

    @Override
    protected void onNewIntent(Intent intent) {
        ConfirmationFragment fragment =
                (ConfirmationFragment) getSupportFragmentManager().findFragmentById(R.id.frag);
        fragment.onNewIntent(intent);
    }
}
