package com.google.android.gms.samples.wallet;

import android.os.Bundle;

/**
 * Payment entry page.
 *
 * @see PaymentFragment
 */
public class PaymentActivity extends BikestoreFragmentActivity {

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_payment);
    }
}
