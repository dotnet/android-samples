
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using Android.App;
using Android.Content;
using Android.Database;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Net;
using Android.Provider;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;

using Uri = Android.Net.Uri;

namespace LNotifications
{
	/**
	 * Fragment that demonstrates how to attach metadata introduced in Android L, such as
	 * priority data, notification category and person data.
	 */
	public class OtherMetaDataFragment : Fragment
	{
		private string TAG = Java.Lang.Class.FromType(typeof(OtherMetaDataFragment)).SimpleName;

		// Request code used for picking a contact
		public const int REQUEST_CODE_PICK_CONTACT = 1;

		// Incremental int for notifications so that each is treated differently
		private int incrementalNotificationId = 0;

		private NotificationManager notificationManager;

		// Button to show a notification
		private Button showNotificationButton;

		// Spinner that holds possible categories for a notification
		private Spinner categorySpinner;

		// Spinner that holds possible priorities for a notification
		private Spinner prioritySpinner;

		// Holds a URI for the person to be attached to the notification
		Uri contactUri;


		public override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);
			notificationManager = (NotificationManager)Activity.GetSystemService (Context.NotificationService);
		}

		public static OtherMetaDataFragment NewInstance()
		{
			var fragment = new OtherMetaDataFragment ();
			fragment.RetainInstance = true;
			return fragment;
		}

		public OtherMetaDataFragment()
		{
		}

		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			// Inflate the layout for this fragment
			return inflater.Inflate (Resource.Layout.fragment_other_metadata, container, false);
		}

		public override void OnViewCreated (View view, Bundle savedInstanceState)
		{
			base.OnViewCreated (view, savedInstanceState);
			showNotificationButton = (Button)view.FindViewById (Resource.Id.show_notification_button);
			showNotificationButton.Click += delegate {
				Priority selectedPriority = Priority.FromString(prioritySpinner.SelectedItem.ToString());
				Category selectedCategory = Category.FromString(categorySpinner.SelectedItem.ToString());
				ShowNotificationClicked(selectedPriority,selectedCategory,contactUri);

			};
			categorySpinner = (Spinner)view.FindViewById (Resource.Id.category_spinner);
			ArrayAdapter<Category> categoryArrayAdapter = new ArrayAdapter<Category> (Activity, Android.Resource.Layout.SimpleSpinnerItem,Category.Values);
			categoryArrayAdapter.SetDropDownViewResource (Android.Resource.Layout.SimpleSpinnerDropDownItem);
			categorySpinner.Adapter = categoryArrayAdapter;

			prioritySpinner = (Spinner)view.FindViewById (Resource.Id.priority_spinner);
			ArrayAdapter<Priority> priorityArrayAdapter = new ArrayAdapter<Priority> (Activity, Android.Resource.Layout.SimpleSpinnerItem,Priority.Values);
			priorityArrayAdapter.SetDropDownViewResource (Android.Resource.Layout.SimpleSpinnerDropDownItem);
			prioritySpinner.Adapter = priorityArrayAdapter;

			view.FindViewById (Resource.Id.attach_person).Click += delegate {
				FindContact ();
			};
			view.FindViewById (Resource.Id.contact_entry).Visibility = ViewStates.Gone;

		}

		public override void OnActivityResult (int requestCode, Result resultCode, Intent data)
		{
			base.OnActivityResult (requestCode, resultCode, data);
			switch (requestCode) {
			case REQUEST_CODE_PICK_CONTACT:
				if (resultCode == Result.Ok) {
					Uri contactUri = data.Data;
					this.contactUri = contactUri;
					UpdateContactEntryFromUri (contactUri);
				}
				break;
			}
		}

		/*
		 * Invoked when showNotificationButton is clicked.
		 * Creates a new notification and sets metadata passed as arguments.
		 */
		public Notification CreateNotification(Priority priority, Category category, Uri contactUri)
		{
			var builder = new Notification.Builder (Activity)
				.SetContentTitle ("Notification with other metadata")
				.SetSmallIcon (Resource.Drawable.ic_launcher_notification)
				.SetPriority (priority.priority)
				.SetCategory (category.ToString ())
				.SetContentText(string.Format("Category {0}, Priority {1}",category.ToString(),priority.ToString()));
			if (contactUri != null) {
				builder.AddPerson (contactUri.ToString ());
				Bitmap photoBitmap = LoadBitmapFromContactUri (contactUri);
				if (photoBitmap != null)
					builder.SetLargeIcon (photoBitmap);
			}
			return builder.Build ();
		}

		public void ShowNotificationClicked(Priority p, Category c, Uri u)
		{
			incrementalNotificationId++;
			notificationManager.Notify (incrementalNotificationId, CreateNotification (p, c, u));
			Toast.MakeText (Activity, "Show Notification clicked", ToastLength.Short).Show ();
		}


		public void FindContact()
		{
			var intent = new Intent (Intent.ActionPick, ContactsContract.Contacts.ContentUri);
			StartActivityForResult (intent, REQUEST_CODE_PICK_CONTACT);
		}

		public Bitmap LoadBitmapFromContactUri(Uri contactUri)
		{
			if(contactUri==null)
				return null;

			Bitmap result = null;
			ICursor cursor = Activity.ContentResolver.Query (contactUri, null, null, null, null);
			if (cursor != null && cursor.MoveToFirst ()) {
				int idx = cursor.GetColumnIndex (ContactsContract.ContactsColumns.PhotoId);
				string hasPhoto = cursor.GetString (idx);
				Uri photoUri = Uri.WithAppendedPath (contactUri, ContactsContract.Contacts.Photo.ContentDirectory);
				if (hasPhoto != null) {
					try {
						result = MediaStore.Images.Media.GetBitmap (Activity.ContentResolver, photoUri);
					} catch (IOException e) {
						Log.Error (TAG, string.Format ("Failed to load resource. Uri {0}", photoUri), e);
					}
				} else {
					Drawable defaultContactDrawable = Activity.Resources.GetDrawable (Resource.Drawable.ic_contact_picture);
					result = ((BitmapDrawable)defaultContactDrawable).Bitmap;
				}
			}
			return result;

		}

		// Updates the contact information on the screen when a contact is picked.
		public void UpdateContactEntryFromUri(Uri uri)
		{
			ICursor cursor = Activity.ContentResolver.Query (contactUri, null, null, null, null);
			if (cursor != null && cursor.MoveToFirst ()) {
				int idx = cursor.GetColumnIndex (ContactsContract.ContactsColumns.DisplayName);
				string name = cursor.GetString (idx);
				idx = cursor.GetColumnIndex (ContactsContract.ContactsColumns.PhotoId);
				string hasPhoto = cursor.GetString (idx);

				Uri photoUri = Uri.WithAppendedPath (contactUri, ContactsContract.Contacts.Photo.ContentDirectory);
				ImageView contactPhoto = (ImageView)Activity.FindViewById (Resource.Id.contact_photo);
				if (hasPhoto != null)
					contactPhoto.SetImageURI (photoUri);
				else {
					Drawable defaultContactDrawable = Activity.Resources.GetDrawable (Resource.Drawable.ic_contact_picture);
					contactPhoto.SetImageDrawable (defaultContactDrawable);
				}
				TextView contactName = (TextView)Activity.FindViewById (Resource.Id.contact_name);
				contactName.SetText (name, TextView.BufferType.Normal);

				Activity.FindViewById (Resource.Id.contact_entry).Visibility = ViewStates.Visible;
				Activity.FindViewById (Resource.Id.attach_person).Visibility = ViewStates.Gone;
				Activity.FindViewById (Resource.Id.click_to_change).Click += delegate {
					FindContact ();
				};
				Log.Info(Tag,string.Format("Contact updated. Name {0}, PhotoUri {1}",name,photoUri));
			}
		}

		// A class for possible categories in notifications
		public sealed class Category : Java.Lang.Object
		{
			private string name;
			private static List<Category> values;

			public static readonly Category ALARM = new Category("Alarm");
			public static readonly Category CALL = new Category("Call");
			public static readonly Category EMAIL = new Category("Email");
			public static readonly Category ERROR = new Category("Error");
			public static readonly Category EVENT = new Category("Event");
			public static readonly Category MESSAGE = new Category("Message");
			public static readonly Category PROGRESS = new Category("Progress");
			public static readonly Category PROMO = new Category("Promo");
			public static readonly Category RECOMMENDATION = new Category("Recommendation");
			public static readonly Category SERVICE = new Category("Service");
			public static readonly Category SOCIAL = new Category("Social");
			public static readonly Category STATUS = new Category("Status");
			public static readonly Category SYSTEM = new Category("System");
			public static readonly Category TRANSPORT = new Category("Transport");
			public static List<Category> Values
			{
				get{
					if (values == null) {
						values = new List<Category> ();
						values.Add (ALARM);
						values.Add (CALL);
						values.Add (EMAIL);
						values.Add (ERROR);
						values.Add (EVENT);
						values.Add (MESSAGE);
						values.Add (PROGRESS);
						values.Add (PROMO);
						values.Add (RECOMMENDATION);
						values.Add (SERVICE);
						values.Add (SOCIAL);
						values.Add (STATUS);
						values.Add (SYSTEM);
						values.Add (TRANSPORT);
					}
					return values;
				}
			}
			private Category(string name)
			{
				this.name = name;
			}

			public override string ToString ()
			{
				return name;
			}

			public static Category FromString(string s)
			{
				switch (s) {
				case "Alarm":
					return ALARM;
				case "Call":
					return CALL;
				case "Email":
					return EMAIL;
				case "Error":
					return ERROR;
				case "Event":
					return EVENT;
				case "Message":
					return MESSAGE;
				case "Progress":
					return PROGRESS;
				case "Promo":
					return PROMO;
				case "Recommendation":
					return RECOMMENDATION;
				case "Service":
					return SERVICE;
				case "Social":
					return SOCIAL;
				case "Status":
					return STATUS;
				case "System":
					return SYSTEM;
				case "Transport":
					return TRANSPORT;
				default:
					return null;
				}

			}
		}

		// A class for possible priorities in notifications
		public sealed class Priority : Java.Lang.Object
		{
			private static List<Priority> values;
			public NotificationPriority priority;

			public static Priority DEFAULT = new Priority(NotificationPriority.Default);
			public static Priority MAX = new Priority(NotificationPriority.Max);
			public static Priority HIGH = new Priority(NotificationPriority.High);
			public static Priority LOW = new Priority(NotificationPriority.Low);
			public static Priority MIN = new Priority(NotificationPriority.Min);
			private Priority(NotificationPriority p)
			{
				priority = p;
			}

			public override string ToString ()
			{
				return priority.ToString ();
			}
			public static List<Priority> Values
			{
				get {
					if (values == null) {
						values = new List<Priority> ();
						values.Add (DEFAULT);
						values.Add (MAX);
						values.Add (HIGH);
						values.Add (LOW);
						values.Add (MIN);
					}
					return values;
				}
			}

			public static Priority FromString(string s)
			{
				switch (s) {
				case "Default":
					return DEFAULT;
				case "Max":
					return MAX;
				case "High":
					return HIGH;
				case "Low":
					return LOW;
				case "Min":
					return MIN;
				default:
					return null;
				}
			}
		}
	}
}

