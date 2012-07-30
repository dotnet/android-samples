package me.kiip.example;

import java.util.LinkedList;
import java.util.List;
import java.util.Map;

import me.kiip.api.Kiip;
import me.kiip.api.Kiip.Position;
import me.kiip.api.Kiip.RequestListener;
import me.kiip.api.Kiip.ViewListener;
import me.kiip.api.KiipException;
import me.kiip.api.Resource;
import android.app.Activity;
import android.content.Intent;
import android.os.Bundle;
import android.util.Log;
import android.view.KeyEvent;
import android.view.View;
import android.view.View.OnClickListener;
import android.view.inputmethod.EditorInfo;
import android.widget.Button;
import android.widget.CompoundButton;
import android.widget.CompoundButton.OnCheckedChangeListener;
import android.widget.EditText;
import android.widget.TextView;
import android.widget.TextView.OnEditorActionListener;
import android.widget.Toast;
import android.widget.ToggleButton;

public class ExampleActivity extends Activity
    implements OnClickListener, ViewListener, OnEditorActionListener {
    private static final String TAG = "example";

    private Button mUnlockAchievement, mSaveLeaderboard;

    private EditText mAchievementId,
        mLeaderboardId;

    private ToggleButton mPositionToggle,
        mRewardActionToggle;

    private LinkedList<Resource> mResources = new LinkedList<Resource>();

    /** Called when the activity is first created. */
    @Override
    public void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setTitle(this.getString(R.string.app_name) + " " + Kiip.VERSION_NAME);
        setContentView(R.layout.main);

        Button button;

        button = (Button)this.findViewById(R.id.getActivePromos);
        button.setOnClickListener(this);

        mAchievementId = (EditText)this.findViewById(R.id.achievementId);
        mAchievementId.setOnEditorActionListener(this);
        mAchievementId.setImeOptions(EditorInfo.IME_ACTION_DONE);
        mLeaderboardId = (EditText)this.findViewById(R.id.leaderboardId);
        mLeaderboardId.setOnEditorActionListener(this);
        mLeaderboardId.setImeOptions(EditorInfo.IME_ACTION_DONE);

        mUnlockAchievement = (Button)this.findViewById(R.id.unlockAchievement);
        mUnlockAchievement.setOnClickListener(this);

        mSaveLeaderboard = (Button)this.findViewById(R.id.saveLeaderboard);
        mSaveLeaderboard.setOnClickListener(this);

        button = (Button)this.findViewById(R.id.showNotification);
        button.setOnClickListener(this);

        button = (Button)this.findViewById(R.id.showFullscreen);
        button.setOnClickListener(this);

        mRewardActionToggle = (ToggleButton)this.findViewById(R.id.rewardActionToggle);
        mRewardActionToggle.setOnCheckedChangeListener(new OnCheckedChangeListener() {
            @Override public void onCheckedChanged(CompoundButton buttonView, boolean isChecked) {
                mPositionToggle.setEnabled(isChecked);
            }
        });
        mPositionToggle = (ToggleButton)this.findViewById(R.id.positionToggle);

        button = (Button)this.findViewById(R.id.newActivity);
        button.setOnClickListener(this);

        if(savedInstanceState != null) {
            // Restore EditText fields
            mAchievementId.setText(savedInstanceState.getString("achievement_id"));
            mLeaderboardId.setText(savedInstanceState.getString("leaderboard_id"));
        }
    }

    @Override
    protected void onStart() {
        super.onStart();

        Kiip.getInstance().startSession(
                this,
                ((ExampleApplication) getApplication()).startSessionListener);
    }

    @Override
    protected void onStop() {
        super.onStop();

        Kiip.getInstance().endSession(
                this,
                ((ExampleApplication) getApplication()).endSessionListener);
    }

    @Override
    public void onSaveInstanceState(Bundle outState) {
        super.onSaveInstanceState(outState);
        // Save EditText fields when rotated
        outState.putString("achievement_id", mAchievementId.getText().toString());
        outState.putString("leaderboard_id", mLeaderboardId.getText().toString());
    }

    @Override
    public boolean onEditorAction(TextView v, int actionId, KeyEvent event) {
        switch (v.getId()) {
            case R.id.achievementId:
                mUnlockAchievement.performClick();
                break;
            case R.id.leaderboardId:
                mSaveLeaderboard.performClick();
                break;
            default:
                return false;
        }
        return false;
    }

    @Override
    public void onClick(View v) {
        Kiip manager = Kiip.getInstance();

        switch (v.getId()) {
            case R.id.getActivePromos:
                manager.getActivePromos(mActivePromosListener);
                break;

            case R.id.unlockAchievement:
                manager.unlockAchievement(mAchievementId.getText().toString(), mRequestListener);
                break;

            case R.id.saveLeaderboard:
                manager.saveLeaderboard(mLeaderboardId.getText().toString(), 100, mRequestListener);
                break;

            case R.id.showNotification:
                if (mResources.size() > 0) {
                    Resource resource;
                    while(mResources.size() > 0) {
                        resource = mResources.removeFirst();
                        manager.showResource(resource);
                    }
                }
                break;

            case R.id.showFullscreen:
                if (mResources.size() > 0) {
                    Resource resource;
                    toast("Showing Fullscreen (" + mResources.size() + ")");
                    while(mResources.size() > 0) {
                        resource = mResources.removeFirst();
                        resource.position = Position.FULLSCREEN;
                        manager.showResource(resource);
                    }
                }
                break;

            case R.id.newActivity:
                Intent intent = new Intent(this, ExampleActivity.class);
                startActivity(intent);
                break;
        }
    }

    private RequestListener<List<Map<String, String>>> mActivePromosListener = new RequestListener<List<Map<String, String>>>() {
        @Override
        public void onFinished(Kiip manager, List<Map<String, String>> response) {
            toast(response.toString());
        }

        @Override
        public void onError(Kiip manager, KiipException error) {
            toast("error (" + error.getCode() + ") " + error.getMessage());
        }
    };

    private RequestListener<Resource> mRequestListener = new RequestListener<Resource>() {
        @Override
        public void onFinished(Kiip manager, Resource response) {
            if (response != null) {
                if (mRewardActionToggle.isChecked()) {
                    manager.showResource(response);
                } else {
                    toast("Reward Queued");
                    mResources.add(response);
                }
            } else {
               toast("No Reward");
            }
        }

        @Override
        public void onError(Kiip manager, KiipException error) {
            toast("error (" + error.getCode() + ") " + error.getMessage());
        }
    };

//==============================================================================
// KPViewListener callbacks
//==============================================================================

    @Override
    public void onNotificationDidShow(Resource resource) {
        toast("NotificationDidShow");
    }

    @Override
    public void onNotificationDidDismiss(Resource resource, boolean clicked) {
        toast("NotificationDidDismiss(" + clicked + ")");
    }

    @Override
    public void onFullscreenDidShow(Resource resource) {
        toast("FullscreenDidShow");
    }

    @Override
    public void onFullscreenDidDismiss(Resource resource) {
        toast("FullscreenDidDismiss");
    }

//==============================================================================
// Util methods
//==============================================================================

    private void toast(String message) {
        Log.v(TAG, message);
        Toast.makeText(this, message, Toast.LENGTH_SHORT).show();
    }
}