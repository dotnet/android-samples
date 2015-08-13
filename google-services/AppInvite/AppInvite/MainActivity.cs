using Android.Net;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Support.V7.App;
using Android;
using Android.Gms.AppInvite;
using Android.Util;
using Android.Support.V4.Content;
using Android.Support.Design.Widget;
using System;

namespace AppInvite
{
	[Activity (Label = "@string/app_name", MainLauncher = true)]
	[IntentFilter (new [] { Intent.ActionView },
		Categories = new [] {
			Intent.CategoryDefault,
			Intent.CategoryBrowsable
		},
		DataScheme = "http",
		DataHost = "example.com"
	)]
	public class MainActivity : AppCompatActivity, View.IOnClickListener
	{
		static readonly string Tag = typeof(MainActivity).Name;
		const int RequestInvite = 0;

		BroadcastReceiver deepLinkReceiver = null;

		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);
			SetContentView (Resource.Layout.main_activity);

			FindViewById (Resource.Id.invite_button).SetOnClickListener (this);

			if (savedInstanceState == null) {
				var intent = Intent;
				if (AppInviteReferral.HasReferral (intent)) {
					LaunchDeepLinkActivity (intent);
				}
			}
		}

		protected override void OnStart ()
		{
			base.OnStart ();
			RegisterDeepLinkReceiver ();
		}

		protected override void OnStop ()
		{
			base.OnStop ();
			UnregisterDeepLinkReceiver ();
		}

		void OnInviteClicked ()
		{
			var intent = new AppInviteInvitation.IntentBuilder (GetString (Resource.String.invitation_title))
				.SetMessage (GetString (Resource.String.invitation_message))
				.SetDeepLink (Android.Net.Uri.Parse (GetString (Resource.String.invitation_deep_link)))
				.Build ();

			StartActivityForResult (intent, RequestInvite);
		}

		protected override void OnActivityResult (int requestCode, Result resultCode, Intent data)
		{
			base.OnActivityResult (requestCode, resultCode, data);
			Log.Debug (Tag, "onActivityResult: requestCode=" + requestCode + ", resultCode=" + resultCode);

			if (requestCode == RequestInvite) {
				if (resultCode == Result.Ok) {
					var ids = AppInviteInvitation.GetInvitationIds ((int)resultCode, data);
					Log.Debug (Tag, ids.Length.ToString() + GetString (Resource.String.sent_invitations_fmt));
				} else {
					// Sending failed or it was canceled, show failure message to the user
					ShowMessage (GetString (Resource.String.send_failed));
				}
			}
		}

		void ShowMessage (string msg)
		{
			var container = FindViewById<ViewGroup> (Resource.Id.snackbar_layout);
			Snackbar.Make (container, msg, Snackbar.LengthShort).Show ();
		}

		public void OnClick (View v)
		{
			switch (v.Id) {
			case Resource.Id.invite_button:
				OnInviteClicked ();
				break;
			}
		}

		void RegisterDeepLinkReceiver ()
		{
			deepLinkReceiver = new BroadcastReceiver ();
			deepLinkReceiver.OnReceiveImpl = (context, intent) => {
				if (AppInviteReferral.HasReferral (intent)) {
					LaunchDeepLinkActivity (intent);
				}
			};
			var intentFilter = new IntentFilter (GetString (Resource.String.action_deep_link));
			LocalBroadcastManager.GetInstance (this).RegisterReceiver (
				deepLinkReceiver, intentFilter);
		}

		void UnregisterDeepLinkReceiver ()
		{
			if (deepLinkReceiver != null) {
				LocalBroadcastManager.GetInstance (this).UnregisterReceiver (deepLinkReceiver);
			}
		}

		void LaunchDeepLinkActivity (Intent intent)
		{
			Log.Debug (Tag, "launchDeepLinkActivity:" + intent);
			var newIntent = new Intent (intent).SetClass (this, typeof(DeepLinkActivity));
			StartActivity (newIntent);
		}

		class BroadcastReceiver : Android.Content.BroadcastReceiver
		{
			public Action<Context, Intent> OnReceiveImpl { get; set; }

			public override void OnReceive (Context context, Intent intent)
			{
				OnReceiveImpl (context, intent);
			}
		}
	}
}


