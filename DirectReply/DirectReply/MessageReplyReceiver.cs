using Android.App;
using Android.Content;
using Android.Support.V4.App;
using Android.Widget;
using RemoteInput = Android.Support.V4.App.RemoteInput;

namespace DirectReply
{
	[BroadcastReceiver(Enabled = true)]
	[Android.App.IntentFilter(new[] { MainActivity.REPLY_ACTION })]
	/// <summary>
	/// A receiver that gets called when a reply is sent to a given conversationId
	/// </summary>
	public class MessageReplyReceiver : BroadcastReceiver
	{
		public override void OnReceive(Context context, Intent intent)
		{
			if (!MainActivity.REPLY_ACTION.Equals(intent.Action))
				return;
			

			var requestId = intent.GetIntExtra(MainActivity.REQUEST_CODE_KEY, -1);
			if (requestId == -1)
				return;

			var reply = GetMessageText(intent);
			using (var notificationManager = NotificationManagerCompat.From(context))
			{
				// Create new notification to display, or re-build existing conversation to update with new response
				var notificationBuilder = new NotificationCompat.Builder(context);
				notificationBuilder.SetSmallIcon(Resource.Drawable.reply);
				notificationBuilder.SetContentText(Application.Context.GetString(Resource.String.replied));
				var repliedNotification = notificationBuilder.Build();


				// Call notify to stop progress spinner. 
				notificationManager.Notify(requestId, repliedNotification);
			}

			Toast.MakeText(context, $"Message sent: {reply}", ToastLength.Long).Show();
		}

		/// <summary>
		/// Get the message text from the intent.
		/// Note that you should call <see cref="Android.Support.V4.App.RemoteInput.GetResultsFromIntent(intent)"/> 
		/// to process the RemoteInput.
		/// </summary>
		/// <returns>The message text.</returns>
		/// <param name="intent">Intent.</param>
		static string GetMessageText(Intent intent)
		{
			var remoteInput = RemoteInput.GetResultsFromIntent(intent);
			return remoteInput != null ? remoteInput.GetCharSequence(MainActivity.KEY_TEXT_REPLY) : string.Empty;
		}
	}
}
