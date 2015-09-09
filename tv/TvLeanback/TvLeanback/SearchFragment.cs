using System;
using System.Collections.Generic;
using System.Globalization;

using Java.Util;

using Android.Annotation;
using Android.Content;
using Android.OS;
using Android.Support.V17.Leanback.Widget;
using Android.Text.Util;
using Android.Util;

namespace TvLeanback
{
	public class SearchFragment : Android.Support.V17.Leanback.App.SearchFragment, IOnItemViewClickedListener,
		Android.Support.V17.Leanback.App.SearchFragment.ISearchResultProvider
	{
		private static  string TAG = "SearchFragment";
		private static  int SEARCH_DELAY_MS = 300;
		private static readonly CultureInfo unitedStates = new CultureInfo ("en-US", false);

		public ArrayObjectAdapter mRowsAdapter {
			private set;
			get;
		}
		public ObjectAdapter ResultsAdapter {
			get {
				return (ObjectAdapter)mRowsAdapter;
			}
		}

		private Handler mHandler = new Handler ();
		private SearchRunnable mDelayedLoad;

		public override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);
			mDelayedLoad = new SearchRunnable (this);
			mRowsAdapter = new ArrayObjectAdapter (new ListRowPresenter ());
			SetSearchResultProvider (this);
			SetOnItemViewClickedListener (this);
		}

		public void OnItemClicked (Presenter.ViewHolder itemViewHolder, Java.Lang.Object item,
			RowPresenter.ViewHolder rowViewHolder, Row row)
		{
			if (item is Movie) {
				var movie = (Movie)item;
				var intent = new Intent (this.Activity, typeof(DetailsActivity));
				intent.PutExtra (GetString (Resource.String.movie), Utils.Serialize(movie));
				StartActivity (intent);
			}
		}

		private void QueryByWords (string words)
		{
			mRowsAdapter.Clear ();
			if (!words.Equals (string.Empty)) {
				mDelayedLoad.searchQuery = words;
				mHandler.RemoveCallbacks (mDelayedLoad);
				mHandler.PostDelayed (mDelayedLoad, SEARCH_DELAY_MS);
			}
		}

		bool SearchFragment.ISearchResultProvider.OnQueryTextChange (string newQuery)
		{
			Log.Info (TAG, String.Format ("Search Query Text Change %s", newQuery));
			QueryByWords (newQuery);
			return true;
		}

		public bool OnQueryTextSubmit (String query)
		{
			Log.Info (TAG, String.Format ("Search Query Text Submit %s", query));
			QueryByWords (query);
			return true;
		}

		protected void LoadRows (String query)
		{
			Dictionary<string, IList<Movie>> movies = VideoProvider.MovieList;
			ArrayObjectAdapter listRowAdapter = new ArrayObjectAdapter (new CardPresenter ());
			foreach (var entry in movies) {
				foreach (Movie movie in entry.Value) {
					if (movie.Title.ToLower (unitedStates)
						.Contains (query.ToLower (unitedStates))
					    || movie.Description.ToLower (unitedStates)
						.Contains (query.ToLower (unitedStates))) {
						listRowAdapter.Add (movie);
					}
				}
			}
			HeaderItem header = new HeaderItem (0, Resources.GetString (Resource.String.search_results));
			mRowsAdapter.Add (new ListRow (header, listRowAdapter));
		}

		internal class SearchRunnable : Java.Lang.Object, Java.Lang.IRunnable //Use Java.Lang.Object to use its Dispose and Handle.get methods
		{
			public string searchQuery {
				private get;
				set;
			}

			private readonly SearchFragment owner;

			public SearchRunnable (SearchFragment me)
			{
				owner = me;
			}

			public void Run ()
			{
				owner.LoadRows (searchQuery);
			}
		}

	}
}