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
using Java.Util;
using Android.Util;

namespace BluetoothLeGatt
{
	/**
 	* Service for managing connection and data communication with a GATT server hosted on a
 	* given Bluetooth LE device.
 	*/
	[Service (Name = "bluetoothlegatt.BluetoothLeService", Enabled = true )]
	public class BluetoothLeService : Service
	{
		public readonly static String TAG = typeof (BluetoothLeService).Name;

		public static BluetoothManager mBluetoothManager;
		public static BluetoothAdapter mBluetoothAdapter;
		public static String mBluetoothDeviceAddress;
		public static BluetoothGatt mBluetoothGatt;
		public static State mConnectionState = State.Disconnected;

		public readonly static String ACTION_GATT_CONNECTED =
			"com.xamarin.bluetooth.le.ACTION_GATT_CONNECTED";
		public readonly static String ACTION_GATT_DISCONNECTED =
			"com.xamarin.bluetooth.le.ACTION_GATT_DISCONNECTED";
		public readonly static String ACTION_GATT_SERVICES_DISCOVERED =
			"com.xamarin.bluetooth.le.ACTION_GATT_SERVICES_DISCOVERED";
		public readonly static String ACTION_DATA_AVAILABLE =
			"com.xamarin.bluetooth.le.ACTION_DATA_AVAILABLE";
		public readonly static String EXTRA_DATA =
			"com.xamarin.bluetooth.le.EXTRA_DATA";

		public readonly static UUID UUID_HEART_RATE_MEASUREMENT =
			UUID.FromString (SampleGattAttributes.HEART_RATE_MEASUREMENT);

		IBinder mBinder;


		public void BroadcastUpdate (String action) 
		{
			Intent intent = new Intent (action);
			SendBroadcast (intent);
		}

		public void BroadcastUpdate (String action, BluetoothGattCharacteristic characteristic) 
		{
			Intent intent = new Intent (action);

			// This is special handling for the Heart Rate Measurement profile.  Data parsing is
			// carried out as per profile specifications:
			// http://developer.bluetooth.org/gatt/characteristics/Pages/CharacteristicViewer.aspx?u=org.bluetooth.characteristic.heart_rate_measurement.xml
			if (UUID_HEART_RATE_MEASUREMENT == (characteristic.Uuid)) {
				GattProperty flag = characteristic.Properties;
				GattFormat format = (GattFormat) (-1);

				if (((int) flag & 0x01) != 0) {
					format = GattFormat.Uint16;
					Log.Debug (TAG, "Heart rate format UINT16.");
				} else {
					format = GattFormat.Uint8;
					Log.Debug (TAG, "Heart rate format UINT8.");
				}

				var heartRate = characteristic.GetIntValue (format, 1);
				Log.Debug (TAG, String.Format ("Received heart rate: {0}", heartRate));
				intent.PutExtra (EXTRA_DATA, heartRate);
			} else {
				// For all other profiles, writes the data formatted in HEX.
				byte[] data = characteristic.GetValue ();

				if (data != null && data.Length > 0) {
					StringBuilder stringBuilder = new StringBuilder (data.Length);
					foreach (byte byteChar in data)
						stringBuilder.Append (String.Format ("{0}02X ", byteChar));
					intent.PutExtra (EXTRA_DATA, Convert.ToBase64String (data) + "\n" + stringBuilder.ToString());
				}
			}

			SendBroadcast (intent);
		}

		public class LocalBinder : Binder 
		{
			BluetoothLeService service;

			public LocalBinder (BluetoothLeService service)
			{
				this.service = service;
			}

			public BluetoothLeService GetService ()
			{
				return service;
			}
		}

		public override IBinder OnBind (Intent intent)
		{
			mBinder = new LocalBinder (this);
			return mBinder;
		}

		public override bool OnUnbind (Intent intent)
		{
			// After using a given device, you should make sure that BluetoothGatt.close() is called
			// such that resources are cleaned up properly.  In this particular example, close() is
			// invoked when the UI is disconnected from the Service.
			Close ();
			return base.OnUnbind (intent);
		}

		/**
     	* Initializes a reference to the local Bluetooth adapter.
     	*
     	* @return Return true if the initialization is successful.
    	 */
		public bool Initialize() 
		{
			// For API level 18 and above, get a reference to BluetoothAdapter through
			// BluetoothManager.
			if (mBluetoothManager == null) {
				mBluetoothManager = (BluetoothManager) GetSystemService (Context.BluetoothService);
				if (mBluetoothManager == null) {
					Log.Error (TAG, "Unable to initialize BluetoothManager.");
					return false;
				}
			}

			mBluetoothAdapter = mBluetoothManager.Adapter;
			if (mBluetoothAdapter == null) {
				Log.Error (TAG, "Unable to obtain a BluetoothAdapter.");
				return false;
			}

			return true;
		}

		/**
    	* Connects to the GATT server hosted on the Bluetooth LE device.
    	*
     	* @param address The device address of the destination device.
     	*
    	* @return Return true if the connection is initiated successfully. The connection result
     	*         is reported asynchronously through the
     	*         {@code BluetoothGattCallback#onConnectionStateChange(android.bluetooth.BluetoothGatt, int, int)}
     	*         callback.
     	*/
		public bool Connect (String address)
		{
			if (mBluetoothAdapter == null || address == null) {
				Log.Warn (TAG, "BluetoothAdapter not initialized or unspecified address.");
				return false;
			}

			// Previously connected device.  Try to reconnect.
			if (mBluetoothDeviceAddress != null && address == mBluetoothDeviceAddress && mBluetoothGatt != null) {
				Log.Debug (TAG, "Trying to use an existing mBluetoothGatt for connection.");
				if (mBluetoothGatt.Connect ()) {
					mConnectionState = State.Connecting;
					return true;
				} else {
					return false;
				}
			}

			BluetoothDevice device = mBluetoothAdapter.GetRemoteDevice (address);
			if (device == null) {
				Log.Warn (TAG, "Device not found.  Unable to connect.");
				return false;
			}
			// We want to directly connect to the device, so we are setting the autoConnect
			// parameter to false.
			mBluetoothGatt = device.ConnectGatt (this, false, new BGattCallback (this));
			Log.Debug (TAG, "Trying to create a new connection.");
			mBluetoothDeviceAddress = address;
			mConnectionState = State.Connecting;
			return true;

		}

		/**
    	* Disconnects an existing connection or cancel a pending connection. The disconnection result
     	* is reported asynchronously through the
     	* {@code BluetoothGattCallback#onConnectionStateChange(android.bluetooth.BluetoothGatt, int, int)}
     	* callback.
     	*/
		public void Disconnect ()
		{
			if (mBluetoothAdapter == null || mBluetoothGatt == null) {
				Log.Warn (TAG, "BluetoothAdapter not initialized");
				return;
			}
			mBluetoothGatt.Disconnect ();
		}

		/**
     	* After using a given BLE device, the app must call this method to ensure resources are
     	* released properly.
     	*/
		public void Close ()
		{
			if (mBluetoothGatt == null) {
				return;
			}
			mBluetoothGatt.Close ();
			mBluetoothGatt = null;
		}

		/**
     	* Request a read on a given {@code BluetoothGattCharacteristic}. The read result is reported
     	* asynchronously through the {@code BluetoothGattCallback#onCharacteristicRead(android.bluetooth.BluetoothGatt, android.bluetooth.BluetoothGattCharacteristic, int)}
     	* callback.
     	*
     	* @param characteristic The characteristic to read from.
     	*/
		public void ReadCharacteristic (BluetoothGattCharacteristic characteristic)
		{
			if (mBluetoothAdapter == null || mBluetoothGatt == null) {
				Log.Warn (TAG, "BluetoothAdapter not initialized");
				return;
			}
			mBluetoothGatt.ReadCharacteristic (characteristic);
		}

		/**
     	* Enables or disables notification on a give characteristic.
     	*
     	* @param characteristic Characteristic to act on.
     	* @param enabled If true, enable notification.  False otherwise.
     	*/
		public void SetCharacteristicNotification (BluetoothGattCharacteristic characteristic, bool enabled)
		{
			if (mBluetoothAdapter == null || mBluetoothGatt == null) {
				Log.Warn (TAG, "BluetoothAdapter not initialized");
				return;
			}
			mBluetoothGatt.SetCharacteristicNotification (characteristic, enabled);

			// This is specific to Heart Rate Measurement.
			if (UUID_HEART_RATE_MEASUREMENT == characteristic.Uuid) {
				BluetoothGattDescriptor descriptor = characteristic.GetDescriptor (
					UUID.FromString (SampleGattAttributes.CLIENT_CHARACTERISTIC_CONFIG));
				descriptor.SetValue (BluetoothGattDescriptor.EnableNotificationValue.ToArray ());
				mBluetoothGatt.WriteDescriptor (descriptor);
			}
		}

		/**
     	* Retrieves a list of supported GATT services on the connected device. This should be
     	* invoked only after {@code BluetoothGatt#discoverServices()} completes successfully.
     	*
     	* @return A {@code List} of supported services.
     	*/
		public IList <BluetoothGattService> GetSupportedGattServices () 
		{
			if (mBluetoothGatt == null) return null;

			return mBluetoothGatt.Services;
		}
	}

	// Implements callback methods for GATT events that the app cares about.  For example,
	// connection change and services discovered.
 	class BGattCallback : BluetoothGattCallback
	{
		BluetoothLeService service;

		public  BGattCallback (BluetoothLeService s)
		{
			service = s;
		}

		public override void OnConnectionStateChange (BluetoothGatt gatt, GattStatus status, ProfileState newState)
		{
			String intentAction;
			if (newState == ProfileState.Connected) {
				intentAction = BluetoothLeService.ACTION_GATT_CONNECTED;
				BluetoothLeService.mConnectionState = State.Connected;
				service.BroadcastUpdate (intentAction);
				Log.Info (BluetoothLeService.TAG, "Connected to GATT server.");
				// Attempts to discover services after successful connection.
				Log.Info (BluetoothLeService.TAG, "Attempting to start service discovery:" +
					BluetoothLeService.mBluetoothGatt.DiscoverServices ());

			} else if (newState == ProfileState.Disconnected) {
				intentAction = BluetoothLeService.ACTION_GATT_DISCONNECTED;
				BluetoothLeService.mConnectionState = State.Disconnected;
				Log.Info (BluetoothLeService.TAG, "Disconnected from GATT server.");
				service.BroadcastUpdate (intentAction);
			}
		}

		public override void OnServicesDiscovered (BluetoothGatt gatt, GattStatus status)
		{
			if (status == GattStatus.Success) {
				service.BroadcastUpdate (BluetoothLeService.ACTION_GATT_SERVICES_DISCOVERED);
			} else {
				Log.Warn (BluetoothLeService.TAG, "onServicesDiscovered received: " + status);
			}
		}

		public override void OnCharacteristicRead (BluetoothGatt gatt, BluetoothGattCharacteristic characteristic, GattStatus status)
		{
			if (status == GattStatus.Success) {
				service.BroadcastUpdate (BluetoothLeService.ACTION_DATA_AVAILABLE, characteristic);
			}
		}

		public override void OnCharacteristicChanged (BluetoothGatt gatt, BluetoothGattCharacteristic characteristic)
		{
			service.BroadcastUpdate (BluetoothLeService.ACTION_DATA_AVAILABLE, characteristic);
		}
	}
}

