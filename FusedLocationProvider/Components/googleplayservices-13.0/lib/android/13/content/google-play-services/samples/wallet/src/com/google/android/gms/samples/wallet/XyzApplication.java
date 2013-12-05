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

import android.app.Application;
import android.content.SharedPreferences;

public class XyzApplication extends Application {
    private static final String USER_PREFS = "com.google.android.gms.samples.wallet.USER_PREFS";
    private static final String KEY_USERNAME = "com.google.android.gms.samples.wallet.KEY_USERNAME";

    private String mUserName;
    private SharedPreferences mPrefs;

    @Override
    public void onCreate() {
        super.onCreate();
        mPrefs = getSharedPreferences(USER_PREFS, MODE_PRIVATE);
        mUserName = mPrefs.getString(KEY_USERNAME, null);
    }

    public boolean isLoggedIn() {
        return mUserName != null;
    }

    public void login(String userName) {
        mUserName = userName;
        mPrefs.edit().putString(KEY_USERNAME, mUserName).commit();
    }

    public void logout() {
        mUserName = null;
        mPrefs.edit().remove(KEY_USERNAME).commit();
    }

    public String getAccountName() {
        return mPrefs.getString(KEY_USERNAME, null);
    }
}
