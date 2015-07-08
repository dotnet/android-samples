using System;
using Android.Util;
using System.Text;

namespace MediaBrowserService
{
	public static class LogHelper
	{

		const string LogPrefix = "sample_";
		static readonly int LogPrefixLength = LogPrefix.Length;
		const int MaxLogTagLength = 23;

		public static string MakeLogTag (string str)
		{
			if (str.Length > MaxLogTagLength - LogPrefixLength) {
				return LogPrefix + str.Substring (0, MaxLogTagLength - LogPrefixLength - 1);
			}
			return LogPrefix + str;
		}

		public static string MakeLogTag (Type cls)
		{
			return MakeLogTag (cls.Name);
		}

		public static void Verbose (string tag, params object[] messages)
		{
			#if DEBUG
			Log (tag, LogPriority.Verbose, null, messages);
			#endif
		}

		public static void Debug (string tag, params object[] messages)
		{
			#if DEBUG
			Log (tag, LogPriority.Debug, null, messages);
			#endif
		}

		public static void Info (string tag, params object[] messages)
		{
			Log (tag, LogPriority.Info, null, messages);
		}

		public static void Warn (string tag, params object[] messages)
		{
			Log (tag, LogPriority.Warn, null, messages);
		}

		public static void Warn (string tag, Exception t, params object[] messages)
		{
			Log (tag, LogPriority.Warn, t, messages);
		}

		public static void Error (string tag, params object[] messages)
		{
			Log (tag, LogPriority.Error, null, messages);
		}

		public static void Error (string tag, Exception t, params object[] messages)
		{
			Log (tag, LogPriority.Error, t, messages);
		}

		public static void Log (string tag, LogPriority level, Exception t, params object[] messages)
		{
			//if (Android.Util.Log.IsLoggable (tag, level)) {
				string message;
				if (t == null && messages != null && messages.Length == 1) {
					message = messages [0].ToString ();
				} else {
					var sb = new StringBuilder ();
					if (messages != null) {
						foreach (var m in messages) {
							sb.Append (m);
						}
					}
					if (t != null) {
						sb.Append ("\n").Append (Android.Util.Log.GetStackTraceString (Java.Lang.Throwable.FromException(t)));
					}
					message = sb.ToString ();
				}
				Console.WriteLine ("[{0}] {1} - {2}", level.ToString (), tag, message);
				//Android.Util.Log.WriteLine((LogPriority)level, tag, message);
			//}
		}
	}
}
