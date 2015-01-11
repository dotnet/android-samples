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
using Android.Text.Format;
using Android.Util;

namespace MonoIO
{
	[Service(Name = "monoio.SyncService")]
	public class SyncService : IntentService
	{
		private static String TAG = "SyncService";
		public static String EXTRA_STATUS_RECEIVER = "monoio.extra.STATUS_RECEIVER";
		public const Result StatusRunning = (Result)0x1;
		public const Result StatusError = (Result)0x2;
		public const Result StatusFinished = (Result)0x3;
		private static int SECOND_IN_MILLIS = (int)DateUtils.SecondInMillis;
	
		/** Root worksheet feed for online data source */
		// TODO: insert your sessions/speakers/vendors spreadsheet doc URL here.
		private static String WORKSHEETS_URL = "INSERT_SPREADSHEET_URL_HERE";
		private static String HEADER_ACCEPT_ENCODING = "Accept-Encoding";
		private static String ENCODING_GZIP = "gzip";
		private static int VERSION_NONE = 0;
		private static int VERSION_CURRENT = 11;
		private LocalExecutor mLocalExecutor;
		// MonoIO TODO: Get remote data working.
		//private RemoteExecutor mRemoteExecutor;

		// We need an empty constructor, so let's not call base(String)...
		public SyncService () // : base(TAG)
		{
		}
		
		public override void OnCreate ()
		{
			base.OnCreate ();
			
			//HttpClient httpClient = getHttpClient(this);
			ContentResolver resolver = ContentResolver;
	
			mLocalExecutor = new LocalExecutor (Resources, resolver);
			//mRemoteExecutor = new RemoteExecutor(httpClient, resolver);
		}
		
		protected override void OnHandleIntent (Intent intent)
		{
			Log.Debug (TAG, "onHandleIntent(intent=" + intent.ToString () + ")");

			ResultReceiver receiver = (ResultReceiver)intent.GetParcelableExtra (EXTRA_STATUS_RECEIVER);
			if (receiver != null)
				receiver.Send (StatusRunning, Bundle.Empty);
	
			Context context = this;
			var prefs = GetSharedPreferences (Prefs.IOSCHED_SYNC, FileCreationMode.Private);
			int localVersion = prefs.GetInt (Prefs.LOCAL_VERSION, VERSION_NONE);
	
			try {
				// Bulk of sync work, performed by executing several fetches from
				// local and online sources.
	
				long startLocal = Java.Lang.JavaSystem.CurrentTimeMillis ();
				bool localParse = localVersion < VERSION_CURRENT;
				Log.Debug (TAG, "found localVersion=" + localVersion + " and VERSION_CURRENT=" + VERSION_CURRENT);
				if (localParse) {
					// Load static local data
					mLocalExecutor.Execute (Resource.Xml.blocks, new LocalBlocksHandler ());
					mLocalExecutor.Execute (Resource.Xml.rooms, new LocalRoomsHandler ());
					mLocalExecutor.Execute (Resource.Xml.tracks, new LocalTracksHandler ());
					mLocalExecutor.Execute (Resource.Xml.search_suggest, new LocalSearchSuggestHandler ());
					mLocalExecutor.Execute (Resource.Xml.sessions, new LocalSessionsHandler ());
	
					// Parse values from local cache first, since spreadsheet copy
					// or network might be down.
//	                mLocalExecutor.execute(context, "cache-sessions.xml", new RemoteSessionsHandler());
//	                mLocalExecutor.execute(context, "cache-speakers.xml", new RemoteSpeakersHandler());
//	                mLocalExecutor.execute(context, "cache-vendors.xml", new RemoteVendorsHandler());
	
					// Save local parsed version
					prefs.Edit ().PutInt (Prefs.LOCAL_VERSION, VERSION_CURRENT).Commit ();
				}
	            
				Log.Debug (TAG, "local sync took " + (Java.Lang.JavaSystem.CurrentTimeMillis () - startLocal) + "ms");
	
				// Always hit remote spreadsheet for any updates
				long startRemote = Java.Lang.JavaSystem.CurrentTimeMillis ();
//		        mRemoteExecutor.executeGet(WORKSHEETS_URL, new RemoteWorksheetsHandler(mRemoteExecutor));
				Log.Debug (TAG, "remote sync took " + (Java.Lang.JavaSystem.CurrentTimeMillis () - startRemote) + "ms");
			
			} catch (Exception e) {
				Log.Error (TAG, "Problem while syncing", e);
		
				if (receiver != null) {
					// Pass back error to surface listener
					Bundle bundle = new Bundle ();
					bundle.PutString (Intent.ExtraText, e.ToString ());
					receiver.Send (StatusError, bundle);
				}        
			}

			// Announce success to any surface listener
			Log.Debug (TAG, "sync finished");
			if (receiver != null)
				receiver.Send (StatusFinished, Bundle.Empty);
		}
			
		public class Prefs
		{
			public static String IOSCHED_SYNC = "iosched_sync";
			public static String LOCAL_VERSION = "local_version";
		}
	}
}

