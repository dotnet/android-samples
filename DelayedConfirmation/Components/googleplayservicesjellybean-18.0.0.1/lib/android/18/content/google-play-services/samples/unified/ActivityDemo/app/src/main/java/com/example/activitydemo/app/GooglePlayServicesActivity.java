/*
 * Copyright (C) 2014 The Android Open Source Project
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

package com.example.activitydemo.app;

import android.app.Activity;
import android.content.ComponentName;
import android.content.Context;
import android.content.DialogInterface;
import android.content.DialogInterface.OnCancelListener;
import android.content.Intent;
import android.content.IntentSender.SendIntentException;
import android.content.ServiceConnection;
import android.os.AsyncTask;
import android.os.Bundle;
import android.os.IBinder;
import android.os.RemoteException;
import android.util.Log;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.ArrayAdapter;
import android.widget.CheckBox;
import android.widget.ListView;
import android.widget.TextView;

import com.example.activitydemo.app.service.GameService;
import com.example.activitydemo.app.service.IActivityGameService;
import com.example.activitydemo.app.service.IActivityGameServiceCallbacks;
import com.google.android.gms.common.ConnectionResult;
import com.google.android.gms.common.GooglePlayServicesUtil;
import com.google.android.gms.common.api.GoogleApiClient;

import com.google.android.gms.drive.Drive;
import com.google.android.gms.games.Games;
import com.google.android.gms.location.ActivityRecognition;

import java.text.SimpleDateFormat;
import java.util.ArrayList;
import java.util.Date;
import java.util.List;
import java.util.Locale;

public class GooglePlayServicesActivity extends Activity implements
        GoogleApiClient.ConnectionCallbacks,
        GoogleApiClient.OnConnectionFailedListener, ServiceConnection {

    private static final String TAG = "GooglePlayServicesActivity";

    private static final String KEY_IN_RESOLUTION = "is_in_resolution";

    /**
     * Request code for auto Google Play Services error resolution.
     */
    protected static final int REQUEST_CODE_RESOLUTION = 1;
    public static final int REQUEST_CODE_NOTIFICATION = 2;
    private static final int REQUEST_CODE_LEADERBOARDS = 3;

    private static final SimpleDateFormat TIME_FORMAT = new SimpleDateFormat("HH:mm:ss.SSS",
            Locale.US);

    private static List<ActivityLog> sActivities = new ArrayList<ActivityLog>();

    /**
     * Google API client.
     */
    private GoogleApiClient mGoogleApiClient;

    /**
     * Determines if the client is in a resolution state, and
     * waiting for resolution intent to return.
     */
    private boolean mIsInResolution;

    private IActivityGameService mGameService = null;
    private boolean mGameServiceBound = false;

    private ActivityLogAdapter mAdapter;

    TextView mScoreTextView;

    private IActivityGameServiceCallbacks mGameCallbacks = new IActivityGameServiceCallbacks.Stub() {

        @Override
        public void onConnected(final int state) throws RemoteException {
            Log.d(TAG, "GameService connected to GooglePlayServices. " + state);
            if (state == GameService.STATE_IN_GAME) {

            }
            runOnUiThread(new Runnable() {
                @Override
                public void run() {
                    if (state == GameService.STATE_PRE_GAME) {
                        findViewById(R.id.start_button).setEnabled(true);
                        findViewById(R.id.stop_button).setEnabled(false);
                    } else {
                        Log.d(TAG, "Game already in progress.");
                        try {
                            long score = mGameService.getScore();
                            mScoreTextView.setText(String.valueOf(score));
                        } catch (RemoteException e) { }

                        findViewById(R.id.start_button).setEnabled(false);
                        findViewById(R.id.stop_button).setEnabled(true);

                        new FetchHistory().execute();


                    }
                }
            });
        }

        @Override
        public void onStartGame() throws RemoteException {
            Log.d(TAG, "Game started");
            runOnUiThread(new Runnable() {
                @Override
                public void run() {
                    mAdapter.clear();
                    findViewById(R.id.start_button).setEnabled(false);
                    findViewById(R.id.stop_button).setEnabled(true);
                }
            });

        }

        @Override
        public void onActivityChanged(final ActivityLog activity) throws RemoteException {
            // TODO display somewhere.
            runOnUiThread(new Runnable() {
                @Override
                public void run() {
                    Log.d(TAG, "Adding " + activity);
                    sActivities.add(activity);
                    mAdapter.notifyDataSetChanged();
                }
            });
        }

        @Override
        public void onScoreChanged(final long score) throws RemoteException {
            // TODO display score on activity.
            Log.d(TAG, "New Score: " + score);
            runOnUiThread(new Runnable() {
                @Override
                public void run() {
                    mScoreTextView.setText(String.valueOf(score));
                }
            });
        }

        @Override
        public void onEndGame() throws RemoteException {
            runOnUiThread(new Runnable() {
                @Override
                public void run() {
                    findViewById(R.id.start_button).setEnabled(true);
                    findViewById(R.id.stop_button).setEnabled(false);
                }
            });
        }
    };

    /**
     * Called when the activity is starting. Restores the activity state.
     */
    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        if (savedInstanceState != null) {
            mIsInResolution = savedInstanceState.getBoolean(KEY_IN_RESOLUTION, false);
        }

        setContentView(R.layout.activity_main);

        mScoreTextView = (TextView) findViewById(R.id.score);
        mAdapter = new ActivityLogAdapter(this, sActivities);


        ListView lv = (ListView) findViewById(android.R.id.list);
        lv.setAdapter(mAdapter);
    }

    public static GoogleApiClient.Builder getGoogleApiClientBuilder(Context context) {
        return new GoogleApiClient.Builder(context)
                .addApi(Games.API)
                .addScope(Games.SCOPE_GAMES)
                .addApi(ActivityRecognition.API)
                .addApi(Drive.API)
                .addScope(Drive.SCOPE_FILE);
                // Optionally, add additional APIs and scopes if required.
    }

    /**
     * Called when the Activity is made visible.
     * A connection to Play Services need to be initiated as
     * soon as the activity is visible. Registers {@code ConnectionCallbacks}
     * and {@code OnConnectionFailedListener} on the
     * activities itself.
     */
    @Override
    protected void onStart() {
        super.onStart();

        if (mGoogleApiClient == null) {
            mGoogleApiClient = getGoogleApiClientBuilder(this)
                    // Optionally, add additional APIs and scopes if required.
                    .addConnectionCallbacks(this)
                    .addOnConnectionFailedListener(this)
                    .setViewForPopups(getWindow().getDecorView())
                    .build();
        }
        mGoogleApiClient.connect();

    }

    /**
     * Called when activity gets invisible. Connection to Play Services needs to
     * be disconnected as soon as an activity is invisible.
     */
    @Override
    protected void onStop() {
        if (mGoogleApiClient != null) {
            mGoogleApiClient.disconnect();
        }
        super.onStop();

        if (mGameServiceBound) {
            Log.d(TAG, "Service unbound");
            unbindService(this);
            mGameServiceBound = false;
        }
    }

    /**
     * Saves the resolution state.
     */
    @Override
    protected void onSaveInstanceState(Bundle outState) {
        super.onSaveInstanceState(outState);
        outState.putBoolean(KEY_IN_RESOLUTION, mIsInResolution);
    }

    /**
     * Handles Google Play Services resolution callbacks.
     */
    @Override
    protected void onActivityResult(int requestCode, int resultCode, Intent data) {
        super.onActivityResult(requestCode, resultCode, data);
        switch (requestCode) {
        case REQUEST_CODE_RESOLUTION:
            retryConnecting();
            break;
        }
    }

    private void retryConnecting() {
        mIsInResolution = false;
        if (!mGoogleApiClient.isConnecting()) {
            mGoogleApiClient.connect();
        }
    }

    /**
     * Called when {@code mGoogleApiClient} is connected.
     */
    @Override
    public void onConnected(Bundle connectionHint) {
        Log.i(TAG, "GoogleApiClient connected");
        Intent service = new Intent(this, GameService.class);
        if (!mGameServiceBound) {
            bindService(service, this, Context.BIND_AUTO_CREATE);
            Log.d(TAG, "Service binding");
            mGameServiceBound = true;
        }

        findViewById(R.id.leaderboards_button).setEnabled(true);
    }

    /**
     * Called when {@code mGoogleApiClient} connection is suspended.
     */
    @Override
    public void onConnectionSuspended(int cause) {
        Log.i(TAG, "GoogleApiClient connection suspended");
        findViewById(R.id.leaderboards_button).setEnabled(false);
    }

    /**
     * Called when {@code mGoogleApiClient} is trying to connect but failed.
     * Handle {@code result.getResolution()} if there is a resolution
     * available.
     */
    @Override
    public void onConnectionFailed(ConnectionResult result) {
        Log.i(TAG, "GoogleApiClient connection failed: " + result.toString());
        findViewById(R.id.leaderboards_button).setEnabled(false);
        if (!result.hasResolution()) {
            // Show a localized error dialog.
            GooglePlayServicesUtil.getErrorDialog(
                    result.getErrorCode(), this, 0, new OnCancelListener() {
                @Override
                public void onCancel(DialogInterface dialog) {
                    retryConnecting();
                }
            }).show();
            return;
        }
        // If there is an existing resolution error being displayed or a resolution
        // activity has started before, do nothing and wait for resolution
        // progress to be completed.
        if (mIsInResolution) {
            return;
        }
        mIsInResolution = true;
        try {
            Log.d(TAG, "Starting resolution.");
            result.startResolutionForResult(this, REQUEST_CODE_RESOLUTION);
        } catch (SendIntentException e) {
            Log.e(TAG, "Exception while starting resolution activity", e);
            retryConnecting();
        }
    }

    public void onStartButton(View view) {
        if (mGameService != null) {
            try {
                CheckBox checkBox = (CheckBox) findViewById(R.id.sound_checkbox);
                GameConfig config = new GameConfig.Builder()
                        .setSoundEnabled(checkBox.isChecked())
                        .build();
                mGameService.startGame(config);

            } catch (RemoteException e) {
                e.printStackTrace();
            }
        }

    }

    public void onStopButton(View view) {
        if (mGameService != null) {
            try {
                mGameService.disconnect();
            } catch (RemoteException e) {
                e.printStackTrace();
            }
        }
    }

    public void onLeaderboardsButton(View view) {
        Intent leaderboardIntent = Games.Leaderboards.getLeaderboardIntent(mGoogleApiClient,
                getString(R.string.leaderboard_high_score_basic_track));
        startActivityForResult(leaderboardIntent, REQUEST_CODE_LEADERBOARDS);
    }

    @Override
    public void onServiceConnected(ComponentName componentName, IBinder iBinder) {
        Log.d(TAG, "Service connected");
        mGameService = IActivityGameService.Stub.asInterface(iBinder);
        try {
            mGameService.connect(mGameCallbacks);
        } catch (RemoteException e) {
            e.printStackTrace();
        }
    }

    @Override
    public void onServiceDisconnected(ComponentName componentName) {
        mGameService = null;
        Log.d(TAG, "Service disconnected");
    }

    private static final class ActivityLogAdapter extends ArrayAdapter<ActivityLog> {
        private final Context mContext;
        public ActivityLogAdapter(Context context, List<ActivityLog> list) {
            super(context, android.R.layout.simple_list_item_2, list);
            mContext = context;
        }
        @Override
        public View getView(int position, View convertView, ViewGroup parent) {

            if (convertView == null) {
                convertView = LayoutInflater.from(mContext)
                        .inflate(android.R.layout.simple_list_item_2, null);
            }
            ActivityLog item = getItem(position);

            String activity = item.getActivityType();
            String time = TIME_FORMAT.format(new Date(item.getDetectedTime()));

            TextView tv1 = (TextView) convertView.findViewById(android.R.id.text1);
            TextView tv2 = (TextView) convertView.findViewById(android.R.id.text2);

            tv1.setText(activity);
            tv2.setText(time);

            return convertView;
        }
    }

    private final class FetchHistory extends AsyncTask<Void, Void, List<ActivityLog>> {

        @Override
        protected List<ActivityLog> doInBackground(Void... voids) {
            try {
                return mGameService.getHistory();
            } catch (RemoteException e) {
                e.printStackTrace();
            }
            return null;
        }

        @Override
        protected void onPostExecute(List<ActivityLog> activityLogs) {
            Log.d(TAG, "Got " + activityLogs.size() + " ActivityLogs.");
            sActivities.clear();
            sActivities.addAll(activityLogs);
            mAdapter.notifyDataSetChanged();
        }
    }
}
