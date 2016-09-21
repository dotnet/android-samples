using Android.App;
using Android.Widget;
using Android.OS;
using Android.Content;
using Android.Support.V4.App;
using Android.Graphics;

namespace DirectReply
{
	[Activity(Label = "DirectReply", MainLauncher = true, Icon = "@mipmap/icon")]
	public class MainActivity : Activity
	{
		int requestCode = 0;

		public const string REPLY_ACTION = "com.xamarin.directreply.REPLY";
		public const string KEY_TEXT_REPLY = "key_text_reply";
		public const string REQUEST_CODE_KEY = "request_code";

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			// Set our view from the "main" layout resource
			SetContentView(Resource.Layout.Main);

			// Get our button from the layout resource,
			// and attach an event to it
			var button = FindViewById<Button>(Resource.Id.myButton);

			// Set button listener
			button.Click += (sender, args) =>
			{
				OnButtonClick();
			};
		}

		private void OnButtonClick()
		{
			requestCode++;

			// Build PendingIntent
			var pendingIntent = MakePendingIntent();

			var replyText = GetString(Resource.String.reply_text);

			// Create remote input that will read text
			var remoteInput = new Android.Support.V4.App.RemoteInput.Builder(KEY_TEXT_REPLY)
										 .SetLabel(replyText)
										 .Build();

			// Build action for noticiation
			var action = new NotificationCompat.Action.Builder(Resource.Drawable.action_reply, replyText, pendingIntent)
											   .AddRemoteInput(remoteInput)
											   .Build();

			// Build notification
			var notification = new NotificationCompat.Builder(this)
													 .SetSmallIcon(Resource.Drawable.reply)
													 .SetLargeIcon(BitmapFactory.DecodeResource(Resources, Resource.Drawable.avatar))
													 .SetContentText("Hey, it is James! What's up?")
													 .SetContentTitle(GetString(Resource.String.message))
													 .SetAutoCancel(true)
													 .AddAction(action)
													 .Build();

			// Notify
			using (var notificationManager = NotificationManagerCompat.From(this))
			{
				notificationManager.Notify(requestCode, notification);
			}
		}

		private PendingIntent MakePendingIntent()
		{
			PendingIntent pendingIntent = null;

			if ((int)Build.VERSION.SdkInt >= (int)BuildVersionCodes.N)
			{
				// >= Android N
				var intent = new Intent(REPLY_ACTION)
					.AddFlags(ActivityFlags.IncludeStoppedPackages)
					.SetAction(REPLY_ACTION)
					.PutExtra(REQUEST_CODE_KEY, requestCode);

				pendingIntent = PendingIntent.GetBroadcast(this,
														   requestCode,
														   intent,
														   PendingIntentFlags.UpdateCurrent);
			}
			else
			{
				// < Android N
				var intent = new Intent(this, typeof(MainActivity));
				intent.AddFlags(ActivityFlags.ClearTop | ActivityFlags.NewTask);

				pendingIntent = PendingIntent.GetActivity(this,
														  requestCode,
														  intent,
														  PendingIntentFlags.UpdateCurrent);
			}

			return pendingIntent;
		}
	}
}

