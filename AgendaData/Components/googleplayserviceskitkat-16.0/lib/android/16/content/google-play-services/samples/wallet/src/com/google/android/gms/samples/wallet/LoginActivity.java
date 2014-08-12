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

import android.os.Bundle;
import android.support.v4.app.Fragment;
import android.support.v4.app.FragmentManager;

public class LoginActivity extends BikestoreFragmentActivity {

    public static final String EXTRA_ACTION = "EXTRA_ACTION";

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_login);
        int loginAction = getIntent().getIntExtra(EXTRA_ACTION, Action.LOGIN);
        FragmentManager fragmentManager = getSupportFragmentManager();
        Fragment fragment = fragmentManager.findFragmentById(R.id.frag);
        if (fragment == null) {
            fragment = LoginFragment.newInstance(loginAction);
            fragmentManager.beginTransaction()
                .add(R.id.frag, fragment)
                .commit();
        }
    }

    public static class Action {
        public static final int LOGIN = 2000;
        public static final int LOGOUT = 2001;
        private Action() {}
    }
}
