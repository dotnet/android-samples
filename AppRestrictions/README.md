App Restrictions
==================
When launched under the primary User account, you can toggle between standard app restriction
types and custom.  When launched under a restricted profile, this activity displays app
restriction settings, if available.

This sample app maintains custom app restriction settings in shared preferences.  When
the activity is invoked (from Settings > Users), the stored settings are used to initialize
the custom configuration on the user interface.  Three sample input types are shown: 
checkbox, single-choice, and multi-choice.  When the settings are modified by the user,
the corresponding restriction entries are saved, which are retrievable under a restricted
profile.

Follow these steps to exercise the feature:
	1. If this is the primary user, go to Settings > Users.
	2. Create a restricted profile, if one doesn't exist already.
	3. Open the profile settings, locate the sample app, and tap the app restriction settings
		icon. Configure app restrictions for the app.
	4. In the lock screen, switch to the user's restricted profile, launch this sample app,
		and see the configured app restrictions displayed.
 	
Based on the Google Android-18 sample "AppRestrictions".
Ported by: Peter Collins