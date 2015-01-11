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

package android.support.v4.content;

import android.content.ComponentName;
import android.content.Intent;

class IntentCompatHoneycomb {
    public static Intent makeMainActivity(ComponentName mainActivity) {
        return Intent.makeMainActivity(mainActivity);
    }

    public static Intent makeRestartActivityTask(ComponentName mainActivity) {
        return Intent.makeRestartActivityTask(mainActivity);
    }
}
