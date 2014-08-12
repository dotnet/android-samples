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

import android.annotation.TargetApi;
import android.content.Intent;
import android.os.Build;
import android.os.Bundle;
import android.view.Menu;
import android.view.MenuInflater;
import android.view.MenuItem;
import android.view.View;
import android.widget.AdapterView;
import android.widget.AdapterView.OnItemClickListener;
import android.widget.ListView;

/**
 * The launcher activity for Bikestore application. This activity hosts two fragments,
 * one fragment shows a promotion that uses Address lookup API while the other fragment has the
 * list of bikes for sale. If the app is launched on a tablet
 * (x-large screen) in landscape mode the UI will display an item list and
 * details for the currently selected item. If the application is launched on
 * any device in any other orientation, only the item list is shown.
 *
 * Because of the extra logic around what to do with an item list click, the
 * activity implements {@link OnItemClickListener} instead of {@link ItemListFragment}.
 *
 */
public class ItemListActivity extends BikestoreFragmentActivity
        implements OnItemClickListener {

    private Menu mMenu;
    private boolean mIsDualFrame = false;
    private ListView mItemList;
    private ItemDetailsFragment mDetailsFragment;

    @Override
    public void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_item_list);

        mItemList = (ListView) findViewById(android.R.id.list);
        mDetailsFragment = (ItemDetailsFragment) getSupportFragmentManager()
                .findFragmentById(R.id.item_details);
        mIsDualFrame = mDetailsFragment != null;
        if (mIsDualFrame) {
            mItemList.setChoiceMode(ListView.CHOICE_MODE_SINGLE);
            mDetailsFragment.setItemId(0);
        }
    }

    @Override
    public void onActivityResult(int requestCode, int resultCode, Intent data) {
        if (requestCode == REQUEST_USER_LOGIN) {
            if (resultCode == RESULT_OK) {
                invalidateOptionsMenu();
            }
        } else {
            super.onActivityResult(requestCode, resultCode, data);
        }
    }

    @Override
    public boolean onCreateOptionsMenu(Menu menu) {
        super.onCreateOptionsMenu(menu);

        mMenu = menu;
        MenuInflater inflater = getMenuInflater();
        if (((BikestoreApplication) getApplication()).isLoggedIn()) {
            inflater.inflate(R.menu.menu_logout, menu);
        } else {
            inflater.inflate(R.menu.menu_login, menu);
        }

        return true;
    }

    @Override
    public boolean onOptionsItemSelected(MenuItem item) {
        switch (item.getItemId()) {
            case R.id.login:
                Intent loginIntent = new Intent(this, LoginActivity.class);
                loginIntent.putExtra(LoginActivity.EXTRA_ACTION, LoginActivity.Action.LOGIN);
                startActivityForResult(loginIntent, REQUEST_USER_LOGIN);
                return true;
            case R.id.logout:
                Intent logoutIntent = new Intent(this, LoginActivity.class);
                logoutIntent.putExtra(LoginActivity.EXTRA_ACTION, LoginActivity.Action.LOGOUT);
                startActivityForResult(logoutIntent, REQUEST_USER_LOGIN);
                return true;
            default:
                return false;
        }
    }

    @Override
    public void onItemClick(AdapterView<?> parent, View view, int position, long id) {
        if (mIsDualFrame) {
            mDetailsFragment.setItemId(position);
        } else {
            Intent intent = new Intent(this, ItemDetailsActivity.class);
            intent.putExtra(Constants.EXTRA_ITEM_ID, position);
            startActivity(intent);
        }
    }

    @TargetApi(11)
    @Override
    public void invalidateOptionsMenu() {
        if (Build.VERSION.SDK_INT >= 11) {
            super.invalidateOptionsMenu();
        } else if (mMenu != null) {
            MenuInflater inflater = getMenuInflater();
            if (((BikestoreApplication) getApplication()).isLoggedIn()) {
                inflater.inflate(R.menu.menu_logout, mMenu);
            } else {
                inflater.inflate(R.menu.menu_login, mMenu);
            }
        }
    }
}
