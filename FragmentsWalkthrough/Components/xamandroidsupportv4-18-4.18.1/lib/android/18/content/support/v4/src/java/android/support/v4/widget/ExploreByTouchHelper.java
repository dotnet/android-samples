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

package android.support.v4.widget;

import android.content.Context;
import android.graphics.Rect;
import android.os.Bundle;
import android.support.v4.view.AccessibilityDelegateCompat;
import android.support.v4.view.MotionEventCompat;
import android.support.v4.view.ViewCompat;
import android.support.v4.view.ViewParentCompat;
import android.support.v4.view.accessibility.AccessibilityEventCompat;
import android.support.v4.view.accessibility.AccessibilityManagerCompat;
import android.support.v4.view.accessibility.AccessibilityNodeInfoCompat;
import android.support.v4.view.accessibility.AccessibilityNodeProviderCompat;
import android.support.v4.view.accessibility.AccessibilityRecordCompat;
import android.view.MotionEvent;
import android.view.View;
import android.view.ViewParent;
import android.view.accessibility.AccessibilityEvent;
import android.view.accessibility.AccessibilityManager;

import java.util.LinkedList;
import java.util.List;

/**
 * ExploreByTouchHelper is a utility class for implementing accessibility
 * support in custom {@link View}s that represent a collection of View-like
 * logical items. It extends {@link AccessibilityNodeProviderCompat} and
 * simplifies many aspects of providing information to accessibility services
 * and managing accessibility focus. This class does not currently support
 * hierarchies of logical items.
 * <p>
 * This should be applied to the parent view using
 * {@link ViewCompat#setAccessibilityDelegate}:
 *
 * <pre>
 * mAccessHelper = ExploreByTouchHelper.create(someView, mAccessHelperCallback);
 * ViewCompat.setAccessibilityDelegate(someView, mAccessHelper);
 * </pre>
 */
public abstract class ExploreByTouchHelper extends AccessibilityDelegateCompat {
    /** Virtual node identifier value for invalid nodes. */
    public static final int INVALID_ID = Integer.MIN_VALUE;

    /** Default class name used for virtual views. */
    private static final String DEFAULT_CLASS_NAME = View.class.getName();

    // Temporary, reusable data structures.
    private final Rect mTempScreenRect = new Rect();
    private final Rect mTempParentRect = new Rect();
    private final Rect mTempVisibleRect = new Rect();
    private final int[] mTempGlobalRect = new int[2];

    /** System accessibility manager, used to check state and send events. */
    private final AccessibilityManager mManager;

    /** View whose internal structure is exposed through this helper. */
    private final View mView;

    /** Node provider that handles creating nodes and performing actions. */
    private ExploreByTouchNodeProvider mNodeProvider;

    /** Virtual view id for the currently focused logical item. */
    private int mFocusedVirtualViewId = INVALID_ID;

    /** Virtual view id for the currently hovered logical item. */
    private int mHoveredVirtualViewId = INVALID_ID;

    /**
     * Factory method to create a new {@link ExploreByTouchHelper}.
     *
     * @param forView View whose logical children are exposed by this helper.
     */
    public ExploreByTouchHelper(View forView) {
        if (forView == null) {
            throw new IllegalArgumentException("View may not be null");
        }

        mView = forView;
        final Context context = forView.getContext();
        mManager = (AccessibilityManager) context.getSystemService(Context.ACCESSIBILITY_SERVICE);
    }

    /**
     * Returns the {@link AccessibilityNodeProviderCompat} for this helper.
     *
     * @param host View whose logical children are exposed by this helper.
     * @return The accessibility node provider for this helper.
     */
    @Override
    public AccessibilityNodeProviderCompat getAccessibilityNodeProvider(View host) {
        if (mNodeProvider == null) {
            mNodeProvider = new ExploreByTouchNodeProvider();
        }
        return mNodeProvider;
    }

    /**
     * Dispatches hover {@link MotionEvent}s to the virtual view hierarchy when
     * the Explore by Touch feature is enabled.
     * <p>
     * This method should be called by overriding
     * {@link View#dispatchHoverEvent}:
     *
     * <pre>&#64;Override
     * public boolean dispatchHoverEvent(MotionEvent event) {
     *   if (mHelper.dispatchHoverEvent(this, event) {
     *     return true;
     *   }
     *   return super.dispatchHoverEvent(event);
     * }
     * </pre>
     *
     * @param event The hover event to dispatch to the virtual view hierarchy.
     * @return Whether the hover event was handled.
     */
    public boolean dispatchHoverEvent(MotionEvent event) {
        if (!mManager.isEnabled()
                || !AccessibilityManagerCompat.isTouchExplorationEnabled(mManager)) {
            return false;
        }

        switch (event.getAction()) {
            case MotionEventCompat.ACTION_HOVER_MOVE:
            case MotionEventCompat.ACTION_HOVER_ENTER:
                final int virtualViewId = getVirtualViewAt(event.getX(), event.getY());
                updateHoveredVirtualView(virtualViewId);
                return (virtualViewId != INVALID_ID);
            case MotionEventCompat.ACTION_HOVER_EXIT:
                if (mFocusedVirtualViewId != INVALID_ID) {
                    updateHoveredVirtualView(INVALID_ID);
                    return true;
                }
                return false;
            default:
                return false;
        }
    }

    /**
     * Populates an event of the specified type with information about an item
     * and attempts to send it up through the view hierarchy.
     * <p>
     * You should call this method after performing a user action that normally
     * fires an accessibility event, such as clicking on an item.
     *
     * <pre>public void performItemClick(T item) {
     *   ...
     *   sendEventForVirtualViewId(item.id, AccessibilityEvent.TYPE_VIEW_CLICKED);
     * }
     * </pre>
     *
     * @param virtualViewId The virtual view id for which to send an event.
     * @param eventType The type of event to send.
     * @return true if the event was sent successfully.
     */
    public boolean sendEventForVirtualView(int virtualViewId, int eventType) {
        if ((virtualViewId == INVALID_ID) || !mManager.isEnabled()) {
            return false;
        }

        final ViewParent parent = mView.getParent();
        if (parent == null) {
            return false;
        }

        final AccessibilityEvent event = createEvent(virtualViewId, eventType);
        return ViewParentCompat.requestSendAccessibilityEvent(parent, mView, event);
    }

    /**
     * Notifies the accessibility framework that the properties of the parent
     * view have changed.
     * <p>
     * You <b>must</b> call this method after adding or removing items from the
     * parent view.
     */
    public void invalidateRoot() {
        invalidateVirtualView(View.NO_ID);
    }

    /**
     * Notifies the accessibility framework that the properties of a particular
     * item have changed.
     * <p>
     * You <b>must</b> call this method after changing any of the properties set
     * in {@link #onPopulateNodeForVirtualView}.
     *
     * @param virtualViewId The virtual view id to invalidate.
     */
    public void invalidateVirtualView(int virtualViewId) {
        sendEventForVirtualView(
                virtualViewId, AccessibilityEventCompat.TYPE_WINDOW_CONTENT_CHANGED);
    }

    /**
     * Returns the virtual view id for the currently focused item,
     *
     * @return A virtual view id, or {@link #INVALID_ID} if no item is
     *         currently focused.
     */
    public int getFocusedVirtualView() {
        return mFocusedVirtualViewId;
    }

    /**
     * Sets the currently hovered item, sending hover accessibility events as
     * necessary to maintain the correct state.
     *
     * @param virtualViewId The virtual view id for the item currently being
     *            hovered, or {@link #INVALID_ID} if no item is hovered within
     *            the parent view.
     */
    private void updateHoveredVirtualView(int virtualViewId) {
        if (mHoveredVirtualViewId == virtualViewId) {
            return;
        }

        final int previousVirtualViewId = mHoveredVirtualViewId;
        mHoveredVirtualViewId = virtualViewId;

        // Stay consistent with framework behavior by sending ENTER/EXIT pairs
        // in reverse order. This is accurate as of API 18.
        sendEventForVirtualView(virtualViewId, AccessibilityEventCompat.TYPE_VIEW_HOVER_ENTER);
        sendEventForVirtualView(
                previousVirtualViewId, AccessibilityEventCompat.TYPE_VIEW_HOVER_EXIT);
    }

    /**
     * Constructs and returns an {@link AccessibilityEvent} for the specified
     * virtual view id, which includes the host view ({@link View#NO_ID}).
     *
     * @param virtualViewId The virtual view id for the item for which to
     *            construct an event.
     * @param eventType The type of event to construct.
     * @return An {@link AccessibilityEvent} populated with information about
     *         the specified item.
     */
    private AccessibilityEvent createEvent(int virtualViewId, int eventType) {
        switch (virtualViewId) {
            case View.NO_ID:
                return createEventForHost(eventType);
            default:
                return createEventForChild(virtualViewId, eventType);
        }
    }

    /**
     * Constructs and returns an {@link AccessibilityEvent} for the host node.
     *
     * @param eventType The type of event to construct.
     * @return An {@link AccessibilityEvent} populated with information about
     *         the specified item.
     */
    private AccessibilityEvent createEventForHost(int eventType) {
        final AccessibilityEvent event = AccessibilityEvent.obtain(eventType);
        ViewCompat.onInitializeAccessibilityEvent(mView, event);
        return event;
    }

    /**
     * Constructs and returns an {@link AccessibilityEvent} populated with
     * information about the specified item.
     *
     * @param virtualViewId The virtual view id for the item for which to
     *            construct an event.
     * @param eventType The type of event to construct.
     * @return An {@link AccessibilityEvent} populated with information about
     *         the specified item.
     */
    private AccessibilityEvent createEventForChild(int virtualViewId, int eventType) {
        final AccessibilityEvent event = AccessibilityEvent.obtain(eventType);
        event.setEnabled(true);
        event.setClassName(DEFAULT_CLASS_NAME);

        // Allow the client to populate the event.
        onPopulateEventForVirtualView(virtualViewId, event);

        // Make sure the developer is following the rules.
        if (event.getText().isEmpty() && (event.getContentDescription() == null)) {
            throw new RuntimeException("Callbacks must add text or a content description in "
                    + "populateEventForVirtualViewId()");
        }

        // Don't allow the client to override these properties.
        event.setPackageName(mView.getContext().getPackageName());

        final AccessibilityRecordCompat record = AccessibilityEventCompat.asRecord(event);
        record.setSource(mView, virtualViewId);

        return event;
    }

    /**
     * Constructs and returns an {@link AccessibilityNodeInfoCompat} for the
     * specified virtual view id, which includes the host view
     * ({@link View#NO_ID}).
     *
     * @param virtualViewId The virtual view id for the item for which to
     *            construct a node.
     * @return An {@link AccessibilityNodeInfoCompat} populated with information
     *         about the specified item.
     */
    private AccessibilityNodeInfoCompat createNode(int virtualViewId) {
        switch (virtualViewId) {
            case View.NO_ID:
                return createNodeForHost();
            default:
                return createNodeForChild(virtualViewId);
        }
    }

    /**
     * Constructs and returns an {@link AccessibilityNodeInfoCompat} for the
     * host view populated with its virtual descendants.
     *
     * @return An {@link AccessibilityNodeInfoCompat} for the parent node.
     */
    private AccessibilityNodeInfoCompat createNodeForHost() {
        final AccessibilityNodeInfoCompat node = AccessibilityNodeInfoCompat.obtain(mView);
        ViewCompat.onInitializeAccessibilityNodeInfo(mView, node);

        // Add the virtual descendants.
        final LinkedList<Integer> virtualViewIds = new LinkedList<Integer>();
        getVisibleVirtualViews(virtualViewIds);

        for (Integer childVirtualViewId : virtualViewIds) {
            node.addChild(mView, childVirtualViewId);
        }

        return node;
    }

    /**
     * Constructs and returns an {@link AccessibilityNodeInfoCompat} for the
     * specified item. Automatically manages accessibility focus actions.
     * <p>
     * Allows the implementing class to specify most node properties, but
     * overrides the following:
     * <ul>
     * <li>{@link AccessibilityNodeInfoCompat#setPackageName}
     * <li>{@link AccessibilityNodeInfoCompat#setClassName}
     * <li>{@link AccessibilityNodeInfoCompat#setParent(View)}
     * <li>{@link AccessibilityNodeInfoCompat#setSource(View, int)}
     * <li>{@link AccessibilityNodeInfoCompat#setVisibleToUser}
     * <li>{@link AccessibilityNodeInfoCompat#setBoundsInScreen(Rect)}
     * </ul>
     * <p>
     * Uses the bounds of the parent view and the parent-relative bounding
     * rectangle specified by
     * {@link AccessibilityNodeInfoCompat#getBoundsInParent} to automatically
     * update the following properties:
     * <ul>
     * <li>{@link AccessibilityNodeInfoCompat#setVisibleToUser}
     * <li>{@link AccessibilityNodeInfoCompat#setBoundsInParent}
     * </ul>
     *
     * @param virtualViewId The virtual view id for item for which to construct
     *            a node.
     * @return An {@link AccessibilityNodeInfoCompat} for the specified item.
     */
    private AccessibilityNodeInfoCompat createNodeForChild(int virtualViewId) {
        final AccessibilityNodeInfoCompat node = AccessibilityNodeInfoCompat.obtain();

        // Ensure the client has good defaults.
        node.setEnabled(true);
        node.setClassName(DEFAULT_CLASS_NAME);

        // Allow the client to populate the node.
        onPopulateNodeForVirtualView(virtualViewId, node);

        // Make sure the developer is following the rules.
        if ((node.getText() == null) && (node.getContentDescription() == null)) {
            throw new RuntimeException("Callbacks must add text or a content description in "
                    + "populateNodeForVirtualViewId()");
        }

        node.getBoundsInParent(mTempParentRect);
        if (mTempParentRect.isEmpty()) {
            throw new RuntimeException("Callbacks must set parent bounds in "
                    + "populateNodeForVirtualViewId()");
        }

        final int actions = node.getActions();
        if ((actions & AccessibilityNodeInfoCompat.ACTION_ACCESSIBILITY_FOCUS) != 0) {
            throw new RuntimeException("Callbacks must not add ACTION_ACCESSIBILITY_FOCUS in "
                    + "populateNodeForVirtualViewId()");
        }
        if ((actions & AccessibilityNodeInfoCompat.ACTION_CLEAR_ACCESSIBILITY_FOCUS) != 0) {
            throw new RuntimeException("Callbacks must not add ACTION_CLEAR_ACCESSIBILITY_FOCUS in "
                    + "populateNodeForVirtualViewId()");
        }

        // Don't allow the client to override these properties.
        node.setPackageName(mView.getContext().getPackageName());
        node.setSource(mView, virtualViewId);
        node.setParent(mView);

        // Manage internal accessibility focus state.
        if (mFocusedVirtualViewId == virtualViewId) {
            node.setAccessibilityFocused(true);
            node.addAction(AccessibilityNodeInfoCompat.ACTION_CLEAR_ACCESSIBILITY_FOCUS);
        } else {
            node.setAccessibilityFocused(false);
            node.addAction(AccessibilityNodeInfoCompat.ACTION_ACCESSIBILITY_FOCUS);
        }

        // Set the visibility based on the parent bound.
        if (intersectVisibleToUser(mTempParentRect)) {
            node.setVisibleToUser(true);
            node.setBoundsInParent(mTempParentRect);
        }

        // Calculate screen-relative bound.
        mView.getLocationOnScreen(mTempGlobalRect);
        final int offsetX = mTempGlobalRect[0];
        final int offsetY = mTempGlobalRect[1];
        mTempScreenRect.set(mTempParentRect);
        mTempScreenRect.offset(offsetX, offsetY);
        node.setBoundsInScreen(mTempScreenRect);

        return node;
    }

    private boolean performAction(int virtualViewId, int action, Bundle arguments) {
        switch (virtualViewId) {
            case View.NO_ID:
                return performActionForHost(action, arguments);
            default:
                return performActionForChild(virtualViewId, action, arguments);
        }
    }

    private boolean performActionForHost(int action, Bundle arguments) {
        return ViewCompat.performAccessibilityAction(mView, action, arguments);
    }

    private boolean performActionForChild(int virtualViewId, int action, Bundle arguments) {
        switch (action) {
            case AccessibilityNodeInfoCompat.ACTION_ACCESSIBILITY_FOCUS:
            case AccessibilityNodeInfoCompat.ACTION_CLEAR_ACCESSIBILITY_FOCUS:
                return manageFocusForChild(virtualViewId, action, arguments);
            default:
                return onPerformActionForVirtualView(virtualViewId, action, arguments);
        }
    }

    private boolean manageFocusForChild(int virtualViewId, int action, Bundle arguments) {
        switch (action) {
            case AccessibilityNodeInfoCompat.ACTION_ACCESSIBILITY_FOCUS:
                return requestAccessibilityFocus(virtualViewId);
            case AccessibilityNodeInfoCompat.ACTION_CLEAR_ACCESSIBILITY_FOCUS:
                return clearAccessibilityFocus(virtualViewId);
            default:
                return false;
        }
    }

    /**
     * Computes whether the specified {@link Rect} intersects with the visible
     * portion of its parent {@link View}. Modifies {@code localRect} to contain
     * only the visible portion.
     *
     * @param localRect A rectangle in local (parent) coordinates.
     * @return Whether the specified {@link Rect} is visible on the screen.
     */
    private boolean intersectVisibleToUser(Rect localRect) {
        // Missing or empty bounds mean this view is not visible.
        if ((localRect == null) || localRect.isEmpty()) {
            return false;
        }

        // Attached to invisible window means this view is not visible.
        if (mView.getWindowVisibility() != View.VISIBLE) {
            return false;
        }

        // An invisible predecessor means that this view is not visible.
        ViewParent viewParent = mView.getParent();
        while (viewParent instanceof View) {
            final View view = (View) viewParent;
            if ((ViewCompat.getAlpha(view) <= 0) || (view.getVisibility() != View.VISIBLE)) {
                return false;
            }
            viewParent = view.getParent();
        }

        // A null parent implies the view is not visible.
        if (viewParent == null) {
            return false;
        }

        // If no portion of the parent is visible, this view is not visible.
        if (!mView.getLocalVisibleRect(mTempVisibleRect)) {
            return false;
        }

        // Check if the view intersects the visible portion of the parent.
        return localRect.intersect(mTempVisibleRect);
    }

    /**
     * Returns whether this virtual view is accessibility focused.
     *
     * @return True if the view is accessibility focused.
     */
    private boolean isAccessibilityFocused(int virtualViewId) {
        return (mFocusedVirtualViewId == virtualViewId);
    }

    /**
     * Attempts to give accessibility focus to a virtual view.
     * <p>
     * A virtual view will not actually take focus if
     * {@link AccessibilityManager#isEnabled()} returns false,
     * {@link AccessibilityManager#isTouchExplorationEnabled()} returns false,
     * or the view already has accessibility focus.
     *
     * @param virtualViewId The id of the virtual view on which to place
     *            accessibility focus.
     * @return Whether this virtual view actually took accessibility focus.
     */
    private boolean requestAccessibilityFocus(int virtualViewId) {
        if (!mManager.isEnabled()
                || !AccessibilityManagerCompat.isTouchExplorationEnabled(mManager)) {
            return false;
        }
        // TODO: Check virtual view visibility.
        if (!isAccessibilityFocused(virtualViewId)) {
            mFocusedVirtualViewId = virtualViewId;
            // TODO: Only invalidate virtual view bounds.
            mView.invalidate();
            sendEventForVirtualView(virtualViewId,
                    AccessibilityEventCompat.TYPE_VIEW_ACCESSIBILITY_FOCUSED);
            return true;
        }
        return false;
    }

    /**
     * Attempts to clear accessibility focus from a virtual view.
     *
     * @param virtualViewId The id of the virtual view from which to clear
     *            accessibility focus.
     * @return Whether this virtual view actually cleared accessibility focus.
     */
    private boolean clearAccessibilityFocus(int virtualViewId) {
        if (isAccessibilityFocused(virtualViewId)) {
            mFocusedVirtualViewId = INVALID_ID;
            mView.invalidate();
            sendEventForVirtualView(virtualViewId,
                    AccessibilityEventCompat.TYPE_VIEW_ACCESSIBILITY_FOCUS_CLEARED);
            return true;
        }
        return false;
    }

    /**
     * Provides a mapping between view-relative coordinates and logical
     * items.
     *
     * @param x The view-relative x coordinate
     * @param y The view-relative y coordinate
     * @return virtual view identifier for the logical item under
     *         coordinates (x,y)
     */
    protected abstract int getVirtualViewAt(float x, float y);

    /**
     * Populates a list with the view's visible items. The ordering of items
     * within {@code virtualViewIds} specifies order of accessibility focus
     * traversal.
     *
     * @param virtualViewIds The list to populate with visible items
     */
    protected abstract void getVisibleVirtualViews(List<Integer> virtualViewIds);

    /**
     * Populates an {@link AccessibilityEvent} with information about the
     * specified item.
     * <p>
     * Implementations <b>must</b> populate the following required fields:
     * <ul>
     * <li>event text, see {@link AccessibilityEvent#getText} or
     * {@link AccessibilityEvent#setContentDescription}
     * </ul>
     * <p>
     * The helper class automatically populates the following fields with
     * default values, but implementations may optionally override them:
     * <ul>
     * <li>item class name, set to android.view.View, see
     * {@link AccessibilityEvent#setClassName}
     * </ul>
     * <p>
     * The following required fields are automatically populated by the
     * helper class and may not be overridden:
     * <ul>
     * <li>package name, set to the package of the host view's
     * {@link Context}, see {@link AccessibilityEvent#setPackageName}
     * <li>event source, set to the host view and virtual view identifier,
     * see {@link AccessibilityRecordCompat#setSource(View, int)}
     * </ul>
     *
     * @param virtualViewId The virtual view id for the item for which to
     *            populate the event
     * @param event The event to populate
     */
    protected abstract void onPopulateEventForVirtualView(
            int virtualViewId, AccessibilityEvent event);

    /**
     * Populates an {@link AccessibilityNodeInfoCompat} with information
     * about the specified item.
     * <p>
     * Implementations <b>must</b> populate the following required fields:
     * <ul>
     * <li>event text, see {@link AccessibilityNodeInfoCompat#setText} or
     * {@link AccessibilityNodeInfoCompat#setContentDescription}
     * <li>bounds in parent coordinates, see
     * {@link AccessibilityNodeInfoCompat#setBoundsInParent}
     * </ul>
     * <p>
     * The helper class automatically populates the following fields with
     * default values, but implementations may optionally override them:
     * <ul>
     * <li>enabled state, set to true, see
     * {@link AccessibilityNodeInfoCompat#setEnabled}
     * <li>item class name, identical to the class name set by
     * {@link #onPopulateEventForVirtualView}, see
     * {@link AccessibilityNodeInfoCompat#setClassName}
     * </ul>
     * <p>
     * The following required fields are automatically populated by the
     * helper class and may not be overridden:
     * <ul>
     * <li>package name, identical to the package name set by
     * {@link #onPopulateEventForVirtualView}, see
     * {@link AccessibilityNodeInfoCompat#setPackageName}
     * <li>node source, identical to the event source set in
     * {@link #onPopulateEventForVirtualView}, see
     * {@link AccessibilityNodeInfoCompat#setSource(View, int)}
     * <li>parent view, set to the host view, see
     * {@link AccessibilityNodeInfoCompat#setParent(View)}
     * <li>visibility, computed based on parent-relative bounds, see
     * {@link AccessibilityNodeInfoCompat#setVisibleToUser}
     * <li>accessibility focus, computed based on internal helper state, see
     * {@link AccessibilityNodeInfoCompat#setAccessibilityFocused}
     * <li>bounds in screen coordinates, computed based on host view bounds,
     * see {@link AccessibilityNodeInfoCompat#setBoundsInScreen}
     * </ul>
     * <p>
     * Additionally, the helper class automatically handles accessibility
     * focus management by adding the appropriate
     * {@link AccessibilityNodeInfoCompat#ACTION_ACCESSIBILITY_FOCUS} or
     * {@link AccessibilityNodeInfoCompat#ACTION_CLEAR_ACCESSIBILITY_FOCUS}
     * action. Implementations must <b>never</b> manually add these actions.
     * <p>
     * The helper class also automatically modifies parent- and
     * screen-relative bounds to reflect the portion of the item visible
     * within its parent.
     *
     * @param virtualViewId The virtual view identifier of the item for
     *            which to populate the node
     * @param node The node to populate
     */
    protected abstract void onPopulateNodeForVirtualView(
            int virtualViewId, AccessibilityNodeInfoCompat node);

    /**
     * Performs the specified accessibility action on the item associated
     * with the virtual view identifier. See
     * {@link AccessibilityNodeInfoCompat#performAction(int, Bundle)} for
     * more information.
     * <p>
     * Implementations <b>must</b> handle any actions added manually in
     * {@link #onPopulateNodeForVirtualView}.
     * <p>
     * The helper class automatically handles focus management resulting
     * from {@link AccessibilityNodeInfoCompat#ACTION_ACCESSIBILITY_FOCUS}
     * and
     * {@link AccessibilityNodeInfoCompat#ACTION_CLEAR_ACCESSIBILITY_FOCUS}
     * actions.
     *
     * @param virtualViewId The virtual view identifier of the item on which
     *            to perform the action
     * @param action The accessibility action to perform
     * @param arguments (Optional) A bundle with additional arguments, or
     *            null
     * @return true if the action was performed
     */
    protected abstract boolean onPerformActionForVirtualView(
            int virtualViewId, int action, Bundle arguments);

    /**
     * Exposes a virtual view hierarchy to the accessibility framework. Only
     * used in API 16+.
     */
    private class ExploreByTouchNodeProvider extends AccessibilityNodeProviderCompat {
        @Override
        public AccessibilityNodeInfoCompat createAccessibilityNodeInfo(int virtualViewId) {
            return ExploreByTouchHelper.this.createNode(virtualViewId);
        }

        @Override
        public boolean performAction(int virtualViewId, int action, Bundle arguments) {
            return ExploreByTouchHelper.this.performAction(virtualViewId, action, arguments);
        }
    }
}
