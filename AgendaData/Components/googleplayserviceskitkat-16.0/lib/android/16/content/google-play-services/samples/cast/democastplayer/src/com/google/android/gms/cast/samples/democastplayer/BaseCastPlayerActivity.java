// Copyright 2013 Google Inc. All Rights Reserved.

package com.google.android.gms.cast.samples.democastplayer;

import com.google.android.gms.cast.CastMediaControlIntent;
import com.google.android.gms.cast.MediaInfo;

import android.annotation.TargetApi;
import android.app.AlertDialog;
import android.app.PendingIntent;
import android.content.ComponentName;
import android.content.Context;
import android.content.DialogInterface;
import android.content.Intent;
import android.content.SharedPreferences;
import android.content.SharedPreferences.OnSharedPreferenceChangeListener;
import android.graphics.Bitmap;
import android.media.AudioManager;
import android.media.RemoteControlClient;
import android.net.Uri;
import android.os.Build;
import android.os.Bundle;
import android.os.Handler;
import android.preference.PreferenceManager;
import android.support.v4.app.DialogFragment;
import android.support.v4.app.Fragment;
import android.support.v4.view.MenuItemCompat;
import android.support.v7.app.ActionBar;
import android.support.v7.app.ActionBarActivity;
import android.support.v7.app.MediaRouteActionProvider;
import android.support.v7.media.MediaRouteSelector;
import android.support.v7.media.MediaRouter;
import android.support.v7.media.MediaRouter.RouteInfo;
import android.text.TextUtils;
import android.util.Log;
import android.view.KeyEvent;
import android.view.Menu;
import android.view.MenuItem;
import android.view.View;
import android.view.View.OnClickListener;
import android.widget.Button;
import android.widget.CheckBox;
import android.widget.CompoundButton;
import android.widget.ImageView;
import android.widget.SeekBar;
import android.widget.Spinner;
import android.widget.TextView;

import java.util.concurrent.TimeUnit;

/**
 * Base class for DemoCastPlayer activities.
 */
abstract class BaseCastPlayerActivity extends ActionBarActivity
        implements OnSharedPreferenceChangeListener {
    private static final String TAG = "BaseCastPlayerActivity";

    private TextView mMediaTitle;
    private TextView mMediaArtist;
    private ImageView mMediaArtImageView;
    protected Button mPlayMediaButton;
    protected Button mLaunchAppButton;
    protected Button mJoinAppButton;
    protected Button mLeaveAppButton;
    protected Button mStopAppButton;
    protected Button mStartSessionButton;
    protected Button mEndSessionButton;
    private View mAppControls;
    private View mSessionControls;
    private TextView mAppStatusTextView;
    protected Button mPlayPauseButton;
    protected Button mStopButton;
    protected CheckBox mAutoplayCheckbox;
    private TextView mCurrentDeviceTextView;
    private TextView mStreamPositionTextView;
    private TextView mStreamDurationTextView;
    private View mStreamVolumeControls;
    private View mSeekBehaviorControls;
    private SeekBar mSeekBar;
    private Spinner mSeekBehaviorSpinner;
    private SeekBar mDeviceVolumeBar;
    protected CheckBox mDeviceMuteCheckBox;
    private SeekBar mStreamVolumeBar;
    protected CheckBox mStreamMuteCheckBox;
    private String mReceiverApplicationId;
    private MediaRouter mMediaRouter;
    private MediaRouteSelector mMediaRouteSelector;
    private MediaRouter.Callback mMediaRouterCallback;
    protected Handler mHandler;
    private Uri mCurrentImageUrl;
    private FetchBitmapTask mFetchBitmapTask;
    private boolean mIsUserSeeking;
    private boolean mIsUserAdjustingVolume;
    private MediaSelectionDialog mMediaSelectionDialog;
    private Runnable mRefreshRunnable;
    private int mPlayerState;
    private boolean mRelaunchApp;
    private boolean mStopAppWhenEndingSession;
    private RemoteControlClient mRemoteControlClient;
    private boolean mRouteSelected;

    protected static final double VOLUME_INCREMENT = 0.05;
    protected static final double MAX_VOLUME_LEVEL = 20;

    protected static final int AFTER_SEEK_DO_NOTHING = 0;
    protected static final int AFTER_SEEK_PLAY = 1;
    protected static final int AFTER_SEEK_PAUSE = 2;

    protected static final int PLAYER_STATE_NONE = 0;
    protected static final int PLAYER_STATE_PLAYING = 1;
    protected static final int PLAYER_STATE_PAUSED = 2;
    protected static final int PLAYER_STATE_BUFFERING = 3;

    private static final String MEDIA_SELECTION_DIALOG_TAG = "media_selection";
    private static final int REFRESH_INTERVAL_MS = (int) TimeUnit.SECONDS.toMillis(1);

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        Log.d(TAG, "onCreate");
        setContentView(R.layout.cast_player_activity);

        ActionBar actionBar = getSupportActionBar();
        actionBar.setDisplayHomeAsUpEnabled(true);

        mCurrentDeviceTextView = (TextView) findViewById(R.id.connected_device);
        mMediaTitle = (TextView) findViewById(R.id.media_title);
        mMediaArtist = (TextView) findViewById(R.id.media_artist);
        mMediaArtImageView = (ImageView) findViewById(R.id.media_art);

        mAppControls = findViewById(R.id.app_controls);
        mLaunchAppButton = (Button) findViewById(R.id.launch_app);
        mJoinAppButton = (Button) findViewById(R.id.join_app);
        mLeaveAppButton = (Button) findViewById(R.id.leave_app);
        mStopAppButton = (Button) findViewById(R.id.stop_app);

        mSessionControls = findViewById(R.id.session_controls);
        mStartSessionButton = (Button) findViewById(R.id.start_session);
        mEndSessionButton = (Button) findViewById(R.id.end_session);

        mAppStatusTextView = (TextView) findViewById(R.id.app_status);

        mPlayMediaButton = (Button) findViewById(R.id.play_media_button);
        mPlayPauseButton = (Button) findViewById(R.id.pause_play);
        mStopButton = (Button) findViewById(R.id.stop);
        mAutoplayCheckbox = (CheckBox) findViewById(R.id.autoplay_checkbox);

        mStreamPositionTextView = (TextView) findViewById(R.id.stream_position);
        mStreamDurationTextView = (TextView) findViewById(R.id.stream_duration);

        mStreamVolumeControls = findViewById(R.id.stream_volume_controls);
        mSeekBehaviorControls = findViewById(R.id.seek_behavior_controls);

        mSeekBar = (SeekBar) findViewById(R.id.seek_bar);
        mSeekBehaviorSpinner = (Spinner) findViewById(R.id.seek_behavior_spinner);

        mDeviceVolumeBar = (SeekBar) findViewById(R.id.device_volume_bar);
        mDeviceMuteCheckBox = (CheckBox) findViewById(R.id.device_mute_checkbox);
        mStreamVolumeBar = (SeekBar) findViewById(R.id.stream_volume_bar);
        mStreamMuteCheckBox = (CheckBox) findViewById(R.id.stream_mute_checkbox);

        mHandler = new Handler();

        mMediaRouter = MediaRouter.getInstance(getApplicationContext());
        mMediaRouteSelector = buildMediaRouteSelector();
        mMediaRouterCallback = new MyMediaRouterCallback();

        SharedPreferences sharedPreferences = PreferenceManager.getDefaultSharedPreferences(this);
        sharedPreferences.registerOnSharedPreferenceChangeListener(this);
        mRelaunchApp = sharedPreferences.getBoolean(AppConstants.PREF_KEY_RELAUNCH_APP, false);
        mStopAppWhenEndingSession = sharedPreferences.getBoolean(
                AppConstants.PREF_KEY_STOP_APP, false);
        mReceiverApplicationId = sharedPreferences.getString(
                AppConstants.PREF_KEY_RECEIVER_APPLICATION_ID, null);

        setPlayerState(PLAYER_STATE_NONE);

        setUpControls();

        // A runnable that will be invoked periodically to update the display
        // (namely, the stream position controls).
        mRefreshRunnable = new Runnable() {
            @Override
            public void run() {
                onRefreshEvent();
                startRefreshTimer();
            }
        };

        // TODO: Add RemoteControlClient support.
        /*
        if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.ICE_CREAM_SANDWICH) {
            setUpRemoteControlClient();
        }
        */
    }

    @Override
    protected void onStart() {
        super.onStart();

        mMediaRouter.addCallback(mMediaRouteSelector, mMediaRouterCallback,
            MediaRouter.CALLBACK_FLAG_PERFORM_ACTIVE_SCAN);
    }

    @Override
    protected void onPause() {
        Fragment dialog = getSupportFragmentManager().findFragmentByTag(MEDIA_SELECTION_DIALOG_TAG);
        if (dialog != null) {
            ((DialogFragment) dialog).dismiss();
        }
        mHandler.removeCallbacksAndMessages(null);

        super.onPause();
    }

    @Override
    protected void onStop() {
        mMediaRouter.removeCallback(mMediaRouterCallback);
        if (mFetchBitmapTask != null) {
            mFetchBitmapTask.cancel(true);
        }
        super.onStop();
    }

    @Override
    public void onDestroy() {
        PreferenceManager.getDefaultSharedPreferences(this)
                .unregisterOnSharedPreferenceChangeListener(this);

        super.onDestroy();
    }

    @Override
    public void onBackPressed() {
        // If a route is selected, deselect it.
        if (mRouteSelected) {
            mMediaRouter.selectRoute(mMediaRouter.getDefaultRoute());
            mRouteSelected = false;
        }
        super.onBackPressed();
    }

    @Override
    public boolean onCreateOptionsMenu(Menu menu) {
        super.onCreateOptionsMenu(menu);
        getMenuInflater().inflate(R.menu.main, menu);
        MenuItem mediaRouteMenuItem = menu.findItem(R.id.media_route_menu_item);
        MediaRouteActionProvider mediaRouteActionProvider =
                (MediaRouteActionProvider) MenuItemCompat.getActionProvider(mediaRouteMenuItem);
        mediaRouteActionProvider.setRouteSelector(mMediaRouteSelector);
        return true;
    }

    @Override
    public boolean onOptionsItemSelected(MenuItem item) {
        if (item.getItemId() == android.R.id.home) {
            Intent intent = new Intent(this, ModeSelectActivity.class);
            intent.addFlags(Intent.FLAG_ACTIVITY_CLEAR_TOP);
            startActivity(intent);
            return true;
        }
        return super.onOptionsItemSelected(item);
    }

    @Override
    public boolean onKeyDown(int keyCode, KeyEvent event) {
        if (keyCode == KeyEvent.KEYCODE_VOLUME_UP) {
            onVolumeChange(VOLUME_INCREMENT);
        } else if (keyCode == KeyEvent.KEYCODE_VOLUME_DOWN) {
            onVolumeChange(-VOLUME_INCREMENT);
        } else {
            return super.onKeyDown(keyCode, event);
        }
        return true;
    }

    private void selectMedia() {
        if (mMediaSelectionDialog == null) {
            mMediaSelectionDialog = new MediaSelectionDialog(this) {
                @Override
                protected void onItemSelected(final MediaInfo item) {
                    onPlayMedia(item);
                }
            };
        }
        mMediaSelectionDialog.show(getSupportFragmentManager(), MEDIA_SELECTION_DIALOG_TAG);
    }

    protected abstract void onVolumeChange(double delta);
    protected abstract void onPlayMedia(MediaInfo media);

    protected void onLaunchAppClicked() { }
    protected void onJoinAppClicked() { }
    protected void onLeaveAppClicked() { }
    protected void onStopAppClicked() { }
    protected void onStartSessionClicked() { }
    protected void onEndSessionClicked() { }

    protected abstract void onPlayClicked();
    protected abstract void onPauseClicked();
    protected abstract void onStopClicked();
    protected abstract void onSeekBarMoved(long position);
    protected abstract void onDeviceVolumeBarMoved(int volume);
    protected abstract void onDeviceMuteToggled(boolean on);
    protected abstract void onStreamVolumeBarMoved(int volume);
    protected abstract void onStreamMuteToggled(boolean on);

    private void setUpControls() {
        mPlayMediaButton.setOnClickListener(new OnClickListener() {
            @Override
            public void onClick(View v) {
                selectMedia();
            }
        });

        mLaunchAppButton.setOnClickListener(new OnClickListener() {
            @Override
            public void onClick(View v) {
                onLaunchAppClicked();
            }
        });

        mJoinAppButton.setOnClickListener(new OnClickListener() {
            @Override
            public void onClick(View v) {
                onJoinAppClicked();
            }
        });

        mLeaveAppButton.setOnClickListener(new OnClickListener() {
            @Override
            public void onClick(View v) {
                onLeaveAppClicked();
            }
        });

        mStopAppButton.setOnClickListener(new OnClickListener() {
            @Override
            public void onClick(View v) {
                onStopAppClicked();
            }
        });

        mStartSessionButton.setOnClickListener(new OnClickListener() {
            @Override
            public void onClick(View v) {
                onStartSessionClicked();
            }
        });

        mEndSessionButton.setOnClickListener(new OnClickListener() {
            @Override
            public void onClick(View v) {
                onEndSessionClicked();
            }
        });

        mPlayPauseButton.setOnClickListener(new OnClickListener() {
            @Override
            public void onClick(View v) {
                if (mPlayerState == PLAYER_STATE_PAUSED) {
                    onPlayClicked();
                } else {
                    onPauseClicked();
                }
            }
        });

        mStopButton.setOnClickListener(new OnClickListener() {
            @Override
            public void onClick(View v) {
                onStopClicked();
            }
        });

        // Seek bar's range is in seconds, to prevent possibility of user seeking to fractions of
        // seconds.
        mSeekBar.setOnSeekBarChangeListener(new SeekBar.OnSeekBarChangeListener() {
            @Override
            public void onStopTrackingTouch(SeekBar seekBar) {
                mIsUserSeeking = false;
                mSeekBar.setSecondaryProgress(0);
                onSeekBarMoved(TimeUnit.SECONDS.toMillis(seekBar.getProgress()));
            }

            @Override
            public void onStartTrackingTouch(SeekBar seekBar) {
                mIsUserSeeking = true;
                mSeekBar.setSecondaryProgress(seekBar.getProgress());
            }

            @Override
            public void onProgressChanged(SeekBar seekBar, int progress, boolean fromUser) {
            }
        });

        setUpVolumeControls(mDeviceVolumeBar, mDeviceMuteCheckBox);
        setUpVolumeControls(mStreamVolumeBar, mStreamMuteCheckBox);

        mIsUserSeeking = false;
        mIsUserAdjustingVolume = false;
    }

    private void setUpVolumeControls(final SeekBar volumeBar, final CheckBox muteCheckBox) {
        volumeBar.setMax((int) MAX_VOLUME_LEVEL);
        volumeBar.setProgress(0);
        volumeBar.setOnSeekBarChangeListener(new SeekBar.OnSeekBarChangeListener() {
            @Override
            public void onStopTrackingTouch(SeekBar seekBar) {
                mIsUserAdjustingVolume = false;
                volumeBar.setSecondaryProgress(0);
                if (volumeBar == mDeviceVolumeBar) {
                    onDeviceVolumeBarMoved(seekBar.getProgress());
                } else {
                    onStreamVolumeBarMoved(seekBar.getProgress());
                }
            }

            @Override
            public void onStartTrackingTouch(SeekBar seekBar) {
                mIsUserAdjustingVolume = true;
                volumeBar.setSecondaryProgress(seekBar.getProgress());
            }

            @Override
            public void onProgressChanged(SeekBar seekBar, int progress, boolean fromUser) {
            }
        });

        muteCheckBox.setOnCheckedChangeListener(new CheckBox.OnCheckedChangeListener() {
            @Override
            public void onCheckedChanged(CompoundButton view, boolean isChecked) {
                if (muteCheckBox == mDeviceMuteCheckBox) {
                    onDeviceMuteToggled(isChecked);
                } else {
                    onStreamMuteToggled(isChecked);
                }
            }
        });
    }

    protected final void showErrorDialog(String errorString) {
        if (!isFinishing()) {
            new AlertDialog.Builder(this).setTitle(R.string.error)
                    .setMessage(errorString)
                    .setPositiveButton(R.string.ok, new DialogInterface.OnClickListener() {
                        @Override
                        public void onClick(DialogInterface dialog, int id) {
                            dialog.cancel();
                        }
                    })
                    .create()
                    .show();
        }
    }

    protected final void setPlayerState(int playerState) {
        mPlayerState = playerState;
        if (mPlayerState == PLAYER_STATE_PAUSED) {
            mPlayPauseButton.setText(R.string.play);
        } else if (mPlayerState == PLAYER_STATE_PLAYING) {
            mPlayPauseButton.setText(R.string.pause);
        }

        mPlayPauseButton.setEnabled((mPlayerState == PLAYER_STATE_PAUSED)
                || (mPlayerState == PLAYER_STATE_PLAYING));
    }

    protected final int getPlayerState() {
        return mPlayerState;
    }

    protected final void setCurrentDeviceName(String name) {
        mCurrentDeviceTextView.setText((name != null) ? name : getString(R.string.no_device));
    }

    protected final void setApplicationStatus(String statusText) {
        mAppStatusTextView.setText(statusText);
    }

    protected final boolean getRelaunchApp() {
        return mRelaunchApp;
    }

    protected final boolean getStopAppWhenEndingSession() {
        return mStopAppWhenEndingSession;
    }

    /**
     * Updates the currently-playing-item metadata display. If the image URL is non-null and is
     * different from the one that is currently displayed, an asynchronous request will be started
     * to fetch the image at that URL.
     */
    protected final void setCurrentMediaMetadata(String title, String subtitle, Uri imageUrl) {
        Log.d(TAG, "setCurrentMediaMetadata: " + title + "," + subtitle + "," + imageUrl);

        mMediaTitle.setText(title);
        mMediaArtist.setText(subtitle);

        if ((imageUrl != null) && !imageUrl.equals(mCurrentImageUrl)) {
            if (mFetchBitmapTask != null) {
                mFetchBitmapTask.cancel(true);
            }

            final FetchBitmapTask fetchBitmapTask = new FetchBitmapTask() {
                @Override
                protected void onPostExecute(Bitmap result) {
                    if (result != null) {
                        mMediaArtImageView.setImageBitmap(result);
                    } else {
                        mCurrentImageUrl = null;
                    }

                    if (this == mFetchBitmapTask) {
                        mFetchBitmapTask = null;
                    }
                }
            };
            mFetchBitmapTask = fetchBitmapTask;
            mFetchBitmapTask.execute(imageUrl);
        } else if (imageUrl == null) {
            mMediaArtImageView.setImageBitmap(null);
        }
        mCurrentImageUrl = imageUrl;
    }

    /**
     *
     * @param position The stream position, or 0 if no media is currently loaded, or -1 to leave
     * the value unchanged.
     * @param duration The stream duration, or 0 if no media is currently loaded, or -1 to leave
     * the value unchanged.
     */
    protected final void refreshPlaybackPosition(long position, long duration) {
        if (!mIsUserSeeking) {
            if (position == 0) {
                mStreamPositionTextView.setText(R.string.no_time);
                mSeekBar.setProgress(0);
            } else if (position > 0) {
                mSeekBar.setProgress((int) TimeUnit.MILLISECONDS.toSeconds(position));
            }
            mStreamPositionTextView.setText(formatTime(position));
        }

        if (duration == 0) {
            mStreamDurationTextView.setText(R.string.no_time);
            mSeekBar.setMax(0);
        } else if (duration > 0) {
            mStreamDurationTextView.setText(formatTime(duration));
            if (!mIsUserSeeking) {
                mSeekBar.setMax((int) TimeUnit.MILLISECONDS.toSeconds(duration));
            }
        }
    }

    protected final void refreshDeviceVolume(double percent, boolean muted) {
        if (!mIsUserAdjustingVolume) {
            mDeviceVolumeBar.setProgress((int) (percent * MAX_VOLUME_LEVEL));
        }
        mDeviceMuteCheckBox.setChecked(muted);
    }

    protected final void refreshStreamVolume(double percent, boolean muted) {
        if (!mIsUserAdjustingVolume) {
            mStreamVolumeBar.setProgress((int) (percent * MAX_VOLUME_LEVEL));
        }
        mStreamMuteCheckBox.setChecked(muted);
    }

    private String formatTime(long millisec) {
        int seconds = (int) (millisec / 1000);
        int hours = seconds / (60 * 60);
        seconds %= (60 * 60);
        int minutes = seconds / 60;
        seconds %= 60;

        String time;
        if (hours > 0) {
            time = String.format("%d:%02d:%02d", hours, minutes, seconds);
        } else {
            time = String.format("%d:%02d", minutes, seconds);
        }
        return time;
    }

    protected final void setAppControlsVisible(boolean visible) {
        int controlsVisibility = visible ? View.VISIBLE : View.GONE;
        mAppControls.setVisibility(controlsVisibility);
        mAppStatusTextView.setVisibility(controlsVisibility);
    }

    protected final void setSessionControlsVisible(boolean visible) {
        int controlsVisibility = visible ? View.VISIBLE : View.GONE;
        mSessionControls.setVisibility(controlsVisibility);
    }

    protected final void setSeekBarEnabled(boolean enabled) {
        mSeekBar.setEnabled(enabled);
    }

    protected final void setDeviceVolumeControlsEnabled(boolean enabled) {
        mDeviceVolumeBar.setEnabled(enabled);
        mDeviceMuteCheckBox.setEnabled(enabled);
    }

    protected final void setStreamVolumeControlsEnabled(boolean enabled) {
        mStreamVolumeBar.setEnabled(enabled);
        mStreamMuteCheckBox.setEnabled(enabled);
    }

    protected final void setSeekBehaviorControlsVisible(boolean visible) {
        int controlsVisibility = visible ? View.VISIBLE : View.GONE;
        mSeekBehaviorControls.setVisibility(controlsVisibility);
    }

    protected final void setStreamVolumeControlsVisible(boolean visible) {
        int controlsVisibility = visible ? View.VISIBLE : View.GONE;
        mStreamVolumeControls.setVisibility(controlsVisibility);
    }

    protected final void setAutoplayCheckboxVisible(boolean visible) {
        mAutoplayCheckbox.setVisibility(visible ? View.VISIBLE : View.GONE);
    }

    protected final int getSeekBehavior() {
        return mSeekBehaviorSpinner.getSelectedItemPosition();
    }

    protected final boolean isAutoplayChecked() {
        return mAutoplayCheckbox.isChecked();
    }

    /**
     * Override to specify the control category to send with the {@link MediaRouteSelector} request.
     */
    protected abstract String getControlCategory();

    protected abstract void onRouteSelected(RouteInfo route);
    protected abstract void onRouteUnselected(RouteInfo route);

    private MediaRouteSelector buildMediaRouteSelector() {
        return new MediaRouteSelector.Builder()
                .addControlCategory(getControlCategory())
                .build();
    }

    private class MyMediaRouterCallback extends MediaRouter.Callback {
        @Override
        public void onRouteSelected(MediaRouter router, RouteInfo route) {
            Log.d(TAG, "onRouteSelected: route=" + route);
            mRouteSelected = true;
            BaseCastPlayerActivity.this.onRouteSelected(route);
        }

        @Override
        public void onRouteUnselected(MediaRouter router, RouteInfo route) {
            Log.d(TAG, "onRouteUnselected: route=" + route);
            mRouteSelected = false;
            BaseCastPlayerActivity.this.onRouteUnselected(route);
        }
    }

    @Override
    public void onSharedPreferenceChanged(SharedPreferences sharedPreferences, String key) {
        Log.d(TAG, "pref changed: " + key);
        if (AppConstants.PREF_KEY_MEDIA_URL.equals(key)) {
            // If the media URL changed, clear the media dialog's list so that it will refetch it.
            if (mMediaSelectionDialog != null) {
                Log.d(TAG, "flushing media list");
                mMediaSelectionDialog.invalidateData();
            }
        } else if (AppConstants.PREF_KEY_RELAUNCH_APP.equals(key)) {
            mRelaunchApp = sharedPreferences.getBoolean(AppConstants.PREF_KEY_RELAUNCH_APP, false);
        } else if (AppConstants.PREF_KEY_STOP_APP.equals(key)) {
            mStopAppWhenEndingSession = sharedPreferences.getBoolean(AppConstants.PREF_KEY_STOP_APP,
                    false);
        } else if (AppConstants.PREF_KEY_RECEIVER_APPLICATION_ID.equals(key)) {
            mReceiverApplicationId = sharedPreferences.getString(
                    AppConstants.PREF_KEY_RECEIVER_APPLICATION_ID, null);
        }
    }

    protected final String getReceiverApplicationId() {
        if (TextUtils.isEmpty(mReceiverApplicationId)) {
            return CastMediaControlIntent.DEFAULT_MEDIA_RECEIVER_APPLICATION_ID;
        }
        return mReceiverApplicationId;
    }

    protected abstract void onRefreshEvent();

    protected final void startRefreshTimer() {
        mHandler.postDelayed(mRefreshRunnable, REFRESH_INTERVAL_MS);
    }

    protected final void cancelRefreshTimer() {
        mHandler.removeCallbacks(mRefreshRunnable);
    }

    protected final boolean isUserSeeking() {
        return mIsUserSeeking;
    }

    @TargetApi(Build.VERSION_CODES.ICE_CREAM_SANDWICH)
    private void setUpRemoteControlClient() {
        AudioManager audioManager = (AudioManager) getSystemService(Context.AUDIO_SERVICE);
        // Very important to request for the focus.
        audioManager.requestAudioFocus(null, AudioManager.STREAM_MUSIC,
                AudioManager.AUDIOFOCUS_GAIN_TRANSIENT);

        // Set up the RemoteControlClient to invoke our BroadcastReceiver.
        ComponentName eventReceiver = new ComponentName(this,
                RemoteControlReceiver.class.getName());
        Intent mediaButtonIntent = new Intent(Intent.ACTION_MEDIA_BUTTON);
        mediaButtonIntent.setComponent(eventReceiver);
        PendingIntent mediaPendingIntent = PendingIntent.
                getBroadcast(getApplicationContext(), 0, mediaButtonIntent, 0);
        mRemoteControlClient = new RemoteControlClient(mediaPendingIntent);

        // Register the remote control client.
        audioManager.registerMediaButtonEventReceiver(eventReceiver);
        audioManager.registerRemoteControlClient(mRemoteControlClient);

        // Add buttons that are needed on the remote control client.
        mRemoteControlClient.setPlaybackState(RemoteControlClient.PLAYSTATE_PLAYING);

        mRemoteControlClient.setTransportControlFlags(
                RemoteControlClient.FLAG_KEY_MEDIA_PLAY_PAUSE
                | RemoteControlClient.FLAG_KEY_MEDIA_NEXT
                | RemoteControlClient.FLAG_KEY_MEDIA_PREVIOUS);

        mMediaRouter.addRemoteControlClient(mRemoteControlClient);
    }

}
