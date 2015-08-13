using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.Media.Browse;
using Android.Media.Session;
using Android.OS;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace MediaBrowserService
{
	public class BrowseFragment : Fragment
	{
		new static readonly string Tag = LogHelper.MakeLogTag (typeof(BrowseFragment).Name);
		public const string ArgMediaId = "media_id";

		public interface IFragmentDataHelper
		{
			void OnMediaItemSelected (MediaBrowser.MediaItem item);
		}

		string mediaId;
		MediaBrowser mediaBrowser;
		BrowseAdapter browserAdapter;

		class SubscriptionCallback : MediaBrowser.SubscriptionCallback
		{
			public Action<string, IList<MediaBrowser.MediaItem>> OnChildrenLoadedImpl { get; set; }

			public Action<string> OnErrorImpl { get; set; }

			public override void OnChildrenLoaded (string parentId, IList<MediaBrowser.MediaItem> children)
			{
				OnChildrenLoadedImpl (parentId, children);
			}

			public override void OnError (string id)
			{
				OnErrorImpl (id);
			}
		}

		class ConnectionCallback : MediaBrowser.ConnectionCallback
		{
			public Action OnConnectedImpl { get; set; }

			public Action OnConnectionFailedImpl { get; set; }

			public Action OnConnectionSuspendedImpl { get; set; }

			public override void OnConnected ()
			{
				OnConnectedImpl ();
			}

			public override void OnConnectionFailed ()
			{
				OnConnectionFailedImpl ();
			}

			public override void OnConnectionSuspended ()
			{
				OnConnectionSuspendedImpl ();
			}
		}

		SubscriptionCallback subscriptionCallback = new SubscriptionCallback ();
		ConnectionCallback connectionCallback = new ConnectionCallback ();

		public static BrowseFragment Create (string mediaId = null)
		{
			var args = new Bundle ();
			args.PutString (ArgMediaId, mediaId);
			var fragment = new BrowseFragment ();
			fragment.Arguments = args;
			return fragment;
		}

		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			View rootView = inflater.Inflate (Resource.Layout.fragment_list, container, false);

			browserAdapter = new BrowseAdapter (Activity);

			View controls = rootView.FindViewById (Resource.Id.controls);
			controls.Visibility = ViewStates.Gone;

			var listView = rootView.FindViewById<ListView> (Resource.Id.list_view);
			listView.Adapter = browserAdapter;

			listView.ItemClick += (sender, e) => {
				MediaBrowser.MediaItem item = browserAdapter.GetItem (e.Position);
				try {
					var listener = (IFragmentDataHelper)Activity;
					listener.OnMediaItemSelected (item);
				} catch (InvalidCastException ex) {
					Log.Error (Tag, "Exception trying to cast to FragmentDataHelper", ex);
				}
			};

			Bundle args = Arguments;
			mediaId = args.GetString (ArgMediaId, null);
			mediaBrowser = new MediaBrowser (Activity,
				new ComponentName (Activity, Java.Lang.Class.FromType (typeof(MusicService))),
				connectionCallback, null);
			
			subscriptionCallback.OnChildrenLoadedImpl = (parentId, children) => {
				browserAdapter.Clear ();
				browserAdapter.NotifyDataSetInvalidated ();
				foreach (MediaBrowser.MediaItem item in children) {
					browserAdapter.Add (item);
				}
				browserAdapter.NotifyDataSetChanged ();
			};

			subscriptionCallback.OnErrorImpl = (id) => Toast.MakeText (Activity, "Error Loading Media", ToastLength.Long).Show ();
			connectionCallback.OnConnectedImpl = () => {
				LogHelper.Debug (Tag, "onConnected: session token " + mediaBrowser.SessionToken);
				if (mediaId == null)
					mediaId = mediaBrowser.Root;
				mediaBrowser.Subscribe (mediaId, subscriptionCallback);
				if (mediaBrowser.SessionToken == null)
					throw new ArgumentNullException ("No Session token");
				var mediaController = new Android.Media.Session.MediaController (Activity, mediaBrowser.SessionToken);
				Activity.MediaController = mediaController;
			};
			connectionCallback.OnConnectionFailedImpl = () => LogHelper.Debug (Tag, "onConnectionFailed");
			connectionCallback.OnConnectionSuspendedImpl = () => {
				LogHelper.Debug (Tag, "onConnectionSuspended");
				Activity.MediaController = null;
			};
			return rootView;
		}

		public override void OnStart ()
		{
			base.OnStart ();
			mediaBrowser.Connect ();
		}

		public override void OnStop ()
		{
			base.OnStop ();
			mediaBrowser.Disconnect ();
		}

		class BrowseAdapter : ArrayAdapter<MediaBrowser.MediaItem>
		{
			public BrowseAdapter (Context context) :
				base (context, Resource.Layout.media_list_item, new List<MediaBrowser.MediaItem> ())
			{
			}

			class ViewHolder : Java.Lang.Object
			{
				public ImageView mImageView;
				public TextView mTitleView;
				public TextView mDescriptionView;
			}

			public override View GetView (int position, View convertView, ViewGroup parent)
			{
				ViewHolder holder;
				if (convertView == null) {
					convertView = LayoutInflater.From (Context)
						.Inflate (Resource.Layout.media_list_item, parent, false);
					holder = new ViewHolder ();
					holder.mImageView = convertView.FindViewById<ImageView> (Resource.Id.play_eq);
					holder.mImageView.Visibility = ViewStates.Gone;
					holder.mTitleView = convertView.FindViewById<TextView> (Resource.Id.title);
					holder.mDescriptionView = convertView.FindViewById<TextView> (Resource.Id.description);
					convertView.Tag = holder;
				} else {
					holder = (ViewHolder)convertView.Tag;
				}
				MediaBrowser.MediaItem item = GetItem (position);
				holder.mTitleView.Text = item.Description.Title;
				holder.mDescriptionView.Text = item.Description.Description;
				if (item.IsPlayable) {
					holder.mImageView.SetImageDrawable (
						Context.GetDrawable (Resource.Drawable.ic_play_arrow_white_24dp));
					holder.mImageView.Visibility = ViewStates.Visible;
				}
				return convertView;
			}
		}
	}
}

