using Android.App;
using Android.OS;
using Android.Views;
using Android.Views.Animations;
using Android.Widget;
using CommonSampleLibrary;
using Java.Lang;

namespace TextSwitcher
{
    [Activity(Label = "TextSwitcher", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity, ViewSwitcher.IViewFactory
    {
        public virtual string TAG
        {
            get { return "MainActivity"; }
        }

        Android.Widget.TextSwitcher switcher;
        private int _mCounter = 0;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            switcher = FindViewById<Android.Widget.TextSwitcher>(Resource.Id.Switcher);

            // BEGIN_INCLUDE(setup)
            // Set the factory used to create TextViews to switch between.
            switcher.SetFactory(this);

            /*
             * Set the in and out animations. Using the fade_in/out animations
             * provided by the framework.
             */
            switcher.InAnimation = AnimationUtils.LoadAnimation(this, Android.Resource.Animation.FadeIn);
            switcher.OutAnimation = AnimationUtils.LoadAnimation(this, Android.Resource.Animation.FadeOut);
            // END_INCLUDE(setup)
            
            /*
             * Setup the 'next' button. The counter is incremented when clicked and
             * the new value is displayed in the TextSwitcher. The change of text is
             * automatically animated using the in/out animations set above.
             */
            Button nextButton = (Button)FindViewById<Button>(Resource.Id.Button);

            nextButton.Click += delegate
            {
                // BEGIN_INCLUDE(settext)
                _mCounter++;
                switcher.SetText(String.ValueOf(_mCounter));
                // END_INCLUDE(settext)
            };

            // Set the initial text without an animation
            switcher.SetCurrentText(String.ValueOf(_mCounter));
        }

        protected override void OnStart()
        {
            base.OnStart();
            InitializeLogging();
        }

        public View MakeView()
        {
            // Create a new TextView
            TextView t = new TextView(this);
            t.Gravity = GravityFlags.Top | GravityFlags.CenterHorizontal;
            t.SetTextAppearance(this, Android.Resource.Style.TextAppearanceLarge);
            return t;
        }

        /** Set up targets to receive log data */
        public virtual void InitializeLogging()
        {
            // Using Log, front-end to the logging chain, emulates android.util.log method signatures.
            // Wraps Android's native log framework
            LogWrapper logWrapper = new LogWrapper();
            Log.LogNode = logWrapper;

            Log.Info(TAG, "Ready");
        }
    }
}

