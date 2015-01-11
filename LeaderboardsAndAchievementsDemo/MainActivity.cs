using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

using GooglePlay.Services.Helpers;
using Android.Gms.Common;

namespace LeaderboardsAndAchievementsDemo
{
	[Activity (Label = "LeaderboardsAndAchievementsDemo", MainLauncher = true, Icon = "@drawable/icon"
		#if __ANDROID_11__
		,HardwareAccelerated=false
		#endif
	)]
	public class MainActivity : Activity
	{
		GameHelper helper;
		SignInButton signInButton;
		Button signOutButton;
		LinearLayout signInLayout;
		LinearLayout controlsLayout;
		Button awardAchievementButton;
		EditText achievementCode;
		Button showAchievements;
		Button showAllLeaderboards;
		Button showLeaderboard;
		Button submitScore;
		EditText leaderboardCode;
		EditText score;


		/* 
		 * Achievements
		"10 Hits","CgkIl8Dgls4FEAIQAg"
		"20 Hits","CgkIl8Dgls4FEAIQAw"
		"50 hits","CgkIl8Dgls4FEAIQBA"
		"100 Hits","CgkIl8Dgls4FEAIQBQ"
		"200 Hits","CgkIl8Dgls4FEAIQBg"
		*/

		/*
		 * Leaderboards
		"Top Scores","CgkIl8Dgls4FEAIQBw"
		*/

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			// Set our view from the "main" layout resource
			SetContentView (Resource.Layout.Main);

			signInLayout = FindViewById<LinearLayout> (Resource.Id.signinlayout);
			controlsLayout = FindViewById<LinearLayout> (Resource.Id.controlslayout);

			signInButton = FindViewById<SignInButton> (Resource.Id.signin);
			signInButton.Click += (object sender, EventArgs e) => {
				if (helper != null && helper.SignedOut) {
					helper.SignIn ();
				}
			};

			signOutButton = FindViewById<Button> (Resource.Id.signout);
			signOutButton.Click += (object sender, EventArgs e) => {
				if (helper != null && !helper.SignedOut) {
					helper.SignOut ();
					signInLayout.Visibility = ViewStates.Visible;
					controlsLayout.Visibility = ViewStates.Gone;
				}
			};

			awardAchievementButton = FindViewById<Button> (Resource.Id.awardachievement);
			achievementCode = FindViewById<EditText> (Resource.Id.achievementcode);

			awardAchievementButton.Click += (object sender, EventArgs e) => {
				if (helper != null && !helper.SignedOut) {
					helper.UnlockAchievement(achievementCode.Text);
				}
			};

			showAchievements = FindViewById<Button> (Resource.Id.showachievement);
			showAchievements.Click += (object sender, EventArgs e) => {
				if (helper != null && !helper.SignedOut) {
					helper.ShowAchievements();
				}
			};

			leaderboardCode = FindViewById<EditText> (Resource.Id.leaderboardcode);
			score = FindViewById<EditText> (Resource.Id.score);

			showAllLeaderboards = FindViewById<Button> (Resource.Id.showallleaderboards);
			showAllLeaderboards.Click += (object sender, EventArgs e) => {
				if (helper != null && !helper.SignedOut) {
					helper.ShowAllLeaderBoardsIntent();
				}
			};

			showLeaderboard = FindViewById<Button> (Resource.Id.showleaderboard);
			showLeaderboard.Click += (object sender, EventArgs e) => {
				if (helper != null && !helper.SignedOut) {
					var code = leaderboardCode.Text;
					helper.ShowLeaderBoardIntentForLeaderboard(code);
				}
			};

			submitScore = FindViewById<Button> (Resource.Id.submitscore);
			submitScore.Click += (object sender, EventArgs e) => {
				if (helper != null && !helper.SignedOut) {
					var code = leaderboardCode.Text;
					var value = int.Parse(score.Text);
					helper.SubmitScore(code, value);
				}
			};
				
			InitializeServices ();

		}

		void InitializeServices() {
			// Setup Google Play Services Helper
			helper = new GameHelper (this);
			// Set Gravity and View for Popups
			helper.GravityForPopups = (GravityFlags.Bottom | GravityFlags.Center);
			helper.ViewForPopups = FindViewById<View>(Resource.Id.container);
			// Hook up events
			helper.OnSignedIn += (object sender, EventArgs e) => {
				signInLayout.Visibility = ViewStates.Gone;
				controlsLayout.Visibility = ViewStates.Visible;
			};
			helper.OnSignInFailed += (object sender, EventArgs e) => {
				signInLayout.Visibility = ViewStates.Visible;
				controlsLayout.Visibility = ViewStates.Gone;
			};

			helper.Initialize ();
		}

		protected override void OnStart ()
		{
			base.OnStart ();
			if (helper != null)
				helper.Start ();
		}

		protected override void OnActivityResult (int requestCode, Result resultCode, Intent data)
		{
			if (helper != null)
				helper.OnActivityResult (requestCode, resultCode, data);
			base.OnActivityResult (requestCode, resultCode, data);
		}

		protected override void OnStop ()
		{
			if (helper != null)
				helper.Stop ();
			base.OnStop ();
		}
	}
}


