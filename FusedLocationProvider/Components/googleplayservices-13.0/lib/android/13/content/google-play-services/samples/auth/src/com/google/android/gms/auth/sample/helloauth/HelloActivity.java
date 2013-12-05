/*
 * Copyright 2012 The Android Open Source Project
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

package com.google.android.gms.auth.sample.helloauth;

import com.google.android.gms.auth.GoogleAuthUtil;
import com.google.android.gms.common.GooglePlayServicesUtil;
import com.google.android.gms.auth.sample.helloauth.R;

import android.accounts.Account;
import android.accounts.AccountManager;
import android.app.Activity;
import android.app.Dialog;
import android.content.Intent;
import android.os.Bundle;
import android.util.Log;
import android.view.View;
import android.view.View.OnClickListener;
import android.widget.ArrayAdapter;
import android.widget.Button;
import android.widget.Spinner;
import android.widget.TextView;

/**
 * The TokenInfoActivity is a simple app that allows users to acquire, inspect and invalidate
 * authentication tokens for a different accounts and scopes.
 *
 * In addition see implementations of {@link AbstractGetNameTask} for an illustration of how to use
 * the {@link GoogleAuthUtil}.
 */
public class HelloActivity extends Activity {
    private static final String TAG = "PlayHelloActivity";
    private static final String SCOPE = "oauth2:https://www.googleapis.com/auth/userinfo.profile";
    public static final String EXTRA_ACCOUNTNAME = "extra_accountname";

    private AccountManager mAccountManager;

    private Spinner mAccountTypesSpinner;

    private TextView mOut;

    static final int REQUEST_CODE_RECOVER_FROM_AUTH_ERROR = 1001;
    static final int REQUEST_CODE_RECOVER_FROM_PLAY_SERVICES_ERROR = 1002;

    private String[] mNamesArray;
    private String mEmail;

    private Type requestType;

    public static String TYPE_KEY = "type_key";
    public static enum Type {FOREGROUND, BACKGROUND, BACKGROUND_WITH_SYNC}

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.accounts_tester);

        mOut = (TextView) findViewById(R.id.message);
        mNamesArray = getAccountNames();
        mAccountTypesSpinner = initializeSpinner(
                R.id.accounts_tester_account_types_spinner, mNamesArray);

        Bundle extras = getIntent().getExtras();
        requestType = Type.valueOf(extras.getString(TYPE_KEY));
        setTitle(getTitle() + " - " + requestType.name());
        initializeFetchButton();
        if (extras.containsKey(EXTRA_ACCOUNTNAME)) {
            mEmail = extras.getString(EXTRA_ACCOUNTNAME);
            mAccountTypesSpinner.setSelection(getIndex(mNamesArray, mEmail));
            getTask(HelloActivity.this, mEmail, SCOPE, REQUEST_CODE_RECOVER_FROM_AUTH_ERROR)
                    .execute();
        }
    }

    @Override
    protected void onActivityResult(int requestCode, int resultCode, Intent data) {
        if (requestCode == REQUEST_CODE_RECOVER_FROM_AUTH_ERROR) {
            handleAuthorizeResult(resultCode, data);
            return;
        }
        super.onActivityResult(requestCode, resultCode, data);
    }

    /**
     * This method is a hook for background threads and async tasks that need to update the UI.
     * It does this by launching a runnable under the UI thread.
     */
    public void show(final String message) {
        runOnUiThread(new Runnable() {
            @Override
            public void run() {
                mOut.setText(message);
            }
        });
    }

    /**
     * This method is a hook for background threads and async tasks that need to launch a dialog.
     * It does this by launching a runnable under the UI thread.
     */
    public void showErrorDialog(final int code) {
        runOnUiThread(new Runnable() {
            @Override
            public void run() {
              Dialog d = GooglePlayServicesUtil.getErrorDialog(
                  code,
                  HelloActivity.this,
                  REQUEST_CODE_RECOVER_FROM_PLAY_SERVICES_ERROR);
              d.show();
            }
        });
    }

    private void handleAuthorizeResult(int resultCode, Intent data) {
        if (data == null) {
            show("Unknown error, click the button again");
            return;
        }
        if (resultCode == RESULT_OK) {
            Log.i(TAG, "Retrying");
            getTask(this, mEmail, SCOPE, REQUEST_CODE_RECOVER_FROM_AUTH_ERROR).execute();
            return;
        }
        if (resultCode == RESULT_CANCELED) {
            show("User rejected authorization.");
            return;
        }
        show("Unknown error, click the button again");
    }

    private String[] getAccountNames() {
        mAccountManager = AccountManager.get(this);
        Account[] accounts = mAccountManager.getAccountsByType(GoogleAuthUtil.GOOGLE_ACCOUNT_TYPE);
        String[] names = new String[accounts.length];
        for (int i = 0; i < names.length; i++) {
            names[i] = accounts[i].name;
        }
        return names;
    }

    private Spinner initializeSpinner(int id, String[] values) {
        ArrayAdapter<String> adapter = new ArrayAdapter<String>(HelloActivity.this,
                android.R.layout.simple_spinner_item, values);
        Spinner spinner = (Spinner) findViewById(id);
        spinner.setAdapter(adapter);
        return spinner;
    }

    private void initializeFetchButton() {
        Button getToken = (Button) findViewById(R.id.greet_me_button);
        getToken.setOnClickListener(new OnClickListener() {
            @Override
            public void onClick(View v) {
                int accountIndex = mAccountTypesSpinner.getSelectedItemPosition();
                if (accountIndex < 0) {
                    // this happens when the sample is run in an emulator which has no google account
                    // added yet.
                    show("No account available. Please add an account to the phone first.");
                    return;
                }
                mEmail = mNamesArray[accountIndex];
                getTask(HelloActivity.this, mEmail, SCOPE,
                        REQUEST_CODE_RECOVER_FROM_AUTH_ERROR).execute();
            }
        });
    }

    /**
     * Note: This approach is for demo purposes only. Clients would normally not get tokens in the
     * background from a Foreground activity.
     */
    private AbstractGetNameTask getTask(
            HelloActivity activity, String email, String scope, int requestCode) {
        switch(requestType) {
            case FOREGROUND:
                return new GetNameInForeground(activity, email, scope, requestCode);
            case BACKGROUND:
                return new GetNameInBackground(activity, email, scope, requestCode);
            case BACKGROUND_WITH_SYNC:
                return new GetNameInBackgroundWithSync(activity, email, scope, requestCode);
            default:
                return new GetNameInBackground(activity, email, scope, requestCode);
        }
    }

    private int getIndex(String[] array, String element) {
        for (int i = 0; i < array.length; i++) {
            if (element.equals(array[i])) {
                return i;
            }
        }
        return 0;  // default to first element.
    }
}