using System;

using Android.App;
using Android.OS;
using Android.Util;
using Android.Views;
using Android.Widget;
using Android.Content.PM;

namespace Mono.Samples.TexturedCube
{
	[Activity (Label = "@string/app_name", MainLauncher = true, Icon = "@drawable/app_texturedcube",
		ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.KeyboardHidden, LaunchMode = LaunchMode.SingleTask)]
	public class TexturedCubeActivity : Activity
	{
		View mMenuContainer, mSwitchTexture;
		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);

			// Inflate our UI from its XML layout description
			SetContentView (Resource.Layout.main);

			PaintingView glp = FindViewById<PaintingView> (Resource.Id.paintingview);

			// Find the views whose visibility will change
			mMenuContainer = FindViewById (Resource.Id.hidecontainer);
			mSwitchTexture = FindViewById (Resource.Id.switch_texture);
			mSwitchTexture.Click += delegate { glp.SwitchTexture (); };

			// Find our buttons
			Button showButton = FindViewById<Button>(Resource.Id.show);
			Button hideButton = FindViewById<Button> (Resource.Id.hide);

			// Wire each button to a click listener
			showButton.Click += delegate { SetVisibility (ViewStates.Visible); };
			hideButton.Click += delegate { SetVisibility (ViewStates.Gone); };
		}

		void SetVisibility (ViewStates state)
		{
			mSwitchTexture.Visibility = state;
			mMenuContainer.Visibility = state;
		}
	}
}
