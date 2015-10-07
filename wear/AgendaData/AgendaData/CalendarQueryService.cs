using System;
using Android.App;
using Android.Views;
using Android.OS;
using Android.Gms.Common.Apis;
using Android.Provider;
using Android.Content;
using Java.Util.Concurrent;
using Android.Text.Format;
using System.Collections.Generic;
using Android.Net;
using Android.Database;
using Android.Graphics;
using Java.IO;
using Android.Util;
using System.Threading.Tasks;
using System.IO;
using Android.Gms.Wearable;
using Android.Content.Res;

namespace AgendaData
{
	[Service()]
	public class CalendarQueryService : IntentService, GoogleApiClient.IConnectionCallbacks, GoogleApiClient.IOnConnectionFailedListener
	{

		private static readonly String[] INSTANCE_PROJECTION = 
		{
			BaseColumns.Id,
			CalendarContract.Instances.EventId,
			CalendarContract.Instances.InterfaceConsts.Title,
			CalendarContract.Instances.Begin,
			CalendarContract.Instances.End,
			CalendarContract.Instances.InterfaceConsts.AllDay,
			CalendarContract.Instances.InterfaceConsts.Description,
			CalendarContract.Instances.InterfaceConsts.Organizer
		};

		private static readonly String[] CONTACT_PROJECTION = new String[] { ContactsContract.Data.InterfaceConsts.Id, ContactsContract.Data.InterfaceConsts.ContactId };
		private const String CONTACT_SELECTION = ContactsContract.CommonDataKinds.Email.Address + " = ?";

		private GoogleApiClient mGoogleApiClient;

		public CalendarQueryService ()
			:base(typeof(CalendarQueryService).Name)
		{

		}

		public override void OnCreate ()
		{
			base.OnCreate ();
			mGoogleApiClient = new GoogleApiClient.Builder (this)
				.AddApi (WearableClass.API)
				.AddConnectionCallbacks (this)
				.AddOnConnectionFailedListener (this)
				.Build();
		}

		protected override async void OnHandleIntent (Intent intent)
		{
			mGoogleApiClient.BlockingConnect (Constants.CONNECTION_TIME_OUT_MS, TimeUnit.Milliseconds);
			// Query calendar events in the next 24 hours
			Time time = new Time ();
			time.SetToNow ();
			long beginTime = time.ToMillis (true);
			time.MonthDay++;
			time.Normalize (true);
			long endTime = time.Normalize (true);

			List<Event> events = QueryEvents(this, beginTime, endTime);

			foreach (Event ev in events) {
				PutDataMapRequest putDataMapRequest = ev.ToPutDataMapRequest ();
				if (mGoogleApiClient.IsConnected) {
					await WearableClass.DataApi.PutDataItemAsync (mGoogleApiClient,
                        putDataMapRequest.AsPutDataRequest ());
				} else {
					Log.Error (Constants.TAG, "Failed to send data item: " + putDataMapRequest
					+ " - Client disconnected from Google Play Services");
				}
			}
			mGoogleApiClient.Disconnect ();
		}

		private static String MakeDataItemPath(long eventId, long beginTime)
		{
			return Constants.CAL_DATA_ITEM_PATH_PREFIX + eventId + "/" + beginTime;
		}

		private static List<Event> QueryEvents(Context context, long beginTime, long endTime)
		{
			ContentResolver contentResolver = context.ContentResolver;
			Android.Net.Uri.Builder builder = CalendarContract.Instances.ContentUri.BuildUpon ();
			ContentUris.AppendId (builder, beginTime);
			ContentUris.AppendId (builder, endTime);
			ICursor cursor = contentResolver.Query (builder.Build (), INSTANCE_PROJECTION,
				null /* selection */, null /* selectionArgs */, null /* sortOrder */);
			try 
			{
				int idIdx = cursor.GetColumnIndex(CalendarContract.Instances.InterfaceConsts.Id);
				int eventIdIdx = cursor.GetColumnIndex(CalendarContract.Instances.EventId);
				int titleIdx = cursor.GetColumnIndex(CalendarContract.Instances.InterfaceConsts.Title);
				int beginIdx = cursor.GetColumnIndex(CalendarContract.Instances.Begin);
				int endIdx = cursor.GetColumnIndex(CalendarContract.Instances.End);
				int allDayIdx = cursor.GetColumnIndex(CalendarContract.Instances.InterfaceConsts.AllDay);
				int descIdx = cursor.GetColumnIndex(CalendarContract.Instances.InterfaceConsts.Description);
				int ownerEmailIdx = cursor.GetColumnIndex(CalendarContract.Instances.InterfaceConsts.Organizer);

				List<Event> events = new List<Event>(cursor.Count);
				while (cursor.MoveToNext())
				{
					Event cEvent = new Event();
					cEvent.Id = cursor.GetLong(idIdx);
					cEvent.EventId = cursor.GetLong(eventIdIdx);
					cEvent.Title = cursor.GetString(titleIdx);
					cEvent.Begin = cursor.GetLong(beginIdx);
					cEvent.End = cursor.GetLong(endIdx);
					cEvent.AllDay = cursor.GetInt(allDayIdx) != 0;
					cEvent.Description = cursor.GetString(descIdx);
					string ownerEmail = cursor.GetString(ownerEmailIdx);
					ICursor contactCursor = contentResolver.Query(ContactsContract.Data.ContentUri, CONTACT_PROJECTION,
						CONTACT_SELECTION, new String[] { ownerEmail }, null);
					int ownerIdIdx = contactCursor.GetColumnIndex(ContactsContract.Data.InterfaceConsts.ContactId);
					long ownerId = -1;
					if (contactCursor.MoveToFirst())
					{
						ownerId = contactCursor.GetLong(ownerIdIdx);
					}
					contactCursor.Close();
					// Use event organizer's profile picture as the notification background
					cEvent.OwnerProfilePic = GetProfilePicture(contentResolver, context, ownerId);
					events.Add(cEvent);
				}
				return events;
			}
			finally {
				cursor.Close ();
			}
			return null;
		}

		public void OnConnected (Bundle p0)
		{
			Log.Verbose (Constants.TAG, "CalendarQueryService client connected.");
		}

		public void OnConnectionSuspended (int p0)
		{
			Log.Verbose (Constants.TAG, "CalendarQueryService client connection suspended.");
		}

		public void OnConnectionFailed (Android.Gms.Common.ConnectionResult p0)
		{
			Log.Error (Constants.TAG, "CalendarQueryService client connection failed.");
		}

		public static Asset GetDefaultProfile(Resources res) {
			Bitmap bitmap = BitmapFactory.DecodeResource (res, Resource.Drawable.nobody);
			return Asset.CreateFromBytes (ToByteArray (bitmap));
		}

		public static Asset GetProfilePicture(ContentResolver contentResolver, Context context, long contactId)
		{
			try 
			{
				if (contactId != -1) {
					// Try to retrieve the profile picture for the givent contact.
					var contactUri = ContentUris.WithAppendedId (Contacts.ContentUri, contactId);
					Stream inputStream = ContactsContract.Contacts.OpenContactPhotoInputStream (contentResolver, contactUri, true);

					if (inputStream != null) {
						try 
						{
							Bitmap bitmap = BitmapFactory.DecodeStream(inputStream);
							if (bitmap != null) {
								return Asset.CreateFromBytes(ToByteArray(bitmap));
							} else {
								Log.Error(Constants.TAG, "Cannot decode profile picture for contact " + contactId);
							}
						}
						finally {
							CloseQuietly (inputStream);
						}
					}
				}
			}
			catch {

			}
			// Use a default background image if the user has no profile picture or there was an error.
			return GetDefaultProfile (context.Resources);
		}

		private static byte[] ToByteArray (Bitmap bitmap)
		{
			using (MemoryStream stream = new MemoryStream ()) {
			bitmap.Compress (Bitmap.CompressFormat.Png, 100, stream);
				byte[] byteArray = new byte[stream.Length];
				stream.Read (byteArray, 0, byteArray.Length);
				return byteArray;
			}
		}

		private static void CloseQuietly (IDisposable closeable)
		{
			try 
			{
				closeable.Dispose();
			}
			catch (Exception ex) {
				Log.Error (Constants.TAG, "EXception while disposing disposable", ex);
			}
		}

		private class Event 
		{
			public long Id;
			public long EventId;
			public String Title;
			public long Begin;
			public long End;
			public bool AllDay;
			public String Description;
			public Asset OwnerProfilePic;

			public PutDataMapRequest ToPutDataMapRequest () {
				PutDataMapRequest putDataMapRequest = PutDataMapRequest.Create(
					MakeDataItemPath(EventId, Begin));
				DataMap data = putDataMapRequest.DataMap;
				data.PutString(Constants.DATA_ITEM_URI, putDataMapRequest.Uri.ToString());
				data.PutLong(Constants.ID, Id);
				data.PutLong(Constants.EVENT_ID, EventId);
				data.PutString(Constants.TITLE, Title);
				data.PutLong(Constants.BEGIN, Begin);
				data.PutLong(Constants.END, End);
				data.PutBoolean(Constants.ALL_DAY, AllDay);
				data.PutString(Constants.DESCRIPTION, Description);
				data.PutAsset(Constants.PROFILE_PIC, OwnerProfilePic);
				return putDataMapRequest;
			}
		}
	}
}

