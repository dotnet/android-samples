using System;
using Android.Util;
using Java.IO;
using Java.Net;

namespace TvLeanback
{
	//We explicitaly set Movie to be a child of Java.Lang.Object so it can be passed around throughout the native Android code.
	public class Movie : Java.Lang.Object
	{
		const long serialVersionUID = 727566175075960653L;
		private string TAG = "Movie";

		public static long count {
			get;
			private set;
		}

		public long Id {
			get;
			set;
		}

		public string Title {
			get;
			set;
		}

		public string Description {
			get;
			set;
		}

		public string BgImageUrl {
			get;
			set;
		}

		public string CardImageUrl {
			get;
			set;
		}

		public string VideoUrl {
			get;
			set;
		}

		public string Studio {
			get;
			set;
		}

		public string Category {
			get;
			set;
		}

		public Movie ()
		{
		}

		public static void incCount ()
		{
			count++;
		}

		public URI GetBackgroundImageURI ()
		{
			try {
				return new URI (BgImageUrl);
			} catch (URISyntaxException e) {
				Log.Error (TAG, BgImageUrl, e);
				return null;
			}
		}

		public URI GetCardImageURI ()
		{
			try {
				return new URI (CardImageUrl);
			} catch (URISyntaxException e) {
				Log.Error (TAG, CardImageUrl, e);
				return null;
			}
		}
	}
}

