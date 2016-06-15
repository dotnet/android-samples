using System;
using System.Collections.Generic;

using Android.OS;
using Android.App;

using Android.Support.V17.Leanback.Widget;
using Android.Support.V17.Leanback.App;
using Squareup.Picasso;

using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Util;
using Android.Content;
using Android.Widget;
using Android.Views;

using Java.Net;
using Java.Util;

namespace TvLeanback
{
	public class MainFragment : BrowseFragment, LoaderManager.ILoaderCallbacks, View.IOnClickListener, IOnItemViewSelectedListener//<Dictionary<string, IList<Movie>>>
	{
		private static readonly String TAG = "MainFragment";

		private static int BACKGROUND_UPDATE_DELAY = 300;
		private static int GRID_ITEM_WIDTH = 130;
		private static int GRID_ITEM_HEIGHT = 130;

		private ArrayObjectAdapter mRowsAdapter;
		private Drawable mDefaultBackground;
		private ITarget mBackgroundTarget;
		private DisplayMetrics mMetrics;
		private Timer mBackgroundTimer;
		private readonly Handler mHandler = new Handler ();
		private URI mBackgroundURI;
		private static String mVideosUrl;

		public override void OnActivityCreated (Bundle savedInstanceState)
		{
			Log.Info (TAG, "onCreate");
			base.OnActivityCreated (savedInstanceState);
			LoadVideoData ();

			PrepareBackgroundManager ();
			SetupUIElements ();
			SetupEventListeners ();
		}

		private void PrepareBackgroundManager ()
		{
			BackgroundManager backgroundManager = BackgroundManager.GetInstance (this.Activity);
			backgroundManager.Attach (this.Activity.Window);
			mBackgroundTarget = new PicassoBackgroundManagerTarget (backgroundManager);
			mDefaultBackground = Resources.GetDrawable (Resource.Drawable.default_background);
			mMetrics = new DisplayMetrics ();
			this.Activity.WindowManager.DefaultDisplay.GetMetrics (mMetrics);
		}

		private void SetupUIElements ()
		{
			BadgeDrawable = this.Activity.Resources.GetDrawable (Resource.Drawable.videos_by_google_banner);
			Title = (GetString (Resource.String.browse_title)); // Badge, when set, takes precedent over title
			HeadersState = BrowseFragment.HeadersEnabled;
			HeadersTransitionOnBackEnabled = true;
			// set fastLane (or headers) background color
			BrandColor = Resources.GetColor (Resource.Color.fastlane_background);
			// set search icon color
			SearchAffordanceColor = Resources.GetColor (Resource.Color.search_opaque);
		}

		private void LoadVideoData ()
		{
			VideoProvider.mContext = this.Activity;
			mVideosUrl = this.Activity.Resources.GetString (Resource.String.catalog_url);
			LoaderManager.InitLoader (0, null, this);
		}

		private void SetupEventListeners ()
		{
			SetOnSearchClickedListener (this);//new View.IOnClickListener(){

			OnItemViewSelectedListener = this;
			ItemViewClicked += (object sender, ItemViewClickedEventArgs e) => {
				var item = e.Item;
				if (item is Movie) {
					Movie movie = (Movie)item;
					Log.Debug (TAG, "Item: " + item.ToString ());
					Intent intent = new Intent (this.Activity, typeof(DetailsActivity));
					intent.PutExtra (GetString (Resource.String.movie), Utils.Serialize (movie));
					StartActivity (intent);
				} else if (item is Java.Lang.String) {
					if (((String)item).IndexOf (Resources.GetString (Resource.String.grid_view)) >= 0) {
						Intent intent = new Intent (this.Activity, typeof(VerticalGridActivity));
						StartActivity (intent);
					} else {
						Toast.MakeText (this.Activity, ((String)item), ToastLength.Short)
							.Show ();
					}
				}
			};
		}

		public void OnItemSelected (Presenter.ViewHolder itemViewHolder, Java.Lang.Object item,
			RowPresenter.ViewHolder rowViewHolder, Row row)
		{
			if (item is Movie) {
				mBackgroundURI = ((Movie)item).GetBackgroundImageURI ();
				StartBackgroundTimer ();
			}
		}

		public void OnClick (View v)
		{
			var intent = new Intent (Activity, typeof(TvLeanback.SearchActivity));
			StartActivity (intent); 
		}
		/*
	     * (non-Javadoc)
	     * @see android.support.v4.app.LoaderManager.LoaderCallbacks#onCreateLoader(int,
	     * android.os.Bundle)
	     */
		public Loader OnCreateLoader (int arg0, Bundle arg1)
		{
			Log.Debug (TAG, "VideoItemLoader created ");
			return (Loader)new VideoItemLoader (this.Activity, mVideosUrl);
		}

		/*
	     * (non-Javadoc)
	     * @see android.support.v4.app.LoaderManager.LoaderCallbacks#onLoadFinished(android
	     * .support.v4.content.Loader, java.lang.Object)
	     */
		public void OnLoadFinished (Android.Content.Loader loader, Java.Lang.Object raw)
		{
			var data = Utils.GetDictionary (raw);
			mRowsAdapter = new ArrayObjectAdapter (new ListRowPresenter ());
			var cardPresenter = new CardPresenter ();

			int i = 0;
			foreach (var entry in  data) {
				var listRowAdapter = new ArrayObjectAdapter (cardPresenter);
				IList<Movie> list = entry.Value;
				foreach (Movie current in list) {
					listRowAdapter.Add (current);
				}
				var header = new HeaderItem (i, entry.Key);
				i++;
				mRowsAdapter.Add (new ListRow (header, listRowAdapter));
			}

			var gridHeader = new HeaderItem (i, Resources.GetString (Resource.String.preferences));
			var gridPresenter = new GridItemPresenter (this);
			var gridRowAdapter = new ArrayObjectAdapter (gridPresenter);

			gridRowAdapter.Add (Resources.GetString (Resource.String.grid_view));
			gridRowAdapter.Add (Resources.GetString (Resource.String.send_feeback));
			gridRowAdapter.Add (Resources.GetString (Resource.String.personal_settings));
			mRowsAdapter.Add (new ListRow (gridHeader, gridRowAdapter));
			this.Adapter = mRowsAdapter;

			UpdateRecommendations ();
		}

		public void OnLoaderReset (Loader arg0)
		{
			mRowsAdapter.Clear ();
		}

		protected void SetDefaultBackground (Drawable background)
		{
			mDefaultBackground = background;
		}

		protected void SetDefaultBackground (int resourceId)
		{
			mDefaultBackground = Resources.GetDrawable (resourceId);
		}

		protected void UpdateBackground (URI uri)
		{
			Picasso.With (this.Activity)
				.Load (uri.ToString ())
				.Resize (mMetrics.WidthPixels, mMetrics.HeightPixels)
				.CenterCrop ()
				.Error (mDefaultBackground)
				.Into (mBackgroundTarget);
			GC.Collect();
		}

		protected void UpdateBackground (Drawable drawable)
		{
			BackgroundManager.GetInstance (this.Activity).Drawable = drawable;
		}

		protected void ClearBackground ()
		{
			BackgroundManager.GetInstance (this.Activity).Drawable = mDefaultBackground;
		}

		private void StartBackgroundTimer ()
		{
			if (null != mBackgroundTimer) {
				mBackgroundTimer.Cancel ();
			}
			mBackgroundTimer = new Timer ();
			mBackgroundTimer.Schedule (new UpdateBackgroundTask (this), BACKGROUND_UPDATE_DELAY);
		}

		private class UpdateBackgroundTask : TimerTask
		{
			MainFragment owner;

			public UpdateBackgroundTask (MainFragment f)
			{
				this.owner = f;
			}

			public override void Run ()
			{
				owner.mHandler.Post (() => {
					if (owner.mBackgroundURI != null) {
						owner.UpdateBackground (owner.mBackgroundURI);
					}
				});
			}
		}

		private class GridItemPresenter : Presenter
		{
			MainFragment owner;

			public GridItemPresenter (MainFragment f)
			{
				this.owner = f;
			}

			public override ViewHolder OnCreateViewHolder (ViewGroup parent)
			{
				var view = new TextView (parent.Context);
				view.LayoutParameters = (new ViewGroup.LayoutParams (GRID_ITEM_WIDTH, GRID_ITEM_HEIGHT));
				view.Focusable = (true);
				view.FocusableInTouchMode = (true);
				view.SetBackgroundColor (owner.Resources.GetColor (Resource.Color.default_background));
				view.SetTextColor (Color.White);
				view.Gravity = GravityFlags.Center;
				return new ViewHolder (view); 

			}

			public override void OnBindViewHolder (ViewHolder viewHolder, Java.Lang.Object item)
			{
				((TextView)viewHolder.View).Text = ((string)item);
			}

			public override void OnUnbindViewHolder (ViewHolder viewHolder)
			{
			}
		}

		private void UpdateRecommendations ()
		{
			var recommendationIntent = new Intent (this.Activity, typeof(UpdateRecommendationsService));
			this.Activity.StartService (recommendationIntent);
		}

	}
}

