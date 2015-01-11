using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Support.V4.App;
using Android.Transitions;
using CommonSampleLibrary;

namespace CustomTransitions
{
	public class CustomTransitionFragment : Fragment, View.IOnClickListener
	{
		private const string STATE_CURRENT_SCENE = "current_scene";
		private const string TAG = "CustomTransitionFragment";

		//This is the current index of scenes.
		private int current_scene;

		//These are the Scenes we use.
		private Scene[] scenes;

		//This is the custom Transition used in the sample.
		private Transition transition;

		public CustomTransitionFragment()
		{
		}

		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			return inflater.Inflate (Resource.Layout.fragment_custom_transition, container, false);
		}

		public override void OnViewCreated (View view, Bundle savedInstanceState)
		{
			FrameLayout container = (FrameLayout)view.FindViewById (Resource.Id.container);
			view.FindViewById (Resource.Id.show_next_scene).SetOnClickListener (this);
			if (null != savedInstanceState) {
				current_scene = savedInstanceState.GetInt (STATE_CURRENT_SCENE);
			}

			//Set up the scenes
			scenes = new Scene[] {
				Scene.GetSceneForLayout (container, Resource.Layout.scene1, Activity),
				Scene.GetSceneForLayout (container, Resource.Layout.scene2, Activity),
				Scene.GetSceneForLayout (container, Resource.Layout.scene3, Activity),
			
			};

			//Show the initial Scene
			transition = new ChangeColor ();
			TransitionManager.Go (scenes [current_scene % scenes.Length]);

		}

		public override void OnSaveInstanceState (Bundle outState)
		{
			base.OnSaveInstanceState (outState);
			outState.PutInt (STATE_CURRENT_SCENE, current_scene);
		}

		public void OnClick(View v) 
		{
			switch (v.Id) {
			case Resource.Id.show_next_scene:{
					current_scene = (current_scene + 1) % scenes.Length;
					Log.Info (TAG, "Transitioning to scene #" + current_scene);

					//Pass the custom Transition as second argument for TransitionManager.go
					TransitionManager.Go (scenes [current_scene], transition);
					break;

				}
			}
		}

	}
}

