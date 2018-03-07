/*
 * Copyright (C) 2014 The Android Open Source Project
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
using Android.Bluetooth;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;

namespace com.xamarin.samples.bluetooth.bluetoothchat
{

    /// <summary>
    /// This Activity appears as a dialog. It lists any paired devices and
    /// devices detected in the area after discovery. When a device is chosen
    /// by the user, the MAC address of the device is sent back to the parent
    /// Activity in the result Intent.
    /// </summary>
    [Activity(Label = "@string/select_device",
                Theme = "@android:style/Theme.Holo.Dialog",
                ConfigurationChanges = Android.Content.PM.ConfigChanges.KeyboardHidden | Android.Content.PM.ConfigChanges.Orientation)]
    public class DeviceListActivity : Activity
    {
        const string TAG = "DeviceListActivity";
        public const string EXTRA_DEVICE_ADDRESS = "device_address";

        BluetoothAdapter btAdapter;
        static ArrayAdapter<string> newDevicesArrayAdapter;
        DeviceDiscoveredReceiver receiver;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Setup the window
            RequestWindowFeature(WindowFeatures.IndeterminateProgress);
            SetContentView(Resource.Layout.activity_device_list);

            // Set result CANCELED incase the user backs out
            SetResult(Result.Canceled);

            // Initialize the button to perform device discovery			
            var scanButton = FindViewById<Button>(Resource.Id.button_scan);
            scanButton.Click += (sender, e) =>
            {
                DoDiscovery();
                (sender as View).Visibility = ViewStates.Gone;
            };

            // Initialize array adapters. One for already paired devices and
            // one for newly discovered devices
            var pairedDevicesArrayAdapter = new ArrayAdapter<string>(this, Resource.Layout.device_name);
            newDevicesArrayAdapter = new ArrayAdapter<string>(this, Resource.Layout.device_name);

            // Find and setup the ListView for paired devices
            var pairedListView = FindViewById<ListView>(Resource.Id.paired_devices);
            pairedListView.Adapter = pairedDevicesArrayAdapter;
            pairedListView.ItemClick += DeviceListView_ItemClick;

            // Find and set up the ListView for newly discovered devices
            var newDevicesListView = FindViewById<ListView>(Resource.Id.new_devices);
            newDevicesListView.Adapter = newDevicesArrayAdapter;
            newDevicesListView.ItemClick += DeviceListView_ItemClick;

            // Register for broadcasts when a device is discovered
            receiver = new DeviceDiscoveredReceiver(this);
            var filter = new IntentFilter(BluetoothDevice.ActionFound);
            RegisterReceiver(receiver, filter);

            // Register for broadcasts when discovery has finished
            filter = new IntentFilter(BluetoothAdapter.ActionDiscoveryFinished);
            RegisterReceiver(receiver, filter);

            // Get the local Bluetooth adapter
            btAdapter = BluetoothAdapter.DefaultAdapter;

            // Get a set of currently paired devices
            var pairedDevices = btAdapter.BondedDevices;

            // If there are paired devices, add each on to the ArrayAdapter
            if (pairedDevices.Count > 0)
            {
                FindViewById(Resource.Id.title_paired_devices).Visibility = ViewStates.Visible;
                foreach (var device in pairedDevices)
                {
                    pairedDevicesArrayAdapter.Add(device.Name + "\n" + device.Address);
                }
            }
            else
            {
                var noDevices = Resources.GetText(Resource.String.none_paired);
                pairedDevicesArrayAdapter.Add(noDevices);
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            // Make sure we're not doing discovery anymore
            if (btAdapter != null)
            {
                btAdapter.CancelDiscovery();
            }

            // Unregister broadcast listeners
            UnregisterReceiver(receiver);
        }

        /// <summary>
        /// Start device discovery with the BluetoothAdapter
        /// </summary>
        void DoDiscovery()
        {
            // Indicate scanning in the title
            SetProgressBarIndeterminateVisibility(true);
            SetTitle(Resource.String.scanning);

            // Turn on sub-title for new devices
            FindViewById<View>(Resource.Id.title_new_devices).Visibility = ViewStates.Visible;

            // If we're already discovering, stop it
            if (btAdapter.IsDiscovering)
            {
                btAdapter.CancelDiscovery();
            }

            // Request discover from BluetoothAdapter
            var x = btAdapter.StartDiscovery();
        }

        void DeviceListView_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            btAdapter.CancelDiscovery();

            // Get the device MAC address, which is the last 17 chars in the View
            var info = ((TextView)e.View).Text;
            var address = info.Substring(info.Length - 17);

            // Create the result intent and include MAC address.
            var intent = new Intent();
            intent.PutExtra(EXTRA_DEVICE_ADDRESS, address);

            SetResult(Result.Ok, intent);
            Finish();

        }

        public class DeviceDiscoveredReceiver : BroadcastReceiver
        {
            Activity chatActivity;

            public DeviceDiscoveredReceiver(Activity chat)
            {
                this.chatActivity = chat;
            }

            public override void OnReceive(Context context, Intent intent)
            {
                string action = intent.Action;

                // When discovery finds a device
                if (action == BluetoothDevice.ActionFound)
                {
                    // Get the BluetoothDevice object from the Intent
                    BluetoothDevice device = (BluetoothDevice)intent.GetParcelableExtra(BluetoothDevice.ExtraDevice);
                    // If it's already paired, skip it, because it's been listed already
                    if (device.BondState != Bond.Bonded)
                    {
                        newDevicesArrayAdapter.Add(device.Name + "\n" + device.Address);
                    }
                    // When discovery is finished, change the Activity title
                }
                else if (action == BluetoothAdapter.ActionDiscoveryFinished)
                {
                    chatActivity.SetProgressBarIndeterminateVisibility(false);
                    chatActivity.SetTitle(Resource.String.select_device);
                    if (newDevicesArrayAdapter.Count == 0)
                    {
                        var noDevices = chatActivity.Resources.GetText(Resource.String.none_found).ToString();
                        newDevicesArrayAdapter.Add(noDevices);

                    }
                }
            }
        }
    }
}