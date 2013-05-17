using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Util;

namespace AppLifecycle
{
	[Activity (Label = "Activity Life", MainLauncher = true)]
	public class MainActivity : Activity
	{
		// declarations
		protected readonly string logTag = "MainActivity";
		protected EventHandler<UpdatingEventArgs> updateHandler;
		protected Button displayAlertDialogButton;
		protected TextView updateStatusText;
		protected AlertDialog alert;
		protected bool showingAlert;

		/// <summary>
		/// Called when the activity is first instantiated
		/// </summary>
		/// <param name="bundle">Bundle.</param>
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			Log.Debug (logTag, "MainActivity.OnCreate");

			// Set our view from the "main" layout resource
			SetContentView (Resource.Layout.Main);

			// get our activity controls
			updateStatusText = FindViewById<TextView> (Resource.Id.UpdateStatusText);
			displayAlertDialogButton = FindViewById<Button> (Resource.Id.DisplayAlertDialogButton);

			// it's safe to wire up this event handler in OnCreate because it's only referenced by this activity
			displayAlertDialogButton.Click += (sender, e) => {
				this.ShowAlertDialog();
			};

			// if we've stored the showingProgress bool, set it
			if(bundle != null)
				this.showingAlert = bundle.GetBoolean("showingAlert");
		}

		/// <summary>
		/// Because this is the only method guaranteed to be called when an activity
		/// is being activated (after the initilization), we need to make sure that 
		/// we undo anything that we might have done in OnPause.
		/// </summary>
		protected override void OnResume ()
		{
			Log.Debug (logTag, "MainActivity.OnResume");
			base.OnResume ();

			// wire up our external handlers
			this.AddHandlers();

			// if we're supposed to be showing the dialog, show it. this could 
			// happen on an application pause
			if(this.showingAlert) {
				Log.Debug (logTag, "MainActivity. showingAlert is true");
				this.ShowAlertDialog();
			}
		}

		/// <summary>
		/// pause can happen if something is brought in front of the activity, partially obscuring
		/// it and preventing user interaction. need to remove any external event handlers, dismiss
		/// dialogs, pause UI updates, etc.
		/// </summary>
		protected override void OnPause ()
		{
			Log.Debug (logTag, "MainActivity.OnPause");
			base.OnPause ();

			// remove our external event handlers
			this.RemoveHandlers ();

			// if we don't dismiss this, and it's shown, it'll get leaked because it's 
			// actually tied to a window context, rather than the Activity.
			if (alert != null)
				alert.Dismiss();
		}

		/// <summary>
		/// called before the activity is destroyed, gives an opportunity for us to save primitive
		/// data in the bundle, which will be given back to the activity when it gets created again.
		/// </summary>
		protected override void OnSaveInstanceState (Bundle outState)
		{
			Log.Debug (logTag, "MainActivity.OnSaveInstanceState");
			base.OnSaveInstanceState (outState);
			// save whether or not the alert is shown, so in the case that we're getting rotated 
			// (or other configuration change) and the activity is getting destroyed and recreated,
			// we can make it still shows
			outState.PutBoolean("showingAlert", this.showingAlert);
		}

		/// <summary>
		/// wire up our handlers to external events
		/// </summary>
		protected void AddHandlers()
		{
			Log.Debug (logTag, "MainActivity.AddHandlers");
			// this handler needs to be cleaned up during OnPause, because the Updated event is 
			// on an external class. as such, it keeps a reference to this activity. if this activity
			// is destroyed by Dalvik, Xamarin will keep a reference to it around on our side, 
			// which will effectively be a dead activity reference.
			updateHandler = (object s, UpdatingEventArgs args) => {
				Console.WriteLine ("Updated event: " + args.Message);
				this.RunOnUiThread (() => {
					this.updateStatusText.Text = args.Message;
				});
			};
			App.Current.Updated += updateHandler;
		}

		/// <summary>
		/// remove our handlers to external events
		/// </summary>
		protected void RemoveHandlers()
		{
			Log.Debug (logTag, "MainActivity.RemoveHandlers");
			App.Current.Updated -= updateHandler;
		}

		/// <summary>
		/// just a helper method to build and show our alert dialog
		/// </summary>
		protected void ShowAlertDialog()
		{
			Log.Debug (logTag, "MainActivity.ShowAlertDialog");
			alert = new AlertDialog.Builder ( this).Create();
			alert.SetMessage ("An AlertDialog! Don't forget to clean me up!");
			alert.SetTitle ("Hey Cool!");
			alert.SetButton ("Ohkaay!", (s,e) => {
				this.showingAlert = false;
				alert.Dismiss();
			});
			alert.Show();
			this.showingAlert = true;
		}
	}
}


