using System;

using Android.Content;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Util;
using Android.Widget;
using Android.Support.V4.App;
using Android.Support.V17.Leanback.App;
using Android.Support.V17.Leanback.Widget;
using Action = Android.Support.V17.Leanback.Widget.Action;

using Java.IO;
using Java.Net;
using Java.Util;
using Squareup.Picasso;

namespace TvLeanback
{
	public class LeanbackDetailsFragment : Android.Support.V17.Leanback.App.DetailsFragment, IBaseOnItemViewClickedListener
	{
		private static readonly string TAG = "DetailsFragment";

		private static readonly int ACTION_WATCH_TRAILER = 1;
		private static readonly int ACTION_RENT = 2;
		private static readonly int ACTION_BUY = 3;

		private static readonly int DETAIL_THUMB_WIDTH = 120;
		private static readonly int DETAIL_THUMB_HEIGHT = 120;

		protected TvLeanback.Movie selectedMovie;

		private Drawable mDefaultBackground;
		private ITarget mBackgroundTarget;
		private DisplayMetrics mMetrics;

		public override void OnCreate (Bundle savedInstanceState)
		{
			Log.Info (TAG, "onCreate DetailsFragment");
			base.OnCreate (savedInstanceState);

			BackgroundManager backgroundManager = BackgroundManager.GetInstance (this.Activity);
			backgroundManager.Attach (this.Activity.Window);
			mBackgroundTarget = new PicassoBackgroundManagerTarget (backgroundManager);

			mDefaultBackground = Resources.GetDrawable (Resource.Drawable.default_background);

			mMetrics = new DisplayMetrics ();
			Activity.WindowManager.DefaultDisplay.GetMetrics (mMetrics);
			//selectedMovie = Utils.Deserialize (this.Activity.Intent.GetSerializableExtra ("Movie"));
			selectedMovie = Utils.Deserialize(this.Activity.Intent.GetStringExtra (GetString(Resource.String.movie))) ; //TODO make this work
			Log.Debug (TAG, "DetailsActivity movie: " + selectedMovie.ToString ());
			new DetailRowBuilderTask (this).Execute (selectedMovie);
			OnItemViewClickedListener = this;
		}

		public void OnItemClicked (Presenter.ViewHolder itemViewHolder, Java.Lang.Object item,
		                           RowPresenter.ViewHolder rowViewHolder, Java.Lang.Object row)
		{
			if (item is Movie) {
				var movie = (Movie)item;
				var intent = new Intent (this.Activity, typeof(DetailsActivity));
				intent.PutExtra (GetString (Resource.String.movie), Utils.Serialize (movie));
				Bundle bundle = ActivityOptionsCompat.MakeSceneTransitionAnimation (Activity,
					((ImageCardView) itemViewHolder.View).MainImageView,
					DetailsActivity.SHARED_ELEMENT_NAME).ToBundle ();
				StartActivity (intent, bundle);
			}
		}

		protected void updateBackground (URI uri)
		{
			Picasso.With (Activity)
				.Load (uri.ToString ())
				.Resize (mMetrics.WidthPixels, mMetrics.HeightPixels)
				.Error (mDefaultBackground)
				.Into (mBackgroundTarget);
		}

		internal class DetailRowBuilderTask : AsyncTask<Movie, int, DetailsOverviewRow>,  IOnActionClickedListener
		{
			private readonly LeanbackDetailsFragment owner;

			public DetailRowBuilderTask (LeanbackDetailsFragment owner) : base ()
			{
				this.owner = owner;
			}

			protected override DetailsOverviewRow RunInBackground (params Movie[] movies)
			{
				return (DetailsOverviewRow)DoInBackground (movies);
			}

			protected override Java.Lang.Object DoInBackground (Java.Lang.Object[] movies)
			{ 
				var selectedMovie = (Movie)movies [0];

				Log.Debug (TAG, "doInBackground: " + selectedMovie.ToString ());
				DetailsOverviewRow row = new DetailsOverviewRow (selectedMovie);
				try {
					var poster = Picasso.With (owner.Activity)
						.Load (selectedMovie.CardImageUrl)
						.Resize (Utils.dpToPx (DETAIL_THUMB_WIDTH, owner.Activity),
						             Utils.dpToPx (DETAIL_THUMB_HEIGHT, owner.Activity))
						.CenterCrop ()
						.Get ();
					row.SetImageBitmap (owner.Activity, poster);
					owner.updateBackground (selectedMovie.GetBackgroundImageURI ());
				} catch (IOException e) {
					Log.Warn (TAG, "Error updating background", e);
				}
				row.AddAction (new Action (ACTION_WATCH_TRAILER, owner.Resources.GetString (
					Resource.String.watch_trailer_1), owner.Resources.GetString (Resource.String.watch_trailer_2)));
				row.AddAction (new Action (ACTION_RENT, owner.Resources.GetString (Resource.String.rent_1),
					owner.Resources.GetString (Resource.String.rent_2)));
				row.AddAction (new Action (ACTION_BUY, owner.Resources.GetString (Resource.String.buy_1),
					owner.Resources.GetString (Resource.String.buy_2)));
				return row;
			}

			protected override void OnPostExecute (Java.Lang.Object raw)
			{
				DetailsOverviewRow detailrow = (DetailsOverviewRow)raw;
				ClassPresenterSelector ps = new ClassPresenterSelector ();
				DetailsOverviewRowPresenter dorPresenter =
					new DetailsOverviewRowPresenter (new DetailsDescriptionPresenter ());
				// set detail background and style
				dorPresenter.BackgroundColor = owner.Resources.GetColor (Resource.Color.detail_background);
				dorPresenter.StyleLarge = true;
				dorPresenter.OnActionClickedListener = (this);
				ps.AddClassPresenter (Utils.ToJavaClass (typeof(DetailsOverviewRow)), dorPresenter);
				ps.AddClassPresenter (Utils.ToJavaClass (typeof(ListRow)), new ListRowPresenter ());
				ArrayObjectAdapter adapter = new ArrayObjectAdapter (ps);
				adapter.Add ((Java.Lang.Object)detailrow);

				string[] subcategories = {
					owner.GetString (Resource.String.related_movies)
				};
				var movies = VideoProvider.MovieList;

				ArrayObjectAdapter listRowAdapter = new ArrayObjectAdapter (new CardPresenter ());
				foreach (var entry in movies) {
					if (owner.selectedMovie.Category.IndexOf (entry.Key) >= 0) {
						foreach (var movie in entry.Value) {
							listRowAdapter.Add (movie);
						}
					}
				}
				HeaderItem header = new HeaderItem (0, subcategories [0]);
				adapter.Add (new ListRow (header, listRowAdapter));
				owner.Adapter = adapter;
			}

			public void OnActionClicked (Action action)
			{
				if (action.Id == ACTION_WATCH_TRAILER) {
					Intent intent = new Intent (owner.Activity, typeof(PlayerActivity));
					intent.PutExtra (owner.Resources.GetString (Resource.String.movie), Utils.Serialize(owner.selectedMovie));
					intent.PutExtra (owner.Resources.GetString (Resource.String.should_start), true);
					owner.StartActivity (intent);
				} else
					Toast.MakeText (owner.Activity, action.ToString (), ToastLength.Short).Show ();

			}
		}
	}
}
