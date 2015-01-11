using System;

using Android.App;
using Android.Content;
//using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Gms.Common.Apis;
using Android.Graphics;
using Java.Util.Concurrent;
using Android.Gms.Wearable;
using System.Collections.Generic;
using Android.Gms.Common.Data;
using Java.Interop;
using Android.Provider;
using Java.IO;
using System.IO;
using Android.Util;
using Android.Content.PM;

namespace DataLayer
{
	/// <summary>
	/// Receives its own events using a listener API designed for foreground activities. Updates a data item every second while it is open.
	/// Also allows user to take a photo and send that as an asset to the paired wearable.
	/// </summary>
	[Activity (MainLauncher = true, Label="@string/app_name", LaunchMode = LaunchMode.SingleTask, Icon = "@drawable/ic_launcher")]
	public class MainActivity : Activity, IDataApiDataListener, IMessageApiMessageListener, INodeApiNodeListener, 
	IGoogleApiClientConnectionCallbacks, IGoogleApiClientOnConnectionFailedListener
	{
		const string Tag = "MainActivity";

		/// <summary>
		/// Request code for launching the Intent to resolve Google Play services errors
		/// </summary>
		const int RequestResolveError = 1000;

		const string StartActivityPath = "/start-activity";
		const string CountPath = "/count";
		const string ImagePath = "/image";
		const string ImageKey = "photo";
		const string CountKey = "count";

		IGoogleApiClient mGoogleApiClient;
		bool mResolvingError = false;
		bool mCameraSupported = false;

		ListView dataItemList;
		Button takePhotoBtn;
		Button sendPhotoBtn;
		ImageView thumbView;
		Bitmap imageBitmap;
		View startActivityBtn;

		private DataItemAdapter dataItemListAdapter;
		private Handler handler;

		// Send DataItems
		private IScheduledExecutorService generatorExecutor;
		private IScheduledFuture dataItemGeneratorFuture;

		const int RequestImageCapture = 1;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			handler = new Handler ();
			LOGD (Tag, "OnCreate");
			mCameraSupported = PackageManager.HasSystemFeature (PackageManager.FeatureCamera);
			SetContentView (Resource.Layout.main_activity);
			SetupViews ();

			// Stores DataItems received by the local broadcaster of from the paired watch
			dataItemListAdapter = new DataItemAdapter (this, Android.Resource.Layout.SimpleListItem1);
			dataItemList.Adapter = dataItemListAdapter;

			generatorExecutor = new ScheduledThreadPoolExecutor (1);

			mGoogleApiClient = new GoogleApiClientBuilder (this)
				.AddApi (WearableClass.Api)
				.AddConnectionCallbacks (this)
				.AddOnConnectionFailedListener (this)
				.Build ();
		}

		protected override void OnActivityResult (int requestCode, Result resultCode, Intent data)
		{
			if (requestCode == RequestImageCapture && resultCode == Result.Ok) {
				Bundle extras = data.Extras;
				imageBitmap = (Bitmap)extras.Get ("data");
				thumbView.SetImageBitmap (imageBitmap);
			}
		}

		protected override void OnStart ()
		{
			base.OnStart ();
			if (!mResolvingError) {
				mGoogleApiClient.Connect ();
			}
		}

		protected override void OnResume ()
		{
			base.OnResume ();
			dataItemGeneratorFuture = generatorExecutor.ScheduleWithFixedDelay (new DataItemGenerator () { Activity = this }, 1, 5, TimeUnit.Seconds);
		}

		protected override void OnPause ()
		{
			base.OnPause ();
			dataItemGeneratorFuture.Cancel (true /* mayInterruptIfRunning */);
		}

		protected override void OnStop ()
		{
			if (!mResolvingError) {
				WearableClass.DataApi.RemoveListener (mGoogleApiClient, this);
				WearableClass.MessageApi.RemoveListener (mGoogleApiClient, this);
				WearableClass.NodeApi.RemoveListener (mGoogleApiClient, this);
				mGoogleApiClient.Disconnect ();
			}
			base.OnStop ();
		}

		public void OnConnected (Bundle connectionHint)
		{
			LOGD (Tag, "Google API CLient was connected");
			mResolvingError = false;
			startActivityBtn.Enabled = true;
			sendPhotoBtn.Enabled = true;
			WearableClass.DataApi.AddListener (mGoogleApiClient, this);
			WearableClass.MessageApi.AddListener (mGoogleApiClient, this);
			WearableClass.NodeApi.AddListener (mGoogleApiClient, this);
		}

		public void OnConnectionSuspended (int cause)
		{
			LOGD (Tag, "Connection to Google API client was suspended");
			startActivityBtn.Enabled = false;
			sendPhotoBtn.Enabled = false;
		}

		public void OnConnectionFailed (Android.Gms.Common.ConnectionResult result)
		{
			if (mResolvingError) {
				// Already attempting to resolve an error
				return;
			} else if (result.HasResolution) {
				try {
					mResolvingError = true;
					result.StartResolutionForResult(this, RequestResolveError);
				} catch (IntentSender.SendIntentException e) {
					// There was an error with the resolution intent. Try again.
					mGoogleApiClient.Connect ();
				} 
			} else {
				Log.Error(Tag, "Connection to Google API client has failed");
				mResolvingError = false;
				startActivityBtn.Enabled = false;
				sendPhotoBtn.Enabled = false;
				WearableClass.DataApi.RemoveListener(mGoogleApiClient, this);
				WearableClass.MessageApi.RemoveListener(mGoogleApiClient, this);
				WearableClass.NodeApi.RemoveListener(mGoogleApiClient, this);
			}
		}

		public void OnDataChanged (DataEventBuffer dataEvents)
		{
			LOGD (Tag, "OnDataChanged: " + dataEvents);
			var events = FreezableUtils.FreezeIterable (dataEvents);

			dataEvents.Close ();
			RunOnUiThread (() => {
				foreach (var e in events)
				{
					var ev = ((Java.Lang.Object)e).JavaCast<IDataEvent>();
					if (ev.Type == DataEvent.TypeChanged) {
						dataItemListAdapter.Add(
							new Event("DataItem Changed", ev.DataItem.ToString()));
					} else if (ev.Type == DataEvent.TypeDeleted) {
						dataItemListAdapter.Add(
							new Event("DataItem Deleted", ev.DataItem.ToString()));
					}
				}
			});
		}

		public void OnMessageReceived (IMessageEvent messageEvent)
		{
			LOGD (Tag, "OnMessageReceived() A message from the watch was received: " + messageEvent.RequestId + " " + messageEvent.Path);
			handler.Post( () => {
				dataItemListAdapter.Add(new Event("Message from watch", messageEvent.ToString()));
			});
		}

		public void OnPeerConnected (INode peer)
		{
			LOGD (Tag, "OnPeerConencted: " + peer);
			handler.Post (() => {
				dataItemListAdapter.Add(new Event("Connected", peer.ToString()));
			});
		}

		public void OnPeerDisconnected (INode peer)
		{
			LOGD (Tag, "OnPeerDisconnected: " + peer);
			handler.Post (() => {
				dataItemListAdapter.Add(new Event("Disconnected", peer.ToString()));
			});
		}

		/// <summary>
		/// A View Adapter for presenting the Event objects in a list
		/// </summary>
		private class DataItemAdapter : ArrayAdapter<Event> {
			private readonly Context mContext;

			public DataItemAdapter(Context context, int unusedResource) 
				:base(context, unusedResource) {
				mContext = context;
			}
			public override View GetView (int position, View convertView, ViewGroup parent)
			{
				ViewHolder holder;
				if (convertView == null) {
					holder = new ViewHolder ();
					LayoutInflater inflater = (LayoutInflater)mContext.GetSystemService (Context.LayoutInflaterService);
					convertView = inflater.Inflate (Android.Resource.Layout.TwoLineListItem, null);
					convertView.Tag = holder;
					holder.Text1 = (TextView)convertView.FindViewById (Android.Resource.Id.Text1);
					holder.Text2 = (TextView)convertView.FindViewById (Android.Resource.Id.Text2);
				} else {
					holder = (ViewHolder)convertView.Tag;
				}
				Event e = GetItem (position);
				holder.Text1.Text = e.Title;
				holder.Text2.Text = e.Text;
				return convertView;
			}
			private class ViewHolder : Java.Lang.Object {
				public TextView Text1;
				public TextView Text2;
			}
		}
		public class Event {
			public String Title, Text;

			public Event(String title, String text) {
				this.Title = title;
				this.Text = text;
			}
		}

		ICollection<string> Nodes {
			get {
				HashSet<string> results = new HashSet<string> ();
				INodeApiGetConnectedNodesResult nodes =
					WearableClass.NodeApi.GetConnectedNodes (mGoogleApiClient).Await ().JavaCast<INodeApiGetConnectedNodesResult>();

				foreach (var node in nodes.Nodes) {
					results.Add (node.Id);
				}
				return results;
			}
		}

		void SendStartActivityMessage(String node) {
			WearableClass.MessageApi.SendMessage (mGoogleApiClient, node, StartActivityPath, new byte[0]).SetResultCallback (
				new ResultCallback() {
					OnResultAction = (Java.Lang.Object result) => {
						var res = result.JavaCast<IMessageApiSendMessageResult>();
						if (!res.Status.IsSuccess)
						{
							Log.Error(Tag, "Failed to send message with status code: " + res.Status.StatusCode);
						}
					}
				}
			);
		}

		class StartWearableActivityTask : AsyncTask
		{
			public MainActivity Activity;
			protected override Java.Lang.Object DoInBackground (params Java.Lang.Object[] @params)
			{
				if (Activity != null) {
					var nodes = Activity.Nodes;
					foreach (var node in nodes) {
						Activity.SendStartActivityMessage (node);
					}
				}
				return null;
			}
		}

		/// <summary>
		/// Sends an RPC to start a fullscreen Activity on the wearable
		/// </summary>
		/// <param name="view"></param>
		[Export("onStartWearableActivityClick")]
		public void OnStartWearableActivityClick(View view) {
			LOGD (Tag, "Generating RPC");

			// Trigger an AsyncTask that will query for a list of connected noded and send a "start-activity" message to each connecte node
			var task = new StartWearableActivityTask () { Activity = this };
			task.Execute ();
		}

		/// <summary>
		/// Generates a DataItem based on an incrementing count
		/// </summary>
		private class DataItemGenerator : Java.Lang.Object, Java.Lang.IRunnable 
		{
			int count = 0;
			public MainActivity Activity;
			public void Run ()
			{
				if (Activity != null) {
					var putDataMapRequest = PutDataMapRequest.Create (CountPath);
					putDataMapRequest.DataMap.PutInt (CountKey, count++);
					var request = putDataMapRequest.AsPutDataRequest ();

					LOGD (Tag, "Generating DataItem: " + request);
					if (!Activity.mGoogleApiClient.IsConnected)
						return;
					WearableClass.DataApi.PutDataItem (Activity.mGoogleApiClient, request)
						.SetResultCallback (
							new ResultCallback() {
								OnResultAction = (Java.Lang.Object result) => {
									try 
									{
										var res = result.JavaCast<IDataApiDataItemResult>();
										if (!res.Status.IsSuccess)
										{
											Log.Error(Tag, "Failed to send message with status code: " + res.Status.StatusCode);
										}
									}
									catch 
									{

									}
								}
							}
					);
				}
			}
		}

		/// <summary>
		/// Dispatches an Intent to take a photo. Result will be returned back in OnActivityResult()
		/// </summary>
		void DispatchTakePictureIntent()
		{
			Intent takePictureIntent = new Intent (MediaStore.ActionImageCapture);
			if (takePictureIntent.ResolveActivity (PackageManager) != null) {
				StartActivityForResult (takePictureIntent, RequestImageCapture);
			}
		}

		/// <summary>
		/// Builds an Asset from a bitmap. The image that we get back from the camera in 'data' is a thumbnail size. Typically your image should
		/// not exceed 320 x 320 and if you want to have zoom and parallax effects in your app, limit the size of your image to 640x400. Resize
		/// your image before transferring to your wearable device.
		/// </summary>
		/// <returns>The asset.</returns>
		/// <param name="bitmap">Bitmap.</param>
		public static Asset ToAsset(Bitmap bitmap) {

			MemoryStream byteStream = null;

			try 
			{
				byteStream = new MemoryStream();
				bitmap.Compress(Bitmap.CompressFormat.Jpeg, 90, byteStream);
				var bytes = byteStream.ToArray();
				return Asset.CreateFromBytes(bytes);
			}
			finally {
				try 
				{
					if (byteStream != null)
						byteStream.Dispose ();
				}
				catch {

				}
			}
		}

		/// <summary>
		/// Sends an asset that was created from the photo we took by adding it to the Data Item store
		/// </summary>
		/// <param name="asset">Asset.</param>
		private void SendPhoto(Asset asset) {
			var dataMap = PutDataMapRequest.Create (ImagePath);
			dataMap.DataMap.PutAsset (ImageKey, asset);
			dataMap.DataMap.PutLong ("time", DateTime.Now.ToBinary ());
			var request = dataMap.AsPutDataRequest ();
			WearableClass.DataApi.PutDataItem (mGoogleApiClient, request)
				.SetResultCallback (new ResultCallback () {
					OnResultAction = (Java.Lang.Object result) =>
					{
						var res = result.JavaCast<IDataApiDataItemResult>();
						LOGD(Tag, "Sending image was successful: " + res.Status.IsSuccess);
					}
			});
		}

		[Export("onTakePhotoClick")]
		public void OnTakePhotoClick(View view) { DispatchTakePictureIntent(); }

		[Export("onSendPhotoClick")]
		public void OnSendPhotoClick(View view)
		{
			if (null != imageBitmap && mGoogleApiClient.IsConnected) {
				SendPhoto (ToAsset (imageBitmap));
			}
		}

		/// <summary>
		/// Sets up UI components and their callback handlers
		/// </summary>
		void SetupViews() 
		{
			takePhotoBtn = (Button)FindViewById (Resource.Id.takePhoto);
			sendPhotoBtn = (Button)FindViewById (Resource.Id.sendPhoto);

			// Shows the image received from the handset
			thumbView = (ImageView)FindViewById (Resource.Id.imageView);
			dataItemList = (ListView)FindViewById (Resource.Id.data_item_list);

			startActivityBtn = FindViewById (Resource.Id.start_wearable_activity);
		}

		/// <summary>
		/// A simple weapper aroound Log.Debug
		/// </summary>
		/// <param name="tag">Tag</param>
		/// <param name="message">Message to log</param>
		static void LOGD(string tag, string message) 
		{
			if (Log.IsLoggable(tag, LogPriority.Debug)) {
				Log.Debug(tag, message);
			}
		}
	}
}


