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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Bluetooth;
using Android.Util;

namespace BluetoothLeGatt
{
	/**
 	* For a given BLE device, this Activity provides the user interface to connect, display data,
 	* and display GATT services and characteristics supported by the device.  The Activity
 	* communicates with {@code BluetoothLeService}, which in turn interacts with the
 	* Bluetooth LE API.
 	*/
	[Activity (Label = "DeviceControlActivity")]			
	public class DeviceControlActivity : Activity
	{
		public readonly static String TAG = typeof (DeviceControlActivity).Name;

		public static readonly String EXTRAS_DEVICE_NAME = "DEVICE_NAME";
		public static readonly String EXTRAS_DEVICE_ADDRESS = "DEVICE_ADDRESS";

		TextView mConnectionState;
		TextView mDataField;
		String mDeviceName;
		public static String mDeviceAddress;
		ExpandableListView mGattServicesList;
		public static BluetoothLeService mBluetoothLeService;
		List <IList <BluetoothGattCharacteristic>> mGattCharacteristics =
			new List <IList <BluetoothGattCharacteristic>> ();
		public static bool mConnected = false;
		BluetoothGattCharacteristic mNotifyCharacteristic;

		private readonly String LIST_NAME = "NAME";
		private readonly String LIST_UUID = "UUID";

		private ServiceManager mServiceManager;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			SetContentView (Resource.Layout.gatt_services_characteristics);

			mServiceManager = new ServiceManager (this);

			Intent intent = Intent;
			mDeviceName = intent.GetStringExtra (EXTRAS_DEVICE_NAME);
			mDeviceAddress = intent.GetStringExtra (EXTRAS_DEVICE_ADDRESS);

			// Sets up UI references.
			(FindViewById <TextView> (Resource.Id.device_address)).Text = mDeviceAddress;
			mGattServicesList = FindViewById <ExpandableListView> (Resource.Id.gatt_services_list) ;

			mGattServicesList.ChildClick += delegate(object sender, ExpandableListView.ChildClickEventArgs e) {

				if (mGattCharacteristics != null) {
					var groupPosition = mGattCharacteristics [e.GroupPosition];
					BluetoothGattCharacteristic characteristic = groupPosition [e.ChildPosition];
					var charaProp = characteristic.Properties;

					if ((charaProp & GattProperty.Read) > 0) {
						// If there is an active notification on a characteristic, clear
						// it first so it doesn't update the data field on the user interface.
						if (mNotifyCharacteristic != null) {
							mBluetoothLeService.SetCharacteristicNotification (
								mNotifyCharacteristic, false);
							mNotifyCharacteristic = null;
						}
						mBluetoothLeService.ReadCharacteristic (characteristic);
					}
					if ((charaProp & GattProperty.Notify) > 0) {
						mNotifyCharacteristic = characteristic;
						mBluetoothLeService.SetCharacteristicNotification (characteristic, true);
					}
				}
			};

			mConnectionState = FindViewById <TextView> (Resource.Id.connection_state);
			mDataField = FindViewById <TextView> (Resource.Id.data_value);

			ActionBar.Title = mDeviceName;
			ActionBar.SetDisplayHomeAsUpEnabled (true);
			Intent gattServiceIntent = new Intent(this, typeof (BluetoothLeService));
			BindService (gattServiceIntent, mServiceManager, Bind.AutoCreate);
		}

		protected override void OnResume ()
		{
			base.OnResume ();

			RegisterReceiver (mServiceManager, MakeGattUpdateIntentFilter ());
			if (mBluetoothLeService != null) {
				bool result = mBluetoothLeService.Connect (mDeviceAddress);
				Log.Debug (TAG, "Connect request result=" + result);
			}
		}

		protected override void OnPause ()
		{
			base.OnPause ();
			UnregisterReceiver (mServiceManager);
		}

		protected override void OnDestroy ()
		{
			base.OnDestroy ();
			UnbindService (mServiceManager);
			mBluetoothLeService = null;
		}

		public override bool OnCreateOptionsMenu (IMenu menu)
		{
			MenuInflater.Inflate (Resource.Menu.gatt_services, menu);
			if (mConnected) {
				menu.FindItem (Resource.Id.menu_connect).SetVisible (false);
				menu.FindItem (Resource.Id.menu_disconnect).SetVisible (true);
			} else {
				menu.FindItem (Resource.Id.menu_connect).SetVisible (true);
				menu.FindItem (Resource.Id.menu_disconnect).SetVisible (false);
			}
			return true;
		}

		public override bool OnOptionsItemSelected (IMenuItem item)
		{
			switch (item.ItemId) {
			case Resource.Id.menu_connect:
				mBluetoothLeService.Connect (mDeviceAddress);
				return true;

			case Resource.Id.menu_disconnect:
				mBluetoothLeService.Disconnect ();
				return true;

			case Android.Resource.Id.Home:
				OnBackPressed();
				return true;
			}

			return base.OnOptionsItemSelected (item);
		}

		public void ClearUI ()
		{
			mGattServicesList.SetAdapter ((SimpleExpandableListAdapter) null);
			mDataField.SetText (Resource.String.no_data);
		}

		public void UpdateConnectionState (int resourceId)
		{
			RunOnUiThread (new Action (delegate {
				mConnectionState.SetText (resourceId);
			}));
		}

		public void DisplayData (String data)
		{
			if (data != null) {
				mDataField.Text = data;
			}
		}

		// Demonstrates how to iterate through the supported GATT Services/Characteristics.
		// In this sample, we populate the data structure that is bound to the ExpandableListView
		// on the UI.
		public void DisplayGattServices (IList<BluetoothGattService> gattServices)
		{
			if (gattServices == null) 
				return;

			String uuid = null;
			String unknownServiceString = Resources.GetString (Resource.String.unknown_service);
			String unknownCharaString = Resources.GetString (Resource.String.unknown_characteristic);
			IList <IDictionary <string, object>> gattServiceData = new JavaList <IDictionary <string, object>> ();
			IList <IList <IDictionary <string, object>>> gattCharacteristicData
			= new JavaList <IList <IDictionary <string, object>>> ();
			mGattCharacteristics = new List <IList <BluetoothGattCharacteristic>> ();

			// Loops through available GATT Services.
			foreach (BluetoothGattService gattService in gattServices) {
				IDictionary <string, object> currentServiceData = new JavaDictionary <string, object>();
				uuid = gattService.Uuid.ToString ();
				currentServiceData.Add (LIST_NAME, SampleGattAttributes.Lookup (uuid, unknownServiceString));
				currentServiceData.Add (LIST_UUID, uuid);
				gattServiceData.Add (currentServiceData);

				IList <IDictionary <string, object>> gattCharacteristicGroupData =new JavaList <IDictionary <string, object>>();
				IList <BluetoothGattCharacteristic> gattCharacteristics =gattService.Characteristics;
				IList <BluetoothGattCharacteristic> charas =new List<BluetoothGattCharacteristic> ();

				// Loops through available Characteristics.
				foreach (BluetoothGattCharacteristic gattCharacteristic in gattCharacteristics) {
					charas.Add (gattCharacteristic);
					IDictionary <string, object> currentCharaData = new JavaDictionary <string, object>();
					uuid = gattCharacteristic.Uuid.ToString();
					currentCharaData.Add (LIST_NAME, SampleGattAttributes.Lookup(uuid, unknownCharaString));
					currentCharaData.Add (LIST_UUID, uuid);
					gattCharacteristicGroupData.Add (currentCharaData);
				}
				mGattCharacteristics.Add (charas);
				gattCharacteristicData.Add (gattCharacteristicGroupData);
			}
			var groupData=(IList<IDictionary<string,object>>) gattServiceData;
			var childData =(IList <IList <IDictionary <string, object>>>) gattCharacteristicData;
			IExpandableListAdapter gattServiceAdapter = new SimpleExpandableListAdapter (
				this,
				groupData,
				Android.Resource.Layout.SimpleExpandableListItem2,
				new String[] {LIST_NAME, LIST_UUID},
				new int[] { Android.Resource.Id.Text1, Android.Resource.Id.Text2 },
				childData,
				Android.Resource.Layout.SimpleExpandableListItem2,
				new String[] {LIST_NAME, LIST_UUID},
				new int[] { Android.Resource.Id.Text1, Android.Resource.Id.Text2 }
			);

			mGattServicesList.SetAdapter (gattServiceAdapter);
		}

		private static IntentFilter MakeGattUpdateIntentFilter () 
		{
			IntentFilter intentFilter = new IntentFilter();
			intentFilter.AddAction (BluetoothLeService.ACTION_GATT_CONNECTED);
			intentFilter.AddAction (BluetoothLeService.ACTION_GATT_DISCONNECTED);
			intentFilter.AddAction (BluetoothLeService.ACTION_GATT_SERVICES_DISCOVERED);
			intentFilter.AddAction (BluetoothLeService.ACTION_DATA_AVAILABLE);
			return intentFilter;
		}
	}

	// Code to manage Service lifecycle.
	// Handles various events fired by the Service.
	// ACTION_GATT_CONNECTED: connected to a GATT server.
	// ACTION_GATT_DISCONNECTED: disconnected from a GATT server.
	// ACTION_GATT_SERVICES_DISCOVERED: discovered GATT services.
	// ACTION_DATA_AVAILABLE: received data from the device.  This can be a result of read
	//                        or notification operations.
	class ServiceManager : BroadcastReceiver, IServiceConnection
	{   
		DeviceControlActivity DCActivity;

		public ServiceManager (DeviceControlActivity dca)
		{
			DCActivity = dca;
		}

		public void OnServiceConnected (ComponentName componentName, IBinder service)
		{
			DeviceControlActivity.mBluetoothLeService = ((BluetoothLeService.LocalBinder) service).GetService ();
			if (!DeviceControlActivity.mBluetoothLeService.Initialize ()) {
				Log.Error (DeviceControlActivity.TAG, "Unable to initialize Bluetooth");
				DCActivity.Finish ();
			}
			// Automatically connects to the device upon successful start-up initialization.
			DeviceControlActivity.mBluetoothLeService.Connect (DeviceControlActivity.mDeviceAddress);
		}

		public void OnServiceDisconnected (ComponentName componentName)
		{
			DeviceControlActivity.mBluetoothLeService = null;
		}

		public override void OnReceive (Context context, Intent intent)
		{
			String action = intent.Action;

			if (BluetoothLeService.ACTION_GATT_CONNECTED == action) {
				DeviceControlActivity.mConnected = true;
				DCActivity.UpdateConnectionState (Resource.String.connected);
				DCActivity.InvalidateOptionsMenu ();

			} else if (BluetoothLeService.ACTION_GATT_DISCONNECTED == action) {
				DeviceControlActivity.mConnected = false;
				DCActivity.UpdateConnectionState(Resource.String.disconnected);
				DCActivity.InvalidateOptionsMenu();
				DCActivity.ClearUI ();

			} else if (BluetoothLeService.ACTION_GATT_SERVICES_DISCOVERED == action) {
				// Show all the supported services and characteristics on the user interface.
				DCActivity.DisplayGattServices (DeviceControlActivity.mBluetoothLeService.GetSupportedGattServices ());

			} else if (BluetoothLeService.ACTION_DATA_AVAILABLE == action) {
				DCActivity.DisplayData (intent.GetStringExtra (BluetoothLeService.EXTRA_DATA));
			}
		}
	}
}

