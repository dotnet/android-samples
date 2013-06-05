/*
 * Copyright (C) 2011 Jake Wharton
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
using System;
using System.IO;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Database;
using Android.OS;
using Android.Provider;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Support.V4.Widget;
using Xamarin.ActionbarSherlockBinding;
using Xamarin.ActionbarSherlockBinding.App;
using Xamarin.ActionbarSherlockBinding.Views;
using Xamarin.ActionbarSherlockBinding.Widget;
using SherlockWindow = Xamarin.ActionbarSherlockBinding.Views.Window;

namespace Mono.ActionbarsherlockTest
{
	[Activity (Name = "mono.actionbarsherlocktest.Progress", Label = "@string/progress")]
	[IntentFilter (new string [] { Intent.ActionMain },
		Categories = new string [] { Constants.DemoCategory })]
	public class Progress : SherlockActivity
	{
		Handler mHandler = new Handler ();
		Runnable mProgressRunner;

		public Progress ()
		{
			mProgressRunner = new Runnable (Run);
		}
		
		public void Run ()
		{
			mProgress += 2;

			//Normalize our progress along the progress bar's scale
			int progress = (SherlockWindow.ProgressEnd - SherlockWindow.ProgressStart) / 100 * mProgress;
			SetSupportProgress (progress);

			if (mProgress < 100) {
				mHandler.PostDelayed (mProgressRunner, 50);
			}
		}

		class Runnable : Java.Lang.Object, Java.Lang.IRunnable
		{
			Action action;
			public Runnable (Action action)
			{
				this.action = action;
			}
			public void Run ()
			{
				action ();
			}
		}

		private int mProgress = 100;

		protected override void OnCreate (Bundle savedInstanceState)
		{
			SetTheme (SampleList.THEME); //Used for theme switching in samples
			base.OnCreate (savedInstanceState);

			//This has to be called before setContentView and you must use the
			//class in com.actionbarsherlock.view and NOT android.view
			RequestWindowFeature (WindowFeatures.Progress);

			SetContentView (Resource.Layout.progress);

			FindViewById (Resource.Id.go).Click += (object sender, EventArgs e) => {
				if (mProgress == 100) {
					mProgress = 0;
					mProgressRunner.Run ();
				}
			};
		}
	}
}

