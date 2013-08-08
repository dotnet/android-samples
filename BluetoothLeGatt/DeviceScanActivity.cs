/*
 * Copyright (C) 2013 The Android Open Source Project
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
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Bluetooth;
using System.Collections.Generic;
using System.Threading;

namespace BluetoothLeGatt
{
	/**
 	* Activity for scanning and displaying available Bluetooth LE devices.
 	*/
	[Activity (Label = "BLE Sample",  MainLauncher = true)]
	public class DeviceScanActivity : ListActivity, BluetoothAdapter.ILeScanCallback
	{
		LeDeviceListAdapter mLeDeviceListAdapter;
		BluetoothAdapter mBluetoothAdapter;
		bool mScanning;
		Handler mHandler;

		static readonly int REQUEST_ENABLE_BT = 1;
		// Stops scanning after 10 seconds.
		static readonly long SCAN_PERIOD = 10000;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			ActionBar.SetTitle (Resource.String.title_devices);

			mHandler = new Handler ();

			// Use this check to determine whether BLE is supported on the device.  Then you can
			// selectively disable BLE-related features.
			if (!PackageManager.HasSystemFeature (Android.Content.PM.PackageManager.FeatureBluetoothLe)) {
				Toast.MakeText (this, Resource.String.ble_not_supported, ToastLength.Short).Show ();
				Finish ();
			}

			// Initializes a Bluetooth adapter.  For API level 18 and above, get a reference to
			// BluetoothAdapter through BluetoothManager.
			BluetoothManager bluetoothManager = (BluetoothManager) GetSystemService (Context.BluetoothService);
			mBluetoothAdapter = bluetoothManager.Adapter;

			// Checks if Bluetooth is supported on the device.
			if (mBluetoothAdapter == null) {
				Toast.MakeText (this, Resource.String.error_bluetooth_not_supported, ToastLength.Short).Show();
				Finish();
				return;
			}
		}

		public override bool OnCreateOptionsMenu (IMenu menu)
		{
			MenuInflater.Inflate (Resource.Menu.main, menu);
			if (!mScanning) {
				menu.FindItem (Resource.Id.menu_stop).SetVisible (false);
				menu.FindItem (Resource.Id.menu_scan).SetVisible (true);
				menu.FindItem (Resource.Id.menu_refresh).SetActionView (null);
			} else {
				menu.FindItem (Resource.Id.menu_stop).SetVisible (true);
				menu.FindItem (Resource.Id.menu_scan).SetVisible (false);
				menu.FindItem (Resource.Id.menu_refresh).SetActionView (
					Resource.Layout.actionbar_indeterminate_progress);
			}
			return true;
		}

		public override bool OnOptionsItemSelected (IMenuItem item)
		{
			switch (item.ItemId) {
			case Resource.Id.menu_scan:
				mLeDeviceListAdapter.Clear ();
				ScanLeDevice (true);
				break;

			case Resource.Id.menu_stop:
				ScanLeDevice (false);
				break;
			}
			return true;
		}

		protected override void OnResume ()
		{
			base.OnResume ();

			// Ensures Bluetooth is enabled on the device.  If Bluetooth is not currently enabled,
			// fire an intent to display a dialog asking the user to grant permission to enable it.
			if (!mBluetoothAdapter.IsEnabled) {
				if (!mBluetoothAdapter.IsEnabled) {
					Intent enableBtIntent = new Intent (BluetoothAdapter.ActionRequestEnable);
					StartActivityForResult (enableBtIntent, REQUEST_ENABLE_BT);
				}
			}

			// Initializes list view adapter.
			mLeDeviceListAdapter = new LeDeviceListAdapter (this);
			ListAdapter = mLeDeviceListAdapter;
			ScanLeDevice (true);
		}

		protected override void OnActivityResult (int requestCode, Result resultCode, Intent data)
		{
			// User chose not to enable Bluetooth.
			if (requestCode == REQUEST_ENABLE_BT && resultCode == Result.Canceled) {
				Finish ();
				return;
			}

			base.OnActivityResult (requestCode, resultCode, data);
		}

		protected override void OnPause ()
		{
			base.OnPause ();
			ScanLeDevice (false);
			mLeDeviceListAdapter.Clear ();
		}

		protected override void OnListItemClick (ListView l, View v, int position, long id)
		{
			BluetoothDevice device = mLeDeviceListAdapter.GetDevice (position);
			if (device == null) 
				return;

			Intent intent = new Intent (this, typeof (DeviceControlActivity));
			intent.PutExtra (DeviceControlActivity.EXTRAS_DEVICE_NAME, device.Name);
			intent.PutExtra (DeviceControlActivity.EXTRAS_DEVICE_ADDRESS, device.Address);
			if (mScanning) {
				mBluetoothAdapter.StopLeScan (this);
				mScanning = false;
			}
			StartActivity (intent);
		}

		void ScanLeDevice (bool enable)
		{
			if (enable) {
				// Stops scanning after a pre-defined scan period.
				mHandler.PostDelayed (new Action (delegate {
					mScanning = false;
					mBluetoothAdapter.StopLeScan (this);
					InvalidateOptionsMenu ();
				}), SCAN_PERIOD);

				mScanning = true;
				mBluetoothAdapter.StartLeScan (this);
			} else {
				mScanning = false;
				mBluetoothAdapter.StopLeScan (this);
			}
			InvalidateOptionsMenu();
		}

		// Device scan callback.
		public void OnLeScan (BluetoothDevice device, int rssi, byte[] scanRecord)
		{
			RunOnUiThread (new Action (delegate {
				mLeDeviceListAdapter.AddDevice (device);
				mLeDeviceListAdapter.NotifyDataSetChanged();
			}));
		}
	}

	// Adapter for holding devices found through scanning.
	class LeDeviceListAdapter : BaseAdapter 
	{
		List <BluetoothDevice> mLeDevices;
		LayoutInflater mInflator;
		Context context;

		public LeDeviceListAdapter (Context c) 
		{
			context = c;
			mLeDevices = new List <BluetoothDevice> ();
			mInflator = LayoutInflater.From (context);

		}

		public void AddDevice (BluetoothDevice device) 
		{
			if (!mLeDevices.Contains (device)) {
				mLeDevices.Add (device);
			}
		}

		public BluetoothDevice GetDevice (int position) 
		{
			return mLeDevices [position];
		}

		public void Clear ()
		{
			mLeDevices.Clear ();
		}

		public override int Count {
			get {
				return mLeDevices.Count;
			}
		}

		public override Java.Lang.Object GetItem (int position)
		{
			return mLeDevices [position];
		}

		public override long GetItemId (int position)
		{
			return position;
		}

		public override View GetView (int position, View convertView, ViewGroup parent)
		{
			ViewHolder viewHolder;

			// General ListView optimization code.
			if (convertView == null) {
				convertView = mInflator.Inflate (Resource.Layout.listitem_device, null);
				viewHolder = new ViewHolder ();
				viewHolder.DeviceAddress = convertView.FindViewById <TextView>  (Resource.Id.device_address);
				viewHolder.DeviceName = convertView.FindViewById <TextView> (Resource.Id.device_name);
				convertView.Tag = viewHolder;
			} else {
				viewHolder = (ViewHolder) convertView.Tag;
			}

			BluetoothDevice device = mLeDevices [position];
			String deviceName = device.Name;
			if (deviceName != null && deviceName.Length > 0)
				viewHolder.DeviceName.Text = deviceName;
			else
				viewHolder.DeviceName.SetText (Resource.String.unknown_device);

			viewHolder.DeviceAddress.Text = device.Address;

			return convertView;
		}

		class ViewHolder : Java.Lang.Object
		{
			public TextView DeviceName { get; set; }
			public TextView DeviceAddress { get; set; }
		}
	}
}

