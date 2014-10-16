
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

namespace LNotifications
{
	// Fragment that demonstrates how notifications with different visibility metadata differ on a lockscreen.
	public class VisibilityMetaDataFragment : Fragment
	{
		private NotificationManager notificationManager;
		private RadioGroup radioGroup;
		int incrementalNotificationId = 0;
		private Button showNotificationButton;

		public override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);
			notificationManager = (NotificationManager)Activity.GetSystemService (Context.NotificationService);
		}

		public static VisibilityMetaDataFragment NewInstance()
		{
			var fragment = new VisibilityMetaDataFragment ();
			fragment.RetainInstance = true;
			return fragment;
		}

		public VisibilityMetaDataFragment()
		{
		}

		public override void OnViewCreated (View view, Bundle savedInstanceState)
		{
			base.OnViewCreated (view, savedInstanceState);
			showNotificationButton = (Button)View.FindViewById (Resource.Id.show_notification_button);
			showNotificationButton.Click += delegate {
				var visibility = GetVisibilityFromSelectedRadio(radioGroup);
				ShowNotificationClicked(visibility);
			};
			radioGroup = (RadioGroup)View.FindViewById (Resource.Id.visibility_radio_group);
		}

		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			// Inflate the layout for this fragment
			return inflater.Inflate (Resource.Layout.fragment_visibility_metadata_notification, container, false);
		}

		// Creates a new notification with a different visibility level.
		public Notification CreateNotification(NotificationVisibility visibility)
		{
			var builder = new Notification.Builder (Activity)
				.SetContentTitle ("Notification for Visibility metadata");

			builder.SetVisibility (visibility);
			builder.SetContentText (string.Format ("Visibility : {0}", 
				NotificationVisibilities.GetDescription(visibility)));
			builder.SetSmallIcon (NotificationVisibilities.GetNotificationIconId (visibility));
			return builder.Build ();
		}

		// Returns a NotificationVisibility depending on which RadioButton is selected.
		public NotificationVisibility GetVisibilityFromSelectedRadio(RadioGroup r)
		{
			switch (radioGroup.CheckedRadioButtonId) {
			case Resource.Id.visibility_public_radio_button:
				return NotificationVisibilities.Public;
			case Resource.Id.visibility_private_radio_button:
				return NotificationVisibilities.Private;
			case Resource.Id.visibility_secret_radio_button:
				return NotificationVisibilities.Secret;
			default:
				return NotificationVisibilities.Public;
			}

		}

		public void ShowNotificationClicked(NotificationVisibility v)
		{
			incrementalNotificationId++;
			notificationManager.Notify (incrementalNotificationId, CreateNotification (v));
			Toast.MakeText (Activity, "Show Notification clicked", ToastLength.Short).Show ();
		}

		// A class indicating possible visibility levels for notifications.
		public class NotificationVisibilities
		{
			public static NotificationVisibility Public = NotificationVisibility.Public;
			public static NotificationVisibility Private = NotificationVisibility.Private;
			public static NotificationVisibility Secret = NotificationVisibility.Secret;

			public static int GetVisibility(NotificationVisibility val)
			{
				return (int)val;
			}

			public static string GetDescription(NotificationVisibility val)
			{
				return val.ToString();
			}

			public static int GetNotificationIconId(NotificationVisibility val)
			{
				switch (val) {
				case NotificationVisibility.Public:
					return Resource.Drawable.ic_public_notification;
				case NotificationVisibility.Private:
					return Resource.Drawable.ic_private_notification;
				case NotificationVisibility.Secret:
					return Resource.Drawable.ic_secret_notification;
				default:
					return Resource.Drawable.ic_public_notification;
				}

			}

		}



	
	}
}

