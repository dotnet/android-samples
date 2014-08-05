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
import android.text.format.DateUtils;

/**
 * Parcelable to send to the Service to configure and start a game.
 */
public class GameConfig implements Parcelable {

    public static final Parcelable.Creator<GameConfig> CREATOR = new Creator();
    public final boolean useSound;
    public final long gameLengthMillis;

    private GameConfig(boolean useSound, long gameLengthMillis) {
        this.useSound = useSound;
        this.gameLengthMillis = gameLengthMillis;
    }

    @Override
    public int describeContents() {
        return Creator.CONTENT_DESCRIPTION;
    }

    @Override
    public void writeToParcel(Parcel parcel, int flags) {
        parcel.writeByte(useSound ? (byte)1 : (byte)0);
        parcel.writeLong(gameLengthMillis);
    }

    public static final class Builder {
        private boolean mUseSound = false;
        private long mGameLength = 60 * DateUtils.SECOND_IN_MILLIS;

        public Builder setSoundEnabled(boolean enabled) {
            mUseSound = enabled;
            return this;
        }

        public Builder setGameLengthMillis(long millis) {
            mGameLength = millis;
            return this;
        }

        public GameConfig build() {
            return new GameConfig(mUseSound, mGameLength);
        }
    }
    private static final class Creator implements Parcelable.Creator<GameConfig> {
        private static final int CONTENT_DESCRIPTION = 0;
        @Override
        public GameConfig createFromParcel(Parcel parcel) {
            boolean useSound = parcel.readByte() == 1 ? true : false;
            long gameLength = parcel.readLong();
            return new GameConfig(useSound, gameLength);
        }

        @Override
        public GameConfig[] newArray(int size) {
            return new GameConfig[size];
        }
    }
}
