using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Widget;
using CommonSampleLibrary;

namespace MultiWindowPlayground
{
	/// <summary>
	/// Activity that logs all key lifecycle callbacks to Log.
	/// Output is also logged to the UI into a LogFragment through InitializeLogging()
	/// and StopLogging().
	/// </summary>
	[Activity(Label = "LoggingActivity")]
	public abstract class LoggingActivity : AppCompatActivity
	{
		protected string logTag { get { return GetType().Name; } }

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			Log.Debug(logTag, "onCreate");
		}

		public override void OnPostCreate(Bundle savedInstanceState, PersistableBundle persistentState)
		{
			base.OnPostCreate(savedInstanceState, persistentState);
			Log.Debug(logTag, "onPostCreate");
		}

		protected override void OnPause()
		{
			base.OnPause();
			Log.Debug(logTag, "onPause");
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
			Log.Debug(logTag, "onDestroy");
		}

		protected override void OnResume()
		{
			base.OnResume();
			Log.Debug(logTag, "onResume");
		}

		public override void OnConfigurationChanged(Android.Content.Res.Configuration newConfig)
		{
			base.OnConfigurationChanged(newConfig);
			Log.Debug(logTag, string.Format("onConfigurationChanged: {0}", newConfig));
		}

		protected override void OnPostCreate(Bundle savedInstanceState)
		{
			base.OnPostCreate(savedInstanceState);
			Log.Debug(logTag, "onPostCreate");
		}

		protected override void OnStart()
		{
			base.OnStart();
			// Start logging to UI.
			InitializeLogging();
			Log.Debug(logTag, "onStart");
		}

		protected override void OnStop()
		{
			base.OnStop();
			// Stop logging to UI when this activity is stopped.
			StopLogging();
			Log.Debug(logTag, "onStop");
		}

		public override void OnMultiWindowModeChanged(bool isInMultiWindowMode)
		{
			base.OnMultiWindowModeChanged(isInMultiWindowMode);
			Log.Debug(logTag, $"OnMultiWindowModeChanged: {isInMultiWindowMode}");
		}

		/// <summary>
		/// Sets up targets to receive log data.
		/// </summary>
		public void InitializeLogging()
		{
			// Using Log, front-end to the logging chain, emulates android.util.log method signatures.
			// Wraps Android's native log framework
			var logWrapper = new LogWrapper();
			Log.LogNode = logWrapper;

			// Filter strips out everything except the message text.
			var msgFilter = new MessageOnlyLogFilter();
			logWrapper.NextNode = msgFilter;

			// On screen logging via a fragment with a TextView.
			var logFragment = (LogFragment) FragmentManager.FindFragmentById(Resource.Id.log_fragment);
			msgFilter.NextNode = logFragment.LogView;
		}

		public void StopLogging()
		{
			Log.LogNode = null;
		}

		/// <summary>
		/// Set the description text if a TextView with the id description is available.
		/// </summary>
		protected void SetDescription(int textId)
		{
			var description = (TextView)FindViewById(Resource.Id.description);
			if (description != null)
				description.SetText(textId);
		}

		/// <summary>
		/// Set the background color for the description text.
		/// </summary>
		protected void SetBackgroundColor(int colorId)
		{
			var scrollView = FindViewById(Resource.Id.scrollview);
			if (scrollView != null)
				scrollView.SetBackgroundResource(colorId);
		}
	}
}

