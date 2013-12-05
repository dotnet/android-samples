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
import android.support.v4.app.Fragment;
import android.view.LayoutInflater;
import android.view.View;
import android.view.View.OnClickListener;
import android.view.ViewGroup;
import android.widget.Button;
import android.widget.ImageView;
import android.widget.TextView;

/**
 * The fragment that displays an item's name, price and image. It's used in
 * {@link ItemListActivity} and {@link ItemDetailsActivity}.
 *
 */
public class ItemDetailsFragment extends Fragment implements OnClickListener {
    private View mRoot;
    protected int mItemId;

    @Override
    public View onCreateView(LayoutInflater inflater, ViewGroup container,
            Bundle savedInstanceState) {
        mRoot = inflater.inflate(R.layout.fragment_item_details, container, false);

        Intent intent = getActivity().getIntent();
        if (intent != null) {
            mItemId = intent.getIntExtra(Constants.EXTRA_ITEM_ID, 0);
        }
        setItemId(mItemId);
        return mRoot;
    }

    @Override
    public void onClick(View v) {
        Intent intent = new Intent(getActivity(), CheckoutActivity.class);
        intent.putExtra(Constants.EXTRA_ITEM_ID, mItemId);
        startActivity(intent);
    }

    /**
     * Updates the item details with the item in {@link Constants#ITEMS_FOR_SALE}
     * at <code>position</code>
     *
     * @param position The index of the item in {@link Constants#ITEMS_FOR_SALE}
     * @see Constants#ITEMS_FOR_SALE
     */
    public void setItemId(int position) {
        mItemId = position;
        ItemInfo itemInfo = Constants.ITEMS_FOR_SALE[mItemId];

        TextView itemName = (TextView) mRoot.findViewById(R.id.text_details_item_name);
        itemName.setText(itemInfo.name);

        TextView itemPrice = (TextView) mRoot.findViewById(R.id.text_details_item_price);
        itemPrice.setText(Util.formatPrice(getActivity(), itemInfo.priceMicros));

        ImageView imageView = (ImageView) mRoot.findViewById(R.id.image_details_item_image);
        imageView.setImageResource(itemInfo.imageResourceId);

        Button button = (Button) mRoot.findViewById(R.id.button_details_button_add);
        button.setOnClickListener(this);
    }
}
