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

package android.support.v4.view;

import android.view.View;
import android.view.Window;

/**
 * Helper for accessing features in {@link Window} introduced after API
 * level 4 in a backwards compatible fashion.
 */
public class WindowCompat {
    /**
     * Flag for enabling the Action Bar.
     * This is enabled by default for some devices. The Action Bar
     * replaces the title bar and provides an alternate location
     * for an on-screen menu button on some devices.
     */
    public static final int FEATURE_ACTION_BAR = 8;

    /**
     * Flag for requesting an Action Bar that overlays window content.
     * Normally an Action Bar will sit in the space above window content, but if this
     * feature is requested along with {@link #FEATURE_ACTION_BAR} it will be layered over
     * the window content itself. This is useful if you would like your app to have more control
     * over how the Action Bar is displayed, such as letting application content scroll beneath
     * an Action Bar with a transparent background or otherwise displaying a transparent/translucent
     * Action Bar over application content.
     *
     * <p>This mode is especially useful with {@link View#SYSTEM_UI_FLAG_FULLSCREEN
     * View.SYSTEM_UI_FLAG_FULLSCREEN}, which allows you to seamlessly hide the
     * action bar in conjunction with other screen decorations.
     *
     * <p>As of {@link android.os.Build.VERSION_CODES#JELLY_BEAN}, when an
     * ActionBar is in this mode it will adjust the insets provided to
     * {@link View#fitSystemWindows(android.graphics.Rect) View.fitSystemWindows(Rect)}
     * to include the content covered by the action bar, so you can do layout within
     * that space.
     */
    public static final int FEATURE_ACTION_BAR_OVERLAY = 9;

    /**
     * Flag for specifying the behavior of action modes when an Action Bar is not present.
     * If overlay is enabled, the action mode UI will be allowed to cover existing window content.
     */
    public static final int FEATURE_ACTION_MODE_OVERLAY = 10;
}
