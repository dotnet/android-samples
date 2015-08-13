using System;
using System.Text;

namespace MediaBrowserService
{
	public static class MediaIDHelper
	{
		public const string MediaIdRoot = "__ROOT__";
		public const string MediaIdMusicsByGenre = "__BY_GENRE__";
		public const string MediaIdMusicsBySearch = "__BY_SEARCH__";

		const char CategorySeparator = '/';
		const char LeafSeparator = '|';

		public static string CreateMediaID (string musicID, params string[] categories)
		{
			var sb = new StringBuilder ();
			if (categories != null && categories.Length > 0) {
				sb.Append (categories [0]);
				for (var i = 1; i < categories.Length; i++) {
					sb.Append (CategorySeparator).Append (categories [i]);
				}
			}
			if (musicID != null) {
				sb.Append (LeafSeparator).Append (musicID);
			}
			return sb.ToString ();
		}

		public static string CreateBrowseCategoryMediaID (string categoryType, string categoryValue)
		{
			return categoryType + CategorySeparator + categoryValue;
		}

		public static string ExtractMusicIDFromMediaID (string mediaId)
		{
			int pos = mediaId.IndexOf (LeafSeparator);
			return pos >= 0 ? mediaId.Substring (pos + 1) : null;
		}

		public static string[] GetHierarchy (string mediaId)
		{
			int pos = mediaId.IndexOf (LeafSeparator);
			if (pos >= 0)
				mediaId = mediaId.Substring (0, pos);
			return mediaId.Split (CategorySeparator);
		}

		public static string ExtractBrowseCategoryValueFromMediaID (string mediaId)
		{
			string[] hierarchy = GetHierarchy (mediaId);
			if (hierarchy != null && hierarchy.Length == 2)
				return hierarchy [1];
			return null;
		}

		static bool IsBrowseable (string mediaId)
		{
			return mediaId.IndexOf (LeafSeparator) < 0;
		}

		public static string GetParentMediaID (string mediaId)
		{
			var hierarchy = GetHierarchy (mediaId);

			if (!IsBrowseable (mediaId))
				return CreateMediaID (null, hierarchy);
			
			if (hierarchy == null || hierarchy.Length <= 1)
				return MediaIdRoot;
			
			var parentHierarchy = new string[hierarchy.Length - 1];
			Array.Copy (hierarchy, parentHierarchy, hierarchy.Length - 1);
			return CreateMediaID (null, parentHierarchy);
		}
	}
}