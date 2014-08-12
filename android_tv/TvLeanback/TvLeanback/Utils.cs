using System;
using System.Collections.Generic;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Views;
using Android.Widget;

using Android.Util;

namespace TvLeanback
{
	public class Utils
	{
		/*
	     * Making sure public utility methods remain static
	     */
		private Utils ()
		{
		}

		/**
	     * Returns the screen/display size
	     * 
	     * @param context
	     * @return
	     */
		public static Point GetDisplaySize (Context context)
		{
			var wm = (IWindowManager)context.GetSystemService (Context.WindowService);
			Display display = wm.DefaultDisplay;
			Point size = new Point ();
			display.GetSize (size);
			int width = size.X;
			int height = size.Y;
			return new Point (width, height);
		}

		/**
	     * Shows an error dialog with a given text message.
	     * 
	     * @param context
	     * @param errorString
	     */
		public static void ShowErrorDialog (Context context, String errorString)
		{
			new AlertDialog.Builder (context).SetTitle (Resource.String.error)
				.SetMessage (errorString)
				.SetPositiveButton (Resource.String.ok, (object s, DialogClickEventArgs e) => {
				if (s is IDialogInterface) {
					((IDialogInterface)s).Cancel (); //TODO test
					Log.Info ("Dialog", "Canceling"); //TODO remove
				}
			}).Create ().Show ();
		}

		/**
	     * Shows a (long) toast
	     * 
	     * @param context
	     * @param msg
	     */
		public static void showToast (Context context, String msg)
		{
			Toast.MakeText (context, msg, ToastLength.Long).Show ();
		}

		/**
	     * Shows a (long) toast.
	     * 
	     * @param context
	     * @param resourceId
	     */
		public static void showToast (Context context, int resourceId)
		{
			Toast.MakeText (context, context.GetString (resourceId), ToastLength.Long).Show ();
		}

		public static int dpToPx (int dp, Context ctx)
		{
			float density = ctx.Resources.DisplayMetrics.Density;
			return (int)Math.Round ((float)dp * density);
		}
		//Start of Xamarin-specific utility functions for handling casting to & from Java objects
		public static Movie Deserialize (string str)
		{
			var movie = new Movie ();
			var tokens = str.Split ('\n');
			movie.Title = tokens [0];
			movie.VideoUrl = tokens [1];
			movie.BgImageUrl = tokens [2];
			movie.CardImageUrl = tokens [3];
			movie.Category = tokens [4];
			return movie;
		}

		public static string Serialize (Movie movie)
		{
			string raw = "{0}\n{1}\n{2}\n{3}\n{4}";
			return String.Format (raw, movie.Title, movie.VideoUrl, movie.BgImageUrl, movie.CardImageUrl, movie.Category);
		}

		public static Java.Lang.Class ToJavaClass (Type t)
		{
			return Java.Lang.Class.FromType (t);
		}

		public static Java.Lang.Object PutDictionary (Dictionary<String, IList<Movie>> input)
		{
			return new DictionaryHelper (input);
		}

		public static Dictionary<String, IList<Movie>> GetDictionary (Java.Lang.Object input)
		{
			if (input is DictionaryHelper)
				return ((DictionaryHelper)input).val;
			else
				throw new ArgumentException ("Cannot get dictionary from non DictionaryHelper object"); 
		}

		public class DictionaryHelper : Java.Lang.Object
		{
			public DictionaryHelper (Dictionary<String, IList<Movie>> val)
			{
				this.val = val;
			}

			public Dictionary<string, IList<Movie>> val;
		}
	}
}

