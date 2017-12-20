using Android.App;
using Android.OS;
using Android.Views;
using Android.Views.Animations;
using Android.Widget;
using Java.Lang;

namespace TextSwitcher
{
	[Activity(Label = "TextSwitcher", MainLauncher = true)]
	public class MainActivity : Activity, ViewSwitcher.IViewFactory
	{
		Android.Widget.TextSwitcher mSwitcher;
		int mCounter = 0;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			SetContentView(Resource.Layout.Main);

			// Get the TextSwitcher view from the layout
			mSwitcher = (Android.Widget.TextSwitcher)FindViewById(Resource.Id.switcher);

			// BEGIN_INCLUDE(setup)
			// Set the factory used to create TextViews to switch between.
			mSwitcher.SetFactory(this);

			/*
         	* Set the in and out animations. Using the fade_in/out animations
         	* provided by the framework.
         	*/
			var animIn = AnimationUtils.LoadAnimation(this, Android.Resource.Animation.FadeIn);
			var animOut = AnimationUtils.LoadAnimation(this, Android.Resource.Animation.FadeOut);
			mSwitcher.InAnimation = animIn;
			mSwitcher.OutAnimation = animOut;
			// END_INCLUDE(setup)

			/*
	         * Setup the 'next' button. The counter is incremented when clicked and
	         * the new value is displayed in the TextSwitcher. The change of text is
	         * automatically animated using the in/out animations set above.
	         */
			var nextButton = (Button)FindViewById(Resource.Id.button);
			nextButton.Click += (sender, e) => 
			{
				mCounter++;
                // BEGIN_INCLUDE(settext)
                mSwitcher.SetText(String.ValueOf(mCounter));
                // END_INCLUDE(settext)
			};

			// Set the initial text without an animation
			mSwitcher.SetCurrentText(String.ValueOf(mCounter));
		}

		public View MakeView()
		{
			// Create a new TextView
			var t = new TextView(this);
			t.Gravity = GravityFlags.Top | GravityFlags.CenterHorizontal;
			t.SetTextAppearance(this, Android.Resource.Style.TextAppearanceLarge);
			return t;
		}
	}
}

