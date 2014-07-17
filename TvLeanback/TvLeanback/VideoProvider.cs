using System;
using System.Json;

using Android.Util;
using Android.Content;

using Java.IO;
using Java.Net;
using System.Text;
using System.Collections.Generic;

namespace TvLeanback
{
	public class VideoProvider
	{
		private const string TAG = "VideoProvider";
		private const string TAG_MEDIA = "videos";
		private const string TAG_GOOGLE_VIDEOS = "googlevideos";
		private const string TAG_CATEGORY = "category";
		private const string TAG_STUDIO = "studio";
		private const string TAG_SOURCES = "sources";
		private const string TAG_DESCRIPTION = "description";
		private const string TAG_CARD_THUMB = "card";
		private const string TAG_BACKGROUND = "background";
		private const string TAG_TITLE = "title";

		public static Dictionary<String, IList<Movie>> MovieList {
			get;
			private set;
		}

		public static Context mContext {
			private get;
			set;
		}

		private static string mPrefixUrl;

		public static Dictionary<String, IList<Movie>> BuildMedia (Context ctx, string url)
		{
			if (null != MovieList) {
				return MovieList;
			}
			MovieList = new  Dictionary<String, IList<Movie>> ();

			JsonObject jsonObj = new VideoProvider ().ParseURL (url);
			JsonArray categories = (JsonArray)jsonObj [TAG_GOOGLE_VIDEOS];
			if (null != categories) {
				Log.Debug (TAG, "category #: " + categories.Count);
				string title, videoUrl, bgImageUrl, cardImageUrl, studio = "";
				int count = 0;
				foreach (JsonObject category in categories) {
					var category_name = category [TAG_CATEGORY].ToString ().Replace ("\"", "");
					var videos = (JsonArray)category [TAG_MEDIA];
					Log.Debug (TAG,
						"category: " + (count++) + " Name: " + category_name + " video length: "
						+ videos.Count);
					var categoryList = new List<Movie> ();
					if (null != videos) {
						foreach (JsonObject video in videos) {
							string description = video [TAG_DESCRIPTION].ToString ();
							JsonArray videoUrls = (JsonArray)video [TAG_SOURCES];
							if (null == videoUrls || videoUrls.Count == 0) {
								continue;
							}
							title = video [TAG_TITLE];
							videoUrl = GetVideoPrefix (category_name, videoUrls [0]);
							bgImageUrl = GetThumbPrefix (category_name, title,
								video [TAG_BACKGROUND].ToString ()).ToString ();
							cardImageUrl = GetThumbPrefix (category_name, title,
								video [TAG_CARD_THUMB].ToString ()).ToString ();
							studio = video [TAG_STUDIO].ToString ();

							categoryList.Add (BuildMovieInfo (category_name, title, description, studio,
								videoUrl, cardImageUrl,
								bgImageUrl));
						}
						MovieList.Add (category_name, categoryList);
					}
				}
			}
			return MovieList;
		}

		protected JsonObject ParseURL (string urlstring)
		{
			Log.Debug (TAG, "Parse URL: " + urlstring);
			InputStream inputStream = null;

			mPrefixUrl = mContext.Resources.GetString (Resource.String.prefix_url);

			try {
				Java.Net.URL url = new Java.Net.URL (urlstring);
				var urlConnection = url.OpenConnection ();
				inputStream = new BufferedInputStream (urlConnection.InputStream);
				var reader = new BufferedReader (new InputStreamReader (
					             urlConnection.InputStream, "iso-8859-1"), 8);
				var sb = new StringBuilder ();
				string line = null;
				while ((line = reader.ReadLine ()) != null) {
					sb.Append (line);
				}
				var json = sb.ToString ();
				return (JsonObject)JsonObject.Parse (json);
			} catch (Exception e) {
				Log.Debug (TAG, "Failed to parse the json for media list", e);
				return null;
			} finally {
				if (null != inputStream) {
					try {
						inputStream.Close ();
					} catch (IOException e) {
						Log.Debug (TAG, "Json feed closed", e);
					}
				}
			}
		}

		private static Movie BuildMovieInfo (string category, string title, string description, 
		                                     string studio, string videoUrl, string cardImageUrl,
		                                     string bgImageUrl)
		{
			Movie movie = new Movie ();
			movie.Id = Movie.count;
			Movie.incCount ();
			movie.Title = title.Replace ("\"", "");
			movie.Description = description.Replace ("\"", "");
			movie.Studio = studio.Replace ("\"", "");
			movie.Category = category.Replace ("\"", "");
			movie.CardImageUrl = cardImageUrl.Replace ("\"", "");
			movie.BgImageUrl = bgImageUrl.Replace ("\"", "");
			movie.VideoUrl = videoUrl.Replace ("\"", "");
			return movie;
		}

		private static string GetVideoPrefix (string category, string videoUrl)
		{
			string ret = "";
			ret = mPrefixUrl + category.Replace (" ", "%20") + '/' +
			videoUrl.Replace (" ", "%20");
			return ret;
		}

		private static string GetThumbPrefix (string category, string title, string imageUrl)
		{
			string ret = "";
			ret = mPrefixUrl + category.Replace (" ", "%20") + '/' +
			title.Replace (" ", "%20") + '/' +
			imageUrl.Replace (" ", "%20");
			return ret;
		}
	}
}

