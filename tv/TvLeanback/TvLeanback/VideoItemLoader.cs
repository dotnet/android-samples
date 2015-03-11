using System;
using System.Collections.Generic;
using Android.Content;
using Android.Util;

namespace TvLeanback
{
	public class VideoItemLoader : Android.Content.AsyncTaskLoader//<Dictionary<string, List<Movie>>>
	{
		private static readonly string TAG = "VideoItemLoader";
		private readonly string mUrl;
		private readonly Context mContext;

		public VideoItemLoader (Context context, string url) : base(context)
		{
			mContext = context;
			mUrl = url;
		}

		public override Java.Lang.Object LoadInBackground ()
		{
			try {
				return Utils.PutDictionary(VideoProvider.BuildMedia (mContext, mUrl));
			} catch (Exception e) {
				Log.Error (TAG, "Failed to catch media data", e);
				return null;
			}
		}

		protected override void OnStartLoading ()
		{
			base.OnStartLoading ();
			ForceLoad ();
		}

		/**
	     * Handles a request to stop the Loader.
	     */
		protected override void OnStopLoading ()
		{
			// Attempt to cancel the current load task if possible.
			CancelLoad ();
		}

	}
}

