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

package com.example.activitydemo.app.service;

import android.content.Context;
import android.hardware.Sensor;
import android.hardware.SensorEvent;
import android.hardware.SensorEventListener;
import android.hardware.SensorManager;
import android.text.format.DateUtils;
import android.util.Log;

import com.example.activitydemo.app.ActivityLog;
import com.example.activitydemo.app.GameConfig;
import com.google.android.gms.location.DetectedActivity;
import com.example.activitydemo.app.R;

import java.util.Random;
import java.util.concurrent.TimeUnit;

/**
 * All the logic for the game part. Implements a game from start to end.
 */
public class GameLogic implements GameCallbacks, GameTrack.OnStageChangedListener {

    private static final String TAG = "GameLogic";

    private GameTrack mGameTrack;
    private GameConfig mGameConfig;
    private long mLastTick;
    private GameService mService;

    private AccelerometerListener mListener;

    private String[] mApprove;
    private String[] mDisapprove;
    private Random mRandom = new Random();
    private long mScore;

    public GameLogic(GameService service, GameConfig config, GameTrack.Stage[] gameTrack) {
        mGameConfig = config;
        mGameTrack = new GameTrack(gameTrack, this);
        mScore = 0;

        mService = service;

        mApprove = mService.getResources().getStringArray(R.array.approve);
        mDisapprove = mService.getResources().getStringArray(R.array.disaprove);
    }

    public long getScore() {
        return mScore;
    }

    @Override
    public boolean onTick(long timeMillis) {
        long delta = timeMillis - mLastTick;
        if (delta > GameService.GameHandler.TICK_LENGTH) {
            delta = GameService.GameHandler.TICK_LENGTH;
        }
        mLastTick = timeMillis;

        boolean running = mGameTrack.tick(delta);

        if (!running) {
            mService.sayString(mService.getString(R.string.game_over));
            mService.sayString("Your score was " + mScore);
        }
        return running;
    }

    @Override
    public void onActivityDetected(ActivityLog log) {
        GameTrack.Stage currentStage = mGameTrack.getCurrentStage();
        if (currentStage == null || currentStage.type == -1) {
            return;
        }
        int activityType = log.getDetectedActivity().getType();
        if (currentStage.type == DetectedActivity.STILL
                && activityType == DetectedActivity.STILL
                && log.getDetectedActivity().getConfidence() != 101) {
            // We are in STILL mode let the accelerometer (which says 101 confidence) be the one.
            return;
        }
        if (currentStage.type == log.getDetectedActivity().getType()) {
            mScore += 100;
            mService.updateScore(mScore);
            String approval = mApprove[mRandom.nextInt(mApprove.length)];
            mService.sayString(approval);

        } else {
            String disapproval = mDisapprove[mRandom.nextInt(mDisapprove.length)];
            mService.sayString(disapproval);
            mScore -= 25;
        }
    }

    @Override
    public void onStageChanged(GameTrack.Stage newStage) {
        if (newStage == null) {
            stopAccelerometer();
            // Game has ended.
            return;
        }
        Log.d(TAG, "New stage " + newStage);
        switch (newStage.type) {
            case -1:
                mService.sayString(mService.getString(R.string.pregame));
                break;
            case DetectedActivity.STILL:
                mService.sayString(mService.getString(R.string.stop));
                startAccelerometer();
                break;
            case DetectedActivity.WALKING:
                mService.sayString(mService.getString(R.string.start_walking));
                stopAccelerometer();
                break;
            case DetectedActivity.RUNNING:
                mService.sayString(mService.getString(R.string.start_running));
                stopAccelerometer();
                break;
        }
    }

    private void startAccelerometer() {
        if (mListener != null) {
            return;
        }
        Log.d(TAG, "Starting accelerometer");
        SensorManager sm = (SensorManager) mService.getSystemService(Context.SENSOR_SERVICE);
        Sensor mSensor = sm.getDefaultSensor(Sensor.TYPE_ACCELEROMETER);
        mListener = new AccelerometerListener();
        if (!sm.registerListener(mListener, mSensor, SensorManager.SENSOR_DELAY_NORMAL)) {
            Log.e(TAG, "Failed to register accelerometer");
        }
    }

    private void stopAccelerometer() {
        if (mListener == null) {
            return;
        }
        Log.d(TAG, "Stopping accelerometer");
        SensorManager sm = (SensorManager) mService.getSystemService(Context.SENSOR_SERVICE);
        sm.unregisterListener(mListener);
        mListener = null;
    }

    public void onStill() {
        Log.d(TAG, "Still detected");
        onActivityDetected(new ActivityLog(new DetectedActivity(DetectedActivity.STILL, 101),
                System.currentTimeMillis()));
    }

    private final class AccelerometerListener implements SensorEventListener {
        private double mLastMagnitude;
        private long mLastTrigger;
        private boolean mStill;

        public AccelerometerListener() {
            mLastMagnitude = Double.NaN;
            mLastTrigger = -1;
            mStill = true;
        }

        @Override
        public void onSensorChanged(SensorEvent sensorEvent) {
            float[] a = sensorEvent.values;
            double magnitude = Math.sqrt(a[0] * a[0] + a[1] * a[1] + a[2] * a[2]);

            //Log.d(TAG, "Magnitude " + magnitude);
            if (mLastMagnitude != Double.NaN && Math.abs(magnitude - 9.8) > 2) {
                Log.d(TAG, "Magnitude jumping " + magnitude);
                mStill = false;
            }

            long millis = TimeUnit.NANOSECONDS.toMillis(sensorEvent.timestamp);
            if (mLastTrigger == -1) {
                mLastTrigger = millis;
            }

            if (millis - mLastTrigger > 2 * DateUtils.SECOND_IN_MILLIS) {
                if (mStill) {
                    onStill();
                }

                mStill = true;
                mLastTrigger = millis;

            }

            mLastMagnitude = magnitude;
        }

        @Override
        public void onAccuracyChanged(Sensor sensor, int i) {
            // Don't care
        }
    }



}
