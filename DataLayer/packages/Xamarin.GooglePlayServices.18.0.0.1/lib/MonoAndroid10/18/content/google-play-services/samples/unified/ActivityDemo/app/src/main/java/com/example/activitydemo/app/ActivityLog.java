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

import android.os.Parcel;
import android.os.Parcelable;

import com.google.android.gms.location.DetectedActivity;

import java.util.HashMap;

/**
 * Parcelable to pass from the Service to the Activity.
 */
public class ActivityLog implements Parcelable {
    public static final Parcelable.Creator<ActivityLog> CREATOR = new Creator();


    public static final HashMap<Integer, String> ACTIVITY_MAP = new HashMap<Integer, String>();

    static {
        ACTIVITY_MAP.put(DetectedActivity.STILL, "STILL");
        ACTIVITY_MAP.put(DetectedActivity.TILTING, "TILTING");
        ACTIVITY_MAP.put(DetectedActivity.RUNNING, "RUNNING");
        ACTIVITY_MAP.put(DetectedActivity.WALKING, "WALKING");
        ACTIVITY_MAP.put(DetectedActivity.ON_FOOT, "ON_FOOT");
    }

    private DetectedActivity mActivity;
    private long mTime;

    public ActivityLog(DetectedActivity activity, long time) {
        mActivity = activity;
        mTime = time;
    }

    public DetectedActivity getDetectedActivity() {
        return mActivity;
    }

    public long getDetectedTime() {
        return mTime;
    }

    @Override
    public int describeContents() {
        return Creator.CONTENT_DESCRIPTION;
    }

    @Override
    public void writeToParcel(Parcel parcel, int flags) {
        parcel.writeInt(mActivity.getConfidence());
        parcel.writeInt(mActivity.getType());
        parcel.writeLong(mTime);
    }

    private static final class Creator implements Parcelable.Creator<ActivityLog> {
        private static final int CONTENT_DESCRIPTION = 0;
        @Override
        public ActivityLog createFromParcel(Parcel parcel) {
            int confidence = parcel.readInt();
            int activity = parcel.readInt();
            long time = parcel.readLong();
            DetectedActivity detectedActivity = new DetectedActivity(activity, confidence);
            return new ActivityLog(detectedActivity, time);
        }

        @Override
        public ActivityLog[] newArray(int size) {
            return new ActivityLog[size];
        }
    }

    public String getActivityType() {
        return ACTIVITY_MAP.get(mActivity.getType());
    }

    @Override
    public String toString() {
        return getActivityType() + "[" + mTime + "]";
    }
}
