using System;

using Android.App;
using Android.OS;
using Android.Widget;
using Android.Transitions;
using Android.Views;

namespace KitKat
{
	[Activity (Label = "TransitionActivity")]			
	public class TransitionActivity : Activity
	{
		RelativeLayout container;
		Button sceneButton;
		Scene scene1;
		Scene scene2;

		Button button;
		TextView text;
		LinearLayout linear;

		Transition transition;

		bool visible;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			SetContentView (Resource.Layout.Transition);

			#region property animation

			button = FindViewById<Button> (Resource.Id.button);
			text = FindViewById<TextView> (Resource.Id.textView);
			linear = FindViewById<LinearLayout> (Resource.Id.linear);

			if(bundle != null) {
				if(bundle.GetBoolean("visible")) {
					text.Visibility = ViewStates.Visible;
				}
				visible = bundle.GetBoolean("visible");
			}

			// This button demonstrates a simple property animation
			button.Click += (o, e) => {

				// OS version check since transition manager is only available in KitKat+
				var version = BuildVersionCodes.Kitkat;
				if(Build.VERSION.SdkInt >= version) {
					TransitionManager.BeginDelayedTransition (linear);
				}

				// show and hide the text, so we can see the smooth transition
				if(text.Visibility != ViewStates.Visible)
				{
					text.Visibility = ViewStates.Visible;
					visible = true;
				}
				else
				{
					text.Visibility = ViewStates.Invisible;
					visible = false;
				}
			};

			#endregion

			#region scenes

			// the container holds the dynamic content what will be changing when we 
			// change Scenes
			container = FindViewById<RelativeLayout> (Resource.Id.container);
			sceneButton = FindViewById<Button> (Resource.Id.sceneButton);

			// Define the transition and inflate it from the xml
			transition = TransitionInflater.From(this).InflateTransition(Resource.Transition.transition);

			// Define scenes that we're going to use

			// NOTE: There is a known Java bug in GetSceneForLayout (https://code.google.com/p/android/issues/detail?id=62450)
			// It may not work as intended the second time an activity is launched (relaunch or configuration change)
			scene1 = Scene.GetSceneForLayout(container, Resource.Layout.Scene1, this);
			scene2 = Scene.GetSceneForLayout(container, Resource.Layout.Scene2, this);

			scene1.Enter();

			sceneButton.Click += (o, e) => {
				// reserve variables. This keeps the animation going both ways.
				Scene temp = scene2;
				scene2 = scene1;
				scene1 = temp;

				TransitionManager.Go (scene1, transition);
			};

			#endregion

		}

		protected override void OnSaveInstanceState (Bundle outState)
		{
			outState.PutBoolean ("visible", visible);
			base.OnSaveInstanceState (outState);
		}
	}
}

