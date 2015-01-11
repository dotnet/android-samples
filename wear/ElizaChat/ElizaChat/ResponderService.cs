using System;
using Android.Content;
using Android.OS;
using Android.App;
using Android.Support.V4.App;
using Android.Support.V4.Content;

using Android.Support.Wearable;
using Android.Text;
using Android.Util;
using Android.Graphics;

//C# using statements bring in the entire namespace when Java imports are one class at a time creating this ambiguity. This allows the code to be cleaner
using RemoteInput = Android.Support.V4.App.RemoteInput;
using StringBuffer = Java.Lang.StringBuffer;

namespace ElizaChat
{
	/**
	 * A service that runs in the background and provides responses to the incoming messages from the
	 * wearable. It also keeps a record of the chat session history, which it can provide upon request.
	 */
	[Service]
	[IntentFilter (new string[]{ ResponderService.ACTION_RESPONSE, MainActivity.ACTION_GET_CONVERSATION })]
	public class ResponderService : Service
	{
		public const string ACTION_INCOMING = "com.example.android.wearable.elizachat.INCOMING";
		public const string ACTION_RESPONSE = "com.example.android.wearable.elizachat.REPLY";
		public const string EXTRA_REPLY = "reply";
		private static readonly string TAG = "ResponderService";
		private static readonly int NOTIFICATION_ID = 0;
		// an arbitrary number to assign our notification

		private string mLastResponse = null;

		private StringBuffer mCompleteConversation = new StringBuffer ();

		private LocalBroadcastManager mBroadcastManager;
		private ElizaResponder mResponder;

		public override void OnCreate ()
		{
			base.OnCreate ();
			if (Log.IsLoggable (TAG, LogPriority.Debug))
				Log.Debug (TAG, "Chat Service Started");
			mResponder = new ElizaResponder ();
			mBroadcastManager = LocalBroadcastManager.GetInstance (this);
			ProcessIncoming (null);
		}

		public override IBinder OnBind (Intent intent)
		{
			return null;
		}

		public override StartCommandResult OnStartCommand (Intent intent, StartCommandFlags flags, int startId)
		{
			if (intent == null || intent.Action == null)
				return StartCommandResult.Sticky;
			var action = intent.Action;
			if (action.Equals (ACTION_RESPONSE)) {
				var remoteInputResults = RemoteInput.GetResultsFromIntent (intent);
				var replyMessage = "";
				if (remoteInputResults != null)
					replyMessage = remoteInputResults.GetCharSequence (EXTRA_REPLY);
				ProcessIncoming (replyMessage.ToString ());
			} else if (action.Equals (MainActivity.ACTION_GET_CONVERSATION))
				BroadcastMessage (mCompleteConversation.ToString ());
			return StartCommandResult.Sticky;
		}

		private void ShowNotification ()
		{
			if (Log.IsLoggable (TAG, LogPriority.Debug))
				Log.Debug (TAG, "Sent: " + mLastResponse);

			var builder = new NotificationCompat.Builder (this)
				.SetContentTitle (GetString (Resource.String.eliza))
				.SetContentText (mLastResponse)
				.SetLargeIcon (BitmapFactory.DecodeResource (Resources, Resource.Drawable.bg_eliza))
				.SetSmallIcon (Resource.Drawable.bg_eliza)
				.SetPriority (NotificationCompat.PriorityMin);

			var intent = new Intent (ACTION_RESPONSE);
			var pendingIntent = PendingIntent.GetService (this, 0, intent, PendingIntentFlags.OneShot | PendingIntentFlags.CancelCurrent);
			var notification = builder
				.Extend (new NotificationCompat.WearableExtender ()
					.AddAction (new NotificationCompat.Action.Builder 
						(Resource.Drawable.ic_full_reply, GetString (Resource.String.reply), pendingIntent)
						.AddRemoteInput (new RemoteInput.Builder (EXTRA_REPLY).SetLabel (GetString (Resource.String.reply)).Build ())
						.Build ()))
				.Build ();
			NotificationManagerCompat.From (this).Notify (NOTIFICATION_ID, notification);
		}

		private void ProcessIncoming (string text)
		{
			if (Log.IsLoggable (TAG, LogPriority.Debug))
				Log.Debug (TAG, "Received: " + text);
			mLastResponse = mResponder.ElzTalk (text);
			var line = TextUtils.IsEmpty (text) ? mLastResponse : text + "\n" + mLastResponse;
			if (!TextUtils.IsEmpty (text))
				BroadcastMessage (line);
			NotificationManagerCompat.From (this).CancelAll ();
			ShowNotification ();
			mCompleteConversation.Append ("\n" + line);
		}

		private void BroadcastMessage (string message)
		{
			var intent = new Intent (MainActivity.ACTION_NOTIFY);
			intent.PutExtra (MainActivity.EXTRA_MESSAGE, message);
			mBroadcastManager.SendBroadcast (intent);
		}

		public override void OnDestroy ()
		{
			if (Log.IsLoggable (TAG, LogPriority.Debug))
				Log.Debug (TAG, "Chat Service Stopped");
			NotificationManagerCompat.From (this).Cancel (0);
			mBroadcastManager = null;
			base.OnDestroy ();
		}
	}
}

