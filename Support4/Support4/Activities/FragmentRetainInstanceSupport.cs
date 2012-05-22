using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Support.V4.App;
using Java.Lang;

namespace Support4
{
	[Activity (Label = "@string/fragment_retain_instance_support")]
	[IntentFilter (new[]{Intent.ActionMain}, Categories = new[]{ "mono.support4demo.sample" })]	
	public class FragmentRetainInstanceSupport : FragmentActivity
	{
		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);
			
			// First time init, create the UI.
			if (savedInstanceState == null) {
				SupportFragmentManager.BeginTransaction ().Add (Android.Resource.Id.Content, new UiFragment ()).Commit ();
			}
		}
		
		protected class UiFragment : Fragment
		{
			RetainedFragment workFragment;
			
			public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
			{
				var v = inflater.Inflate (Resource.Layout.fragment_retain_instance, container, false);

				// Watch for button clicks.
				var button = v.FindViewById<Button> (Resource.Id.restart);
				button.Click += (sender, e) => {
					//workFragment.Restart ();
				};
	
				return v;
			}
			
			public override void OnActivityCreated (Bundle savedInstanceState)
			{
				base.OnActivityCreated (savedInstanceState);
				
				var fm = FragmentManager;
	
				// Check to see if we have retained the worker fragment.
				workFragment = (RetainedFragment)fm.FindFragmentByTag ("work");
	
				// If not retained (or first time running), we need to create it.
				if (workFragment == null) {
					workFragment = new RetainedFragment ();
					// Tell it who it is working with.
					workFragment.SetTargetFragment (this, 0);
					fm.BeginTransaction ().Add (workFragment, "work").Commit ();
				}
			}
			
		}
		
		protected class RetainedFragment : Fragment
		{
			ProgressBar progressBar;
			int position;
			bool ready = false;
			bool quiting = false;
			MyThread myThread;
			
			public override void OnCreate (Bundle p0)
			{
				base.OnCreate (p0);
				
				// Tell the framework to try to keep this fragment around
				// during a configuration change.
				RetainInstance = true;
	
				// Start up the worker thread.
				myThread = new MyThread (this);
				myThread.Start ();
			}
			
			public override void OnActivityCreated (Bundle savedInstanceState)
			{
				base.OnActivityCreated (savedInstanceState);
				
				// Retrieve the progress bar from the target's view hierarchy.
				progressBar = (ProgressBar)TargetFragment.View.FindViewById (Resource.Id.progress_horizontal);
				// We are ready for our thread to go.
				lock (myThread) {
					ready = true;
					myThread.Notify ();
				}
			}
			
			public override void OnDestroy ()
			{
				
				// Make the thread go away.
				lock (myThread) {
					ready = false;
					quiting = true;
					myThread.Notify ();
				}
				
				base.OnDestroy ();
			}
			
			public override void OnDetach ()
			{
				// This fragment is being detached from its activity.  We need
				// to make sure its thread is not going to touch any activity
				// state after returning from this function.
				lock (myThread) {
					progressBar = null;
					ready = false;
					myThread.Notify ();
				}
				
				base.OnDetach ();
			}
			
			protected class MyThread : Thread
			{
				RetainedFragment _fragment;
				
				public MyThread (RetainedFragment fragment)
				{
					_fragment = fragment;	
				}
				
				public override void Run ()
				{
					int max = 10000;

					// This thread runs almost forever.
					while (true) {
	
						// Update our shared state with the UI.
						lock (_fragment) {
							// Our thread is stopped if the UI is not ready
							// or it has completed its work.
							while (!_fragment.ready || _fragment.position >= max) {
								if (_fragment.quiting) {
									return;
								}
								try {
									Wait ();
								} catch (InterruptedException e) {
								}
							}
	
							// Now update the progress.  Note it is important that
							// we touch the progress bar with the lock held, so it
							// doesn't disappear on us.
							_fragment.position++;
							max = _fragment.progressBar.Max;
							_fragment.progressBar.Progress = _fragment.position;
						}
	
						// Normally we would be doing some work, but put a kludge
						// here to pretend like we are.
						lock (this) {
							try {
								Wait (50);
							} catch (InterruptedException e) {
							}
						}
					}
				}
			}
			
			/**
	         * API for our UI to restart the progress thread.
	         */
			public void Restart ()
			{
				lock (myThread) {
					position = 0;
					myThread.Notify ();
				}
			}
		}
	}
}

