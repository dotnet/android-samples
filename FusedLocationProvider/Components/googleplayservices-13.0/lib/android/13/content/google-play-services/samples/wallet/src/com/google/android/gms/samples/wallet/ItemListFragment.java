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

import android.content.Context;
import android.content.Intent;
import android.os.Bundle;
import android.support.v4.app.ListFragment;
import android.text.Html;
import android.view.LayoutInflater;
import android.view.View;
import android.view.View.OnClickListener;
import android.view.ViewGroup;
import android.widget.AdapterView.OnItemClickListener;
import android.widget.ArrayAdapter;
import android.widget.ImageView;
import android.widget.ListView;
import android.widget.TextView;

public class ItemListFragment extends ListFragment implements OnClickListener {

    /**
     * The index of the XY000 item in {@link Constants#ITEMS_FOR_SALE} sent
     * to {@link ItemDetailsActivity} if the user clicks on the promo section.
     */
    private final int XY000_POSITION = 0;

    @Override
    public View onCreateView(LayoutInflater inflater, ViewGroup container,
            Bundle savedInstanceState) {
        View root = inflater.inflate(R.layout.fragment_item_list, container);

        // Styling the header with HTML elements in TextView
        TextView promoTitle = (TextView) root.findViewById(R.id.promo_title);
        promoTitle.setText(Html.fromHtml(getString(R.string.promo)));

        View promo = root.findViewById(R.id.promotion);
        promo.setOnClickListener(this);

        return root;
    }

    @Override
    public void onActivityCreated(Bundle savedInstanceState) {
        super.onActivityCreated(savedInstanceState);

        ArrayAdapter<ItemInfo> adapter = new ItemAdapter(getActivity(),
                Constants.ITEMS_FOR_SALE);
        setListAdapter(adapter);
    }

    @Override
    public void onClick(View view) {
        Intent intent = new Intent(getActivity(), ItemDetailsActivity.class);
        intent.putExtra(Constants.EXTRA_ITEM_ID, XY000_POSITION);
        startActivity(intent);
    }

    @Override
    public void onListItemClick(ListView list, View v, int position, long id) {
        ((OnItemClickListener) getActivity()).onItemClick(list, v, position, id);
    }

    private static class ItemAdapter extends ArrayAdapter<ItemInfo> {
        private LayoutInflater mInflater;
        private Context mContext;

        public ItemAdapter(Context context, ItemInfo[] objects) {
            super(context, R.layout.list_item, R.id.name, objects);
            mInflater = LayoutInflater.from(context);
            mContext = context;
        }

        @Override
        public View getView(int position, View view, ViewGroup parent) {
            if (view == null) {
                view = mInflater.inflate(R.layout.list_item, parent, false);
            }

            ItemInfo info = getItem(position);
            TextView title = (TextView) view.findViewById(R.id.name);
            TextView price = (TextView) view.findViewById(R.id.price);
            ImageView image = (ImageView) view.findViewById(R.id.image);

            title.setText(info.name);
            price.setText(Util.formatPrice(mContext, info.priceMicros));
            image.setImageResource(info.imageResourceId);

            return view;
        }
    }
}
