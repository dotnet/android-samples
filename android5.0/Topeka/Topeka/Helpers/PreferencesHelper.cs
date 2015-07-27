using System;
using Android.Content;

namespace Topeka.Helpers
{
	public static class PreferencesHelper
	{
		const string PlayerPreferences = "playerPreferences";
		static readonly string PreferenceFirstName = string.Format ("{0}.firstName", PlayerPreferences);
		static readonly string PreferenceLastName = string.Format ("{0}.lastInitial", PlayerPreferences);
		static readonly string PreferenceAvatar = string.Format ("{0}.avatar", PlayerPreferences);

		public static void WriteToPreferences (Context context, Player player)
		{
			var editor = GetEditor (context);
			editor.PutString (PreferenceFirstName, player.FirstName);
			editor.PutString (PreferenceLastName, player.LastInitial);
			editor.PutString (PreferenceAvatar, player.Avatar.ToString ());
			editor.Apply ();
		}

		public static Player GetPlayer (Context context)
		{
			var preferences = GetSharedPreferences (context);
			var firstName = preferences.GetString (PreferenceFirstName, null);
			var lastInitial = preferences.GetString (PreferenceLastName, null);
			var avatarPreference = preferences.GetString (PreferenceAvatar, null);

			Avatar avatar = avatarPreference != null ? (Avatar)Enum.Parse (typeof(Avatar), avatarPreference) : 0;

			if (firstName == null && lastInitial == null)
				return null;
			return new Player (firstName, lastInitial, avatar);
		}

		public static void SignOut (Context context)
		{
			var editor = GetEditor (context);
			editor.Remove (PreferenceFirstName);
			editor.Remove (PreferenceLastName);
			editor.Remove (PreferenceAvatar);
			editor.Apply ();
		}

		public static bool IsSignedIn (Context context)
		{
			var preferences = GetSharedPreferences (context);
			return preferences.Contains (PreferenceFirstName) &&
			preferences.Contains (PreferenceLastName) &&
			preferences.Contains (PreferenceAvatar);
		}

		static ISharedPreferencesEditor GetEditor (Context context)
		{
			var preferences = GetSharedPreferences (context);
			return preferences.Edit ();
		}

		static ISharedPreferences GetSharedPreferences (Context context)
		{
			return context.GetSharedPreferences (PlayerPreferences, FileCreationMode.Private);
		}
	}
}

