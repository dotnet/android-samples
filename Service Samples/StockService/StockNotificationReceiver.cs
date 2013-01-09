using System;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Util;

namespace StockService
{
	[BroadcastReceiver]
	[IntentFilter(new string[]{StockService.StocksUpdatedAction}, Priority = (int)IntentFilterPriority.LowPriority)]
	public class StockNotificationReceiver : BroadcastReceiver
	{
		public StockNotificationReceiver ()
		{
		}

		public override void OnReceive (Context context, Intent intent)
		{
			var nMgr = (NotificationManager)context.GetSystemService (Context.NotificationService);
			var notification = new Notification (Resource.Drawable.icon, "New stock data is available");
			var pendingIntent = PendingIntent.GetActivity (context, 0, new Intent (context, typeof(StockActivity)), 0);
			notification.SetLatestEventInfo (context, "Stocks Updated", "New stock data is available", pendingIntent);
			nMgr.Notify (0, notification);
		}
	}
}

