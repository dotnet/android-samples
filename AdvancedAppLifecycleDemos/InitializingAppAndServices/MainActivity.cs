using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using AppLifecycle.AppLayer;
using AppLifecycle.Services;
using Android.Util;

namespace AppLifecycle
{
	[Activity (Label = "Multi Init", MainLauncher = true)]
	public class MainActivity : ActivityBase
	{
		protected int count;

		protected TextView updateStatusText1;
		protected TextView updateStatusText2;

		EventHandler<UpdatingEventArgs> service1UpdateHandler;
		EventHandler<UpdatingEventArgs> service2UpdateHandler;

		protected bool subscribed = false;
		protected object locker = new object();

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			// Set our view from the "main" layout resource
			SetContentView (Resource.Layout.Main);

			// Get our button from the layout resource,
			// and attach an event to it
			Button button = FindViewById<Button> (Resource.Id.myButton);
			
			button.Click += delegate {
				button.Text = string.Format ("{0} clicks!", count++);
			};

			updateStatusText1 = FindViewById<TextView> (Resource.Id.UpdateStatusText1);
			updateStatusText2 = FindViewById<TextView> (Resource.Id.UpdateStatusText2);
		}

		// this allows the scren to update, even if the app is backgrounded
		// or rotated
		protected override void OnResume ()
		{
			base.OnResume ();

			//GC.Collect ();

			Console.WriteLine ("MainActivity.OnResume");
			if (App.Current.IsInitialized)
				SubscribeToUpdates ();
		}

		protected override void OnPause ()
		{
			Console.WriteLine ("MainActivity.OnPause");
			base.OnPause ();
			this.UnsubscribeToUpdates ();
		}

		protected override void FinishedInitializing ()
		{
			Console.WriteLine ("MainActivity.FinishedInitializing");

			// put anything here that you want to run after the application is fully initialized
			Console.WriteLine ("MainActivity.Starting Updates");
			App.Current.Service1.StartUpdating ();
			App.Current.Service2.StartUpdating ();
			this.SubscribeToUpdates ();
		}


		protected void SubscribeToUpdates()
		{
			lock(this.locker) {
				// if the app gets rotated while initializing, this could get called twice
				if (this.subscribed) {
					Console.WriteLine ("MainActivity.SusbcribeToUpdates, Already Subscribed");
					return;
				}

				Console.WriteLine ("MainActivity.SubscribeToUpdates");

				service1UpdateHandler = (object s, UpdatingEventArgs args) => {
					Console.WriteLine ("Updated1 event: " + args.Message);
					this.RunOnUiThread (() => {
						this.updateStatusText1.Text = args.Message;
					});
				};
				service2UpdateHandler = (object s, UpdatingEventArgs args) => {
					Console.WriteLine ("Updated2 event: " + args.Message);
					this.RunOnUiThread (() => {
						this.updateStatusText2.Text = args.Message;
					});
				};

				App.Current.Service1.Updated += service1UpdateHandler;
				App.Current.Service2.Updated += service2UpdateHandler;

				this.subscribed = true;
			}
		}

		/// <summary>
		/// if we don't remove these handlers, then the runtime will keep a reference to
		/// a dead activity around (activity gets destroyed on the java side)
		/// </summary>
		protected void UnsubscribeToUpdates()
		{
			Console.WriteLine ("MainActivity.UnsubscribeToUpdates");
			if(App.Current.Service1 != null)
				App.Current.Service1.Updated -= service1UpdateHandler;
			if(App.Current.Service2 != null)
				App.Current.Service2.Updated -= service2UpdateHandler;
		}

	}
}


