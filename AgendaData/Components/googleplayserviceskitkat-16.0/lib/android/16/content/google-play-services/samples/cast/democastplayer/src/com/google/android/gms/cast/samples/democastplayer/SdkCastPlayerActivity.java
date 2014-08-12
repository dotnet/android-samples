// Copyright 2013 Google Inc. All Rights Reserved.

package com.google.android.gms.cast.samples.democastplayer;

import com.google.android.gms.cast.ApplicationMetadata;
import com.google.android.gms.cast.Cast;
import com.google.android.gms.cast.Cast.ApplicationConnectionResult;
import com.google.android.gms.cast.CastDevice;
import com.google.android.gms.cast.CastMediaControlIntent;
import com.google.android.gms.cast.CastStatusCodes;
import com.google.android.gms.cast.MediaInfo;
import com.google.android.gms.cast.MediaMetadata;
import com.google.android.gms.cast.MediaStatus;
import com.google.android.gms.cast.RemoteMediaPlayer;
import com.google.android.gms.cast.RemoteMediaPlayer.MediaChannelResult;
import com.google.android.gms.common.ConnectionResult;
import com.google.android.gms.common.api.GoogleApiClient;
import com.google.android.gms.common.api.ResultCallback;
import com.google.android.gms.common.api.Status;
import com.google.android.gms.common.images.WebImage;

import android.net.Uri;
import android.os.Bundle;
import android.support.v7.media.MediaRouter.RouteInfo;
import android.util.Log;

import java.io.IOException;
import java.util.List;

/**
 * Activity to cast media to a Cast device using the MediaRouter APIs for discovery, and the
 * Cast SDK for media selection and playback.
 */
public class SdkCastPlayerActivity extends BaseCastPlayerActivity {
    private static final String TAG = "SdkCastPlayerActivity";

    private CastDevice mSelectedDevice;
    private GoogleApiClient mApiClient;
    private CastListener mCastListener;
    private ConnectionCallbacks mConnectionCallbacks;
    private ConnectionFailedListener mConnectionFailedListener;
    private RemoteMediaPlayer mMediaPlayer;
    private boolean mShouldPlayMedia;
    private MediaInfo mSelectedMedia;
    private ApplicationMetadata mAppMetadata;
    private boolean mSeeking;
    private boolean mWaitingForReconnect;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);

        setCurrentDeviceName(null);
        setAutoplayCheckboxVisible(true);
        setStreamVolumeControlsVisible(true);
        setSeekBehaviorControlsVisible(true);
        setAppControlsVisible(true);
        setSessionControlsVisible(false);
        mConnectionCallbacks = new ConnectionCallbacks();
        mConnectionFailedListener = new ConnectionFailedListener();

        mCastListener = new CastListener();
    }

    @Override
    public void onResume() {
        super.onResume();
        updateButtonStates();
        // Restart the timer, if there's a media connection.
        if ((mMediaPlayer != null) && !mWaitingForReconnect) {
            startRefreshTimer();
        }
    }

    @Override
    protected void onVolumeChange(double delta) {
        if (!mApiClient.isConnected()) {
            return;
        }

        try {
            double volume = Cast.CastApi.getVolume(mApiClient) + delta;
            refreshDeviceVolume(volume, Cast.CastApi.isMute(mApiClient));
            Cast.CastApi.setVolume(mApiClient, volume);
        } catch (IOException e) {
            Log.w(TAG, "Unable to change volume", e);
        } catch (IllegalStateException e) {
            Log.w(TAG, "Unable to change volume", e);
        }
    }

    /*
     * Connects to the device (if necessary), and then casts the currently selected video.
     */
    @Override
    protected void onPlayMedia(final MediaInfo media) {
        mSelectedMedia = media;

        if (mAppMetadata == null) {
            return;
        }

        playMedia(mSelectedMedia);
    }

    @Override
    protected void onLaunchAppClicked() {
        if (!mApiClient.isConnected()) {
            return;
        }

        Cast.CastApi.launchApplication(mApiClient, getReceiverApplicationId(), getRelaunchApp())
                .setResultCallback(new ApplicationConnectionResultCallback("LaunchApp"));
    }

    @Override
    protected void onJoinAppClicked() {
        if (!mApiClient.isConnected()) {
            return;
        }

        Cast.CastApi.joinApplication(mApiClient, getReceiverApplicationId()).setResultCallback(
                new ApplicationConnectionResultCallback("JoinApplication"));
    }

    @Override
    protected void onLeaveAppClicked() {
        if (!mApiClient.isConnected()) {
            return;
        }

        Cast.CastApi.leaveApplication(mApiClient).setResultCallback(new ResultCallback<Status>() {
            @Override
            public void onResult(Status result) {
                if (result.isSuccess()) {
                    mAppMetadata = null;
                    detachMediaPlayer();
                    updateButtonStates();
                } else {
                    showErrorDialog(getString(R.string.error_app_leave_failed));
                }
            }
        });
    }

    @Override
    protected void onStopAppClicked() {
        if (!mApiClient.isConnected()) {
            return;
        }

        Cast.CastApi.stopApplication(mApiClient).setResultCallback(new ResultCallback<Status>() {
            @Override
            public void onResult(Status result) {
                if (result.isSuccess()) {
                    mAppMetadata = null;
                    detachMediaPlayer();
                    updateButtonStates();
                } else {
                    showErrorDialog(getString(R.string.error_app_stop_failed));
                }
            }
        });
    }

    @Override
    protected void onPlayClicked() {
        if (mMediaPlayer == null) {
            return;
        }
        mMediaPlayer.play(mApiClient).setResultCallback(
                new MediaResultCallback(getString(R.string.mediaop_play)));
    }

    @Override
    protected void onPauseClicked() {
        if (mMediaPlayer == null) {
            return;
        }
        mMediaPlayer.pause(mApiClient).setResultCallback(
                new MediaResultCallback(getString(R.string.mediaop_pause)));
    }

    @Override
    protected void onStopClicked() {
        if (mMediaPlayer == null) {
            return;
        }
        mMediaPlayer.stop(mApiClient).setResultCallback(
                new MediaResultCallback(getString(R.string.mediaop_stop)));

    }

    @Override
    protected void onSeekBarMoved(long position) {
        if (mMediaPlayer == null) {
            return;
        }

        refreshPlaybackPosition(position, -1);

        int behavior = getSeekBehavior();

        int resumeState;
        switch (behavior) {
            case AFTER_SEEK_PLAY:
                resumeState = RemoteMediaPlayer.RESUME_STATE_PLAY;
                break;
            case AFTER_SEEK_PAUSE:
                resumeState = RemoteMediaPlayer.RESUME_STATE_PAUSE;
                break;
            case AFTER_SEEK_DO_NOTHING:
            default:
                resumeState = RemoteMediaPlayer.RESUME_STATE_UNCHANGED;
        }
        mSeeking = true;
        mMediaPlayer.seek(mApiClient, position, resumeState).setResultCallback(
                new MediaResultCallback(getString(R.string.mediaop_seek)) {
                    @Override
                    protected void onFinished() {
                        mSeeking = false;
                    }
                });
    }

    @Override
    protected void onDeviceVolumeBarMoved(int volume) {
        if (!mApiClient.isConnected()) {
            return;
        }
        try {
            Cast.CastApi.setVolume(mApiClient, volume / MAX_VOLUME_LEVEL);
        } catch (IOException e) {
            Log.w(TAG, "Unable to change volume");
        } catch (IllegalStateException e) {
            showErrorDialog(e.getMessage());
        }
    }

    @Override
    protected void onDeviceMuteToggled(boolean on) {
        if (!mApiClient.isConnected()) {
            return;
        }
        try {
            Cast.CastApi.setMute(mApiClient, on);
        } catch (IOException e) {
            Log.w(TAG, "Unable to toggle mute");
        } catch (IllegalStateException e) {
            showErrorDialog(e.getMessage());
        }
    }

    @Override
    protected void onStreamVolumeBarMoved(int volume) {
        if (mMediaPlayer == null) {
            return;
        }
        try {
            mMediaPlayer.setStreamVolume(mApiClient, volume / MAX_VOLUME_LEVEL).setResultCallback(
                    new MediaResultCallback(getString(R.string.mediaop_set_stream_volume)));
        } catch (IllegalStateException e) {
            showErrorDialog(e.getMessage());
        }
    }

    @Override
    protected void onStreamMuteToggled(boolean on) {
        if (mMediaPlayer == null) {
            return;
        }
        try {
            mMediaPlayer.setStreamMute(mApiClient, on).setResultCallback(
                    new MediaResultCallback(getString(R.string.mediaop_toggle_stream_mute)));
        } catch (IllegalStateException e) {
            showErrorDialog(e.getMessage());
        }
    }

    private void clearMediaState() {
        setCurrentMediaMetadata(null, null, null);
        refreshPlaybackPosition(0, 0);
    }

    private void attachMediaPlayer() {
        if (mMediaPlayer != null) {
            return;
        }

        mMediaPlayer = new RemoteMediaPlayer();
        mMediaPlayer.setOnStatusUpdatedListener(new RemoteMediaPlayer.OnStatusUpdatedListener() {

            @Override
            public void onStatusUpdated() {
                Log.d(TAG, "MediaControlChannel.onStatusUpdated");
                // If item has ended, clear metadata.
                MediaStatus mediaStatus = mMediaPlayer.getMediaStatus();
                if ((mediaStatus != null)
                        && (mediaStatus.getPlayerState() == MediaStatus.PLAYER_STATE_IDLE)) {
                    clearMediaState();
                }

                updatePlaybackPosition();
                updateStreamVolume();
                updateButtonStates();
            }
        });

        mMediaPlayer.setOnMetadataUpdatedListener(
                new RemoteMediaPlayer.OnMetadataUpdatedListener() {
            @Override
            public void onMetadataUpdated() {
                Log.d(TAG, "MediaControlChannel.onMetadataUpdated");
                String title = null;
                String artist = null;
                Uri imageUrl = null;

                MediaInfo mediaInfo = mMediaPlayer.getMediaInfo();
                if (mediaInfo != null) {
                    MediaMetadata metadata = mediaInfo.getMetadata();
                    if (metadata != null) {
                        title = metadata.getString(MediaMetadata.KEY_TITLE);

                        artist = metadata.getString(MediaMetadata.KEY_ARTIST);
                        if (artist == null) {
                            artist = metadata.getString(MediaMetadata.KEY_STUDIO);
                        }

                        List<WebImage> images = metadata.getImages();
                        if ((images != null) && !images.isEmpty()) {
                            WebImage image = images.get(0);
                            imageUrl = image.getUrl();
                        }
                    }
                    setCurrentMediaMetadata(title, artist, imageUrl);
                }
            }
        });

        try {
            Cast.CastApi.setMessageReceivedCallbacks(mApiClient, mMediaPlayer.getNamespace(),
                    mMediaPlayer);
        } catch (IOException e) {
            Log.w(TAG, "Exception while launching application", e);
        }
    }

    private void requestMediaStatus() {
        if (mMediaPlayer == null) {
            return;
        }

        Log.d(TAG, "requesting current media status");
        mMediaPlayer.requestStatus(mApiClient).setResultCallback(
                new ResultCallback<RemoteMediaPlayer.MediaChannelResult>() {
                    @Override
                    public void onResult(MediaChannelResult result) {
                        Status status = result.getStatus();
                        if (!status.isSuccess()) {
                            Log.w(TAG, "Unable to request status: " + status.getStatusCode());
                        }
                    }
                });
    }

    private void detachMediaPlayer() {
        if ((mMediaPlayer != null) && (mApiClient != null)) {
            try {
                Cast.CastApi.removeMessageReceivedCallbacks(mApiClient,
                        mMediaPlayer.getNamespace());
            } catch (IOException e) {
                Log.w(TAG, "Exception while detaching media player", e);
            }
        }
        mMediaPlayer = null;
    }

    /*
     * Begins playback of the currently selected video.
     */
    private void playMedia(MediaInfo media) {
        Log.d(TAG, "playMedia: " + media);
        if (media == null) {
            return;
        }
        if (mMediaPlayer == null) {
            Log.e(TAG, "Trying to play a video with no active media session");
            return;
        }

        mMediaPlayer.load(mApiClient, media, isAutoplayChecked()).setResultCallback(
                new MediaResultCallback(getString(R.string.mediaop_load)));
    }

    private void updateButtonStates() {
        boolean hasDeviceConnection = (mApiClient != null) && mApiClient.isConnected()
                && !mWaitingForReconnect;
        boolean hasAppConnection = (mAppMetadata != null) && !mWaitingForReconnect;
        boolean hasMediaConnection = (mMediaPlayer != null) && !mWaitingForReconnect;
        boolean hasMedia = false;

        if (hasMediaConnection) {
            MediaStatus mediaStatus = mMediaPlayer.getMediaStatus();
            if (mediaStatus != null) {
                int mediaPlayerState = mediaStatus.getPlayerState();
                int playerState = PLAYER_STATE_NONE;
                if (mediaPlayerState == MediaStatus.PLAYER_STATE_PAUSED) {
                    playerState = PLAYER_STATE_PAUSED;
                } else if (mediaPlayerState == MediaStatus.PLAYER_STATE_PLAYING) {
                    playerState = PLAYER_STATE_PLAYING;
                } else if (mediaPlayerState == MediaStatus.PLAYER_STATE_BUFFERING) {
                    playerState = PLAYER_STATE_BUFFERING;
                }
                setPlayerState(playerState);

                hasMedia = mediaStatus.getPlayerState() != MediaStatus.PLAYER_STATE_IDLE;
            }
        } else {
            setPlayerState(PLAYER_STATE_NONE);
        }


        mLaunchAppButton.setEnabled(hasDeviceConnection && !hasAppConnection);
        mJoinAppButton.setEnabled(hasDeviceConnection && !hasAppConnection);
        mLeaveAppButton.setEnabled(hasDeviceConnection && hasAppConnection);
        mStopAppButton.setEnabled(hasDeviceConnection && hasAppConnection);
        mAutoplayCheckbox.setEnabled(hasDeviceConnection && hasAppConnection);

        mPlayMediaButton.setEnabled(hasMediaConnection);
        mStopButton.setEnabled(hasMediaConnection && hasMedia);
        setSeekBarEnabled(hasMediaConnection && hasMedia);
        setDeviceVolumeControlsEnabled(hasDeviceConnection);
        setStreamVolumeControlsEnabled(hasMediaConnection && hasMedia);
    }

    private void updatePlaybackPosition() {
        if (mMediaPlayer == null) {
            return;
        }
        refreshPlaybackPosition(mMediaPlayer.getApproximateStreamPosition(),
                mMediaPlayer.getStreamDuration());
    }

    private void updateStreamVolume() {
        if (mMediaPlayer == null) {
            return;
        }
        MediaStatus mediaStatus = mMediaPlayer.getMediaStatus();
        if (mediaStatus != null) {
            double streamVolume = mediaStatus.getStreamVolume();
            boolean muteState = mediaStatus.isMute();
            refreshStreamVolume(streamVolume, muteState);
        }
    }

    private void setSelectedDevice(CastDevice device) {
        mSelectedDevice = device;
        setCurrentDeviceName(mSelectedDevice != null ? mSelectedDevice.getFriendlyName() : null);

        if (mSelectedDevice == null) {
            detachMediaPlayer();
            if ((mApiClient != null) && mApiClient.isConnected()) {
                mApiClient.disconnect();
            }
        } else {
            Log.d(TAG, "acquiring controller for " + mSelectedDevice);
            try {
                Cast.CastOptions.Builder apiOptionsBuilder = Cast.CastOptions.builder(
                        mSelectedDevice, mCastListener);
                apiOptionsBuilder.setVerboseLoggingEnabled(true);

                mApiClient = new GoogleApiClient.Builder(this)
                        .addApi(Cast.API, apiOptionsBuilder.build())
                        .addConnectionCallbacks(mConnectionCallbacks)
                        .addOnConnectionFailedListener(mConnectionFailedListener)
                        .build();
                mApiClient.connect();
            } catch (IllegalStateException e) {
                Log.w(TAG, "error while creating a device controller", e);
                showErrorDialog(getString(R.string.error_no_controller));
            }
        }
    }

    @Override
    protected String getControlCategory() {
        return CastMediaControlIntent.categoryForCast(getReceiverApplicationId());
    }

    @Override
    protected void onRouteSelected(RouteInfo route) {
        Log.d(TAG, "onRouteSelected: " + route);

        CastDevice device = CastDevice.getFromBundle(route.getExtras());
        setSelectedDevice(device);
        updateButtonStates();
    }

    @Override
    protected void onRouteUnselected(RouteInfo route) {
        Log.d(TAG, "onRouteUnselected: " + route);
        setSelectedDevice(null);
        mAppMetadata = null;
        clearMediaState();
        updateButtonStates();
    }

    @Override
    protected void onRefreshEvent() {
        if (!mSeeking) {
            updatePlaybackPosition();
        }
        updateStreamVolume();
        updateButtonStates();
    }

    private class ConnectionCallbacks implements GoogleApiClient.ConnectionCallbacks {
        @Override
        public void onConnectionSuspended(int cause) {
            Log.d(TAG, "ConnectionCallbacks.onConnectionSuspended");
            mHandler.post(new Runnable() {
                @Override
                public void run() {
                    // TODO: need to disable all controls, and possibly display a
                    // "reconnecting..." dialog or overlay
                    mWaitingForReconnect = true;
                    cancelRefreshTimer();
                    detachMediaPlayer();
                    updateButtonStates();
                }
            });
        }

        @Override
        public void onConnected(final Bundle connectionHint) {
            Log.d(TAG, "ConnectionCallbacks.onConnected");
            mHandler.post(new Runnable() {
                @Override
                public void run() {
                    if (!mApiClient.isConnected()) {
                        // We got disconnected while this runnable was pending execution.
                        return;
                    }
                    try {
                        Cast.CastApi.requestStatus(mApiClient);
                    } catch (IOException e) {
                        Log.d(TAG, "error requesting status", e);
                    }
                    setDeviceVolumeControlsEnabled(true);
                    mLaunchAppButton.setEnabled(true);
                    mJoinAppButton.setEnabled(true);

                    if (mWaitingForReconnect) {
                        mWaitingForReconnect = false;
                        if ((connectionHint != null)
                                && connectionHint.getBoolean(Cast.EXTRA_APP_NO_LONGER_RUNNING)) {
                            Log.d(TAG, "App  is no longer running");
                            detachMediaPlayer();
                            mAppMetadata = null;
                            clearMediaState();
                            updateButtonStates();
                        } else {
                            attachMediaPlayer();
                            requestMediaStatus();
                            startRefreshTimer();
                        }
                    }
                }
            });
        }
    }

    private class ConnectionFailedListener implements GoogleApiClient.OnConnectionFailedListener {
        @Override
        public void onConnectionFailed(ConnectionResult result) {
            Log.d(TAG, "onConnectionFailed");
            mHandler.post(new Runnable() {
                @Override
                public void run() {
                    updateButtonStates();
                    clearMediaState();
                    cancelRefreshTimer();
                    showErrorDialog(getString(R.string.error_no_device_connection));
                }
            });
        }
    }

    private class CastListener extends Cast.Listener {
        @Override
        public void onVolumeChanged() {
            refreshDeviceVolume(Cast.CastApi.getVolume(mApiClient),
                    Cast.CastApi.isMute(mApiClient));
        }

        @Override
        public void onApplicationStatusChanged() {
            String status = Cast.CastApi.getApplicationStatus(mApiClient);
            Log.d(TAG, "onApplicationStatusChanged; status=" + status);
            setApplicationStatus(status);
        }

        @Override
        public void onApplicationDisconnected(int statusCode) {
            Log.d(TAG, "onApplicationDisconnected: statusCode=" + statusCode);
            mAppMetadata = null;
            detachMediaPlayer();
            clearMediaState();
            updateButtonStates();
            if (statusCode != CastStatusCodes.SUCCESS) {
                // This is an unexpected disconnect.
                setApplicationStatus(getString(R.string.status_app_disconnected));
            }
        }
    }

    private final class ApplicationConnectionResultCallback implements
            ResultCallback<Cast.ApplicationConnectionResult> {
        private final String mClassTag;

        public ApplicationConnectionResultCallback(String suffix) {
            mClassTag = TAG + "_" + suffix;
        }

        @Override
        public void onResult(ApplicationConnectionResult result) {
            Status status = result.getStatus();
            Log.d(mClassTag, "ApplicationConnectionResultCallback.onResult: statusCode"
                    + status.getStatusCode());
            if (status.isSuccess()) {
                ApplicationMetadata applicationMetadata = result.getApplicationMetadata();
                String sessionId = result.getSessionId();
                String applicationStatus = result.getApplicationStatus();
                boolean wasLaunched = result.getWasLaunched();
                Log.d(mClassTag, "application name: " + applicationMetadata.getName()
                        + ", status: " + applicationStatus + ", sessionId: " + sessionId
                        + ", wasLaunched: " + wasLaunched);
                setApplicationStatus(applicationStatus);
                attachMediaPlayer();
                mAppMetadata = applicationMetadata;
                startRefreshTimer();
                updateButtonStates();
                Log.d(mClassTag, "mShouldPlayMedia is " + mShouldPlayMedia);
                if (mShouldPlayMedia) {
                    mShouldPlayMedia = false;
                    Log.d(mClassTag, "now loading media");
                    playMedia(mSelectedMedia);
                } else {
                    // Synchronize with the receiver's state.
                    requestMediaStatus();
                }
            } else {
                showErrorDialog(getString(R.string.error_app_launch_failed));
            }
        }
    }

    private class MediaResultCallback implements ResultCallback<MediaChannelResult> {
        private final String mOperationName;

        public MediaResultCallback(String operationName) {
            mOperationName = operationName;
        }

        @Override
        public void onResult(MediaChannelResult result) {
            Status status = result.getStatus();
            if (!status.isSuccess()) {
                Log.w(TAG, mOperationName + " failed: " + status.getStatusCode());
                showErrorDialog(getString(R.string.error_operation_failed, mOperationName));
            }
            onFinished();
        }

        protected void onFinished() {
        }
    }

}
