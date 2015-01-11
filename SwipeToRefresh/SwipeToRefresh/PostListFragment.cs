using System;
using System.Net;
using System.Net.Http;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections.Concurrent;

using Newtonsoft.Json;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Net.Http;

namespace SwipeToRefresh
{
	public class PostListFragment : Android.Support.V4.App.ListFragment
	{
		bool loading;
		PostListAdapter adapter;

		public static PostListFragment Instantiate (string forumID, string forumName)
		{
			return new PostListFragment {
				ForumID = forumID,
				ForumName = forumName
			};
		}

		public string ForumID {
			get;
			private set;
		}

		public string ForumName {
			get;
			private set;
		}

		public override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);
		}

		public override void OnAttach (Activity activity)
		{
			base.OnAttach (activity);

			adapter = new PostListAdapter (activity);
			FetchItems ();
		}

		public async Task FetchItems (int pageID = 0, bool clear = false)
		{
			const string Url = "http://forums.xamarin.com/api/v1/discussions/category.json?CategoryIdentifier=";

			if (loading)
				return;
			loading = true;
			var client = new HttpClient ();
			//client.DefaultRequestHeaders.Host = "forums.xamarin.com";
			try {
				var json = await client.GetStringAsync (Url + ForumID);
				var forum = JsonConvert.DeserializeObject<Forum> (json);
				if (clear)
					adapter.ClearData ();
				adapter.FeedData (forum.Discussions.OrderByDescending (d => d.DateInserted));
			} catch (Exception e) {
				Android.Util.Log.Error ("FetchItems", e.ToString ());
			}
			if (pageID == 0)
				ListAdapter = adapter;
			loading = false;
		}

		class PostListAdapter : BaseAdapter
		{
			List<Discussion> discussions = new List<Discussion> ();
			Drawable noFacePicture;
			Activity context;
			ConcurrentDictionary<string, Task<byte[]>> faceCache = new ConcurrentDictionary<string, Task<byte[]>> ();

			public PostListAdapter (Activity context)
			{
				this.context = context;
				this.noFacePicture = new ColorDrawable (Color.White);
			}

			public void ClearData ()
			{
				discussions.Clear ();
				NotifyDataSetChanged ();
			}

			public void FeedData (IEnumerable<Discussion> newData)
			{
				discussions.AddRange (newData);
				NotifyDataSetChanged ();
			}

			public Discussion GetDiscussionAtPosition (int position)
			{
				return discussions [position];
			}

			public override View GetView (int position, View convertView, ViewGroup parent)
			{
				var view = EnsureView (convertView);
				var item = discussions [position];
				var version = Interlocked.Increment (ref view.VersionNumber);

				var title = view.FindViewById<TextView> (Resource.Id.PostTitle);
				var body = view.FindViewById<TextView> (Resource.Id.PostText);
				var time = view.FindViewById<TextView> (Resource.Id.PostTime);
				var timeSecondary = view.FindViewById<TextView> (Resource.Id.PostTimeSecondary);
				var avatar = view.FindViewById<ImageView> (Resource.Id.PostAvatar);
				var author = view.FindViewById<TextView> (Resource.Id.AuthorName);

				body.Text = PrepareBody (item.Body);
				title.Text = item.Name.Length > 2 && char.IsLower (item.Name[0]) ?
					char.ToUpper (item.Name[0]) + item.Name.Substring (1) : item.Name;
				author.Text = item.FirstName;

				string timeFirst, timeSecond;
				PrepareTime (item, out timeFirst, out timeSecond);
				time.Text = timeFirst;
				timeSecondary.Text = timeSecond;

				avatar.SetImageDrawable (noFacePicture);
				FetchAvatar (item.FirstPhoto, view, version);

				return view;
			}

			DiscussionView EnsureView (View convertView)
			{
				DiscussionView view = convertView as DiscussionView;
				if (view == null)
					view = new DiscussionView (context);
				return view;
			}

			string PrepareBody (string input)
			{
				return input.Replace ("\r\n", " ");
			}

			void PrepareTime (Discussion discussion, out string timeFirst, out string timeSecond)
			{
				DateTime now = DateTime.UtcNow;
				var referenceTime = discussion.DateInserted;

				var diff = now - referenceTime;

				if (diff < TimeSpan.FromDays (3)) {
					timeFirst = string.Empty;
					if (diff < TimeSpan.FromMinutes (1))
						timeFirst = ((int)diff.TotalSeconds) + " sec";
					else if (diff < TimeSpan.FromHours (1))
						timeFirst = ((int)diff.TotalMinutes) + " min";
					else if (diff < TimeSpan.FromDays (1))
						timeFirst = ((int)diff.TotalHours) + " hours";
					else
						timeFirst = ((int)diff.TotalDays) + " days";
					timeSecond = "ago";
				} else {
					timeFirst = referenceTime.ToString ("MMM d");
					timeSecond = referenceTime.ToString ("yyyy");
				}
			}

			void FetchAvatar (string url, DiscussionView view, long version)
			{
				var data = faceCache.GetOrAdd (url, u => Task.Run (() => {
					u = ResizeUrl (u);
					var client = new WebClient ();
					try {
						return client.DownloadData (u);
					} catch {
						return null;
					}
				}));

				if (data.IsCompleted && data.Result != null) {
					var avatar = view.FindViewById<ImageView> (Resource.Id.PostAvatar);
					var bmp = BitmapFactory.DecodeByteArray (data.Result, 0, data.Result.Length);
					bmp = ResizeBitmap (bmp);
					avatar.SetImageDrawable (new VignetteDrawable (bmp, withEffect: false));
				} else {
					data.ContinueWith (t => {
						if (t.Result != null && view.VersionNumber == version) {
							var bmp = BitmapFactory.DecodeByteArray (data.Result, 0, data.Result.Length);
							bmp = ResizeBitmap (bmp);
							context.RunOnUiThread (() => {
								if (view.VersionNumber == version) {
									var avatar = view.FindViewById<ImageView> (Resource.Id.PostAvatar);
									avatar.SetImageDrawable (new VignetteDrawable (bmp, withEffect: false));
								}
							});
						}
					});
				}
			}

			string ResizeUrl (string inputUrl)
			{
				var uri = new Uri (inputUrl);
				if (uri.Host != "www.gravatar.com")
					return inputUrl;
				return inputUrl.Replace ("&amp;size=130", string.Empty)
					+ "&size="
					+ TypedValue.ApplyDimension (ComplexUnitType.Dip, 48, context.Resources.DisplayMetrics);
			}

			Bitmap ResizeBitmap (Bitmap inputBitmap)
			{
				var size = (int)TypedValue.ApplyDimension (ComplexUnitType.Dip, 48, context.Resources.DisplayMetrics);
				return Bitmap.CreateScaledBitmap (inputBitmap, size, size, true);
			}

			public override long GetItemId (int position)
			{
				return long.Parse (discussions[position].DiscussionID);
			}

			public override Java.Lang.Object GetItem (int position)
			{
				return new Java.Lang.String (discussions[position].Name);
			}

			public override int Count {
				get {
					return discussions == null ? 0 : discussions.Count;
				}
			}

			public override int ViewTypeCount {
				get {
					return 1;
				}
			}
		}

		class DiscussionView : FrameLayout
		{
			public DiscussionView (Context ctx) : base (ctx)
			{
				var inflater = ctx.GetSystemService (Context.LayoutInflaterService).JavaCast<LayoutInflater> ();
				inflater.Inflate (Resource.Layout.PostItemLayout, this, true);
			}

			public long VersionNumber;
		}
	}
}

