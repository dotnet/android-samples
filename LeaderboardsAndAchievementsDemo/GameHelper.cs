using System;
using Android.Gms.Common;
using Android.Gms.Common.Apis;
using Android.Gms.Games;
using Android.Gms.Games.Achievement;
using Android.Gms.Games.LeaderBoard;
using Android.App;
using Android.Content;
using Android.Views;
using Java.Interop;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GooglePlay.Services.Helpers
{
	/// <summary>
	/// Basic wrapper for interfacing with the GooglePlayServices Game API's
	/// </summary>
	public class GameHelper: Java.Lang.Object, GoogleApiClient.IConnectionCallbacks, GoogleApiClient.IOnConnectionFailedListener
	{
		GoogleApiClient client;
		Activity activity;
		bool signedOut = true;
		bool signingin = false;
		bool resolving = false;
		List<IAchievement> achievments = new List<IAchievement>();
		Dictionary<string, List<ILeaderboardScore>> scores = new Dictionary<string, List<ILeaderboardScore>> ();
		
		const int REQUEST_LEADERBOARD = 9002;
		const int REQUEST_ALL_LEADERBOARDS = 9003;
		const int REQUEST_ACHIEVEMENTS = 9004;
		const int RC_RESOLVE = 9001;

		/// <summary>
		/// Gets or sets a value indicating whether the user is signed out or not.
		/// </summary>
		/// <value><c>true</c> if signed out; otherwise, <c>false</c>.</value>
		public bool SignedOut {
			get { return signedOut; }
			set {
				if (signedOut != value) {
					signedOut = value;
					// Store if we Signed Out so we don't bug the player next time.
					using (var settings = this.activity.GetSharedPreferences ("googleplayservicessettings", FileCreationMode.Private)) {
						using (var e = settings.Edit ()) {
							e.PutBoolean ("SignedOut", signedOut);
							e.Commit ();
						}
					}
				}
			}
		}

		/// <summary>
		/// Gets or sets the gravity for the GooglePlay Popups. 
		/// Defaults to Bottom|Center
		/// </summary>
		/// <value>The gravity for popups.</value>
		public GravityFlags GravityForPopups { get; set; }

		/// <summary>
		/// The View on which the Popups should show
		/// </summary>
		/// <value>The view for popups.</value>
		public View ViewForPopups {get;set;}

		/// <summary>
		/// This event is fired when a user successfully signs in
		/// </summary>
		public event EventHandler OnSignedIn;

		/// <summary>
		/// This event is fired when the Sign in fails for any reason
		/// </summary>
		public event EventHandler OnSignInFailed;

		/// <summary>
		/// This event is fired when the user Signs out
		/// </summary>
		public event EventHandler OnSignedOut;

		/// <summary>
		/// List of Achievements. Populated by LoadAchievements
		/// </summary>
		/// <value>The achievements.</value>
		public List<IAchievement> Achievements { 
			get { return achievments; }
		}

		public GameHelper (Activity activity)
		{
			this.activity = activity;
			this.GravityForPopups = GravityFlags.Bottom | GravityFlags.Center;			
		}

		public void Initialize() {

			var settings = this.activity.GetSharedPreferences ("googleplayservicessettings", FileCreationMode.Private);
			signedOut = settings.GetBoolean ("SignedOut", true);

			if (!signedOut)
				CreateClient ();
		}

		private void CreateClient() {

			// did we log in with a player id already? If so we don't want to ask which account to use
			var settings = this.activity.GetSharedPreferences ("googleplayservicessettings", FileCreationMode.Private);
			var id = settings.GetString ("playerid", String.Empty);

			var builder = new GoogleApiClient.Builder (activity, this, this);
			builder.AddApi (Android.Gms.Games.GamesClass.API);
			builder.AddScope (Android.Gms.Games.GamesClass.ScopeGames);
			builder.SetGravityForPopups ((int)GravityForPopups);
			if (ViewForPopups != null)
				builder.SetViewForPopups (ViewForPopups);
			if (!string.IsNullOrEmpty (id)) {
				builder.SetAccountName (id);
			}
			client = builder.Build ();
		}

		/// <summary>
		/// Start the GooglePlayClient. This should be called from your Activity Start
		/// </summary>
		public void Start() {

			if(SignedOut && !signingin)
				return;

			if (client != null && !client.IsConnected) {
				client.Connect ();
			}
		}

		/// <summary>
		/// Disconnects from the GooglePlayClient. This should be called from your Activity Stop
		/// </summary>
		public void Stop() {

			if (client != null && client.IsConnected) {
				client.Disconnect ();
			}
		}

		/// <summary>
		/// Reconnect to google play.
		/// </summary>
		public void Reconnect() {
			if (client != null)
				client.Reconnect ();
		}

		/// <summary>
		/// Sign out of Google Play and make sure we don't try to auto sign in on the next startup
		/// </summary>
		public void SignOut() {

			SignedOut = true;
			if (client.IsConnected) {
				GamesClass.SignOut (client);
				Stop ();
				using (var settings = this.activity.GetSharedPreferences ("googleplayservicessettings", FileCreationMode.Private)) {
					using (var e = settings.Edit ()) {
						e.PutString ("playerid",String.Empty);
						e.Commit ();
					}
				}
				client.Dispose ();
				client = null;
				if (OnSignedOut != null)
					OnSignedOut (this, EventArgs.Empty);
			}
		}

		/// <summary>
		/// Attempt to Sign in to Google Play
		/// </summary>
		public void SignIn() {

			signingin = true;
			if (client == null)
				CreateClient ();

			if (client.IsConnected)
				return;

			if (client.IsConnecting)
				return;
				
			var result = GoogleApiAvailability.Instance.IsGooglePlayServicesAvailable (activity);
			if (result != ConnectionResult.Success) {
				return;
			}
				
			Start ();

		}

		/// <summary>
		/// Unlocks the achievement.
		/// </summary>
		/// <param name="achievementCode">Achievement code from you applications Google Play Game Services Achievements Page</param>
		public void UnlockAchievement(string achievementCode) {
			GamesClass.Achievements.Unlock (client, achievementCode);
		}

		public void IncrementAchievement(string achievementCode, int progress) {
			GamesClass.Achievements.Increment (client, achievementCode, progress);
		}

		/// <summary>
		/// Show the built in google Achievements Activity. This will cause your application to go into a Paused State
		/// </summary>
		public void ShowAchievements() {
			var  intent = GamesClass.Achievements.GetAchievementsIntent (client);
			activity.StartActivityForResult (intent, REQUEST_ACHIEVEMENTS);
		}
			
		/// <summary>
		/// Submit a score to google play. The score will only be updated if it is greater than the existing score. 
		/// This is not immediate but will occur at the next sync of the google play client.
		/// </summary>
		/// <param name="leaderboardCode">Leaderboard code from you applications Google Play Game Services Leaderboards Page</param>
		/// <param name="value">The value of the score</param>
		public void SubmitScore(string leaderboardCode, long value) {
			GamesClass.Leaderboards.SubmitScore (client, leaderboardCode, value);
		}

		/// <summary>
		/// Submit a score to google play. The score will only be updated if it is greater than the existing score. 
		/// This is not immediate but will occur at the next sync of the google play client.
		/// </summary>
		/// <param name="leaderboardCode">Leaderboard code from you applications Google Play Game Services Leaderboards Page</param>
		/// <param name="value">The value of the score</param>
		/// <param name="value">Additional MetaData to attach. Must be a URI safe string with a max length of 64 characters</param>
		public void SubmitScore(string leaderboardCode, long value, string metadata) {
			GamesClass.Leaderboards.SubmitScore (client, leaderboardCode, value, metadata);
		}

		/// <summary>
		/// Show the built in leaderboard activity for the leaderboard code.
		/// </summary>
		/// <param name="leaderboardCode">Leaderboard code from you applications Google Play Game Services Leaderboards Page</param>
		public void ShowLeaderBoardIntentForLeaderboard(string leaderboardCode) {
			var intent = GamesClass.Leaderboards.GetLeaderboardIntent (client, leaderboardCode);
			activity.StartActivityForResult (intent, REQUEST_LEADERBOARD);
		}

		/// <summary>
		/// Show the built in leaderboard activity for all the leaderboards setup for your application
		/// </summary>
		public void ShowAllLeaderBoardsIntent() {
			var intent = GamesClass.Leaderboards.GetAllLeaderboardsIntent (client);
			activity.StartActivityForResult (intent, REQUEST_ALL_LEADERBOARDS);
		}

		/// <summary>
		/// Load the Achievments. This populates the Achievements property
		/// </summary>
		public async Task LoadAchievements() {
			var ar = await GamesClass.Achievements.LoadAsync (client, false);
            if (ar != null) {
                achievments.Clear ();
                achievments.AddRange (ar.Achievements);
            }
		}

        public async Task LoadTopScores(string leaderboardCode) {
			var ar = await GamesClass.Leaderboards.LoadTopScoresAsync (client, leaderboardCode, 2, 0, 25);
            if (ar != null) {
                var id = ar.Leaderboard.LeaderboardId;
                if (!scores.ContainsKey (id)) {
                    scores.Add (id, new List<ILeaderboardScore> ());
                }
                scores [id].Clear ();
                scores [id].AddRange (ar.Scores);
            }
		}

		#region IGoogleApiClientConnectionCallbacks implementation

		public void OnConnected (Android.OS.Bundle connectionHint)
		{
			resolving = false;
			SignedOut = false;
			signingin = false;

			using (var settings = this.activity.GetSharedPreferences ("googleplayservicessettings", FileCreationMode.Private)) {
				using (var e = settings.Edit ()) {
					e.PutString ("playerid",GamesClass.GetCurrentAccountName(client));
					e.Commit ();
				}
			}

			if (OnSignedIn != null)
				OnSignedIn (this, EventArgs.Empty);
		}

		public void OnConnectionSuspended (int resultCode)
		{
			resolving = false;
			SignedOut = false;
			signingin = false;
			client.Disconnect ();
			if (OnSignInFailed != null)
				OnSignInFailed (this, EventArgs.Empty);
		}
			
		public void OnConnectionFailed (ConnectionResult result)
		{
			if (resolving)
				return;

			if (result.HasResolution) {
				resolving = true;
				result.StartResolutionForResult (activity, RC_RESOLVE);
				return;
			}

			resolving = false;
			SignedOut = false;
			signingin = false;
			if (OnSignInFailed != null)
				OnSignInFailed (this, EventArgs.Empty);
		}
		#endregion

		/// <summary>
		/// Processes the Activity Results from the Signin process. MUST be called from your activity OnActivityResult override.
		/// </summary>
		/// <param name="requestCode">Request code.</param>
		/// <param name="resultCode">Result code.</param>
		/// <param name="data">Data.</param>
		public void OnActivityResult (int requestCode, Result resultCode, Intent data) {

			if (requestCode == RC_RESOLVE) {
				if (resultCode == Result.Ok) {
					Start ();
				} else {
					if (OnSignInFailed != null)
						OnSignInFailed (this, EventArgs.Empty);
				}
			}
		}            
	}
}

