/*
 * Copyright (C) 2013 The Android Open Source Project
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

package com.google.android.gms.analytics.samples.mobileplayground;

import android.os.Bundle;
import android.support.v4.app.Fragment;
import android.view.KeyEvent;
import android.view.LayoutInflater;
import android.view.View;
import android.view.View.OnClickListener;
import android.view.View.OnKeyListener;
import android.view.ViewGroup;
import android.widget.Button;
import android.widget.EditText;
import android.widget.TextView;
import android.widget.Toast;

import java.util.Map;

import com.google.android.gms.analytics.GoogleAnalytics;
import com.google.android.gms.analytics.HitBuilders;
import com.google.android.gms.analytics.Tracker;
import com.google.android.gms.analytics.samples.mobileplayground.AnalyticsSampleApp.TrackerName;
import com.google.android.gms.analytics.samples.mobileplayground.MobilePlayground.UserInputException;

/**
 * Class to exercise Ecommerce hits.
 */
public class EcommerceFragment extends Fragment {

    @Override
    public View onCreateView(LayoutInflater inflater, ViewGroup container,
            Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);

        View view = inflater.inflate(R.layout.ecommerce, container, false);

        setUniqueOrderId(view);
        calculate(view);

        setupAutoCalculate(view, R.id.item1Quantity);
        setupAutoCalculate(view, R.id.item1Price);
        setupAutoCalculate(view, R.id.item2Quantity);
        setupAutoCalculate(view, R.id.item2Price);

        final Button sendButton = (Button) view.findViewById(R.id.ecommerceSend);
        sendButton.setOnClickListener(new OnClickListener() {
            @Override
            public void onClick(View v) {
                sendEcommerce();
            }
        });

        final Button dispatchButton = (Button) view.findViewById(R.id.ecommerceDispatch);
        dispatchButton.setOnClickListener(new OnClickListener() {
            @Override
            public void onClick(View v) {
                // Manually start a dispatch (Unnecessary if the tracker has a dispatch interval).
                GoogleAnalytics.getInstance(getActivity().getBaseContext()).dispatchLocalHits();
            }
        });
        return view;
    }

    private void sendEcommerce() {
        try {
            sendDataToTwoTrackers(new HitBuilders.TransactionBuilder()
                    .setTransactionId(getOrderId()).setAffiliation(getStoreName())
                    .setRevenue(getTotalOrder()).setTax(getTotalTax())
                    .setShipping(getShippingCost()).setCurrencyCode("USD").build());

            sendDataToTwoTrackers(new HitBuilders.ItemBuilder().setTransactionId(getOrderId())
                    .setName(getItemName(1)).setSku(getItemSku(1)).setCategory(getItemCategory(1))
                    .setPrice(getItemPrice(getView(), 1)).setQuantity(getItemQuantity(getView(), 1))
                    .setCurrencyCode("USD").build());

            sendDataToTwoTrackers(new HitBuilders.ItemBuilder().setTransactionId(getOrderId())
                    .setName(getItemName(2)).setSku(getItemSku(2)).setCategory(getItemCategory(2))
                    .setPrice(getItemPrice(getView(), 2)).setQuantity(getItemQuantity(getView(), 2))
                    .setCurrencyCode("USD").build());
        } catch (UserInputException e) {
            Toast.makeText(getActivity(), e.getMessage(), Toast.LENGTH_SHORT).show();
        }
        setUniqueOrderId(getView());
    }

    private void sendDataToTwoTrackers(Map<String, String> params) {
        AnalyticsSampleApp app = ((AnalyticsSampleApp) getActivity().getApplication());
        Tracker appTracker = app.getTracker(TrackerName.APP_TRACKER);
        Tracker ecommerceTracker = app.getTracker(TrackerName.ECOMMERCE_TRACKER);
        appTracker.send(params);
        ecommerceTracker.send(params);
    }

    private double calculate(View view) {
        double item1Total = getItemQuantity(view, 1) * getItemPrice(view, 1);
        ((TextView) view.findViewById(R.id.item1Total)).setText(Double.toString(item1Total));
        double item2Total = getItemQuantity(view, 2) * getItemPrice(view, 2);
        ((TextView) view.findViewById(R.id.item2Total)).setText(Double.toString(item2Total));
        double itemTotal = item1Total + item2Total;
        ((TextView) view.findViewById(R.id.itemTotal)).setText(Double.toString(itemTotal));
        return itemTotal;
    }

    private void setUniqueOrderId(View view) {
        final EditText orderIdButton = (EditText) view.findViewById(R.id.orderId);
        orderIdButton.setText(getString(R.string.orderId) + System.currentTimeMillis());
    }

    private void setupAutoCalculate(View view, int editTextId) {
        final EditText editText = (EditText) view.findViewById(editTextId);
        editText.setOnKeyListener(new OnKeyListener() {
            @Override
            public boolean onKey(View v, int keyCode, KeyEvent event) {
                calculate(getView());
                return false;
            }
        });
    }

    private String getStoreName() {
        String storeName = ((EditText) getView().findViewById(R.id.storeName)).getText().toString()
                .trim();
        if (storeName.length() == 0) {
            return null;
        }
        return storeName;
    }

    private String getOrderId() throws UserInputException {
        String orderId = ((EditText) getView().findViewById(R.id.orderId)).getText().toString()
                .trim();
        if (orderId.length() == 0) {
            throw new UserInputException(getString(R.string.orderIdWarning));
        }
        return orderId;
    }

    private double getTotalOrder() {
        String total = ((TextView) getView().findViewById(R.id.itemTotal)).getText().toString()
                .trim();
        if (total.length() == 0) {
            return 0;
        }
        return Double.valueOf(total);
    }

    private double getTotalTax() {
        String tax = ((EditText) getView().findViewById(R.id.totalTax)).getText().toString().trim();
        if (tax.length() == 0) {
            return 0;
        }
        return Double.valueOf(tax);
    }

    private double getShippingCost() {
        String shipping = ((EditText) getView().findViewById(R.id.shippingCost)).getText()
                .toString().trim();
        if (shipping.length() == 0) {
            return 0;
        }
        return Double.valueOf(shipping);
    }

    private String getItemName(int index) {
        int buttonId = index == 1 ? R.id.item1Name : R.id.item2Name;
        String name = ((EditText) getView().findViewById(buttonId)).getText().toString().trim();
        if (name.length() == 0) {
            return null;
        }
        return name;
    }

    private String getItemCategory(int index) {
        int buttonId = index == 1 ? R.id.item1Category : R.id.item2Category;
        String name = ((EditText) getView().findViewById(buttonId)).getText().toString().trim();
        if (name.length() == 0) {
            return null;
        }
        return name;
    }

    private String getItemSku(int index) throws UserInputException {
        int buttonId = index == 1 ? R.id.item1Sku : R.id.item2Sku;
        String sku = ((EditText) getView().findViewById(buttonId)).getText().toString().trim();
        if (sku.length() == 0) {
            int warningId = index == 1 ? R.string.item1SkuWarning : R.string.item2SkuWarning;
            throw new UserInputException(getString(warningId));
        }
        return sku;
    }

    private long getItemQuantity(View view, int index) {
        int buttonId = index == 1 ? R.id.item1Quantity : R.id.item2Quantity;
        String quantity = ((EditText) view.findViewById(buttonId)).getText().toString().trim();
        if (quantity.length() == 0) {
            return 0;
        }
        return Long.valueOf(quantity);
    }

    private double getItemPrice(View view, int index) {
        int buttonId = index == 1 ? R.id.item1Price : R.id.item2Price;
        String price = ((EditText) view.findViewById(buttonId)).getText().toString().trim();
        if (price.length() == 0) {
            return 0;
        }
        return Double.valueOf(price);
    }
}
