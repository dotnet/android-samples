namespace AccelerometerPlay;

/*
 * Copyright (C) 2010 The Android Open Source Project
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

// This is an example of using the accelerometer to integrate the device's
// acceleration to a position using the Verlet method. This is illustrated with
// a very simple particle system comprised of a few iron balls freely moving on
// an inclined wooden table. The inclination of the virtual table is controlled
// by the device's accelerometer.

using Android.Hardware;
using Android.Views;

using Java.Interop;

[Activity (Label = "Accelerometer Demo", MainLauncher = true, Icon = "@mipmap/appicon", ScreenOrientation = Android.Content.PM.ScreenOrientation.Landscape)]
public class AccelerometerActivity : Activity
{
	SimulationView? sim_view;
	SensorManager? sensor_manager;
	IWindowManager? window_manager;

	protected override void OnCreate (Bundle? bundle)
	{
		base.OnCreate (bundle);

		// Remove the title bar
		RequestWindowFeature (WindowFeatures.NoTitle);

		// Keep the screen from turning off while we're running
		Window?.AddFlags (WindowManagerFlags.KeepScreenOn);

		// Get an instance of the SensorManager
		sensor_manager = (SensorManager?)GetSystemService (Activity.SensorService) ?? throw new InvalidOperationException ("Unable to obtain the sensor service instance");

		// Get an instance of the WindowManager
		window_manager = GetSystemService (Activity.WindowService).JavaCast<IWindowManager> () ?? throw new InvalidOperationException ("Unable to obtain the window manager service instance");

		// Instantiate our simulation view and set it as the activity's content
		sim_view = new SimulationView (this, sensor_manager, window_manager);
		SetContentView (sim_view);
	}

	protected override void OnResume ()
	{
		base.OnResume ();

		// Start tracking the sensor
		sim_view!.StartSimulation ();
	}

	protected override void OnPause ()
	{
		base.OnPause ();

		// Stop tracking the sensor
		sim_view!.StopSimulation ();
	}
}
