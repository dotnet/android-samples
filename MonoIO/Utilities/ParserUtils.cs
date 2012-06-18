using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Text.Format;
using Pattern = Java.Util.Regex.Pattern;
using Org.XmlPull.V1;

namespace MonoIO
{
	class ParserUtils
	{
		public const string BlockTitleBreakoutSessions = "Breakout sessions";

	    public const string BlockTypeFood = "food";
	    public const string BlockTypeSession = "session";
	    public const string BlockTypeOfficeHours = "officehours";
		
		// TODO: factor this out into a separate data file.
	    public static HashSet<string> LOCAL_TRACK_IDS = new HashSet<string>(new string[]{
	            "accessibility", "android", "appengine", "chrome", "commerce", "developertools",
	            "gamedevelopment", "geo", "googleapis", "googleapps", "googletv", "techtalk",
	            "webgames", "youtube"});
		
		/** Used to sanitize a string to be {@link Uri} safe. */
	    private static Pattern sSanitizePattern = Pattern.Compile("[^a-z0-9-_]");
	    private static Pattern sParenPattern = Pattern.Compile("\\(.*?\\)");
	
	    /** Used to split a comma-separated string. */
	    private Pattern sCommaPattern = Pattern.Compile("\\s*,\\s*");

		private static Time sTime = new Time();
		
		/**
	     * Sanitize the given string to be {@link Uri} safe for building
	     * {@link ContentProvider} paths.
	     */
	    public static String SanitizeId(String input) {
	        return SanitizeId(input, false);
	    }
	
	    /**
	     * Sanitize the given string to be {@link Uri} safe for building
	     * {@link ContentProvider} paths.
	     */
	    public static String SanitizeId(String input, bool stripParen) {
	        if (input == null) return null;
	        if (stripParen) {
	            // Strip out all parenthetical statements when requested.
	            input = sParenPattern.Matcher(input).ReplaceAll("");
	        }
	        return sSanitizePattern.Matcher(input.ToLower()).ReplaceAll("");
	    }
		
		
		/**
	     * Parse the given string as a RFC 3339 timestamp, returning the value as
	     * milliseconds since the epoch.
	     */
	    public static long ParseTime(String time) 
		{
	        sTime.Parse3339(time);
	        return sTime.ToMillis(false);
	    }
		
		/**
	     * Return a {@link Blocks#BLOCK_ID} matching the requested arguments.
	     */
	    public static String FindBlock(String title, long startTime, long endTime) {
	        // TODO: in future we might check provider if block exists
	        return ScheduleContract.Blocks.GenerateBlockId(startTime, endTime);
	    }
		
		/**
	     * Translate an incoming {@link Tracks#TRACK_ID}, usually passing directly
	     * through, but returning a different value when a local alias is defined.
	     */
	    public static string TranslateTrackIdAlias(string trackId)
		{
			//if ("gwt".equals(trackId)) {
			//    return "googlewebtoolkit";
			//} else {
			return trackId;
			//}
		}
		
		/**
	     * Translate a possibly locally aliased {@link Tracks#TRACK_ID} to its real value;
	     * this usually is a pass-through.
	     */
		public static String TranslateTrackIdAliasInverse(String trackId)
		{
			//if ("googlewebtoolkit".equals(trackId)) {
			//    return "gwt";
			//} else {
			return trackId;
			//}
		}
	}
}

