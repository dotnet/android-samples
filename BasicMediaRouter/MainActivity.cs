/*
 * Copyright 2013 The Android Open Source Project
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *       http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

/**
 * <p>
 * This sample demonstrates the use of the MediaRouter API to show content on a
 * secondary display using a {@link Presentation}.
 * </p>
 * <p>
 * The activity uses the {@link MediaRouter} API to automatically detect when a
 * presentation display is available and to allow the user to control the media
 * routes using a menu item provided by the {@link MediaRouteActionProvider}.
 * When a presentation display is available a {@link Presentation} (implemented
 * as a {@link SamplePresentation}) is shown on the preferred display. A button
 * toggles the background color of the secondary screen to show the interaction
 * between the primary and secondary screens.
 * </p>
 * <p>
 * This sample requires an HDMI or Wifi display. Alternatively, the
 * "Simulate secondary displays" feature in Development Settings can be enabled
 * to simulate secondary displays.
 * </p>
 *
 * @see Presentation
 * @see MediaRouter
 */
using Android.Media;

namespace BasicMediaRouter
{
	[Activity (Label = "BasicMediaRouter", MainLauncher = true, Theme="@style/AppTheme")]
	public class MainActivity : Activity
	{
		MediaRouter mMediaRouter;
		MediaRouterCallback mMediaRouterCallback;

		// Active Presentation, set to null if no secondary screen is enabled
		SamplePresentation mPresentation;

		// Views used to display status information on the primary screen
		TextView mTextStatus;
		Button mButton;

		// selected color index
		int mColor = 0;

		// background colors
		int[] mColors;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			SetContentView (Resource.Layout.Main);

			mMediaRouterCallback = new MediaRouterCallback (this);

			mTextStatus = FindViewById <TextView> (Resource.Id.textStatus);

			// get the list of background colors
			mColors = Resources.GetIntArray (Resource.Array.androidcolors);

			// Enable clicks on the 'change color' button
			mButton = FindViewById <Button> (Resource.Id.button1);
			mButton.Click += delegate {
				ShowNextColor();
			};

			// Get the MediaRouter service
			mMediaRouter = (MediaRouter) GetSystemService (Context.MediaRouterService);
		}

		protected override void OnResume ()
		{
			base.OnResume ();

			// Register a callback for all events related to live video devices
			mMediaRouter.AddCallback (MediaRouteType.LiveVideo, mMediaRouterCallback);

			// Show the 'Not connected' status message
			mButton.Enabled = false;
			mTextStatus.SetText (Resource.String.secondary_notconnected);

			// Update the displays based on the currently active routes
			UpdatePresentation ();
		}

		protected override void OnPause ()
		{
			base.OnPause ();

			// Stop listening for changes to media routes.
			mMediaRouter.RemoveCallback (mMediaRouterCallback);
		}

		protected override void OnStop ()
		{
			base.OnStop ();

			// Dismiss the presentation when the activity is not visible.
			if (mPresentation != null) {
				mPresentation.Dismiss ();
				mPresentation = null;
			}
		}

		/**
     	* Inflates the ActionBar or options menu. The menu file defines an item for
     	* the {@link MediaRouteActionProvider}, which is registered here for all
     	* live video devices using {@link MediaRouter#ROUTE_TYPE_LIVE_VIDEO}.
     	*/
		public override bool OnCreateOptionsMenu (IMenu menu)
		{
			base.OnCreateOptionsMenu (menu);

			MenuInflater.Inflate (Resource.Menu.main, menu);

			// Configure the media router action provider
			var mediaRouteMenuItem = menu.FindItem (Resource.Id.menu_media_route);
			MediaRouteActionProvider mediaRouteActionProvider = (MediaRouteActionProvider) mediaRouteMenuItem.ActionProvider;
			mediaRouteActionProvider.SetRouteTypes (MediaRouteType.LiveVideo);

			return true;
		}

		void ShowNextColor()
		{
			if (mPresentation != null) {
				// a second screen is active and initialized, show the next color
				mPresentation.SetColor (mColors [mColor]);
				mColor = (mColor + 1) % mColors.Length;
			}
		}

		/**
     	* Updates the displayed presentation to enable a secondary screen if it has
     	* been selected in the {@link MediaRouter} for the
     	* {@link MediaRouter#ROUTE_TYPE_LIVE_VIDEO} type. If no screen has been
     	* selected by the {@link MediaRouter}, the current screen is disabled.
     	* Otherwise a new {@link SamplePresentation} is initialized and shown on
     	* the secondary screen.
     	*/
		public void UpdatePresentation ()
		{
			// Get the selected route for live video
			var selectedRoute = mMediaRouter.GetSelectedRoute (MediaRouteType.LiveVideo);

			// Get its Display if a valid route has been selected
			Display selectedDisplay = null;
			if (selectedRoute != null)
				selectedDisplay = selectedRoute.PresentationDisplay;

			/*
			 * Dismiss the current presentation if the display has changed or no new
			 * route has been selected
			 */
			if (mPresentation != null && mPresentation.Display != selectedDisplay) {
				mPresentation.Dismiss ();
				mPresentation = null;
				mButton.Enabled = false;
				mTextStatus.SetText (Resource.String.secondary_notconnected);
			}

			/*
			 * Show a new presentation if the previous one has been dismissed and a
			 * route has been selected.
			 */
			if (mPresentation == null && selectedDisplay != null) {

				// Initialise a new Presentation for the Display
				mPresentation = new SamplePresentation (this, selectedDisplay);
				mPresentation.DismissEvent += delegate (object sender, EventArgs e) {
					if (sender == mPresentation) {
						mPresentation = null;
					}
				};

				// Try to show the presentation, this might fail if the display has
				// gone away in the mean time
				try {
					mPresentation.Show ();
					mTextStatus.Text =  Resources.GetString (Resource.String.secondary_connected, selectedRoute.Name);
					mButton.Enabled = true;
					ShowNextColor ();
				} catch (WindowManagerInvalidDisplayException ex) {
					// Couldn't show presentation - display was already removed
					mPresentation = null;
					Console.WriteLine (ex);
				}
			}
		}
	}
	
	/**
     * Implementing a {@link MediaRouter.Callback} to update the displayed
     * {@link Presentation} when a route is selected, unselected or the
     * presentation display has changed. The provided stub implementation
     * {@link MediaRouter.SimpleCallback} is extended and only
     * {@link MediaRouter.SimpleCallback#onRouteSelected(MediaRouter, int, RouteInfo)}
     * ,
     * {@link MediaRouter.SimpleCallback#onRouteUnselected(MediaRouter, int, RouteInfo)}
     * and
     * {@link MediaRouter.SimpleCallback#onRoutePresentationDisplayChanged(MediaRouter, RouteInfo)}
     * are overridden to update the displayed {@link Presentation} in
     * {@link #updatePresentation()}. These callbacks enable or disable the
     * second screen presentation based on the routing provided by the
     * {@link MediaRouter} for {@link MediaRouter#ROUTE_TYPE_LIVE_VIDEO}
     * streams. @
     */
	class MediaRouterCallback : MediaRouter.SimpleCallback
	{
		MainActivity main;

		public MediaRouterCallback (MainActivity m)
		{
			main = m;
		}

		public override void OnRouteSelected (MediaRouter router, MediaRouteType type, MediaRouter.RouteInfo info)
		{
			main.UpdatePresentation ();
		}

		public override void OnRouteUnselected (MediaRouter router, MediaRouteType type, MediaRouter.RouteInfo info)
		{
			main.UpdatePresentation ();
		}

		public override void OnRoutePresentationDisplayChanged (MediaRouter router, MediaRouter.RouteInfo info)
		{
			main.UpdatePresentation ();
		}
	}
}


