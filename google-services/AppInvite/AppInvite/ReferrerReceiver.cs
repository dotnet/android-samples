
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Gms.AppInvite;
using Android;
using Android.Support.V4.Content;

namespace AppInvite
{	
	[BroadcastReceiver (Exported = true)]
	[IntentFilter(new [] {"com.android.vending.INSTALL_REFERRER"})]
	public class ReferrerReceiver : BroadcastReceiver
	{
		public override void OnReceive (Context context, Intent intent)
		{
			var deepLinkIntent = AppInviteReferral.AddPlayStoreReferrerToIntent (intent, new Intent (context.GetString (Resource.String.action_deep_link)));
			LocalBroadcastManager.GetInstance (context).SendBroadcast (deepLinkIntent);
		}
	}
}

