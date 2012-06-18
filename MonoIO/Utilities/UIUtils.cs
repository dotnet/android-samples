using System;
using System.Collections.Generic;
using System.Text;
using Android.OS;
using Android.Content;
using Android.Content.Res;
using Android.Text.Format;
using Android.Widget;
using Android.Text;
using Android.Text.Style;
using Android.Graphics;
using Android.Content.PM;
using Android.Graphics.Drawables;
using Android.Preferences;

namespace MonoIO.Utilities
{
	static class UIUtils
	{
		public static Java.Util.TimeZone ConferenceTimeZone = Java.Util.TimeZone.GetTimeZone ("America/Los_Angeles");
		private const FormatStyleFlags TIME_FLAGS = FormatStyleFlags.ShowTime | FormatStyleFlags.ShowWeekday | FormatStyleFlags.AbbrevWeekday;
		public static long CONFERENCE_START_MILLIS = ParserUtils.ParseTime ("2011-05-10T09:00:00.000-07:00");
		public static long CONFERENCE_END_MILLIS = ParserUtils.ParseTime ("2011-05-11T17:30:00.000-07:00");
		public static bool IsHoneycomb;
		private static StyleSpan sBoldSpan = new StyleSpan (TypefaceStyle.Bold);

		static UIUtils ()
		{
			IsHoneycomb = Build.VERSION.SdkInt >= BuildVersionCodes.Honeycomb;
		}

		public static bool IsTablet (Context context)
		{
			return (context.Resources.Configuration.ScreenLayout & ScreenLayout.SizeMask) >= ScreenLayout.SizeLarge;
		}

		public static bool IsHoneycombTablet (Context context)
		{
			return IsHoneycomb && IsTablet (context);
		}
		
		public static long GetCurrentTime (Context context)
		{
			//SharedPreferences prefs = context.getSharedPreferences("mock_data", 0);
			//prefs.edit().commit();
			//return prefs.getLong("mock_current_time", System.currentTimeMillis());
			return Java.Lang.JavaSystem.CurrentTimeMillis ();
		}
		
		public static Type GetMapActivityClass (Context context)
		{
	        if (UIUtils.IsHoneycombTablet(context)) {
	            return typeof(MapMultiPaneActivity);
	        }
			return typeof(MapActivity);
		}
		
		/**
	     * Format and return the given {@link Blocks} and {@link Rooms} values using
	     * {@link #CONFERENCE_TIME_ZONE}.
	     */
		public static string FormatSessionSubtitle (long blockStart, long blockEnd, String roomName, Context context)
		{
			Java.Util.TimeZone.Default = ConferenceTimeZone;
	
			// NOTE: There is an efficient version of formatDateRange in Eclair and
			// beyond that allows you to recycle a StringBuilder.
			string timeString = DateUtils.FormatDateRange (context, blockStart, blockEnd, TIME_FLAGS);
	
			return context.GetString (Resource.String.session_subtitle, timeString, roomName);
		}
		
		/**
	     * Populate the given {@link TextView} with the requested text, formatting
	     * through {@link Html#fromHtml(String)} when applicable. Also sets
	     * {@link TextView#setMovementMethod} so inline links are handled.
	     */
		public static void SetTextMaybeHtml (TextView view, String text)
		{
			if (TextUtils.IsEmpty (text)) {
				view.Text = "";
				return;
			}
			if (text.Contains ("<") && text.Contains (">")) {
				view.TextFormatted = Html.FromHtml (text);
				view.MovementMethod = Android.Text.Method.LinkMovementMethod.Instance;
			} else {
				view.Text = text;
			}
		}
		
		public static void SetSessionTitleColor (long blockStart, long blockEnd, TextView title, TextView subtitle)
		{
			long currentTimeMillis = Java.Lang.JavaSystem.CurrentTimeMillis ();
			int colorId = Resource.Color.body_text_1;
			int subColorId = Resource.Color.body_text_2;
	
			if (currentTimeMillis > blockEnd && currentTimeMillis < CONFERENCE_END_MILLIS) {
				colorId = subColorId = Resource.Color.body_text_disabled;
			}
	
			Resources res = title.Resources;
			title.SetTextColor (res.GetColor (colorId));
			subtitle.SetTextColor (res.GetColor (subColorId));
		}
		
		/**
	     * Given a snippet string with matching segments surrounded by curly
	     * braces, turn those areas into bold spans, removing the curly braces.
	     */
		public static ISpannable BuildStyledSnippet (Java.Lang.String snippet)
		{
			SpannableStringBuilder builder = new SpannableStringBuilder (snippet);
	
			// Walk through string, inserting bold snippet spans
			int startIndex = -1, endIndex = -1, delta = 0;
			while ((startIndex = snippet.IndexOf('{', endIndex)) != -1) {
				endIndex = snippet.IndexOf ('}', startIndex);
	
				// Remove braces from both sides
				builder.Delete (startIndex - delta, startIndex - delta + 1);
				builder.Delete (endIndex - delta - 1, endIndex - delta);
	
				// Insert bold style
				builder.SetSpan (sBoldSpan, startIndex - delta, endIndex - delta - 1, SpanTypes.ExclusiveExclusive);
	
				delta += 2;
			}
	
			return builder;
		}
		
		public static String GetLastUsedTrackID (Context context)
		{
			ISharedPreferences sp = PreferenceManager.GetDefaultSharedPreferences (context);
			return sp.GetString ("last_track_id", null);
		}

		public static void SetLastUsedTrackID (Context context, String trackID)
		{
			ISharedPreferences sp = PreferenceManager.GetDefaultSharedPreferences (context);
			sp.Edit ().PutString ("last_track_id", trackID).Commit ();
		}
		
		private static int BRIGHTNESS_THRESHOLD = 130;

		
		/**
	     * Calculate whether a color is light or dark, based on a commonly known
	     * brightness formula.
	     *
	     * @see {@literal http://en.wikipedia.org/wiki/HSV_color_space%23Lightness}
	     */
		public static bool IsColorDark (int color)
		{
			return ((30 * Color.GetRedComponent (color) + 59 * Color.GetGreenComponent (color) + 11 * Color.GetBlueComponent (color)) / 100) <= BRIGHTNESS_THRESHOLD;
		}
		
		public static Drawable GetIconForIntent (Context context, Intent i)
		{
			PackageManager pm = context.PackageManager;
			var infos = pm.QueryIntentActivities (i, PackageInfoFlags.MatchDefaultOnly);
			if (infos.Count > 0) {
				return infos [0].LoadIcon (pm);
			}
			return null;
		}
	}
}
