using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using System.Collections.Generic;
using Android.Views.InputMethods;
using ME.Kiip.Api;
using Android.Util;
using Android.Telephony;

namespace KiipSampleApplication
{
    [Activity(Label = "KiipSampleApplication", MainLauncher = true, Icon = "@drawable/icon")]
    public class ExampleActivity : Activity, Android.Widget.TextView.IOnEditorActionListener, Android.Views.View.IOnClickListener, ME.Kiip.Api.Kiip.IViewListener
    {

        private static readonly string TAG = "example";

        private Button mUnlockAchievement, mSaveLeaderboard, mShowNotification, mShowFullscreen, mGetActivePromos, mNewActivity;

        private EditText mAchievementId, mLeaderboardId;

        private static ToggleButton mPositionToggle, mRewardActionToggle;

        private List<ME.Kiip.Api.Resource> mResources = new List<ME.Kiip.Api.Resource>();

        private RewardRequestListener mRewardsListener;
        private ActivePromosRequestListener mActivePromosListener;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            SetTitle(Resource.String.app_name);

            mGetActivePromos = (Button)FindViewById(Resource.Id.getActivePromos);
            mGetActivePromos.SetOnClickListener(this);

            mAchievementId = (EditText)FindViewById(Resource.Id.achievementId);
            mAchievementId.SetOnEditorActionListener(this);
            mAchievementId.ImeOptions = ImeAction.Done;
            mLeaderboardId = (EditText)FindViewById(Resource.Id.leaderboardId);
            mLeaderboardId.SetOnEditorActionListener(this);
            mLeaderboardId.ImeOptions = ImeAction.Done;

            mUnlockAchievement = (Button)FindViewById(Resource.Id.unlockAchievement);
            mUnlockAchievement.SetOnClickListener(this);

            mSaveLeaderboard = (Button)FindViewById(Resource.Id.saveLeaderboard);
            mSaveLeaderboard.SetOnClickListener(this);

            mShowNotification = (Button)FindViewById(Resource.Id.showNotification);
            mShowNotification.SetOnClickListener(this);

            mShowFullscreen = (Button)FindViewById(Resource.Id.showFullscreen);
            mShowFullscreen.SetOnClickListener(this);

            mRewardActionToggle = (ToggleButton)FindViewById(Resource.Id.rewardActionToggle);
            mPositionToggle = (ToggleButton)FindViewById(Resource.Id.positionToggle);

            mNewActivity = (Button)FindViewById(Resource.Id.newActivity);
            mNewActivity.SetOnClickListener(this);

            if (bundle != null)
            {
                // Restore EditText fields
                mAchievementId.Text = bundle.GetString("achievement_id");
                mLeaderboardId.Text = bundle.GetString("leaderboard_id");
            }
            else
            {
                mAchievementId.Text = "com.jmawebtechnologies.gemfindermt.beatgame";
                mLeaderboardId.Text = "com.jmawebtechnologies.gemfindermt.leaderboard.onethousand";
            }

            mActivePromosListener = new ActivePromosRequestListener(this);
            mRewardsListener = new RewardRequestListener(this);
        }


        public bool OnEditorAction(TextView v, ImeAction actionId, KeyEvent e)
        {
            switch (v.Id)
            {
                case Resource.Id.achievementId:
                    mUnlockAchievement.PerformClick();
                    break;
                case Resource.Id.leaderboardId:
                    mSaveLeaderboard.PerformClick();
                    break;
                default:
                    return false;
            }
            return false;
        }

        protected override void OnStart()
        {
            base.OnStart();
            Kiip.Instance.StartSession(
                this, ((ExampleApplication)Application).startSessionListener);
        }

        protected override void OnStop()
        {
            base.OnStop();
            Kiip.Instance.StartSession(
                this, ((ExampleApplication)Application).endSessionListener);

        }

        protected override void OnSaveInstanceState(Bundle bundle)
        {
            base.OnSaveInstanceState(bundle);
            // Save EditText fields when rotated
            bundle.PutString("achievement_id", mAchievementId.Text);
            bundle.PutString("leaderboard_id", mLeaderboardId.Text);
        }

        class OnCheckedChangedListener : Android.Widget.RadioGroup.IOnCheckedChangeListener
        {

            public void OnCheckedChanged(RadioGroup group, int checkedId)
            {
                if (checkedId == 0)
                {
                    mPositionToggle.Enabled = false;
                }
                else
                {
                    mPositionToggle.Enabled = true;
                }
            }

            public void Dispose()
            {

            }

            public IntPtr Handle { get; set; }
        }

        class RewardRequestListener : Java.Lang.Object, Kiip.IRequestListener
        {
            ExampleActivity example = new ExampleActivity();

            public RewardRequestListener(ExampleActivity example)
            {
                this.example = example;
            }

            public void OnError(Kiip p0, KiipException p1)
            {
                example.toast("error (" + p1.Code + ") " + p1.Message);
            }

            public void OnFinished(Kiip p0, Java.Lang.Object p1)
            {
                ME.Kiip.Api.Resource response = p1.JavaCast<ME.Kiip.Api.Resource>();
                if (response != null)
                {
                    if (mRewardActionToggle.Checked)
                    {
                        p0.ShowResource(response);
                    }
                    else
                    {
                        example.toast("Reward Queued");
                        example.mResources.Add(response);
                    }
                }
                else
                {
                    example.toast("No Reward");
                }
            }
        }

        class ActivePromosRequestListener : Java.Lang.Object, Kiip.IRequestListener
        {
            ExampleActivity example = new ExampleActivity();

            public ActivePromosRequestListener(ExampleActivity example)
            {
                this.example = example;
            }

            public void OnError(Kiip p0, KiipException p1)
            {
                example.toast("error (" + p1.Code + ") " + p1.Message);
      
            }

            public void OnFinished(Kiip p0, Java.Lang.Object p1)
            {
                ME.Kiip.Api.Resource response = p1.JavaCast<ME.Kiip.Api.Resource>();
                example.toast(response.ToString());
            }
        }

        public void OnClick(View v)
        {
            Kiip manager = Kiip.Instance;

            switch (v.Id)
            {
                case Resource.Id.getActivePromos:
                    manager.GetActivePromos(mActivePromosListener);
                    break;

                case Resource.Id.unlockAchievement:
                    manager.UnlockAchievement(mAchievementId.Text, new RewardRequestListener(this){});
                    break;

                case Resource.Id.saveLeaderboard:
                    manager.SaveLeaderboard(mLeaderboardId.Text, 100, mRewardsListener);
                    break;

                case Resource.Id.showNotification:
                    if (mResources.Count > 0)
                    {
                        ME.Kiip.Api.Resource resource = mResources[0];
                        manager.ShowResource(resource);
                    }
                    break;

                case Resource.Id.showFullscreen:
                    if (mResources.Count > 0)
                    {
                        toast("Showing Fullscreen (" + mResources.Count + ")");

                        ME.Kiip.Api.Resource resource = mResources[0];
                        resource.Position = ME.Kiip.Api.Kiip.Position.Fullscreen;
                        manager.ShowResource(resource);
                    }
                    break;

                case Resource.Id.newActivity:
                    Intent intent = new Intent(this, this.Class);
                    StartActivity(intent);
                    break;
            }
        }
    
        //==============================================================================
        // Util methods
        //==============================================================================

        private void toast(string message)
        {
            Log.Verbose(TAG, message);
            Toast.MakeText(this, message, ToastLength.Short).Show();
        }

        //==============================================================================
        // KPViewListener callbacks
        //==============================================================================

        public void OnFullscreenDidDismiss(ME.Kiip.Api.Resource p0)
        {
            toast("FullscreenDidDismiss");
        }

        public void OnFullscreenDidShow(ME.Kiip.Api.Resource p0)
        {
            toast("FullscreenDidShow");
        }

        public void OnNotificationDidDismiss(ME.Kiip.Api.Resource p0, bool p1)
        {
            toast("NotificationDidDismiss(" + p1 + ")");
        }

        public void OnNotificationDidShow(ME.Kiip.Api.Resource p0)
        {
            toast("Notification did show");
        }
    }

}

