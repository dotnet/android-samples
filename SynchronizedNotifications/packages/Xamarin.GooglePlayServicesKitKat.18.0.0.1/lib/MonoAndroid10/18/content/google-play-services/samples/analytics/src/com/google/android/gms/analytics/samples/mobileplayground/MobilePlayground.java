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
import android.support.v4.app.FragmentTransaction;
import android.support.v7.app.ActionBar;
import android.support.v7.app.ActionBar.Tab;
import android.support.v7.app.ActionBarActivity;

/**
 * Initial Activity used to setup the tracker.
 */
public class MobilePlayground extends ActionBarActivity {

    /**
     * Represents user input error as an exception
     */
    public static class UserInputException extends Exception {
        private static final long serialVersionUID = -3780674072556176735L;

        public UserInputException(String message) {
            super(message);
        }
    }

    @Override
    public void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setupDisplay();
    }

    private void setupDisplay() {
        final ActionBar bar = this.getSupportActionBar();
        bar.setNavigationMode(ActionBar.NAVIGATION_MODE_TABS);
        bar.setDisplayOptions(0, ActionBar.DISPLAY_SHOW_TITLE);

        bar.addTab(bar.newTab().setText(R.string.screenTabName).setTabListener(
                new TabListener<AppviewFragment>(this, "appview", AppviewFragment.class)));

        bar.addTab(bar.newTab().setText(R.string.eventTabName).setTabListener(
                new TabListener<EventFragment>(this, "event", EventFragment.class)));

        bar.addTab(bar.newTab().setText(R.string.exceptionTabName).setTabListener(
                new TabListener<ExceptionFragment>(this, "exception", ExceptionFragment.class)));

        bar.addTab(bar.newTab().setText(R.string.socialTabName).setTabListener(
                new TabListener<SocialFragment>(this, "social", SocialFragment.class)));

        bar.addTab(bar.newTab().setText(R.string.timingTabName).setTabListener(
                new TabListener<TimingFragment>(this, "timing", TimingFragment.class)));

        bar.addTab(bar.newTab().setText(R.string.ecommerceTabName).setTabListener(
                new TabListener<EcommerceFragment>(this, "ecommerce", EcommerceFragment.class)));

    }

    public static class TabListener<T extends Fragment> implements ActionBar.TabListener {
        private final ActionBarActivity mActivity;

        private final String mTag;

        private final Class<T> mClass;

        private final Bundle mArgs;

        private Fragment mFragment;

        public TabListener(ActionBarActivity activity, String tag, Class<T> clz) {
            this(activity, tag, clz, null);
        }

        public TabListener(ActionBarActivity activity, String tag, Class<T> clz, Bundle args) {
            mActivity = activity;
            mTag = tag;
            mClass = clz;
            mArgs = args;

            mFragment = mActivity.getSupportFragmentManager().findFragmentByTag(mTag);
            if (mFragment != null && !mFragment.isDetached()) {
                FragmentTransaction ft = mActivity.getSupportFragmentManager().beginTransaction();
                ft.detach(mFragment);
                ft.commit();
            }
        }

        @Override
        public void onTabSelected(Tab tab, FragmentTransaction ft) {
            if (mFragment == null) {
                mFragment = Fragment.instantiate(mActivity, mClass.getName(), mArgs);
                ft.add(android.R.id.content, mFragment, mTag);
            } else {
                ft.attach(mFragment);
            }
            // TODO: Add AutoTracking here once the autoTracking fixes make it into the SDK.
        }

        @Override
        public void onTabUnselected(Tab tab, FragmentTransaction ft) {
            if (mFragment != null) {
                ft.detach(mFragment);
            }
        }

        @Override
        public void onTabReselected(Tab tab, FragmentTransaction ft) { }
    }

}
