/*
 * Copyright (C) 2007 The Android Open Source Project
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

using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Util;
using Android.Widget;

namespace MonoDroid.ApiDemo
{
	public class LocalServiceActivities
	{
		/**
     	* Example of explicitly starting and stopping the local service.
     	* This demonstrates the implementation of a service that runs in the same
     	* process as the rest of the application, which is explicitly started and stopped
     	* as desired.
     	* 
     	* Note that this is implemented as an inner class only keep the sample
     	* all together; typically this code would appear in some separate class.
     	*/
		[Activity (Label = "@string/activity_local_service_controller")]
		[IntentFilter (new[] { Intent.ActionMain },
				Categories = new string[] { ApiDemo.SAMPLE_CATEGORY })]
		public class Controller : Activity
		{
			protected override void OnCreate (Bundle savedInstanceState)
			{
				base.OnCreate (savedInstanceState);
				SetContentView (Resource.Layout.local_service_controller);

				// Watch for button clicks.
				var button = FindViewById<Button>(Resource.Id.start);
				button.Click += delegate {
					StartService (new Intent (this, typeof (LocalService)));
				};
				button = FindViewById<Button>(Resource.Id.stop);
				button.Click += delegate {
					StopService (new Intent (this, typeof (LocalService)));
				};
			}
		}

		// ----------------------------------------------------------------------

		/**
     	* Example of binding and unbinding to the local service.
     	* This demonstrates the implementation of a service which the client will
     	* bind to, receiving an object through which it can communicate with the service.</p>
     	* 
     	* <p>Note that this is implemented as an inner class only keep the sample
     	* all together; typically this code would appear in some separate class.
     	*/
		[Activity (Label = "@string/activity_local_service_binding")]
		[IntentFilter (new[] { Intent.ActionMain },
				Categories = new string[] { ApiDemo.SAMPLE_CATEGORY })]
		public class Binding : Activity
		{
			bool isBound;
			LocalService boundService;
			IServiceConnection connection;

			public Binding ()
			{
				connection = new MyServiceConnection (this);
			}

			protected override void OnCreate (Bundle savedInstanceState)
			{
				base.OnCreate (savedInstanceState);
				SetContentView (Resource.Layout.local_service_binding);

				// Watch for button clicks.
				var button = FindViewById <Button> (Resource.Id.bind);
				button.Click += delegate {
					BindService ();
				};
				button = FindViewById <Button> (Resource.Id.unbind);
				button.Click += delegate {
					UnbindService ();
				};
			}

			void BindService ()
			{
				// Establish a connection with the service.  We use an explicit
				// class name because we want a specific service implementation that
				// we know will be running in our own process (and thus won't be
				// supporting component replacement by other applications).
				base.BindService (new Intent (this, typeof (LocalService)),
							connection, Bind.AutoCreate);
				isBound = true;
			}

			void UnbindService ()
			{
				if (isBound) {
					// Detach our existing connection.
					base.UnbindService (connection);
					isBound = false;
				}
			}

			protected override void OnDestroy ()
			{
				base.OnDestroy ();
				UnbindService ();
			}

			class MyServiceConnection : Java.Lang.Object, IServiceConnection
			{
				Binding self;

				public MyServiceConnection (Binding self)
				{
					this.self = self;
				}

				public void OnServiceConnected (ComponentName className, IBinder service)
				{
					// This is called when the connection with the service has been
					// established, giving us the service object we can use to
					// interact with the service.  Because we have bound to a explicit
					// service that we know is running in our own process, we can
					// cast its IBinder to a concrete class and directly access it.
					self.boundService = ((LocalService.LocalBinder) service).Service;

					// Tell the user about this for our demo.
					Toast.MakeText (self, Resource.String.local_service_connected, ToastLength.Short).Show ();
				}

				public void OnServiceDisconnected (ComponentName className)
				{
					// This is called when the connection with the service has been
					// unexpectedly disconnected -- that is, its process crashed.
					// Because it is running in our same process, we should never
					// see this happen.
					self.boundService = null;
					Toast.MakeText (self, Resource.String.local_service_disconnected, ToastLength.Short).Show ();
				}
			}
		}
	}
}

