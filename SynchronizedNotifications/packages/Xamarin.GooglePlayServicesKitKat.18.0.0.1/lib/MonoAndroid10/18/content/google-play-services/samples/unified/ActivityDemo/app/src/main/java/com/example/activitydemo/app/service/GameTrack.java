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

import android.text.format.DateUtils;

import com.example.activitydemo.app.ActivityLog;
import com.google.android.gms.location.DetectedActivity;

/**
 * Manages stages for a game being played.
 */
public class GameTrack {

    private Stage[] mStage;
    private int mStageIndex;
    private long mStageElapsed;
    private OnStageChangedListener mListener;
    private boolean mStarted;

    public static final class Stage {
        public final int type;
        public final long lengthMillis;
        public Stage(int type, long lengthMillis) {
            this.type = type;
            this.lengthMillis = lengthMillis;
        }

        @Override
        public String toString() {
            return ActivityLog.ACTIVITY_MAP.get(type) + "[" + lengthMillis + "]";
        }
    }

    public interface OnStageChangedListener {
        public void onStageChanged(Stage newStage);
    }

    public static final Stage[] BASIC_TRACK = {
            new Stage(-1, 5 * DateUtils.SECOND_IN_MILLIS),
            new Stage(DetectedActivity.STILL, 5 * DateUtils.SECOND_IN_MILLIS),
            new Stage(DetectedActivity.WALKING, 20 * DateUtils.SECOND_IN_MILLIS),
            new Stage(DetectedActivity.STILL, 7 * DateUtils.SECOND_IN_MILLIS),
            new Stage(DetectedActivity.WALKING, 15 * DateUtils.SECOND_IN_MILLIS),
            new Stage(DetectedActivity.RUNNING, 15 * DateUtils.SECOND_IN_MILLIS),
            new Stage(DetectedActivity.STILL, 5 * DateUtils.SECOND_IN_MILLIS),
    };

    public GameTrack(Stage[] stages, OnStageChangedListener listener) {
        if (stages == null) {
            throw new IllegalArgumentException("Need to provide Stages");
        }
        if (listener == null) {
            throw new IllegalArgumentException("Need to provide a listener");
        }

        mStage = stages;
        mStageElapsed = 0;
        mStageIndex = 0;
        mStarted = false;

        mListener = listener;
    }

    public Stage getCurrentStage() {
        if (mStageIndex >= mStage.length) {
            return null;
        }
        return mStage[mStageIndex];
    }

    /**
     *
     * @param delta
     * @return true if the track is still going.
     */
    public boolean tick(long delta) {
        if (!mStarted) {
            mListener.onStageChanged(mStage[mStageIndex]);
            mStarted = true;
        }
        mStageElapsed += delta;
        Stage stage = mStage[mStageIndex];
        if (mStageElapsed > stage.lengthMillis) {
            mStageElapsed -= stage.lengthMillis;
            return nextStage();
        }
        return true;
    }

    public boolean nextStage() {
        mStageIndex++;

        if (mStageIndex >= mStage.length) {
            mListener.onStageChanged(null);
            return false;
        }

        mListener.onStageChanged(mStage[mStageIndex]);
        return true;
    }
}
