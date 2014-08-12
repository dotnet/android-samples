// Copyright 2013 Google Inc. All Rights Reserved.

package com.google.android.gms.cast.samples.democastplayer;

import com.google.android.gms.cast.CastMediaControlIntent;
import com.google.android.gms.cast.MediaInfo;
import com.google.android.gms.cast.MediaMetadata;
import com.google.android.gms.common.images.WebImage;

import android.app.PendingIntent;
import android.content.BroadcastReceiver;
import android.content.Context;
import android.content.Intent;
import android.content.IntentFilter;
import android.net.Uri;
import android.os.Bundle;
import android.os.SystemClock;
import android.support.v7.media.MediaControlIntent;
import android.support.v7.media.MediaItemMetadata;
import android.support.v7.media.MediaItemStatus;
import android.support.v7.media.MediaRouter;
import android.support.v7.media.MediaRouter.RouteInfo;
import android.support.v7.media.MediaSessionStatus;
import android.text.TextUtils;
import android.util.Log;
import android.view.View;

import java.util.List;

/**
 * Activity to cast media to a Cast enabled device using only the MediaRouter API.
 */
public class MrpCastPlayerActivity extends BaseCastPlayerActivity {
    private static final String TAG = "MrpCastPlayerActivity";
    private static final String ACTION_RECEIVE_SESSION_STATUS_UPDATE =
            "com.google.android.gms.cast.samples.democastplayer.RECEIVE_SESSION_STATUS_UPDATE";
    private static final String ACTION_RECEIVE_MEDIA_STATUS_UPDATE =
            "com.google.android.gms.cast.samples.democastplayer.RECEIVE_MEDIA_STATUS_UPDATE";

    private RouteInfo mCurrentRoute;
    private String mLastRouteId;
    private String mSessionId;
    private boolean mSessionActive;
    private PendingIntent mSessionStatusUpdateIntent;
    private IntentFilter mSessionStatusBroadcastIntentFilter;
    private BroadcastReceiver mSessionStatusBroadcastReceiver;
    private String mCurrentItemId;
    private PendingIntent mMediaStatusUpdateIntent;
    private IntentFilter mMediaStatusBroadcastIntentFilter;
    private BroadcastReceiver mMediaStatusBroadcastReceiver;
    private long mStreamPositionTimestamp;
    private long mLastKnownStreamPosition;
    private long mStreamDuration;
    private boolean mStreamAdvancing;
    private ResultBundleHandler mMediaResultHandler;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);

        setAppControlsVisible(false);
        setSessionControlsVisible(true);
        setAutoplayCheckboxVisible(false);
        setStreamVolumeControlsVisible(false);
        setSeekBehaviorControlsVisible(false);
        mDeviceMuteCheckBox.setVisibility(View.GONE);
        setCurrentDeviceName(null);

        // Construct a broadcast receiver and a PendingIntent for receiving session status
        // updates from the MRP.
        mSessionStatusBroadcastReceiver = new BroadcastReceiver() {
                @Override
                public void onReceive(Context context, Intent intent) {
                    Log.d(TAG, "Got a session status broadcast intent from the MRP: " + intent);
                    processSessionStatusBundle(intent.getExtras());
                }
        };
        mSessionStatusBroadcastIntentFilter = new IntentFilter(
                ACTION_RECEIVE_SESSION_STATUS_UPDATE);

        Intent intent = new Intent(ACTION_RECEIVE_SESSION_STATUS_UPDATE);
        intent.setComponent(getCallingActivity());
        mSessionStatusUpdateIntent = PendingIntent.getBroadcast(this, 0, intent,
                PendingIntent.FLAG_UPDATE_CURRENT);

        // Construct a broadcast receiver and a PendingIntent for receiving media status
        // updates from the MRP.
        mMediaStatusBroadcastReceiver = new BroadcastReceiver() {
                @Override
                public void onReceive(Context context, Intent intent) {
                    Log.d(TAG, "Got a media status broadcast intent from the MRP: " + intent);
                    processMediaStatusBundle(intent.getExtras());
                }
        };
        mMediaStatusBroadcastIntentFilter = new IntentFilter(ACTION_RECEIVE_MEDIA_STATUS_UPDATE);

        intent = new Intent(ACTION_RECEIVE_MEDIA_STATUS_UPDATE);
        intent.setComponent(getCallingActivity());
        mMediaStatusUpdateIntent = PendingIntent.getBroadcast(this, 0, intent,
                PendingIntent.FLAG_UPDATE_CURRENT);

        mMediaResultHandler = new ResultBundleHandler() {

            @Override
            public void handleResult(Bundle bundle) {
                processMediaStatusBundle(bundle);
                updateButtonStates();
            }
        };


        clearStreamState();
    }

    @Override
    public void onPause() {
        cancelRefreshTimer();
        unregisterReceiver(mSessionStatusBroadcastReceiver);
        unregisterReceiver(mMediaStatusBroadcastReceiver);
        super.onPause();
    }

    @Override
    public void onResume() {
        super.onResume();
        startRefreshTimer();
        updateButtonStates();
        registerReceiver(mSessionStatusBroadcastReceiver, mSessionStatusBroadcastIntentFilter);
        registerReceiver(mMediaStatusBroadcastReceiver, mMediaStatusBroadcastIntentFilter);

        requestSessionStatus();
    }

    @Override
    protected void onVolumeChange(double delta) {
        if ((mCurrentItemId != null) && (mCurrentRoute != null)) {
            mCurrentRoute.requestUpdateVolume((int) (delta * MAX_VOLUME_LEVEL));
            refreshDeviceVolume(mCurrentRoute.getVolume() / MAX_VOLUME_LEVEL, false);
        }
    }

    @Override
    protected void onPlayMedia(final MediaInfo media) {
        if (media == null) {
            return;
        }
        MediaMetadata metadata = media.getMetadata();
        Log.d(TAG, "Casting " + metadata.getString(MediaMetadata.KEY_TITLE) + " ("
                + media.getContentType() + ")");

        Intent intent = new Intent(MediaControlIntent.ACTION_PLAY);
        intent.addCategory(MediaControlIntent.CATEGORY_REMOTE_PLAYBACK);
        intent.setDataAndType(Uri.parse(media.getContentId()), media.getContentType());
        intent.putExtra(MediaControlIntent.EXTRA_SESSION_ID, mSessionId);
        intent.putExtra(MediaControlIntent.EXTRA_ITEM_STATUS_UPDATE_RECEIVER,
                mMediaStatusUpdateIntent);

        Bundle metadataBundle = new Bundle();

        String title = metadata.getString(MediaMetadata.KEY_TITLE);
        if (!TextUtils.isEmpty(title)) {
            metadataBundle.putString(MediaItemMetadata.KEY_TITLE, title);
        }

        List<WebImage> images = metadata.getImages();
        String artist = metadata.getString(MediaMetadata.KEY_ARTIST);
        if (artist == null) {
            artist = metadata.getString(MediaMetadata.KEY_STUDIO);
        }
        if (!TextUtils.isEmpty(artist)) {
            metadataBundle.putString(MediaItemMetadata.KEY_ARTIST, artist);
        }

        if ((images != null) && !images.isEmpty()) {
            Uri imageUrl = images.get(0).getUrl();
            if (imageUrl != null) {
                metadataBundle.putString(MediaItemMetadata.KEY_ARTWORK_URI, imageUrl.toString());
            }
        }

        intent.putExtra(MediaControlIntent.EXTRA_ITEM_METADATA, metadataBundle);

        sendIntentToRoute(intent, mMediaResultHandler);
    }

    @Override
    protected void onStartSessionClicked() {
        startSession();
    }

    @Override
    protected void onEndSessionClicked() {
        endSession();
    }

    @Override
    protected void onPlayClicked() {
        if (mCurrentItemId == null) {
            return;
        }

        Intent intent = new Intent(MediaControlIntent.ACTION_RESUME);
        intent.addCategory(MediaControlIntent.CATEGORY_REMOTE_PLAYBACK);
        intent.putExtra(MediaControlIntent.EXTRA_SESSION_ID, mSessionId);
        sendIntentToRoute(intent, mMediaResultHandler);
    }

    @Override
    protected void onPauseClicked() {
        if (mCurrentItemId == null) {
            return;
        }

        Intent intent = new Intent(MediaControlIntent.ACTION_PAUSE);
        intent.addCategory(MediaControlIntent.CATEGORY_REMOTE_PLAYBACK);
        intent.putExtra(MediaControlIntent.EXTRA_SESSION_ID, mSessionId);
        sendIntentToRoute(intent, mMediaResultHandler);
    }

    @Override
    protected void onStopClicked() {
        if (mCurrentItemId == null) {
            return;
        }

        Intent intent = new Intent(MediaControlIntent.ACTION_STOP);
        intent.addCategory(MediaControlIntent.CATEGORY_REMOTE_PLAYBACK);
        intent.putExtra(MediaControlIntent.EXTRA_SESSION_ID, mSessionId);
        sendIntentToRoute(intent, mMediaResultHandler);
    }

    @Override
    protected void onSeekBarMoved(long position) {
        if (mCurrentItemId == null) {
            return;
        }

        refreshPlaybackPosition(position, -1);

        Intent intent = new Intent(MediaControlIntent.ACTION_SEEK);
        intent.addCategory(MediaControlIntent.CATEGORY_REMOTE_PLAYBACK);
        intent.putExtra(MediaControlIntent.EXTRA_ITEM_ID, mCurrentItemId);
        intent.putExtra(MediaControlIntent.EXTRA_ITEM_CONTENT_POSITION, position);
        intent.putExtra(MediaControlIntent.EXTRA_SESSION_ID, mSessionId);
        sendIntentToRoute(intent, mMediaResultHandler);
    }

    @Override
    protected void onDeviceVolumeBarMoved(int volume) {
        if (mCurrentRoute != null) {
            mCurrentRoute.requestSetVolume(volume);
        }
    }

    @Override
    protected void onDeviceMuteToggled(boolean on) {
        // Not supported.
    }

    @Override
    protected void onStreamVolumeBarMoved(int volume) {
        // Not supported.
    }

    @Override
    protected void onStreamMuteToggled(boolean on) {
        // Not supported.
    }

    // Session control.

    private void startSession() {
        Intent intent = new Intent(MediaControlIntent.ACTION_START_SESSION);
        intent.addCategory(MediaControlIntent.CATEGORY_REMOTE_PLAYBACK);
        intent.putExtra(MediaControlIntent.EXTRA_SESSION_STATUS_UPDATE_RECEIVER,
                mSessionStatusUpdateIntent);
        intent.putExtra(CastMediaControlIntent.EXTRA_CAST_APPLICATION_ID,
                getReceiverApplicationId());
        intent.putExtra(CastMediaControlIntent.EXTRA_CAST_RELAUNCH_APPLICATION,
                getRelaunchApp());
        intent.putExtra(CastMediaControlIntent.EXTRA_DEBUG_LOGGING_ENABLED, true);
        if (getStopAppWhenEndingSession()) {
            intent.putExtra(CastMediaControlIntent.EXTRA_CAST_STOP_APPLICATION_WHEN_SESSION_ENDS,
                    true);
        }
        sendIntentToRoute(intent, new ResultBundleHandler() {
            @Override
            public void handleResult(Bundle bundle) {
                mSessionId = bundle.getString(MediaControlIntent.EXTRA_SESSION_ID);
                Log.d(TAG, "Got a session ID of: " + mSessionId);
            }
        });
    }

    private void syncStatus() {
        Log.d(TAG, "Invoking SYNC_STATUS request");
        Intent intent = new Intent(CastMediaControlIntent.ACTION_SYNC_STATUS);
        intent.addCategory(CastMediaControlIntent.categoryForRemotePlayback());
        intent.putExtra(MediaControlIntent.EXTRA_SESSION_ID, mSessionId);
        intent.putExtra(MediaControlIntent.EXTRA_ITEM_STATUS_UPDATE_RECEIVER,
                mMediaStatusUpdateIntent);
        sendIntentToRoute(intent, mMediaResultHandler);
    }

    private void requestSessionStatus() {
        if (mSessionId == null) {
            return;
        }

        Intent intent = new Intent(MediaControlIntent.ACTION_GET_SESSION_STATUS);
        intent.addCategory(MediaControlIntent.CATEGORY_REMOTE_PLAYBACK);
        intent.putExtra(MediaControlIntent.EXTRA_SESSION_ID, mSessionId);
        sendIntentToRoute(intent, null);
    }

    private void endSession() {
        if (mSessionId == null) {
            return;
        }

        Intent intent = new Intent(MediaControlIntent.ACTION_END_SESSION);
        intent.addCategory(MediaControlIntent.CATEGORY_REMOTE_PLAYBACK);
        intent.putExtra(MediaControlIntent.EXTRA_SESSION_ID, mSessionId);
        sendIntentToRoute(intent, new ResultBundleHandler() {
            @Override
            public void handleResult(final Bundle bundle) {
                MediaSessionStatus status = MediaSessionStatus.fromBundle(
                        bundle.getBundle(MediaControlIntent.EXTRA_SESSION_STATUS));
                int sessionState = status.getSessionState();
                Log.d(TAG, "session state after ending session: " + sessionState);
                clearCurrentMediaItem();
            }
        });
        mSessionId = null;
    }

    /*
     * Sends a prebuilt media control intent to the selected route.
     */
    private void sendIntentToRoute(final Intent intent, final ResultBundleHandler resultHandler) {
        String sessionId = intent.getStringExtra(MediaControlIntent.EXTRA_SESSION_ID);
        Log.d(TAG, "sending intent to route: " + intent + ", session: " + sessionId);
        if ((mCurrentRoute == null) || !mCurrentRoute.supportsControlRequest(intent)) {
            Log.d(TAG, "route is null or doesn't support this request");
            return;
        }

        mCurrentRoute.sendControlRequest(intent, new MediaRouter.ControlRequestCallback() {
            @Override
            public void onResult(Bundle data) {
                Log.d(TAG, "got onResult for " + intent.getAction() + " with bundle " + data);
                if (data != null) {
                    if (resultHandler != null) {
                        resultHandler.handleResult(data);
                    }
                } else {
                    Log.w(TAG, "got onResult with a null bundle");
                }
            }

            @Override
            public void onError(String message, Bundle data) {
                showErrorDialog(message != null ? message
                        : getString(R.string.mrp_request_failed));
            }
        });
    }

    /*
     * Processes a received session status bundle and updates the UI accordingly.
     */
    private void processSessionStatusBundle(Bundle statusBundle) {
        Log.d(TAG, "processSessionStatusBundle()");

        String sessionId = statusBundle.getString(MediaControlIntent.EXTRA_SESSION_ID);
        MediaSessionStatus status = MediaSessionStatus.fromBundle(
                statusBundle.getBundle(MediaControlIntent.EXTRA_SESSION_STATUS));
        int sessionState = status.getSessionState();

        Log.d(TAG, "got a session status update for session " + sessionId + ", state = "
                + sessionState + ", mSessionId=" + mSessionId);

        if (mSessionId == null) {
            return;
        }

        if (!mSessionId.equals(sessionId)) {
            // Got status on a session other than the one we're tracking. Ignore it.
            Log.d(TAG, "Received status for unknown session: " + sessionId);
            return;
        }

        switch (sessionState) {
            case MediaSessionStatus.SESSION_STATE_ACTIVE:
                Log.d(TAG, "session " + sessionId + " is ACTIVE");
                mSessionActive = true;
                syncStatus();
                break;

            case MediaSessionStatus.SESSION_STATE_ENDED:
                Log.d(TAG, "session " + sessionId + " is ENDED");
                mSessionId = null;
                mSessionActive = false;
                clearCurrentMediaItem();
                break;

            case MediaSessionStatus.SESSION_STATE_INVALIDATED:
                Log.d(TAG, "session " + sessionId + " is INVALIDATED");
                mSessionId = null;
                mSessionActive = false;
                clearCurrentMediaItem();
                break;

            default:
                Log.d(TAG, "Received unexpected session state: " + sessionState);
                break;
        }

        updateButtonStates();
    }

    /*
     * Processes a received media status bundle and updates the UI accordingly.
     */
    private void processMediaStatusBundle(Bundle statusBundle) {
        Log.d(TAG, "processMediaStatusBundle()");
        String itemId = statusBundle.getString(MediaControlIntent.EXTRA_ITEM_ID);
        Log.d(TAG, "itemId = " + itemId);

        String title = null;
        String artist = null;
        Uri imageUrl = null;

        // Extract item metadata, if available.
        if (statusBundle.containsKey(MediaControlIntent.EXTRA_ITEM_METADATA)) {
            Bundle metadataBundle = (Bundle) statusBundle.getParcelable(
                    MediaControlIntent.EXTRA_ITEM_METADATA);

            title = metadataBundle.getString(MediaItemMetadata.KEY_TITLE);
            artist = metadataBundle.getString(MediaItemMetadata.KEY_ARTIST);
            if (metadataBundle.containsKey(MediaItemMetadata.KEY_ARTWORK_URI)) {
                imageUrl = Uri.parse(metadataBundle.getString(MediaItemMetadata.KEY_ARTWORK_URI));
            }
        } else {
            Log.d(TAG, "status bundle had no metadata!");
        }

        // Extract the item status, if available.
        if ((itemId != null) && statusBundle.containsKey(MediaControlIntent.EXTRA_ITEM_STATUS)) {
            Bundle itemStatusBundle = (Bundle) statusBundle.getParcelable(
                    MediaControlIntent.EXTRA_ITEM_STATUS);
            MediaItemStatus itemStatus = MediaItemStatus.fromBundle(itemStatusBundle);

            int playbackState = itemStatus.getPlaybackState();
            Log.d(TAG, "playbackState=" + playbackState);

            if ((playbackState == MediaItemStatus.PLAYBACK_STATE_CANCELED)
                    || (playbackState == MediaItemStatus.PLAYBACK_STATE_INVALIDATED)
                    || (playbackState == MediaItemStatus.PLAYBACK_STATE_ERROR)
                    || (playbackState == MediaItemStatus.PLAYBACK_STATE_FINISHED)) {
                clearCurrentMediaItem();
                mStreamAdvancing = false;
            } else if ((playbackState == MediaItemStatus.PLAYBACK_STATE_PAUSED)
                    || (playbackState == MediaItemStatus.PLAYBACK_STATE_PLAYING)
                    || (playbackState == MediaItemStatus.PLAYBACK_STATE_BUFFERING)) {

                int playerState = PLAYER_STATE_NONE;
                if (playbackState == MediaItemStatus.PLAYBACK_STATE_PAUSED) {
                    playerState = PLAYER_STATE_PAUSED;
                } else if (playbackState == MediaItemStatus.PLAYBACK_STATE_PLAYING) {
                    playerState = PLAYER_STATE_PLAYING;
                } else if (playbackState == MediaItemStatus.PLAYBACK_STATE_BUFFERING) {
                    playerState = PLAYER_STATE_BUFFERING;
                }

                setPlayerState(playerState);
                mCurrentItemId = itemId;
                setCurrentMediaMetadata(title, artist, imageUrl);
                updateButtonStates();

                mStreamDuration = itemStatus.getContentDuration();
                mLastKnownStreamPosition = itemStatus.getContentPosition();
                mStreamPositionTimestamp = itemStatus.getTimestamp();

                Log.d(TAG, "stream position now: " + mLastKnownStreamPosition);

                // Only refresh playback position if stream is moving.
                mStreamAdvancing = (playbackState == MediaItemStatus.PLAYBACK_STATE_PLAYING);
                if (mStreamAdvancing) {
                    refreshPlaybackPosition(mLastKnownStreamPosition, mStreamDuration);
                }
            } else {
                Log.d(TAG, "Unexpected playback state: " + playbackState);
            }

            Bundle extras = itemStatus.getExtras();
            if (extras != null) {
                if (extras.containsKey(MediaItemStatus.EXTRA_HTTP_STATUS_CODE)) {
                    int httpStatus = extras.getInt(MediaItemStatus.EXTRA_HTTP_STATUS_CODE);
                    Log.d(TAG, "HTTP status: " + httpStatus);
                }
                if (extras.containsKey(MediaItemStatus.EXTRA_HTTP_RESPONSE_HEADERS)) {
                    Bundle headers = extras.getBundle(MediaItemStatus.EXTRA_HTTP_RESPONSE_HEADERS);
                    Log.d(TAG, "HTTP headers: " + headers);
                }
            }
        }
    }

    private void clearStreamState() {
        mStreamAdvancing = false;
        mStreamPositionTimestamp = 0;
        mLastKnownStreamPosition = 0;
        setPlayerState(PLAYER_STATE_NONE);
        refreshPlaybackPosition(0, 0);
    }

    private void setSelectedRoute(RouteInfo route) {
        clearStreamState();
        mCurrentRoute = route;
        setCurrentDeviceName(route != null ? route.getName() : null);
    }

    private void updateButtonStates() {
        boolean hasRoute = (mCurrentRoute != null);
        boolean hasSession = (mSessionId != null);
        boolean hasMedia = (mCurrentItemId != null);

        mStartSessionButton.setEnabled(hasRoute && !hasSession);
        mEndSessionButton.setEnabled(hasRoute && hasSession && mSessionActive);
        setDeviceVolumeControlsEnabled(hasRoute);
        mPlayMediaButton.setEnabled(hasRoute && hasSession);
        mStopButton.setEnabled(hasMedia);
        setSeekBarEnabled(hasMedia);
        updateVolume();
    }

    @Override
    protected void onRefreshEvent() {
        if ((mStreamPositionTimestamp != 0) && !isUserSeeking() && mStreamAdvancing
                && (mCurrentItemId != null) && (mLastKnownStreamPosition < mStreamDuration)) {

            long extrapolatedStreamPosition = mLastKnownStreamPosition
                    + (SystemClock.uptimeMillis() - mStreamPositionTimestamp);
            if (extrapolatedStreamPosition > mStreamDuration) {
                extrapolatedStreamPosition = mStreamDuration;
            }
            refreshPlaybackPosition(extrapolatedStreamPosition, -1);
        }
        updateButtonStates();
    }

    private void updateVolume() {
        if (mCurrentRoute != null) {
            refreshDeviceVolume(mCurrentRoute.getVolume() / MAX_VOLUME_LEVEL, false);
        }
    }

    @Override
    protected void onRouteSelected(RouteInfo route) {
        setSelectedRoute(route);
        updateButtonStates();
        String routeId = route.getId();
        if (routeId.equals(mLastRouteId) && (mSessionId != null)) {
            // Try to rejoin the session by requesting status.
            Log.d(TAG, "Trying to rejoin previous session");
            requestSessionStatus();
            updateButtonStates();
        }
        mLastRouteId = routeId;
    }

    @Override
    protected void onRouteUnselected(RouteInfo route) {
        setSelectedRoute(null);
        clearCurrentMediaItem();
        mSessionActive = false;
        mSessionId = null;
        updateButtonStates();
    }

    @Override
    protected String getControlCategory() {
        return CastMediaControlIntent.categoryForRemotePlayback(getReceiverApplicationId());
    }

    private void clearCurrentMediaItem() {
        setCurrentMediaMetadata(null, null, null);
        refreshPlaybackPosition(0, 0);
        setPlayerState(PLAYER_STATE_NONE);
        mCurrentItemId = null;
        updateButtonStates();
    }

    private interface ResultBundleHandler {
        public void handleResult(Bundle bundle);
    }

}
