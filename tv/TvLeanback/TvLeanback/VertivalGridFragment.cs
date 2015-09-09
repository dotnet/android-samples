using System;
using System.Collections.Generic;

using Java.Util;

using Android.Content;
using Android.OS;
using Android.Support.V17.Leanback.Widget;
using Android.Util;

namespace TvLeanback
{
	public class VertivalGridFragment : Android.Support.V17.Leanback.App.VerticalGridFragment, IOnItemViewClickedListener, IOnItemViewSelectedListener
	{
		private static string TAG = "VerticalGridFragment";

		private static int NUM_COLUMNS = 5;

		private ArrayObjectAdapter mAdapter;

		public override void OnCreate(Bundle savedInstanceState) {
			Log.Info(TAG, "onCreate");
			base.OnCreate(savedInstanceState);

			Title = GetString(Resource.String.vertical_grid_title);

			SetupFragment();
		}

		private void SetupFragment() {
			var gridPresenter = new VerticalGridPresenter();
			gridPresenter.NumberOfColumns = NUM_COLUMNS;
			this.GridPresenter = gridPresenter;
			mAdapter = new ArrayObjectAdapter (new CardPresenter ());

			long seed = System.Diagnostics.Stopwatch.GetTimestamp(); //a pseudorandom seed based on system time
			var movies = VideoProvider.MovieList;

			foreach (var entry in movies) {
				//TODO may need to write Util.Shuffle method, will go in Utils.cs
				var list = (List<Object>)entry.Value;
				Collections.Shuffle (list, new Java.Util.Random (seed));
				foreach (var movie in list)
					mAdapter.Add ((Movie) movie);
			}

			this.Adapter = mAdapter;
			OnItemViewClickedListener = this;

			SetOnItemViewSelectedListener (this);
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

		public void OnItemSelected (Presenter.ViewHolder itemViewHolder, Java.Lang.Object item,
			RowPresenter.ViewHolder rowViewHolder, Row row)
		{
			//Do nothing
		}
	}
}

