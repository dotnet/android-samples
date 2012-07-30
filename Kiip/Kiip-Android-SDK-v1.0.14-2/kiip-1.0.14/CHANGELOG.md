# Changelog

Version 1.0.14

* Various small bug fixes

Version 1.0.13

* Attempt to start a session when an internet connection is established and no session is present
* Revamped session handling to make it simpler
* Improved reward loading (added caching/pre-caching)
* Removed KP naming scheme
* Smaller in-memory footprint
* Small bug fixes
* Small performance fixes

Version 1.0.12

* Supports hardware acceleration
* Fixed crash on non-standard app icon filenames

Version 1.0.11

* Fixed obfuscating KPSwarmListener and all parameter names
* Small internal improvements

Version 1.0.10

* Added persistent views across activities
* Fixed matching hiding or not hiding status bar on fullscreen reward unit
* Fixed bottom notifications set from the Developer Panel

Version 1.0.9

* Added in-game content rewards (Kiip Branded Moments)
* Added flash competitions (Kiip Swarm)
* Added email address caching in rewards units for Android 2.1+
* Added ability to define a custom reward notification layout
* Added dynamic notification layouts
* Added KPManager setUserInfo that takes a map of user information. Currently supports:
    * alias - player's name to be displayed in a swarm.
    * email - pre-populate reward units with the user's email address.
* Added support for landscape promos
* Added touch actions to the notification
* Updated example code
* Performance optimizations
* Bug fixes

Version 1.0.8

New:

* Added ability to add tags to:
	- KPManager#startSession
	- KPManager#unlockAchievement
	- KPManager#saveLeaderboard
* KPManager#startSession now receives promos.
* Added ability to completely customize notifications
* An example of how to handle Kiip sessions is now included in the example project. Feel free to use the Application wrapper in your project to handle Kiip sessions better.
* An example of how to retrieve the user's location is now included in the example project. Feel free to use the LocationHelper in your project to retrieve user locations.
* Unit position can now be toggled from the web interface at http://kiip.me (Notification Top|Bottom, Fullscreen)

If updating from a previous version:

* Kiip is now KPManager. To get singleton instance, use:
    KPManager manager = new KPManager(context, new KPSettings("_my_app_key_", "_my_app_secret_"));
    KPManager.setInstance();
* KiipViewPosition is now KPPosition
* KiipSettings is now KPSettings
* KiipDelegate is now KPViewListener
* Added KPRequestListener which receives callbacks from the following methods:
    - KPManager#startSession
    - KPManager#endSession
    - KPManager#unlockAchievement
    - KPManager#saveLeaderboard
    - KPManager#getActivePromos
* Deprecated KPManager#getPromo.
* Deprecated KPManager#setLocation. Instead, just add the correct permissions to the application's AndroidManifest.xml and Kiip will automatically grab the location.
* Deprecated KPManager#showView, KPManager#showNotification, KPManager#showFullscreen. Instead, set the KPResource.position and call KPManager#showResource.
* Updated notification layout
* Fix fullscreen units on Android 1.6
* Bug fixes

Version 1.0.7

* Removed need for user-permission READ_PHONE_STATE
* Improved reward display
* startSession(location) changed to startSession(activity, location, delegate)
* Updated delegate code
* Improved exception catching

Version 1.0.6

* Updated asynchronous http request code

Version 1.0.5

* Updated code to uniquely identify devices
* Improved memory management
* Added a background image for the Kiip loading indicator

Version 1.0.4

* Optimized image loading

Version 1.0.3

* Improved memory management
* Updated activity change functionality
* Added native loading dialog close button
* Added new endpoint Kiip.destroy to clear out all Kiip windows before closing application

Version 1.0.2

* Improved internal methods

Version 1.0.1

* Updated documentation.
* Updated session handling.

Version 1.0.0

* First version.
