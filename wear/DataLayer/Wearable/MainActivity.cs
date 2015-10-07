using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Gms.Common.Apis;
using Android.Gms.Wearable;
using Android.Gms.Common.Data;
using System.Collections.Generic;
using System.Collections;
using Android.Graphics;
using Android.Graphics.Drawables;
using System.IO;
using Android.Util;
using Android.Content.PM;
using System.Threading.Tasks;

namespace Wearable
{

	/// <summary>
	/// Shows events and photo from the Wearable APIs
	/// </summary>
	[Activity (Label = "Wearable", 
		MainLauncher = true,
		ScreenOrientation = ScreenOrientation.Portrait,
		Icon = "@drawable/icon"),
		IntentFilter( new string[]{ "android.intent.action.MAIN" }, Categories = new string[]{ "android.intent.category.LAUNCHER" }),
		IntentFilter( new string[]{ "com.example.android.wearable.datalayer.EXAMPLE" }, 
			Categories = new string[]{ "android.intent.category.DEFAULT" })]
	public class MainActivity : Activity, GoogleApiClient.IConnectionCallbacks, GoogleApiClient.IOnConnectionFailedListener, IDataApiDataListener,
	IMessageApiMessageListener, INodeApiNodeListener
	{
		public const string Tag = "MainActivity";

		GoogleApiClient googleApiClient;
		ListView dataItemList;
		TextView introText;
		DataItemAdapter dataItemListAdapter;
		View layout;
		Handler handler;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			handler = new Handler ();
			DataLayerListenerService.LOGD (Tag, "OnCreate");
			SetContentView (Resource.Layout.main_activity);
			Window.AddFlags (WindowManagerFlags.KeepScreenOn);
			dataItemList = (ListView)FindViewById (Resource.Id.dataItem_list);
			introText = (TextView)FindViewById (Resource.Id.intro);
			layout = FindViewById (Resource.Id.layout);

			// Stores data events received by the local broadcaster.
			dataItemListAdapter = new DataItemAdapter (this, Android.Resource.Layout.SimpleListItem1);
			dataItemList.Adapter = dataItemListAdapter;

			googleApiClient = new GoogleApiClient.Builder (this)
				.AddApi (WearableClass.API)
				.AddConnectionCallbacks (this)
				.AddOnConnectionFailedListener (this)
				.Build ();
		}

		protected override void OnResume ()
		{
			base.OnResume ();
			googleApiClient.Connect ();
		}

		protected override async void OnPause ()
		{
			base.OnPause ();
			await WearableClass.DataApi.RemoveListenerAsync (googleApiClient, this);
            await WearableClass.MessageApi.RemoveListenerAsync (googleApiClient, this);
            await WearableClass.NodeApi.RemoveListenerAsync (googleApiClient, this);
			googleApiClient.Disconnect ();
		}

		public async void OnConnected (Bundle bundle)
		{
			DataLayerListenerService.LOGD (Tag, "OnConnected(): Successfully connected to Google API client");
			await WearableClass.DataApi.AddListenerAsync (googleApiClient, this);
			await WearableClass.MessageApi.AddListenerAsync (googleApiClient, this);
			await WearableClass.NodeApi.AddListenerAsync (googleApiClient, this);
		}

		public void OnConnectionSuspended (int p0)
		{
			DataLayerListenerService.LOGD (Tag, "OnConnectionSuspended(): Connection to Google API clinet was suspended");
		}

		public void OnConnectionFailed (Android.Gms.Common.ConnectionResult result)
		{
			DataLayerListenerService.LOGD (Tag, "OnConnectionFailed(): Failed to connect, with result: " + result);
		}

		void GenerateEvent(string title, string text)
		{
			RunOnUiThread (() => {
				introText.Visibility = ViewStates.Invisible;
				dataItemListAdapter.Add(new Event(title, text));
			});
		}

		public async void OnDataChanged (DataEventBuffer dataEvents)
		{
			DataLayerListenerService.LOGD (Tag, "OnDatachanged() : " + dataEvents);

			IList events = FreezableUtils.FreezeIterable (dataEvents);
			dataEvents.Close ();
			foreach (var ev in events) {
				var e = ((Java.Lang.Object)ev).JavaCast<IDataEvent> ();
				if (e.Type == DataEvent.TypeChanged) {
					String path = e.DataItem.Uri.Path;
					if (DataLayerListenerService.ImagePath.Equals (path)) {
						DataMapItem dataMapItem = DataMapItem.FromDataItem (e.DataItem);
						Asset photo = dataMapItem.DataMap.GetAsset (DataLayerListenerService.ImageKey);
						Bitmap bitmap = await LoadBitmapFromAsset (googleApiClient, photo);
						handler.Post (() => {
							DataLayerListenerService.LOGD (Tag, "Setting background image..");
							layout.SetBackgroundDrawable (new BitmapDrawable (Resources, bitmap));
						});
					} else if (DataLayerListenerService.CountPath.Equals (path)) {
						DataLayerListenerService.LOGD (Tag, "Data Chaged for CountPath");
						GenerateEvent ("DataItem Changed", e.DataItem.ToString ());
					} else {
						DataLayerListenerService.LOGD (Tag, "Unrecognized path: " + path);
					}
				} else if (e.Type == DataEvent.TypeDeleted) {
					GenerateEvent ("DataItem Changed", e.DataItem.ToString ());
				} else {
					DataLayerListenerService.LOGD ("Unknown data event type", "Type = " + e.Type);
				}
			}
		}

		/// <summary>
		/// Extracts Bitmap data from the Asset
		/// </summary>
		/// <returns>A Bitmap generated from the supplied Asset</returns>
		/// <param name="apiClient"></param>
		/// <param name="asset"></param>
		async Task<Bitmap> LoadBitmapFromAsset(GoogleApiClient apiClient, Asset asset)
		{
			if (asset == null) {
				throw new NullReferenceException ("Asset must be non-null");
			}

            var fd = await WearableClass.DataApi.GetFdForAssetAsync (apiClient, asset);

            if (fd == null || !fd.Status.IsSuccess || fd.InputStream == null) {
				Log.Warn (Tag, "Requested an unknown Asset");
				return null;
			}
			//byte[] byteArray = assetStream.ToArray ();
            Bitmap bitmap = BitmapFactory.DecodeStream (fd.InputStream);
			return bitmap;
		}

		public void OnMessageReceived (IMessageEvent ev)
		{
			DataLayerListenerService.LOGD(Tag, "OnMessageReceived: " + ev);
			GenerateEvent("Message", ev.ToString());
		}

		public void OnPeerConnected (INode node)
		{
			GenerateEvent ("Node Connected", node.Id);
		}

		public void OnPeerDisconnected (INode node)
		{
			GenerateEvent ("Node disonnected", node.Id);
		}

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
	}
}


